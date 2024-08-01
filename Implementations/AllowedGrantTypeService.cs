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
    public class AllowedGrantTypeService : IAllowedGrantTypeService
    {
        private readonly IDbConnection _dbConnection;

        public AllowedGrantTypeService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAllowedGrantType(CreateAllowedGrantTypeDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var allowedGrantType = new AllowedGrantType
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            const string sql = @"INSERT INTO AllowedGrantTypes (Id, Name, Version, Created, Changed) VALUES (@Id, @Name, @Version, @Created, @Changed)";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, allowedGrantType, transaction);
                    transaction.Commit();
                    return allowedGrantType.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<AllowedGrantType> GetAllowedGrantType(AllowedGrantTypeRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"SELECT * FROM AllowedGrantTypes WHERE Id = @Id";

            var result = await _dbConnection.QuerySingleOrDefaultAsync<AllowedGrantType>(sql, new { request.Id });

            if (result == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return result;
        }

        public async Task<string> UpdateAllowedGrantType(UpdateAllowedGrantTypeDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"SELECT * FROM AllowedGrantTypes WHERE Id = @Id";
            var existingGrantType = await _dbConnection.QuerySingleOrDefaultAsync<AllowedGrantType>(selectSql, new { request.Id });

            if (existingGrantType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingGrantType.Name = request.Name;
            existingGrantType.Version += 1;
            existingGrantType.Changed = DateTime.UtcNow;

            const string updateSql = @"UPDATE AllowedGrantTypes SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, existingGrantType, transaction);
                    transaction.Commit();
                    return existingGrantType.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteAllowedGrantType(DeleteAllowedGrantTypeDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"SELECT * FROM AllowedGrantTypes WHERE Id = @Id";
            var existingGrantType = await _dbConnection.QuerySingleOrDefaultAsync<AllowedGrantType>(selectSql, new { request.Id });

            if (existingGrantType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"DELETE FROM AllowedGrantTypes WHERE Id = @Id";

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

        public async Task<List<AllowedGrantType>> GetListAllowedGrantType(ListAllowedGrantTypeRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            var sortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

            var sql = $@"SELECT * FROM AllowedGrantTypes ORDER BY {sortField} {sortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            try
            {
                var result = await _dbConnection.QueryAsync<AllowedGrantType>(sql, new { request.PageOffset, request.PageLimit });
                return result.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
