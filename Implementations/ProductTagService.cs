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
    public class ProductTagService : IProductTagService
    {
        private readonly IDbConnection _dbConnection;

        public ProductTagService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateProductTag(CreateProductTagDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new ProductTag object
            var productTag = new ProductTag
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.Now
            };

            // Step 3: Insert the new ProductTag object to the database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sql = "INSERT INTO ProductTags (Id, Name, Version, Created) VALUES (@Id, @Name, @Version, @Created)";
                    await _dbConnection.ExecuteAsync(sql, productTag, transaction);
                    transaction.Commit();
                    return productTag.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<ProductTag> GetProductTag(ProductTagRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == null && string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Initialize a ProductTag null object
            ProductTag productTag = null;

            // Step 3: Fetch ProductTag from the database
            if (request.Id != null && string.IsNullOrEmpty(request.Name))
            {
                var sql = "SELECT * FROM ProductTags WHERE Id = @Id";
                productTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(sql, new { Id = request.Id });
            }
            else if (!string.IsNullOrEmpty(request.Name) && request.Id == null)
            {
                var sql = "SELECT * FROM ProductTags WHERE Name = @Name";
                productTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(sql, new { Name = request.Name });
            }
            else
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            return productTag;
        }

        public async Task<string> UpdateProductTag(UpdateProductTagDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the ProductTag from the database
            var sqlSelect = "SELECT * FROM ProductTags WHERE Id = @Id";
            var productTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(sqlSelect, new { Id = request.Id });
            if (productTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update the ProductTag object
            productTag.Name = request.Name ?? productTag.Name;
            productTag.Version += 1;
            productTag.Changed = DateTime.Now;

            // Step 4: Insert the updated ProductTag object to the database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlUpdate = "UPDATE ProductTags SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sqlUpdate, productTag, transaction);
                    transaction.Commit();
                    return productTag.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteProductTag(DeleteProductTagDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the ProductTag from the database
            var sqlSelect = "SELECT * FROM ProductTags WHERE Id = @Id";
            var productTag = await _dbConnection.QuerySingleOrDefaultAsync<ProductTag>(sqlSelect, new { Id = request.Id });
            if (productTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the ProductTag object from the database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlDelete = "DELETE FROM ProductTags WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sqlDelete, new { Id = request.Id }, transaction);
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<List<ProductTag>> GetListProductTag(ListProductTagRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Set default sorting values if not provided
            request.SortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            request.SortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

            // Step 3: Fetch the list of ProductTags from the database
            var sql = $"SELECT * FROM ProductTags ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var productTags = await _dbConnection.QueryAsync<ProductTag>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });

            return productTags.ToList();
        }
    }
}
