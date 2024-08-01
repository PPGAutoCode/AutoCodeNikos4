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
    public class UserTypeService : IUserTypeService
    {
        private readonly IDbConnection _dbConnection;

        public UserTypeService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateUserType(CreateUserTypeDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var userType = new UserType
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                CreatorId = Guid.NewGuid(), // Assuming some default or logic to set this
                ChangedUser = Guid.NewGuid() // Assuming some default or logic to set this
            };

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sql = "INSERT INTO UserTypes (Id, Name, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @Name, @Version, @Created, @Changed, @CreatorId, @ChangedUser)";
                    await _dbConnection.ExecuteAsync(sql, userType, transaction);
                    transaction.Commit();
                    return userType.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<UserType> GetUserType(UserTypeRequestDto request)
        {
            if ((request.Id == Guid.Empty || request.Id == null) && string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            UserType userType = null;

            if (request.Id != Guid.Empty && request.Id != null && string.IsNullOrWhiteSpace(request.Name))
            {
                var sql = "SELECT * FROM UserTypes WHERE Id = @Id";
                userType = await _dbConnection.QuerySingleOrDefaultAsync<UserType>(sql, new { Id = request.Id });
            }
            else if (!string.IsNullOrWhiteSpace(request.Name) && request.Id == Guid.Empty)
            {
                var sql = "SELECT * FROM UserTypes WHERE Name = @Name";
                userType = await _dbConnection.QuerySingleOrDefaultAsync<UserType>(sql, new { Name = request.Name });
            }
            else
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            return userType;
        }

        public async Task<string> UpdateUserType(UpdateUserTypeDto request)
        {
            if (request.Id == Guid.Empty || request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sqlSelect = "SELECT * FROM UserTypes WHERE Id = @Id";
            var userType = await _dbConnection.QuerySingleOrDefaultAsync<UserType>(sqlSelect, new { Id = request.Id });

            if (userType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            userType.Name = request.Name;
            userType.Version += 1;
            userType.Changed = DateTime.Now;

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlUpdate = "UPDATE UserTypes SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sqlUpdate, userType, transaction);
                    transaction.Commit();
                    return userType.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteUserType(DeleteUserTypeDto request)
        {
            if (request.Id == Guid.Empty || request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sqlSelect = "SELECT * FROM UserTypes WHERE Id = @Id";
            var userType = await _dbConnection.QuerySingleOrDefaultAsync<UserType>(sqlSelect, new { Id = request.Id });

            if (userType == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlDelete = "DELETE FROM UserTypes WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sqlDelete, new { Id = request.Id }, transaction);
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

        public async Task<List<UserType>> GetListUserType(ListUserTypeRequestDto request)
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

            var sql = $"SELECT * FROM UserTypes ORDER BY {request.SortField} {request.SortOrder} OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";

            try
            {
                var userTypes = await _dbConnection.QueryAsync<UserType>(sql);
                return userTypes.ToList();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
