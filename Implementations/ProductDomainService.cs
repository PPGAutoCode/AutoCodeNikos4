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
    public class ProductDomainService : IProductDomainService
    {
        private readonly IDbConnection _dbConnection;

        public ProductDomainService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateProductDomain(CreateProductDomainDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var productDomain = new ProductDomain
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                CreatorId = Guid.NewGuid(), // Assuming there's a way to get the creator ID
                ChangedUser = Guid.NewGuid() // Assuming there's a way to get the changed user ID initially
            };

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    const string sql = @"INSERT INTO ProductDomains (Id, Name, Version, Created, Changed, CreatorId, ChangedUser) 
                                         VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";
                    await _dbConnection.ExecuteAsync(sql, productDomain, transaction);
                    transaction.Commit();
                    return productDomain.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<ProductDomain> GetProductDomain(ProductDomainRequestDto request)
        {
            if ((request.Id == Guid.Empty && string.IsNullOrWhiteSpace(request.Name)) || 
                (request.Id != Guid.Empty && !string.IsNullOrWhiteSpace(request.Name)))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            ProductDomain productDomain = null;

            if (request.Id != Guid.Empty)
            {
                const string sql = "SELECT * FROM ProductDomains WHERE Id = @Id";
                productDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(sql, new { request.Id });
            }
            else if (!string.IsNullOrWhiteSpace(request.Name))
            {
                const string sql = "SELECT * FROM ProductDomains WHERE Name = @Name";
                productDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(sql, new { request.Name });
            }

            return productDomain;
        }

        public async Task<string> UpdateProductDomain(UpdateProductDomainDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM ProductDomains WHERE Id = @Id";
            var productDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(selectSql, new { request.Id });

            if (productDomain == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            productDomain.Name = request.Name;
            productDomain.Version += 1;
            productDomain.Changed = DateTime.Now;

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    const string updateSql = @"UPDATE ProductDomains 
                                               SET Name = @Name, Version = @Version, Changed = @Changed 
                                               WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(updateSql, productDomain, transaction);
                    transaction.Commit();
                    return productDomain.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteProductDomain(DeleteProductDomainDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM ProductDomains WHERE Id = @Id";
            var productDomain = await _dbConnection.QuerySingleOrDefaultAsync<ProductDomain>(selectSql, new { request.Id });

            if (productDomain == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    const string deleteSql = "DELETE FROM ProductDomains WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(deleteSql, new { request.Id }, transaction);
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

        public async Task<List<ProductDomain>> GetListProductDomain(ListProductDomainRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            if (string.IsNullOrWhiteSpace(request.SortField) || string.IsNullOrWhiteSpace(request.SortOrder))
            {
                request.SortField = "Id";
                request.SortOrder = "asc";
            }

            var sql = $"SELECT * FROM ProductDomains ORDER BY {request.SortField} {request.SortOrder} " +
                      $"OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";

            try
            {
                var productDomains = await _dbConnection.QueryAsync<ProductDomain>(sql);
                return productDomains.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
