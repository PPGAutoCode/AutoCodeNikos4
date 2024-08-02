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
    public class GoLiveDeveloperTypeService : IGoLiveDeveloperTypeService
    {
        private readonly IDbConnection _dbConnection;

        public GoLiveDeveloperTypeService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateGoLiveDeveloperType(CreateGoLiveDeveloperTypeDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var newGoLiveDeveloperType = new GoLiveDeveloperType
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                CreatorId = Guid.NewGuid(), // Assuming a creator ID is generated or fetched
                ChangedUser = Guid.NewGuid() // Assuming a changed user ID is generated or fetched
            };

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sql = "INSERT INTO GoLiveDeveloperTypes (Id, Name, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";
                    await _dbConnection.ExecuteAsync(sql, newGoLiveDeveloperType, transaction);
                    transaction.Commit();
                    return newGoLiveDeveloperType.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<GoLiveDeveloperType> GetGoLiveDeveloperType(GoLiveDeveloperTypeRequestDto request)
        {
            if ((request.Id == Guid.Empty || request.Id == null) && string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            GoLiveDeveloperType goLiveDeveloperType = null;

            if (request.Id != Guid.Empty && request.Id != null && string.IsNullOrWhiteSpace(request.Name))
            {
                var sql = "SELECT * FROM GoLiveDeveloperTypes WHERE Id = @Id";
                goLiveDeveloperType = await _dbConnection.QuerySingleOrDefaultAsync<GoLiveDeveloperType>(sql, new { Id = request.Id });
            }
            else if (!string.IsNullOrWhiteSpace(request.Name) && request.Id == Guid.Empty)
            {
                var sql = "SELECT * FROM GoLiveDeveloperTypes WHERE Name = @Name";
                goLiveDeveloperType = await _dbConnection.QuerySingleOrDefaultAsync<GoLiveDeveloperType>(sql, new { Name = request.Name });
            }
            else
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            return goLiveDeveloperType;
        }

        public async Task<string> UpdateGoLiveDeveloperType(UpdateGoLiveDeveloperTypeDto request)
        {
            if (request.Id == Guid.Empty || request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingGoLiveDeveloperType = await GetGoLiveDeveloperType(new GoLiveDeveloperTypeRequestDto { Id = request.Id });
            if (existingGoLiveDeveloperType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingGoLiveDeveloperType.Name = request.Name;
            existingGoLiveDeveloperType.Version += 1;
            existingGoLiveDeveloperType.Changed = DateTime.Now;

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sql = "UPDATE GoLiveDeveloperTypes SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sql, existingGoLiveDeveloperType, transaction);
                    transaction.Commit();
                    return existingGoLiveDeveloperType.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteGoLiveDeveloperType(DeleteGoLiveDeveloperTypeDto request)
        {
            if (request.Id == Guid.Empty || request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingGoLiveDeveloperType = await GetGoLiveDeveloperType(new GoLiveDeveloperTypeRequestDto { Id = request.Id });
            if (existingGoLiveDeveloperType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sql = "DELETE FROM GoLiveDeveloperTypes WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sql, new { Id = request.Id }, transaction);
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

        public async Task<List<GoLiveDeveloperType>> GetListGoLiveDeveloperType(ListGoLiveDeveloperTypeRequestDto request)
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

            var sql = $"SELECT * FROM GoLiveDeveloperTypes ORDER BY {request.SortField} {request.SortOrder} OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";

            try
            {
                var goLiveDeveloperTypes = await _dbConnection.QueryAsync<GoLiveDeveloperType>(sql);
                return goLiveDeveloperTypes.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}