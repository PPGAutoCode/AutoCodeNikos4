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
    public class SupportTicketStateService : ISupportTicketStateService
    {
        private readonly IDbConnection _dbConnection;

        public SupportTicketStateService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSupportTicketState(CreateSupportTicketStateDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var supportTicketState = new SupportTicketState
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow
            };

            const string sql = @"INSERT INTO SupportTicketStates (Id, Name, Version, Created) VALUES (@Id, @Name, @Version, @Created)";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, supportTicketState, transaction);
                    transaction.Commit();
                    return supportTicketState.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<SupportTicketState> GetSupportTicketState(SupportTicketStateRequestDto request)
        {
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = @"SELECT * FROM SupportTicketStates WHERE Id = @Id";

            var result = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketState>(sql, new { Id = request.Id });

            if (result == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return result;
        }

        public async Task<string> UpdateSupportTicketState(UpdateSupportTicketStateDto request)
        {
            if (request.Id == null || string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"SELECT * FROM SupportTicketStates WHERE Id = @Id";
            var supportTicketState = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketState>(selectSql, new { Id = request.Id });

            if (supportTicketState == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            supportTicketState.Name = request.Name;
            supportTicketState.Version += 1;
            supportTicketState.Changed = DateTime.UtcNow;

            const string updateSql = @"UPDATE SupportTicketStates SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, supportTicketState, transaction);
                    transaction.Commit();
                    return supportTicketState.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteSupportTicketState(DeleteSupportTicketStateDto request)
        {
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = @"SELECT * FROM SupportTicketStates WHERE Id = @Id";
            var supportTicketState = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketState>(selectSql, new { Id = request.Id });

            if (supportTicketState == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = @"DELETE FROM SupportTicketStates WHERE Id = @Id";

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

        public async Task<List<SupportTicketState>> GetListSupportTicketState(ListSupportTicketStateRequestDto request)
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

            var sql = $"SELECT * FROM SupportTicketStates ORDER BY {request.SortField} {request.SortOrder} OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";

            try
            {
                var result = await _dbConnection.QueryAsync<SupportTicketState>(sql);
                return result.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
