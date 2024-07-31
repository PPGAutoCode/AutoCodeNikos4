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
    public class ContactService : IContactService
    {
        private readonly IDbConnection _dbConnection;

        public ContactService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateContact(CreateContactDto request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Mail) || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Message))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Mail = request.Mail,
                Subject = request.Subject,
                Message = request.Message,
                Created = DateTime.UtcNow
            };

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sql = "INSERT INTO Contacts (Id, Name, Mail, Subject, Message, Created) VALUES (@Id, @Name, @Mail, @Subject, @Message, @Created)";
                    await _dbConnection.ExecuteAsync(sql, contact, transaction);
                    transaction.Commit();
                    return contact.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<Contact> GetContact(ContactRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = "SELECT * FROM Contacts WHERE Id = @Id";
            var contact = await _dbConnection.QuerySingleOrDefaultAsync<Contact>(sql, new { Id = request.Id });

            if (contact == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return contact;
        }

        public async Task<string> UpdateContact(UpdateContactDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sqlSelect = "SELECT * FROM Contacts WHERE Id = @Id";
            var contact = await _dbConnection.QuerySingleOrDefaultAsync<Contact>(sqlSelect, new { Id = request.Id });

            if (contact == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            contact.Name = request.Name ?? contact.Name;
            contact.Mail = request.Mail ?? contact.Mail;
            contact.Subject = request.Subject ?? contact.Subject;
            contact.Message = request.Message ?? contact.Message;
            contact.Changed = DateTime.UtcNow;

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlUpdate = "UPDATE Contacts SET Name = @Name, Mail = @Mail, Subject = @Subject, Message = @Message, Changed = @Changed WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sqlUpdate, contact, transaction);
                    transaction.Commit();
                    return contact.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteContact(DeleteContactDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sqlSelect = "SELECT * FROM Contacts WHERE Id = @Id";
            var contact = await _dbConnection.QuerySingleOrDefaultAsync<Contact>(sqlSelect, new { Id = request.Id });

            if (contact == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlDelete = "DELETE FROM Contacts WHERE Id = @Id";
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

        public async Task<List<Contact>> GetListContact(ListContactRequestDto request)
        {
            if (request.PageLimit <= 0 && request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            request.SortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            request.SortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

            var sql = $"SELECT * FROM Contacts ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var contacts = await _dbConnection.QueryAsync<Contact>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });

            if (contacts == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return contacts.ToList();
        }
    }
}