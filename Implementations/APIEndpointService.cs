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
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var apiEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            if (apiEndpoint == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            if (request.AppEnvironment != null)
            {
                var appEnvironment = await _appEnvironmentService.GetAppEnvironment(new AppEnvironmentRequestDto { Id = request.AppEnvironment.Value });
                if (appEnvironment == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            async Task<Guid?> HandleAttachmentUpdate(UpdateAttachmentDto attachmentDto, Guid? existingAttachmentId)
            {
                if (attachmentDto != null)
                {
                    if (attachmentDto.Id == null)
                    {
                        var createdAttachmentId = await _attachmentService.CreateAttachment(new CreateAttachmentDto
                        {
                            FileName = attachmentDto.FileName,
                            FileData = attachmentDto.FileData
                        });
                        return Guid.Parse(createdAttachmentId);
                    }
                    else
                    {
                        var updatedAttachmentId = await _attachmentService.UpdateAttachment(attachmentDto);
                        return Guid.Parse(updatedAttachmentId);
                    }
                }
                return existingAttachmentId;
            }

            var documentationId = await HandleAttachmentUpdate(request.Documentation, apiEndpoint.Documentation);
            var swaggerId = await HandleAttachmentUpdate(request.Swagger, apiEndpoint.Swagger);
            var tourId = await HandleAttachmentUpdate(request.Tour, apiEndpoint.Tour);

            List<Guid> apiTagsToRemove = new List<Guid>();
            List<ApiTag> apiTagsToAdd = new List<ApiTag>();

            if (request.ApiTags != null)
            {
                var allApiTags = (await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags")).ToList();
                var existingApiTagsIds = (await _dbConnection.QueryAsync<Guid>("SELECT APITagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = request.Id })).ToList();
                var existingApiTags = allApiTags.Where(tag => existingApiTagsIds.Contains(tag.Id)).ToList();

                apiTagsToRemove = existingApiTags.Where(existingTag => !request.ApiTags.Contains(existingTag.Name)).Select(tag => tag.Id).ToList();
                var newApiTagNames = request.ApiTags.Except(existingApiTags.Select(tag => tag.Name)).ToList();
                var existedNewApiTags = allApiTags.Where(tag => newApiTagNames.Contains(tag.Name)).ToList();
                var apiTagsToCreate = newApiTagNames.Except(existedNewApiTags.Select(tag => tag.Name)).ToList();

                if (apiTagsToCreate != null)
                {
                    foreach (var apiTagToCreate in apiTagsToCreate)
                    {
                        var createdApiTagId = await _apiTagService.CreateApiTag(new CreateApiTagDto { Name = apiTagToCreate });
                        apiTagsToAdd.Add(new ApiTag { Id = Guid.Parse(createdApiTagId), Name = apiTagToCreate });
                    }
                }
            }

            apiEndpoint.ApiName = request.ApiName ?? apiEndpoint.ApiName;
            apiEndpoint.ApiScope = request.ApiScope ?? apiEndpoint.ApiScope;
            apiEndpoint.ApiScopeProduction = request.ApiScopeProduction ?? apiEndpoint.ApiScopeProduction;
            apiEndpoint.Deprecated = request.Deprecated ?? apiEndpoint.Deprecated;
            apiEndpoint.Description = request.Description ?? apiEndpoint.Description;
            apiEndpoint.Documentation = documentationId ?? apiEndpoint.Documentation;
            apiEndpoint.EndpointUrls = request.EndpointUrls ?? apiEndpoint.EndpointUrls;
            apiEndpoint.AppEnvironment = request.AppEnvironment ?? apiEndpoint.AppEnvironment;
            apiEndpoint.Swagger = swaggerId ?? apiEndpoint.Swagger;
            apiEndpoint.Tour = tourId ?? apiEndpoint.Tour;
            apiEndpoint.ApiVersion = request.ApiVersion ?? apiEndpoint.ApiVersion;
            apiEndpoint.Langcode = request.Langcode ?? apiEndpoint.Langcode;
            apiEndpoint.Sticky = request.Sticky ?? apiEndpoint.Sticky;
            apiEndpoint.Promote = request.Promote ?? apiEndpoint.Promote;
            apiEndpoint.UrlAlias = request.UrlAlias ?? apiEndpoint.UrlAlias;
            apiEndpoint.Published = request.Published ?? apiEndpoint.Published;
            apiEndpoint.Version += 1;
            apiEndpoint.Changed = DateTime.UtcNow;

            var apiEndpointTags = new List<APIEndpointTag>();
            foreach (var apiTag in apiTagsToAdd)
            {
                apiEndpointTags.Add(new APIEndpointTag
                {
                    Id = Guid.NewGuid(),
                    APIEndpointId = apiEndpoint.Id,
                    APITagId = apiTag.Id
                });
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("UPDATE APIEndpoints SET ApiName = @ApiName, ApiScope = @ApiScope, ApiScopeProduction = @ApiScopeProduction, Deprecated = @Deprecated, Description = @Description, Documentation = @Documentation, EndpointUrls = @EndpointUrls, AppEnvironment = @AppEnvironment, Swagger = @Swagger, Tour = @Tour, ApiVersion = @ApiVersion, Langcode = @Langcode, Sticky = @Sticky, Promote = @Promote, UrlAlias = @UrlAlias, Published = @Published, Version = @Version, Changed = @Changed WHERE Id = @Id", apiEndpoint, transaction);

                    if (apiTagsToRemove.Any())
                    {
                        await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId AND APITagId IN @APITagIds", new { APIEndpointId = apiEndpoint.Id, APITagIds = apiTagsToRemove }, transaction);
                    }

                    if (apiEndpointTags.Any())
                    {
                        await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (Id, APIEndpointId, APITagId) VALUES (@Id, @APIEndpointId, @APITagId)", apiEndpointTags, transaction);
                    }

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

        public async Task<bool> DeleteAPIEndpoint(DeleteAPIEndpointDto deleteDto)
        {
            if (deleteDto.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>(
                "SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = deleteDto.Id });

            if (existingEndpoint == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            if (deleteDto.FieldsToDelete != null)
            {
                foreach (var field in deleteDto.FieldsToDelete)
                {
                    var updateQuery = $"UPDATE APIEndpoints SET {field} = NULL WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(updateQuery, new { Id = deleteDto.Id });
                }
            }
            else
            {
                if (existingEndpoint.Documentation != null)
                {
                    await _attachmentService.DeleteAttachment(new DeleteAttachmentDto { Id = existingEndpoint.Documentation });
                }
                if (existingEndpoint.Swagger != null)
                {
                    await _attachmentService.DeleteAttachment(new DeleteAttachmentDto { Id = existingEndpoint.Swagger });
                }
                if (existingEndpoint.Tour != null)
                {
                    await _attachmentService.DeleteAttachment(new DeleteAttachmentDto { Id = existingEndpoint.Tour });
                }

                using (var transaction = _dbConnection.BeginTransaction())
                {
                    try
                    {
                        await _dbConnection.ExecuteAsync(
                            "DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId",
                            new { APIEndpointId = deleteDto.Id },
                            transaction);

                        await _dbConnection.ExecuteAsync(
                            "DELETE FROM APIEndpoints WHERE Id = @Id",
                            new { Id = deleteDto.Id },
                            transaction);

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw new TechnicalException("DP-500", "Technical Error");
                    }
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

