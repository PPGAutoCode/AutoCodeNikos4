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
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IDbConnection _dbConnection;

        public ProductCategoryService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateProductCategory(CreateProductCategoryDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            if (request.Parent != Guid.Empty)
            {
                var parentExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM ProductCategories WHERE Id = @Parent",
                    new { Parent = request.Parent });

                if (!parentExists)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            var productCategory = new ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                UserQuestionnaire = request.UserQuestionnaire,
                Description = request.Description,
                Parent = request.Parent,
                UrlAlias = request.UrlAlias,
                Weight = request.Weight
            };

            try
            {
                await _dbConnection.ExecuteAsync(
                    "INSERT INTO ProductCategories (Id, Name, UserQuestionnaire, Description, Parent, UrlAlias, Weight) VALUES (@Id, @Name, @UserQuestionnaire, @Description, @Parent, @UrlAlias, @Weight)",
                    productCategory);

                return productCategory.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<ProductCategory> GetProductCategory(ProductCategoryRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var productCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(
                "SELECT * FROM ProductCategories WHERE Id = @Id",
                new { Id = request.Id });

            if (productCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return productCategory;
        }

        public async Task<string> UpdateProductCategory(UpdateProductCategoryDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            if (request.Parent != null && request.Parent != Guid.Empty)
            {
                var parentExists = await _dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM ProductCategories WHERE Id = @Parent",
                    new { Parent = request.Parent });

                if (!parentExists)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            var productCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(
                "SELECT * FROM ProductCategories WHERE Id = @Id",
                new { Id = request.Id });

            if (productCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            productCategory.Name = request.Name ?? productCategory.Name;
            productCategory.UserQuestionnaire = request.UserQuestionnaire ?? productCategory.UserQuestionnaire;
            productCategory.Description = request.Description ?? productCategory.Description;
            productCategory.Parent = request.Parent ?? productCategory.Parent;
            productCategory.UrlAlias = request.UrlAlias ?? productCategory.UrlAlias;
            productCategory.Weight = request.Weight ?? productCategory.Weight;

            try
            {
                await _dbConnection.ExecuteAsync(
                    "UPDATE ProductCategories SET Name = @Name, UserQuestionnaire = @UserQuestionnaire, Description = @Description, Parent = @Parent, UrlAlias = @UrlAlias, Weight = @Weight WHERE Id = @Id",
                    productCategory);

                return productCategory.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteProductCategory(DeleteProductCategoryDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var productCategory = await _dbConnection.QuerySingleOrDefaultAsync<ProductCategory>(
                "SELECT * FROM ProductCategories WHERE Id = @Id",
                new { Id = request.Id });

            if (productCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            try
            {
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM ProductCategories WHERE Id = @Id",
                    new { Id = request.Id });

                return true;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<List<ProductCategory>> GetListProductCategory(ListProductCategoryRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            var sortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

            var query = $"SELECT * FROM ProductCategories ORDER BY {sortField} {sortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var productCategories = await _dbConnection.QueryAsync<ProductCategory>(
                    query,
                    new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });

                return productCategories.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
