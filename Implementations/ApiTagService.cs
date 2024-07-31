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
    public class ApiTagService : IApiTagService
    {
        private readonly IDbConnection _dbConnection;

        public ApiTagService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateApiTag(CreateApiTagDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var apiTag = new ApiTag
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.Now
            };

            const string sql = "INSERT INTO ApiTags (Id, Name, Version, Created) VALUES (@Id, @Name, @Version, @Created)";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, apiTag, transaction);
                    transaction.Commit();
                    return apiTag.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<ApiTag> GetApiTag(ApiTagRequestDto request)
        {
            if (request.Id == null && string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            ApiTag apiTag = null;

            if (request.Id != null && string.IsNullOrWhiteSpace(request.Name))
            {
                const string sql = "SELECT * FROM ApiTags WHERE Id = @Id";
                apiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(sql, new { Id = request.Id });
            }
            else if (!string.IsNullOrWhiteSpace(request.Name) && request.Id == null)
            {
                const string sql = "SELECT * FROM ApiTags WHERE Name = @Name";
                apiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(sql, new { Name = request.Name });
            }
            else
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            return apiTag;
        }

        public async Task<string> UpdateApiTag(UpdateApiTagDto request)
        {
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM ApiTags WHERE Id = @Id";
            var apiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(selectSql, new { Id = request.Id });

            if (apiTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            apiTag.Name = request.Name ?? apiTag.Name;
            apiTag.Version += 1;
            apiTag.Changed = DateTime.Now;

            const string updateSql = "UPDATE ApiTags SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, apiTag, transaction);
                    transaction.Commit();
                    return apiTag.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteApiTag(DeleteApiTagDto request)
        {
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM ApiTags WHERE Id = @Id";
            var apiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(selectSql, new { Id = request.Id });

            if (apiTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM ApiTags WHERE Id = @Id";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id }, transaction);
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

        public async Task<List<ApiTag>> GetListApiTag(ListApiTagRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            request.SortField = string.IsNullOrWhiteSpace(request.SortField) ? "Id" : request.SortField;
            request.SortOrder = string.IsNullOrWhiteSpace(request.SortOrder) ? "asc" : request.SortOrder;

            var sql = $"SELECT * FROM ApiTags ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var apiTags = await _dbConnection.QueryAsync<ApiTag>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });
                return apiTags.ToList();
            }
            catch
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
