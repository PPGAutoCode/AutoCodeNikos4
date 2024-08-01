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
    public class FAQCategoryService : IFAQCategoryService
    {
        private readonly IDbConnection _dbConnection;

        public FAQCategoryService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateFAQCategory(CreateFAQCategoryDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var faqCategory = new FAQCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            const string sql = "INSERT INTO FAQCategories (Id, Name, Description) VALUES (@Id, @Name, @Description)";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, faqCategory, transaction);
                    transaction.Commit();
                    return faqCategory.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<FAQCategory> GetFAQCategory(FAQCategoryRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM FAQCategories WHERE Id = @Id";
            var result = await _dbConnection.QuerySingleOrDefaultAsync<FAQCategory>(sql, new { request.Id });

            if (result == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return result;
        }

        public async Task<string> UpdateFAQCategory(UpdateFAQCategoryDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM FAQCategories WHERE Id = @Id";
            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<FAQCategory>(selectSql, new { request.Id });

            if (existingCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingCategory.Name = request.Name;
            existingCategory.Description = request.Description ?? existingCategory.Description;

            const string updateSql = "UPDATE FAQCategories SET Name = @Name, Description = @Description WHERE Id = @Id";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, existingCategory, transaction);
                    transaction.Commit();
                    return existingCategory.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteFAQCategory(DeleteFAQCategoryDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM FAQCategories WHERE Id = @Id";
            var existingCategory = await _dbConnection.QuerySingleOrDefaultAsync<FAQCategory>(selectSql, new { request.Id });

            if (existingCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM FAQCategories WHERE Id = @Id";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
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

        public async Task<List<FAQCategory>> GetListFAQCategory(ListFAQCategoryRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            request.SortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            request.SortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

            var sql = $"SELECT * FROM FAQCategories ORDER BY {request.SortField} {request.SortOrder} OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";
            var result = await _dbConnection.QueryAsync<FAQCategory>(sql);

            if (result == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return result.ToList();
        }
    }
}