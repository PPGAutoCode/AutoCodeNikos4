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
    public class SupportTicketPriorityService : ISupportTicketPriorityService
    {
        private readonly IDbConnection _dbConnection;

        public SupportTicketPriorityService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSupportTicketPriority(CreateSupportTicketPriorityDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var supportTicketPriority = new SupportTicketPriority
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                CreatorId = Guid.NewGuid(), // Assuming there's a way to get the creator ID
                ChangedUser = Guid.NewGuid() // Assuming there's a way to get the changer ID
            };

            const string sql = @"INSERT INTO SupportTicketPriorities (Id, Name, Version, Created, Changed, CreatorId, ChangedUser) 
                                 VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, supportTicketPriority, transaction);
                    transaction.Commit();
                    return supportTicketPriority.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<SupportTicketPriority> GetSupportTicketPriority(SupportTicketPriorityRequestDto request)
        {
            if (request.Id == Guid.Empty && string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            SupportTicketPriority supportTicketPriority = null;

            if (request.Id != Guid.Empty && string.IsNullOrWhiteSpace(request.Name))
            {
                const string sql = "SELECT * FROM SupportTicketPriorities WHERE Id = @Id";
                supportTicketPriority = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketPriority>(sql, new { Id = request.Id });
            }
            else if (!string.IsNullOrWhiteSpace(request.Name) && request.Id == Guid.Empty)
            {
                const string sql = "SELECT * FROM SupportTicketPriorities WHERE Name = @Name";
                supportTicketPriority = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketPriority>(sql, new { Name = request.Name });
            }
            else
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            return supportTicketPriority;
        }

        public async Task<string> UpdateSupportTicketPriority(UpdateSupportTicketPriorityDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM SupportTicketPriorities WHERE Id = @Id";
            var existingSupportTicketPriority = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketPriority>(selectSql, new { Id = request.Id });

            if (existingSupportTicketPriority == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingSupportTicketPriority.Name = request.Name;
            existingSupportTicketPriority.Version += 1;
            existingSupportTicketPriority.Changed = DateTime.Now;

            const string updateSql = @"UPDATE SupportTicketPriorities 
                                       SET Name = @Name, Version = @Version, Changed = @Changed 
                                       WHERE Id = @Id";

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, existingSupportTicketPriority, transaction);
                    transaction.Commit();
                    return existingSupportTicketPriority.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteSupportTicketPriority(DeleteSupportTicketPriorityDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM SupportTicketPriorities WHERE Id = @Id";
            var existingSupportTicketPriority = await _dbConnection.QuerySingleOrDefaultAsync<SupportTicketPriority>(selectSql, new { Id = request.Id });

            if (existingSupportTicketPriority == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM SupportTicketPriorities WHERE Id = @Id";

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

        public async Task<List<SupportTicketPriority>> GetListSupportTicketPriority(ListSupportTicketPriorityRequestDto request)
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

            var sql = $"SELECT * FROM SupportTicketPriorities ORDER BY {request.SortField} {request.SortOrder} OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";

            try
            {
                var supportTicketPriorities = await _dbConnection.QueryAsync<SupportTicketPriority>(sql);
                return supportTicketPriorities.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
