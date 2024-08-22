using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class APIEndpointService : IAPIEndpointService
    {
        private readonly IDbConnection _dbConnection;
        private readonly IAppEnvironmentService _appEnvironmentService;
        private readonly IApiTagService _apiTagService;
        private readonly IAttachmentService _attachmentService;

        public APIEndpointService(IDbConnection dbConnection, IAppEnvironmentService appEnvironmentService,
            IApiTagService apiTagService, IAttachmentService attachmentService)
        {
            _dbConnection = dbConnection;
            _appEnvironmentService = appEnvironmentService;
            _apiTagService = apiTagService;
            _attachmentService = attachmentService;
        }

        public async Task<string> CreateAPIEndpoint(CreateAPIEndpointDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.ApiName) || request.AppEnvironment == Guid.Empty ||
                string.IsNullOrEmpty(request.Langcode) || string.IsNullOrEmpty(request.UrlAlias))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch and Validate AppEnvironment
            var appEnvironmentRequest = new AppEnvironmentRequestDto { Id = request.AppEnvironment };
            var appEnvironment = await _appEnvironmentService.GetAppEnvironment(appEnvironmentRequest);
            if (appEnvironment == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Fetch or Create ApiTags
            List<Guid> apiTags = new List<Guid>();
            if (request.ApiTags != null)
            {
                var allApiTags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags");
                var newApiTags = request.ApiTags.Except(allApiTags.Select(at => at.Name)).ToList();

                foreach (var newApiTag in newApiTags)
                {
                    var createApiTagDto = new CreateApiTagDto { Name = newApiTag };
                    var apiTagId = await _apiTagService.CreateApiTag(createApiTagDto);
                    apiTags.Add(Guid.Parse(apiTagId));
                }

                apiTags.AddRange(allApiTags.Where(at => request.ApiTags.Contains(at.Name)).Select(at => at.Id));
            }

            // Step 4: Create Attachment Files
            Guid? documentationId = null, swaggerId = null, tourId = null;
            if (request.Documentation != null)
            {
                documentationId = Guid.Parse(await _attachmentService.CreateAttachment(request.Documentation));
            }

            if (request.Swagger != null)
            {
                swaggerId = Guid.Parse(await _attachmentService.CreateAttachment(request.Swagger));
            }

            if (request.Tour != null)
            {
                tourId = Guid.Parse(await _attachmentService.CreateAttachment(request.Tour));
            }

            // Step 5: Create a new APIEndpoint object
            var apiEndpoint = new APIEndpoint
            {
                Id = Guid.NewGuid(),
                ApiName = request.ApiName,
                ApiScope = request.ApiScope,
                ApiScopeProduction = request.ApiScopeProduction,
                Deprecated = request.Deprecated,
                Description = request.Description,
                Documentation = documentationId,
                EndpointUrls = request.EndpointUrls,
                AppEnvironment = request.AppEnvironment,
                Swagger = swaggerId,
                Tour = tourId,
                ApiVersion = request.ApiVersion,
                Langcode = request.Langcode,
                Sticky = request.Sticky,
                Promote = request.Promote,
                UrlAlias = request.UrlAlias,
                Published = request.Published,
                Version = 1,
                Created = DateTime.UtcNow
            };

            // Step 6: Create a new list of APIEndpointTags objects
            var apiEndpointTags = apiTags.Select(apiTagId => new APIEndpointTag
            {
                Id = Guid.NewGuid(),
                APIEndpointId = apiEndpoint.Id,
                APITagId = apiTagId
            }).ToList();

            // Step 7: Perform Database Operations in a single transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO APIEndpoints (Id, ApiName, ApiScope, ApiScopeProduction, Deprecated, Description, Documentation, EndpointUrls, AppEnvironment, Swagger, Tour, ApiVersion, Langcode, Sticky, Promote, UrlAlias, Published, Version, Created) VALUES (@Id, @ApiName, @ApiScope, @ApiScopeProduction, @Deprecated, @Description, @Documentation, @EndpointUrls, @AppEnvironment, @Swagger, @Tour, @ApiVersion, @Langcode, @Sticky, @Promote, @UrlAlias, @Published, @Version, @Created)",
                        apiEndpoint, transaction);

                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO APIEndpointTags (Id, APIEndpointId, APITagId) VALUES (@Id, @APIEndpointId, @APITagId)",
                        apiEndpointTags, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            return apiEndpoint.Id.ToString();
        }

        public async Task<APIEndpointDto> GetAPIEndpoint(APIEndpointRequestDto request)
        {
            // Step 1: Validate Request Payload
            if (request.Id == null && string.IsNullOrEmpty(request.ApiName))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch APIEndpoint
            APIEndpoint apiEndpoint;
            if (request.Id != null)
            {
                apiEndpoint =
                    await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>(
                        "SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            }
            else
            {
                apiEndpoint = await _dbConnection.QueryFirstOrDefaultAsync<APIEndpoint>(
                    "SELECT TOP 1 * FROM APIEndpoints WHERE ApiName = @ApiName", new { ApiName = request.ApiName });
            }

            if (apiEndpoint == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Retrieve Attachments
            async Task<Attachment> GetAttachment(Guid? attachmentId)
            {
                if (attachmentId != null && attachmentId != Guid.Empty)
                {
                    var attachmentRequest = new AttachmentRequestDto { Id = (Guid)attachmentId };
                    return await _attachmentService.GetAttachment(attachmentRequest);
                }

                return null;
            }

            var documentation = await GetAttachment(apiEndpoint.Documentation);
            var swagger = await GetAttachment(apiEndpoint.Swagger);
            var tour = await GetAttachment(apiEndpoint.Tour);

            // Step 4: Fetch Associated ApiTags
            var apiTagIds =
                (await _dbConnection.QueryAsync<Guid>(
                    "SELECT APITagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId",
                    new { APIEndpointId = apiEndpoint.Id })).ToList();
            var temporaryApiTags = new List<ApiTag>();

            foreach (var apiTagId in apiTagIds)
            {
                var apiTagRequest = new ApiTagRequestDto { Id = apiTagId };
                var apiTag = await _apiTagService.GetApiTag(apiTagRequest);
                if (apiTag != null)
                {
                    temporaryApiTags.Add(apiTag);
                }
            }

            // Step 5: Map db object to APIEndpointDto and return
            return new APIEndpointDto
            {
                Id = apiEndpoint.Id,
                ApiName = apiEndpoint.ApiName,
                ApiScope = apiEndpoint.ApiScope,
                ApiScopeProduction = apiEndpoint.ApiScopeProduction,
                ApiTags = temporaryApiTags,
                Deprecated = apiEndpoint.Deprecated,
                Description = apiEndpoint.Description,
                Documentation = documentation,
                EndpointUrls = apiEndpoint.EndpointUrls,
                AppEnvironment = apiEndpoint.AppEnvironment,
                Swagger = swagger,
                Tour = tour,
                ApiVersion = apiEndpoint.ApiVersion,
                Langcode = apiEndpoint.Langcode,
                Sticky = apiEndpoint.Sticky,
                Promote = apiEndpoint.Promote,
                UrlAlias = apiEndpoint.UrlAlias,
                Published = apiEndpoint.Published,
                Version = apiEndpoint.Version,
                Created = apiEndpoint.Created,
                Changed = apiEndpoint.Changed,
                CreatorId = apiEndpoint.CreatorId,
                ChangedUser = apiEndpoint.ChangedUser
            };
        }

        public async Task<string> UpdateAPIEndpoint(UpdateAPIEndpointDto request)
        {
            // Step 1: Validate that the UpdateAPIEndpointDto contains the necessary parameter: "Id"
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 3: Fetch the existing APIEndpoint object from the database table APIEndpoints
            var existingAPIEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            if (existingAPIEndpoint == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 5: Fetch and Validate AppEnvironment
            if (request.AppEnvironment != Guid.Empty)
            {
                var appEnvironmentRequest = new AppEnvironmentRequestDto { Id = request.AppEnvironment };
                var appEnvironment = await _appEnvironmentService.GetAppEnvironment(appEnvironmentRequest);
                if (appEnvironment == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 6: Handle Attachments
            async Task<Guid?> HandleAttachment(Guid? existingAttachmentId, UpdateAttachmentDto updateAttachmentDto)
            {
                if (existingAttachmentId.HasValue && updateAttachmentDto != null)
                {
                    await _attachmentService.UpdateAttachment(updateAttachmentDto);
                    return existingAttachmentId;
                }
                else if (!existingAttachmentId.HasValue && updateAttachmentDto != null)
                {
                    var createAttachmentResult = await _attachmentService.CreateAttachment(new CreateAttachmentDto
                    {
                        FileName = updateAttachmentDto.FileName,
                        FileData = updateAttachmentDto.FileData
                    });
                    return Guid.Parse(createAttachmentResult);
                }
                return existingAttachmentId;
            }

            var documentationId = await HandleAttachment(existingAPIEndpoint.Documentation, request.Documentation);
            var swaggerId = await HandleAttachment(existingAPIEndpoint.Swagger, request.Swagger);
            var tourId = await HandleAttachment(existingAPIEndpoint.Tour, request.Tour);

            // Step 7: Fetch or Create ApiTags
            List<ApiTag> allApiTags = (await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags")).ToList();
            List<Guid> existingApiTagsIds = (await _dbConnection.QueryAsync<Guid>("SELECT APITagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = request.Id })).ToList();
            List<ApiTag> existingApiTags = allApiTags.Where(tag => existingApiTagsIds.Contains(tag.Id)).ToList();

            List<Guid> apiTagsToRemove = existingApiTags.Where(existingTag => !request.ApiTags.Contains(existingTag.Name)).Select(tag => tag.Id).ToList();
            List<string> newApiTagNames = request.ApiTags.Where(newTag => !existingApiTags.Select(existingTag => existingTag.Name).Contains(newTag)).ToList();
            List<ApiTag> newApiTags = allApiTags.Where(tag => newApiTagNames.Contains(tag.Name)).ToList();
            List<ApiTag> apiTagsToAdd = newApiTags.Where(newTag => allApiTags.Contains(newTag)).ToList();
            List<string> addApiTagNames = newApiTagNames.Where(newTagName => !allApiTags.Select(tag => tag.Name).Contains(newTagName)).ToList();

            if (addApiTagNames != null)
            {
                foreach (var addApiTagName in addApiTagNames)
                {
                    var createApiTagResult = await _apiTagService.CreateApiTag(new CreateApiTagDto { Name = addApiTagName });
                    apiTagsToAdd.Add(new ApiTag { Id = Guid.Parse(createApiTagResult), Name = addApiTagName });
                }
            }

            // Step 8: Update the APIEndpoint object with the parameters from UpdateAPIEndpointDto
            existingAPIEndpoint.ApiName = request.ApiName ?? existingAPIEndpoint.ApiName;
            existingAPIEndpoint.ApiScope = request.ApiScope ?? existingAPIEndpoint.ApiScope;
            existingAPIEndpoint.ApiScopeProduction = request.ApiScopeProduction ?? existingAPIEndpoint.ApiScopeProduction;
            existingAPIEndpoint.Deprecated = request.Deprecated;
            existingAPIEndpoint.Description = request.Description ?? existingAPIEndpoint.Description;
            existingAPIEndpoint.Documentation = documentationId ?? existingAPIEndpoint.Documentation;
            existingAPIEndpoint.EndpointUrls = request.EndpointUrls ?? existingAPIEndpoint.EndpointUrls;
            existingAPIEndpoint.AppEnvironment = request.AppEnvironment;
            existingAPIEndpoint.Swagger = swaggerId ?? existingAPIEndpoint.Swagger;
            existingAPIEndpoint.Tour = tourId ?? existingAPIEndpoint.Tour;
            existingAPIEndpoint.ApiVersion = request.ApiVersion ?? existingAPIEndpoint.ApiVersion;
            existingAPIEndpoint.Langcode = request.Langcode ?? existingAPIEndpoint.Langcode;
            existingAPIEndpoint.Sticky = request.Sticky;
            existingAPIEndpoint.Promote = request.Promote;
            existingAPIEndpoint.UrlAlias = request.UrlAlias ?? existingAPIEndpoint.UrlAlias;
            existingAPIEndpoint.Published = request.Published;
            existingAPIEndpoint.Version += 1;
            existingAPIEndpoint.Changed = DateTime.UtcNow;

            // Step 9: Create new list of APIEndpointTags objects
            List<APIEndpointTag> aPIEndpointTags = apiTagsToAdd.Select(apiTag => new APIEndpointTag
            {
                Id = Guid.NewGuid(),
                APIEndpointId = request.Id,
                APITagId = apiTag.Id
            }).ToList();

            // Step 10: In a single SQL Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("UPDATE APIEndpoints SET ApiName = @ApiName, ApiScope = @ApiScope, ApiScopeProduction = @ApiScopeProduction, Deprecated = @Deprecated, Description = @Description, Documentation = @Documentation, EndpointUrls = @EndpointUrls, AppEnvironment = @AppEnvironment, Swagger = @Swagger, Tour = @Tour, ApiVersion = @ApiVersion, Langcode = @Langcode, Sticky = @Sticky, Promote = @Promote, UrlAlias = @UrlAlias, Published = @Published, Version = @Version, Changed = @Changed WHERE Id = @Id", existingAPIEndpoint, transaction);

                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId AND APITagId IN @ApiTagsToRemove", new { APIEndpointId = request.Id, ApiTagsToRemove = apiTagsToRemove }, transaction);

                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (Id, APIEndpointId, APITagId) VALUES (@Id, @APIEndpointId, @APITagId)", aPIEndpointTags, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            return existingAPIEndpoint.Id.ToString();
        }

        public async Task<bool> DeleteAPIEndpoint(DeleteAPIEndpointDto request)
    {
        // Step 1: Validate Request Payload
        if (request.Id == Guid.Empty)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Fetch Existing API Endpoint
        var apiEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>(
            "SELECT * FROM APIEndpoints WHERE Id = @Id", new { request.Id });

        if (apiEndpoint == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 3: Delete Related Attachments
        if (apiEndpoint.Documentation != null)
        {
            await _attachmentService.DeleteAttachment(new DeleteAttachmentDto { Id = apiEndpoint.Documentation });
        }
        if (apiEndpoint.Swagger != null)
        {
            await _attachmentService.DeleteAttachment(new DeleteAttachmentDto { Id = apiEndpoint.Swagger });
        }
        if (apiEndpoint.Tour != null)
        {
            await _attachmentService.DeleteAttachment(new DeleteAttachmentDto { Id = apiEndpoint.Tour });
        }

        // Step 4: Perform Database Updates in a Single Transaction
        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                // Delete all APIEndpoint Tags associations
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM APIEndpointTags WHERE APIEndpointId = @Id", new { apiEndpoint.Id }, transaction);

                // Delete ApiEndpoint
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM APIEndpoints WHERE Id = @Id", new { apiEndpoint.Id }, transaction);

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        return true;
    }

        public async Task<List<APIEndpointDto>> GetListAPIEndpoint(ListAPIEndpointRequestDto requestDto)
    {
        // Step 1: Validate the requestDto
        if (requestDto.PageLimit <= 0 || requestDto.PageOffset < 0)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Set default sorting if not provided
        if (string.IsNullOrEmpty(requestDto.SortField))
        {
            requestDto.SortField = "Id";
        }
        if (string.IsNullOrEmpty(requestDto.SortOrder))
        {
            requestDto.SortOrder = "asc";
        }

        // Step 3: Fetch the list of ApiEndpoints from the database
        string query = $@"SELECT * FROM ApiEndpoints ORDER BY {requestDto.SortField} {requestDto.SortOrder} 
                          OFFSET {requestDto.PageOffset} ROWS FETCH NEXT {requestDto.PageLimit} ROWS ONLY";

        var apiEndpoints = await _dbConnection.QueryAsync<APIEndpoint>(query);

        var apiEndpointDtos = new List<APIEndpointDto>();

        foreach (var apiEndpoint in apiEndpoints)
        {
            var apiEndpointDto = new APIEndpointDto
            {
                Id = apiEndpoint.Id,
                ApiName = apiEndpoint.ApiName,
                ApiScope = apiEndpoint.ApiScope,
                ApiScopeProduction = apiEndpoint.ApiScopeProduction,
                Deprecated = apiEndpoint.Deprecated,
                Description = apiEndpoint.Description,
                EndpointUrls = apiEndpoint.EndpointUrls,
                AppEnvironment = apiEndpoint.AppEnvironment,
                ApiVersion = apiEndpoint.ApiVersion,
                Langcode = apiEndpoint.Langcode,
                Sticky = apiEndpoint.Sticky,
                Promote = apiEndpoint.Promote,
                UrlAlias = apiEndpoint.UrlAlias,
                Published = apiEndpoint.Published,
                Version = apiEndpoint.Version,
                Created = apiEndpoint.Created,
                Changed = apiEndpoint.Changed,
                CreatorId = apiEndpoint.CreatorId,
                ChangedUser = apiEndpoint.ChangedUser
            };

            // Step 4: Fetch and map attachments
            if (apiEndpoint.Documentation != null)
            {
                var attachmentRequestDto = new AttachmentRequestDto { Id = apiEndpoint.Documentation };
                var documentation = await _attachmentService.GetAttachment(attachmentRequestDto);
                apiEndpointDto.Documentation = documentation;
            }
            if (apiEndpoint.Swagger != null)
            {
                var attachmentRequestDto = new AttachmentRequestDto { Id = apiEndpoint.Swagger };
                var swagger = await _attachmentService.GetAttachment(attachmentRequestDto);
                apiEndpointDto.Swagger = swagger;
            }
            if (apiEndpoint.Tour != null)
            {
                var attachmentRequestDto = new AttachmentRequestDto { Id = apiEndpoint.Tour };
                var tour = await _attachmentService.GetAttachment(attachmentRequestDto);
                apiEndpointDto.Tour = tour;
            }

            // Step 5: Fetch and map related ApiTags
            string tagQuery = $"SELECT ApiTagId FROM ApiEndpointTags WHERE ApiEndpointId = '{apiEndpoint.Id}'";
            var apiTagIds = await _dbConnection.QueryAsync<Guid>(tagQuery);

            if (apiTagIds.Any())
            {
                var apiTags = new List<ApiTag>();
                foreach (var apiTagId in apiTagIds)
                {
                    var apiTagRequestDto = new ApiTagRequestDto { Id = apiTagId };
                    var apiTag = await _apiTagService.GetApiTag(apiTagRequestDto);
                    apiTags.Add(apiTag);
                }
                apiEndpointDto.ApiTags = apiTags;
            }

            apiEndpointDtos.Add(apiEndpointDto);
        }

        return apiEndpointDtos;
    }
    }
}

