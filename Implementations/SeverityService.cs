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
    public class SeverityService : ISeverityService
    {
        private readonly IDbConnection _dbConnection;

        public SeverityService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateSeverity(CreateSeverityDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var severity = new Severity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.UtcNow
            };

            const string sql = "INSERT INTO Severities (Id, Name, Version, Created) VALUES (@Id, @Name, @Version, @Created)";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(sql, severity, transaction);
                    transaction.Commit();
                    return severity.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<Severity> GetSeverity(SeverityRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM Severities WHERE Id = @Id";
            var severity = await _dbConnection.QuerySingleOrDefaultAsync<Severity>(sql, new { Id = request.Id });

            if (severity == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return severity;
        }

        public async Task<string> UpdateSeverity(UpdateSeverityDto request)
        {
            if (request.Id == Guid.Empty || string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM Severities WHERE Id = @Id";
            var severity = await _dbConnection.QuerySingleOrDefaultAsync<Severity>(selectSql, new { Id = request.Id });

            if (severity == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            severity.Name = request.Name;
            severity.Version += 1;
            severity.Changed = DateTime.UtcNow;

            const string updateSql = "UPDATE Severities SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(updateSql, severity, transaction);
                    transaction.Commit();
                    return severity.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteSeverity(DeleteSeverityDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM Severities WHERE Id = @Id";
            var severity = await _dbConnection.QuerySingleOrDefaultAsync<Severity>(selectSql, new { Id = request.Id });

            if (severity == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM Severities WHERE Id = @Id";
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

        public async Task<List<Severity>> GetListSeverity(ListSeverityRequestDto request)
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

            var sql = $"SELECT * FROM Severities ORDER BY {request.SortField} {request.SortOrder} OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";
            var severities = await _dbConnection.QueryAsync<Severity>(sql);

            return severities.ToList();
        }
    }
}
