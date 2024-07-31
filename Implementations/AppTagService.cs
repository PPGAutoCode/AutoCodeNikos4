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
    public class AppTagService : IAppTagService
    {
        private readonly IDbConnection _dbConnection;

        public AppTagService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAppTag(CreateAppTagDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var appTag = new AppTag
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.Now
            };

            const string sql = "INSERT INTO AppTags (Id, Name, Version, Created) VALUES (@Id, @Name, @Version, @Created)";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, appTag, transaction);
                    transaction.Commit();
                    return appTag.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<AppTag> GetAppTag(AppTagRequestDto request)
        {
            if ((request.Id == null || request.Id == Guid.Empty) && string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            AppTag appTag = null;

            if (request.Id != null && request.Id != Guid.Empty && string.IsNullOrWhiteSpace(request.Name))
            {
                const string sql = "SELECT * FROM AppTags WHERE Id = @Id";
                appTag = await _dbConnection.QuerySingleOrDefaultAsync<AppTag>(sql, new { Id = request.Id });
            }
            else if (!string.IsNullOrWhiteSpace(request.Name) && (request.Id == null || request.Id == Guid.Empty))
            {
                const string sql = "SELECT * FROM AppTags WHERE Name = @Name";
                appTag = await _dbConnection.QuerySingleOrDefaultAsync<AppTag>(sql, new { Name = request.Name });
            }
            else
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            return appTag;
        }

        public async Task<string> UpdateAppTag(UpdateAppTagDto request)
        {
            if (request.Id == null || request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM AppTags WHERE Id = @Id";
            var appTag = await _dbConnection.QuerySingleOrDefaultAsync<AppTag>(selectSql, new { Id = request.Id });

            if (appTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            appTag.Name = request.Name ?? appTag.Name;
            appTag.Version += 1;
            appTag.Changed = DateTime.Now;

            const string updateSql = "UPDATE AppTags SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, appTag, transaction);
                    transaction.Commit();
                    return appTag.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteAppTag(DeleteAppTagDto request)
        {
            if (request.Id == null || request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM AppTags WHERE Id = @Id";
            var appTag = await _dbConnection.QuerySingleOrDefaultAsync<AppTag>(selectSql, new { Id = request.Id });

            if (appTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM AppTags WHERE Id = @Id";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(deleteSql, new { Id = request.Id }, transaction);
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

        public async Task<List<AppTag>> GetListAppTag(ListAppTagRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            request.SortField = string.IsNullOrWhiteSpace(request.SortField) ? "Id" : request.SortField;
            request.SortOrder = string.IsNullOrWhiteSpace(request.SortOrder) ? "asc" : request.SortOrder;

            var sql = $"SELECT * FROM AppTags ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var appTags = await _dbConnection.QueryAsync<AppTag>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });

            return appTags.ToList();
        }
    }
}
