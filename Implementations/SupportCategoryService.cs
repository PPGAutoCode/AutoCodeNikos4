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
    public class SupportCategoryService : ISupportCategoryService
    {
        private readonly IDbConnection _dbConnection;

        public SupportCategoryService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSupportCategory(CreateSupportCategoryDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var supportCategory = new SupportCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow
            };

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sql = "INSERT INTO SupportCategories (Id, Name, Version, Created) VALUES (@Id, @Name, @Version, @Created)";
                    await _dbConnection.ExecuteAsync(sql, supportCategory, transaction);
                    transaction.Commit();
                    return supportCategory.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<SupportCategory> GetSupportCategory(SupportCategoryRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = "SELECT * FROM SupportCategories WHERE Id = @Id";
            var supportCategory = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>(sql, new { Id = request.Id });

            if (supportCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return supportCategory;
        }

        public async Task<string> UpdateSupportCategory(UpdateSupportCategoryDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sqlSelect = "SELECT * FROM SupportCategories WHERE Id = @Id";
            var supportCategory = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>(sqlSelect, new { Id = request.Id });

            if (supportCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            supportCategory.Name = request.Name;
            supportCategory.Version += 1;
            supportCategory.Changed = DateTime.UtcNow;

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlUpdate = "UPDATE SupportCategories SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sqlUpdate, supportCategory, transaction);
                    transaction.Commit();
                    return supportCategory.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteSupportCategory(DeleteSupportCategoryDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sqlSelect = "SELECT * FROM SupportCategories WHERE Id = @Id";
            var supportCategory = await _dbConnection.QuerySingleOrDefaultAsync<SupportCategory>(sqlSelect, new { Id = request.Id });

            if (supportCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlDelete = "DELETE FROM SupportCategories WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sqlDelete, new { Id = request.Id }, transaction);
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<List<SupportCategory>> GetListSupportCategory(ListSupportCategoryRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            var sortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

            var sql = $"SELECT * FROM SupportCategories ORDER BY {sortField} {sortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var supportCategories = await _dbConnection.QueryAsync<SupportCategory>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });

            if (supportCategories == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return supportCategories.ToList();
        }
    }
}
