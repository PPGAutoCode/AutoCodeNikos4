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
    public class AppEnvironmentService : IAppEnvironmentService
    {
        private readonly IDbConnection _dbConnection;

        public AppEnvironmentService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAppEnvironment(CreateAppEnvironmentDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var appEnvironment = new AppEnvironment
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = Guid.NewGuid(), // Assuming a creator ID is generated or fetched
                ChangedUser = Guid.NewGuid() // Assuming a changed user ID is generated or fetched
            };

            const string sql = @"INSERT INTO AppEnvironments (Id, Name, Version, Created, Changed, CreatorId, ChangedUser) 
                                 VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, appEnvironment, transaction);
                    transaction.Commit();
                    return appEnvironment.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<AppEnvironment> GetAppEnvironment(AppEnvironmentRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM AppEnvironments WHERE Id = @Id";
            var result = await _dbConnection.QuerySingleOrDefaultAsync<AppEnvironment>(sql, new { request.Id });

            if (result == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return result;
        }

        public async Task<string> UpdateAppEnvironment(UpdateAppEnvironmentDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM AppEnvironments WHERE Id = @Id";
            var appEnvironment = await _dbConnection.QuerySingleOrDefaultAsync<AppEnvironment>(selectSql, new { request.Id });

            if (appEnvironment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            appEnvironment.Name = request.Name;
            appEnvironment.Version += 1;
            appEnvironment.Changed = DateTime.UtcNow;

            const string updateSql = @"UPDATE AppEnvironments 
                                       SET Name = @Name, Version = @Version, Changed = @Changed 
                                       WHERE Id = @Id";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, appEnvironment, transaction);
                    transaction.Commit();
                    return appEnvironment.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteAppEnvironment(DeleteAppEnvironmentDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM AppEnvironments WHERE Id = @Id";
            var appEnvironment = await _dbConnection.QuerySingleOrDefaultAsync<AppEnvironment>(selectSql, new { request.Id });

            if (appEnvironment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM AppEnvironments WHERE Id = @Id";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(deleteSql, new { request.Id }, transaction);
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

        public async Task<List<AppEnvironment>> GetListAppEnvironment(ListAppEnvironmentRequestDto request)
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

            var sql = $"SELECT * FROM AppEnvironments ORDER BY {request.SortField} {request.SortOrder} " +
                      $"OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";

            try
            {
                var results = await _dbConnection.QueryAsync<AppEnvironment>(sql);
                return results.ToList();
            }
            catch
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
