using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class AppStatusService : IAppStatusService
    {
        private readonly IDbConnection _dbConnection;

        public AppStatusService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAppStatus(CreateAppStatusDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var appStatus = new AppStatus
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            const string sql = @"INSERT INTO AppStatuses (Id, Name, Version, Created, Changed) VALUES (@Id, @Name, @Version, @Created, @Changed)";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, appStatus, transaction);
                    transaction.Commit();
                    return appStatus.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<AppStatus> GetAppStatus(AppStatusRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"SELECT * FROM AppStatuses WHERE Id = @Id";

            var result = await _dbConnection.QuerySingleOrDefaultAsync<AppStatus>(sql, new { request.Id });

            if (result == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return result;
        }

        public async Task<string> UpdateAppStatus(UpdateAppStatusDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"SELECT * FROM AppStatuses WHERE Id = @Id";
            var appStatus = await _dbConnection.QuerySingleOrDefaultAsync<AppStatus>(selectSql, new { request.Id });

            if (appStatus == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            appStatus.Name = request.Name;
            appStatus.Version += 1;
            appStatus.Changed = DateTime.UtcNow;

            const string updateSql = @"UPDATE AppStatuses SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, appStatus, transaction);
                    transaction.Commit();
                    return appStatus.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteAppStatus(DeleteAppStatusDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"SELECT * FROM AppStatuses WHERE Id = @Id";
            var appStatus = await _dbConnection.QuerySingleOrDefaultAsync<AppStatus>(selectSql, new { request.Id });

            if (appStatus == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"DELETE FROM AppStatuses WHERE Id = @Id";

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

        public async Task<List<AppStatus>> GetListAppStatus(ListAppStatusRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            request.SortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            request.SortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

            var sql = $"SELECT * FROM AppStatuses ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var results = await _dbConnection.QueryAsync<AppStatus>(sql, new { request.PageOffset, request.PageLimit });
                return results.AsList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
