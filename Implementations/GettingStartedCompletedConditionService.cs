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
    public class GettingStartedCompletedConditionService : IGettingStartedCompletedConditionService
    {
        private readonly IDbConnection _dbConnection;

        public GettingStartedCompletedConditionService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateGettingStartedCompletedCondition(CreateGettingStartedCompletedConditionDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var newCondition = new GettingStartedCompletedCondition
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                CreatorId = Guid.NewGuid(), // Assuming a method to get the creator ID
                ChangedUser = Guid.NewGuid() // Assuming a method to get the changed user ID
            };

            const string sql = @"
                INSERT INTO GettingStartedCompletedConditions (Id, Name, Version, Created, Changed, CreatorId, ChangedUser)
                VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, newCondition, transaction);
                    transaction.Commit();
                    return newCondition.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<GettingStartedCompletedCondition> GetGettingStartedCompletedCondition(GettingStartedCompletedConditionRequestDto request)
        {
            if (request.Id == Guid.Empty && string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            GettingStartedCompletedCondition condition = null;

            if (request.Id != Guid.Empty && string.IsNullOrWhiteSpace(request.Name))
            {
                const string sql = "SELECT * FROM GettingStartedCompletedConditions WHERE Id = @Id";
                condition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(sql, new { Id = request.Id });
            }
            else if (!string.IsNullOrWhiteSpace(request.Name) && request.Id == Guid.Empty)
            {
                const string sql = "SELECT * FROM GettingStartedCompletedConditions WHERE Name = @Name";
                condition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(sql, new { Name = request.Name });
            }
            else
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            return condition;
        }

        public async Task<string> UpdateGettingStartedCompletedCondition(UpdateGettingStartedCompletedConditionDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM GettingStartedCompletedConditions WHERE Id = @Id";
            var existingCondition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(selectSql, new { Id = request.Id });

            if (existingCondition == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingCondition.Name = request.Name;
            existingCondition.Version += 1;
            existingCondition.Changed = DateTime.Now;

            const string updateSql = @"
                UPDATE GettingStartedCompletedConditions
                SET Name = @Name, Version = @Version, Changed = @Changed
                WHERE Id = @Id";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, existingCondition, transaction);
                    transaction.Commit();
                    return existingCondition.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteGettingStartedCompletedCondition(DeleteGettingStartedCompletedConditionDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM GettingStartedCompletedConditions WHERE Id = @Id";
            var existingCondition = await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>(selectSql, new { Id = request.Id });

            if (existingCondition == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM GettingStartedCompletedConditions WHERE Id = @Id";

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

        public async Task<List<GettingStartedCompletedCondition>> GetListGettingStartedCompletedCondition(ListGettingStartedCompletedConditionRequestDto request)
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

            var sql = $"SELECT * FROM GettingStartedCompletedConditions ORDER BY {request.SortField} {request.SortOrder} OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";

            try
            {
                var conditions = await _dbConnection.QueryAsync<GettingStartedCompletedCondition>(sql);
                return conditions.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}