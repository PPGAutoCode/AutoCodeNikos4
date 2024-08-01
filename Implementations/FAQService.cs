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
    public class FAQService : IFAQService
    {
        private readonly IDbConnection _dbConnection;
        private readonly IFAQCategoryService _faqCategoryService;

        public FAQService(IDbConnection dbConnection, IFAQCategoryService faqCategoryService)
        {
            _dbConnection = dbConnection;
            _faqCategoryService = faqCategoryService;
        }

        public async Task<string> CreateFAQ(CreateFAQDto request)
        {
            // Validation Logic
            if (string.IsNullOrEmpty(request.Question) || string.IsNullOrEmpty(request.Answer) || string.IsNullOrEmpty(request.Langcode) || request.Status == null || request.FaqOrder == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var faq = new FAQ
            {
                Id = Guid.NewGuid(),
                Question = request.Question,
                Answer = request.Answer,
                Langcode = request.Langcode,
                Status = request.Status.Value,
                FaqOrder = request.FaqOrder.Value,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            var fAQFAQCategories = new List<FAQFAQCategory>();
            foreach (var categoryId in request.FAQCategories)
            {
                var categoryRequestDto = new FAQCategoryRequestDto { Id = categoryId };
                var category = await _faqCategoryService.GetFAQCategory(categoryRequestDto);
                if (category == null)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }
                fAQFAQCategories.Add(new FAQFAQCategory
                {
                    Id = Guid.NewGuid(),
                    FAQId = faq.Id,
                    FAQCategoryId = categoryId
                });
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var sql = "INSERT INTO FAQs (Id, Question, Answer, Langcode, Status, FaqOrder, Created, Changed) VALUES (@Id, @Question, @Answer, @Langcode, @Status, @FaqOrder, @Created, @Changed)";
                    await _dbConnection.ExecuteAsync(sql, faq, transaction);

                    var sqlCategories = "INSERT INTO FAQFAQCategories (Id, FAQId, FAQCategoryId) VALUES (@Id, @FAQId, @FAQCategoryId)";
                    await _dbConnection.ExecuteAsync(sqlCategories, fAQFAQCategories, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            return faq.Id.ToString();
        }

        public async Task<FAQ> GetFAQ(FAQRequestDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = "SELECT * FROM FAQs WHERE Id = @Id";
            var faq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>(sql, new { request.Id });

            if (faq == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            var categoryIds = await _dbConnection.QueryAsync<Guid>("SELECT FAQCategoryId FROM FAQFAQCategories WHERE FAQId = @FAQId", new { FAQId = faq.Id });

            var faqCategories = new List<FAQCategory>();
            foreach (var categoryId in categoryIds)
            {
                var categoryRequestDto = new FAQCategoryRequestDto { Id = categoryId };
                var category = await _faqCategoryService.GetFAQCategory(categoryRequestDto);
                if (category == null)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }
                faqCategories.Add(category);
            }

            faq.FAQCategories = faqCategories.Select(c => c.Id).ToList();
            return faq;
        }

        public async Task<string> UpdateFAQ(UpdateFAQDto request)
        {
            // Validation Logic
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Question) || string.IsNullOrEmpty(request.Answer) || string.IsNullOrEmpty(request.Langcode) || request.Status == null || request.FaqOrder == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingFaq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>("SELECT * FROM FAQs WHERE Id = @Id", new { request.Id });
            if (existingFaq == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            existingFaq.Question = request.Question;
            existingFaq.Answer = request.Answer;
            existingFaq.Langcode = request.Langcode;
            existingFaq.Status = request.Status.Value;
            existingFaq.FaqOrder = request.FaqOrder.Value;
            existingFaq.Changed = DateTime.UtcNow;

            var existingCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT FAQCategoryId FROM FAQFAQCategories WHERE FAQId = @FAQId", new { FAQId = existingFaq.Id });
            var categoriesToRemove = existingCategoryIds.Except(request.FAQCategories).ToList();
            var categoriesToAdd = request.FAQCategories.Except(existingCategoryIds).ToList();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("UPDATE FAQs SET Question = @Question, Answer = @Answer, Langcode = @Langcode, Status = @Status, FaqOrder = @FaqOrder, Changed = @Changed WHERE Id = @Id", existingFaq, transaction);

                    if (categoriesToRemove.Any())
                    {
                        await _dbConnection.ExecuteAsync("DELETE FROM FAQFAQCategories WHERE FAQId = @FAQId AND FAQCategoryId IN @CategoriesToRemove", new { FAQId = existingFaq.Id, CategoriesToRemove = categoriesToRemove }, transaction);
                    }

                    foreach (var categoryId in categoriesToAdd)
                    {
                        var categoryRequestDto = new FAQCategoryRequestDto { Id = categoryId };
                        var category = await _faqCategoryService.GetFAQCategory(categoryRequestDto);
                        if (category == null)
                        {
                            throw new BusinessException("DP-404", "Technical Error");
                        }
                        await _dbConnection.ExecuteAsync("INSERT INTO FAQFAQCategories (Id, FAQId, FAQCategoryId) VALUES (@Id, @FAQId, @FAQCategoryId)", new { Id = Guid.NewGuid(), FAQId = existingFaq.Id, FAQCategoryId = categoryId }, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            return existingFaq.Id.ToString();
        }

        public async Task<bool> DeleteFAQ(DeleteFAQDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingFaq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>("SELECT * FROM FAQs WHERE Id = @Id", new { request.Id });
            if (existingFaq == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("DELETE FROM FAQFAQCategories WHERE FAQId = @FAQId", new { FAQId = existingFaq.Id }, transaction);
                    await _dbConnection.ExecuteAsync("DELETE FROM FAQs WHERE Id = @Id", new { existingFaq.Id }, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            return true;
        }

        public async Task<List<FAQ>> GetListFAQ(ListFAQRequestDto request)
        {
            if (request.PageLimit == 0 || request.PageOffset == 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = "SELECT * FROM FAQs";
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                sql += $" ORDER BY {request.SortField} {request.SortOrder}";
            }
            sql += " OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            var faqs = await _dbConnection.QueryAsync<FAQ>(sql, new { request.PageOffset, request.PageLimit });

            if (!faqs.Any())
            {
                throw new BusinessException("DP-400", "Technical Error");
            }

            var faqIds = faqs.Select(f => f.Id).ToList();
            var categoryIds = await _dbConnection.QueryAsync<Guid>("SELECT FAQCategoryId FROM FAQFAQCategories WHERE FAQId IN @FAQIds", new { FAQIds = faqIds });

            var faqCategories = new List<FAQCategory>();
            foreach (var categoryId in categoryIds)
            {
                var categoryRequestDto = new FAQCategoryRequestDto { Id = categoryId };
                var category = await _faqCategoryService.GetFAQCategory(categoryRequestDto);
                if (category == null)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }
                faqCategories.Add(category);
            }

            foreach (var faq in faqs)
            {
                faq.FAQCategories = faqCategories.Where(c => c.Id == faq.Id).Select(c => c.Id).ToList();
            }

            return faqs.ToList();
        }
    }
}
