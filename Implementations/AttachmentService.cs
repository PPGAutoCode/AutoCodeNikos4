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
    public class AttachmentService : IAttachmentService
    {
        private readonly IDbConnection _dbConnection;

        public AttachmentService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAttachment(CreateAttachmentDto request)
        {
            if (string.IsNullOrEmpty(request.FileData))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = request.FileName,
                FileData = request.FileData,
                Version = 1,
                Created = DateTime.Now
            };

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sql = "INSERT INTO Attachments (Id, FileName, FileData, Version, Created) VALUES (@Id, @FileName, @FileData, @Version, @Created)";
                    await _dbConnection.ExecuteAsync(sql, attachment, transaction);
                    transaction.Commit();
                    return attachment.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<Attachment> GetAttachment(AttachmentRequestDto request)
        {
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = "SELECT * FROM Attachments WHERE Id = @Id";
            var attachment = await _dbConnection.QuerySingleOrDefaultAsync<Attachment>(sql, new { Id = request.Id });

            if (attachment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return attachment;
        }

        public async Task<string> UpdateAttachment(UpdateAttachmentDto request)
        {
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sqlSelect = "SELECT * FROM Attachments WHERE Id = @Id";
            var attachment = await _dbConnection.QuerySingleOrDefaultAsync<Attachment>(sqlSelect, new { Id = request.Id });

            if (attachment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            if (!string.IsNullOrEmpty(request.FileName))
            {
                attachment.FileName = request.FileName;
            }

            if (!string.IsNullOrEmpty(request.FileData))
            {
                attachment.FileData = request.FileData;
            }

            attachment.Version += 1;
            attachment.Changed = DateTime.Now;

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlUpdate = "UPDATE Attachments SET FileName = @FileName, FileData = @FileData, Version = @Version, Changed = @Changed WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sqlUpdate, attachment, transaction);
                    transaction.Commit();
                    return attachment.Id.ToString();
                }
                catch
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteAttachment(DeleteAttachmentDto request)
        {
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sqlSelect = "SELECT * FROM Attachments WHERE Id = @Id";
            var attachment = await _dbConnection.QuerySingleOrDefaultAsync<Attachment>(sqlSelect, new { Id = request.Id });

            if (attachment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sqlDelete = "DELETE FROM Attachments WHERE Id = @Id";
                    await _dbConnection.ExecuteAsync(sqlDelete, new { Id = request.Id }, transaction);
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

        public async Task<List<Attachment>> GetListAttachment(ListAttachmentRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            request.SortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            request.SortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

            var sql = $"SELECT * FROM Attachments ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
            var attachments = await _dbConnection.QueryAsync<Attachment>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });

            return attachments.ToList();
        }

        public async Task<string> UpsertAttachment(UpdateAttachmentDto request)
        {
            if (request.Id == null)
            {
                var createAttachmentDto = new CreateAttachmentDto
                {
                    FileData = request.FileData,
                    FileName = request.FileName
                };
                return await CreateAttachment(createAttachmentDto);
            }
            else if (!string.IsNullOrEmpty(request.FileName) || !string.IsNullOrEmpty(request.FileData))
            {
                return await UpdateAttachment(request);
            }
            else
            {
                await DeleteAttachment(new DeleteAttachmentDto { Id = request.Id });
                return request.Id.ToString();
            }
        }
    }
}
