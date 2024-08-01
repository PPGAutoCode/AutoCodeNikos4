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
            if (string.IsNullOrEmpty(request.Question) || string.IsNullOrEmpty(request.Answer) ||
                string.IsNullOrEmpty(request.Langcode) || request.Status == null || request.FaqOrder == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Fetch FAQ Category Details
            foreach (var categoryId in request.FAQCategories)
            {
                var categoryRequest = new FAQCategoryRequestDto { Id = categoryId };
                var category = await _faqCategoryService.GetFAQCategory(categoryRequest);
                if (category == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Create FAQ Object
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

            // Create FAQFAQCategories List
            var fAQFAQCategories = request.FAQCategories.Select(categoryId => new FAQFAQCategory
            {
                Id = Guid.NewGuid(),
                FAQId = faq.Id,
                FAQCategoryId = categoryId
            }).ToList();

            // Database Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("INSERT INTO FAQs (Id, Question, Answer, Langcode, Status, FaqOrder, Created, Changed) VALUES (@Id, @Question, @Answer, @Langcode, @Status, @FaqOrder, @Created, @Changed)", faq, transaction);
                    await _dbConnection.ExecuteAsync("INSERT INTO FAQFAQCategories (Id, FAQId, FAQCategoryId) VALUES (@Id, @FAQId, @FAQCategoryId)", fAQFAQCategories, transaction);
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

            var faq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>("SELECT * FROM FAQs WHERE Id = @Id", new { Id = request.Id });
            if (faq == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            var categoryIds = await _dbConnection.QueryAsync<Guid>("SELECT FAQCategoryId FROM FAQFAQCategories WHERE FAQId = @FAQId", new { FAQId = faq.Id });
            foreach (var categoryId in categoryIds)
            {
                var categoryRequest = new FAQCategoryRequestDto { Id = categoryId };
                var category = await _faqCategoryService.GetFAQCategory(categoryRequest);
                if (category == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            return faq;
        }

        public async Task<string> UpdateFAQ(UpdateFAQDto request)
        {
            // Validation Logic
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Question) || string.IsNullOrEmpty(request.Answer) ||
                string.IsNullOrEmpty(request.Langcode) || request.Status == null || request.FaqOrder == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingFAQ = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>("SELECT * FROM FAQs WHERE Id = @Id", new { Id = request.Id });
            if (existingFAQ == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Fetch and Validate FAQ Categories
            var existingCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT FAQCategoryId FROM FAQFAQCategories WHERE FAQId = @FAQId", new { FAQId = request.Id });
            var categoriesToRemove = existingCategoryIds.Except(request.FAQCategories).ToList();
            var categoriesToAdd = request.FAQCategories.Except(existingCategoryIds).ToList();

            foreach (var categoryId in categoriesToAdd)
            {
                var categoryRequest = new FAQCategoryRequestDto { Id = categoryId };
                var category = await _faqCategoryService.GetFAQCategory(categoryRequest);
                if (category == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Update FAQ Object
            existingFAQ.Question = request.Question;
            existingFAQ.Answer = request.Answer;
            existingFAQ.Langcode = request.Langcode;
            existingFAQ.Status = request.Status.Value;
            existingFAQ.FaqOrder = request.FaqOrder.Value;
            existingFAQ.Changed = DateTime.UtcNow;

            // Database Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("UPDATE FAQs SET Question = @Question, Answer = @Answer, Langcode = @Langcode, Status = @Status, FaqOrder = @FaqOrder, Changed = @Changed WHERE Id = @Id", existingFAQ, transaction);
                    await _dbConnection.ExecuteAsync("DELETE FROM FAQFAQCategories WHERE FAQId = @FAQId AND FAQCategoryId IN @CategoriesToRemove", new { FAQId = request.Id, CategoriesToRemove = categoriesToRemove }, transaction);
                    await _dbConnection.ExecuteAsync("INSERT INTO FAQFAQCategories (Id, FAQId, FAQCategoryId) VALUES (@Id, @FAQId, @FAQCategoryId)", categoriesToAdd.Select(categoryId => new { Id = Guid.NewGuid(), FAQId = request.Id, FAQCategoryId = categoryId }), transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            return existingFAQ.Id.ToString();
        }

        public async Task<bool> DeleteFAQ(DeleteFAQDto request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingFAQ = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>("SELECT * FROM FAQs WHERE Id = @Id", new { Id = request.Id });
            if (existingFAQ == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("DELETE FROM FAQs WHERE Id = @Id", new { Id = request.Id }, transaction);
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
                throw new TechnicalException("DP-422", "Client Error");
            }

            var query = "SELECT * FROM FAQs";
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                query += $" ORDER BY {request.SortField} {request.SortOrder}";
            }
            query += " OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            var faqs = await _dbConnection.QueryAsync<FAQ>(query, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });
            if (faqs == null || !faqs.Any())
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            var faqIds = faqs.Select(f => f.Id).ToList();
            var categoryIds = await _dbConnection.QueryAsync<Guid>("SELECT FAQCategoryId FROM FAQFAQCategories WHERE FAQId IN @FAQIds", new { FAQIds = faqIds });
            foreach (var categoryId in categoryIds)
            {
                var categoryRequest = new FAQCategoryRequestDto { Id = categoryId };
                var category = await _faqCategoryService.GetFAQCategory(categoryRequest);
                if (category == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            return faqs.ToList();
        }
    }
}
