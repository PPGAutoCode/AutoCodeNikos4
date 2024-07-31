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
    public class BasicPageService : IBasicPageService
    {
        private readonly IDbConnection _dbConnection;
        private readonly IImageService _imageService;

        public BasicPageService(IDbConnection dbConnection, IImageService imageService)
        {
            _dbConnection = dbConnection;
            _imageService = imageService;
        }

        public async Task<string> CreateBasicPage(CreateBasicPageDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            List<Guid> temporaryImageIds = null;
            if (request.Images != null)
            {
                temporaryImageIds = new List<Guid>();
                foreach (var createImageDto in request.Images)
                {
                    var imageId = await _imageService.CreateImage(createImageDto);
                    temporaryImageIds.Add(Guid.Parse(imageId));
                }
            }

            var basicPage = new BasicPage
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Body = request.Body
            };

            var basicPageImages = new List<BasicPageImage>();
            if (temporaryImageIds != null)
            {
                foreach (var temporaryImageId in temporaryImageIds)
                {
                    basicPageImages.Add(new BasicPageImage
                    {
                        Id = Guid.NewGuid(),
                        BasicPageId = basicPage.Id,
                        ImageId = temporaryImageId
                    });
                }
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO BasicPages (Id, Name, Body) VALUES (@Id, @Name, @Body)",
                        basicPage, transaction);

                    if (basicPageImages.Count > 0)
                    {
                        await _dbConnection.ExecuteAsync(
                            "INSERT INTO BasicPageImages (Id, BasicPageId, ImageId) VALUES (@Id, @BasicPageId, @ImageId)",
                            basicPageImages, transaction);
                    }

                    transaction.Commit();
                    return basicPage.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<BasicPageDto> GetBasicPage(BasicPageRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var basicPage = await _dbConnection.QuerySingleOrDefaultAsync<BasicPage>(
                "SELECT * FROM BasicPages WHERE Id = @Id", new { Id = request.Id });

            if (basicPage == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            var imageIds = (await _dbConnection.QueryAsync<Guid>(
                "SELECT ImageId FROM BasicPageImages WHERE BasicPageId = @BasicPageId",
                new { BasicPageId = basicPage.Id })).ToList();

            var images = new List<Image>();
            foreach (var imageId in imageIds)
            {
                var imageRequestDto = new ImageRequestDto { Id = imageId };
                var image = await _imageService.GetImage(imageRequestDto);
                images.Add(image);
            }

            return new BasicPageDto
            {
                Id = basicPage.Id,
                Name = basicPage.Name,
                Body = basicPage.Body,
                Images = images
            };
        }

        public async Task<string> UpdateBasicPage(UpdateBasicPageDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var basicPage = await _dbConnection.QuerySingleOrDefaultAsync<BasicPage>(
                "SELECT * FROM BasicPages WHERE Id = @Id", new { Id = request.Id });

            if (basicPage == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            List<Guid> temporaryImageIds = null;
            if (request.Images != null)
            {
                temporaryImageIds = new List<Guid>();
                foreach (var updateImageDto in request.Images.Where(i => i.Id == null))
                {
                    var createImageDto = new CreateImageDto
                    {
                        ImageName = updateImageDto.ImageName,
                        ImageFile = updateImageDto.ImageFile,
                        AltText = updateImageDto.AltText
                    };
                    var imageId = await _imageService.CreateImage(createImageDto);
                    temporaryImageIds.Add(Guid.Parse(imageId));
                }

                foreach (var updateImageDto in request.Images.Where(i => i.Id != null))
                {
                    await _imageService.UpdateImage(updateImageDto);
                    temporaryImageIds.Add(updateImageDto.Id.Value);
                }
            }

            if (request.Name != null)
            {
                basicPage.Name = request.Name;
            }
            if (request.Body != null)
            {
                basicPage.Body = request.Body;
            }

            var basicPageImages = new List<BasicPageImage>();
            if (temporaryImageIds != null)
            {
                foreach (var temporaryImageId in temporaryImageIds)
                {
                    basicPageImages.Add(new BasicPageImage
                    {
                        Id = Guid.NewGuid(),
                        BasicPageId = basicPage.Id,
                        ImageId = temporaryImageId
                    });
                }
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    if ((temporaryImageIds != null && temporaryImageIds.Count > 0) || (request.Images != null && !request.Images.Any(i => i.Id != null)))
                    {
                        await _dbConnection.ExecuteAsync(
                            "DELETE FROM BasicPageImages WHERE BasicPageId = @BasicPageId",
                            new { BasicPageId = basicPage.Id }, transaction);
                    }

                    await _dbConnection.ExecuteAsync(
                        "UPDATE BasicPages SET Name = @Name, Body = @Body WHERE Id = @Id",
                        new { basicPage.Name, basicPage.Body, basicPage.Id }, transaction);

                    if (basicPageImages.Count > 0)
                    {
                        await _dbConnection.ExecuteAsync(
                            "INSERT INTO BasicPageImages (Id, BasicPageId, ImageId) VALUES (@Id, @BasicPageId, @ImageId)",
                            basicPageImages, transaction);
                    }

                    transaction.Commit();
                    return basicPage.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteBasicPage(DeleteBasicPageDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var basicPage = await _dbConnection.QuerySingleOrDefaultAsync<BasicPage>(
                "SELECT * FROM BasicPages WHERE Id = @Id", new { Id = request.Id });

            if (basicPage == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            var imageIds = (await _dbConnection.QueryAsync<Guid>(
                "SELECT ImageId FROM BasicPageImages WHERE BasicPageId = @BasicPageId",
                new { BasicPageId = basicPage.Id })).ToList();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(
                        "DELETE FROM BasicPageImages WHERE BasicPageId = @BasicPageId",
                        new { BasicPageId = basicPage.Id }, transaction);

                    await _dbConnection.ExecuteAsync(
                        "DELETE FROM BasicPages WHERE Id = @Id",
                        new { Id = basicPage.Id }, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            if (imageIds != null)
            {
                foreach (var imageId in imageIds)
                {
                    var deleteImageDto = new DeleteImageDto { Id = imageId };
                    await _imageService.DeleteImage(deleteImageDto);
                }
            }

            return true;
        }

        public async Task<List<BasicPageDto>> GetListBasicPage(ListBasicPageRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            if (string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                request.SortField = "Id";
                request.SortOrder = "asc";
            }

            var basicPages = await _dbConnection.QueryAsync<BasicPage>(
                $"SELECT * FROM BasicPages ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY",
                new { request.PageOffset, request.PageLimit });

            var basicPageDtos = new List<BasicPageDto>();
            foreach (var basicPage in basicPages)
            {
                var imageIds = (await _dbConnection.QueryAsync<Guid>(
                    "SELECT ImageId FROM BasicPageImages WHERE BasicPageId = @BasicPageId",
                    new { BasicPageId = basicPage.Id })).ToList();

                var images = new List<Image>();
                foreach (var imageId in imageIds)
                {
                    var imageRequestDto = new ImageRequestDto { Id = imageId };
                    var image = await _imageService.GetImage(imageRequestDto);
                    images.Add(image);
                }

                basicPageDtos.Add(new BasicPageDto
                {
                    Id = basicPage.Id,
                    Name = basicPage.Name,
                    Body = basicPage.Body,
                    Images = images
                });
            }

            return basicPageDtos;
        }
    }
}