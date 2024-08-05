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
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.Question) || string.IsNullOrEmpty(request.Answer) || string.IsNullOrEmpty(request.Langcode))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Validate FAQCategories existence
            if (request.FAQCategories != null)
            {
                foreach (var categoryId in request.FAQCategories)
                {
                    var categoryRequestDto = new FAQCategoryRequestDto { Id = categoryId };
                    var category = await _faqCategoryService.GetFAQCategory(categoryRequestDto);
                    if (category == null)
                    {
                        throw new BusinessException("DP-404", "Technical Error");
                    }
                }
            }

            // Step 3: Create a new FAQ object
            var faq = new FAQ
            {
                Id = Guid.NewGuid(),
                Question = request.Question,
                Answer = request.Answer,
                Langcode = request.Langcode,
                Status = request.Status,
                FaqOrder = request.FaqOrder,
                Version = 1,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                CreatorId = Guid.NewGuid(), // Assuming a method to get the creator ID
                ChangedUser = Guid.NewGuid() // Assuming a method to get the changed user ID
            };

            var fAQFAQCategories = new List<FAQFAQCategory>();
            if (request.FAQCategories != null)
            {
                foreach (var item in request.FAQCategories)
                {
                    fAQFAQCategories.Add(new FAQFAQCategory
                    {
                        Id = Guid.NewGuid(),
                        FAQId = faq.Id,
                        FAQCategoryId = item
                    });
                }
            }

            // Step 4: Database Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("INSERT INTO FAQs (Id, Question, Answer, Langcode, Status, FaqOrder, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @Question, @Answer, @Langcode, @Status, @FaqOrder, @Version, @Created, @Changed, @CreatorId, @ChangedUser)", faq, transaction);

                    if (fAQFAQCategories.Any())
                    {
                        await _dbConnection.ExecuteAsync("INSERT INTO FAQFAQCategories (Id, FAQId, FAQCategoryId) VALUES (@Id, @FAQId, @FAQCategoryId)", fAQFAQCategories, transaction);
                    }

                    transaction.Commit();
                    return faq.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<FAQDto> GetFAQ(FAQRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch faq from the Database
            var faq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>("SELECT * FROM FAQs WHERE Id = @Id", new { Id = request.Id });
            if (faq == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Fetch Associated FAQCategories
            var faqCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT FAQCategoryId FROM FAQFAQCategories WHERE FAQId = @FAQId", new { FAQId = faq.Id });
            var temporaryFaqCategories = new List<FAQCategory>();

            foreach (var categoryId in faqCategoryIds)
            {
                var categoryRequestDto = new FAQCategoryRequestDto { Id = categoryId };
                var category = await _faqCategoryService.GetFAQCategory(categoryRequestDto);
                if (category == null)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }
                temporaryFaqCategories.Add(category);
            }

            // Step 4: Map db object to FAQDto and return
            var faqDto = new FAQDto
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer,
                FAQCategories = temporaryFaqCategories,
                Langcode = faq.Langcode,
                Status = faq.Status,
                FaqOrder = faq.FaqOrder,
                Version = faq.Version,
                Created = faq.Created,
                Changed = faq.Changed,
                CreatorId = faq.CreatorId,
                ChangedUser = faq.ChangedUser
            };

            return faqDto;
        }

        public async Task<string> UpdateFAQ(UpdateFAQDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing faq object from the database
            var faq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>("SELECT * FROM FAQs WHERE Id = @Id", new { Id = request.Id });
            if (faq == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Fetch Associated FAQCategories
            var existingFAQCategoriesIds = await _dbConnection.QueryAsync<Guid>("SELECT FAQCategoryId FROM FAQFAQCategories WHERE FAQId = @FAQId", new { FAQId = faq.Id });
            var temporaryFAQCategoriesIds = request.FAQCategories?.ToList() ?? existingFAQCategoriesIds.ToList();

            // Step 4: Define FAQ Categories for removal and addition
            var categoriesToBeRemoved = existingFAQCategoriesIds.Except(temporaryFAQCategoriesIds).ToList();
            var categoriesToBeAdded = temporaryFAQCategoriesIds.Except(existingFAQCategoriesIds).ToList();

            foreach (var categoryId in categoriesToBeAdded)
            {
                var categoryRequestDto = new FAQCategoryRequestDto { Id = categoryId };
                var category = await _faqCategoryService.GetFAQCategory(categoryRequestDto);
                if (category == null)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }
            }

            // Step 5: Update the FAQ object
            faq.Question = request.Question ?? faq.Question;
            faq.Answer = request.Answer ?? faq.Answer;
            faq.Langcode = request.Langcode ?? faq.Langcode;
            faq.Status = request.Status ?? faq.Status;
            faq.FaqOrder = request.FaqOrder ?? faq.FaqOrder;
            faq.Version += 1;
            faq.Changed = DateTime.Now;

            // Step 6: Database Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("UPDATE FAQs SET Question = @Question, Answer = @Answer, Langcode = @Langcode, Status = @Status, FaqOrder = @FaqOrder, Version = @Version, Changed = @Changed WHERE Id = @Id", faq, transaction);

                    if (categoriesToBeRemoved.Any())
                    {
                        await _dbConnection.ExecuteAsync("DELETE FROM FAQFAQCategories WHERE FAQId = @FAQId AND FAQCategoryId IN @CategoriesToBeRemoved", new { FAQId = faq.Id, CategoriesToBeRemoved = categoriesToBeRemoved }, transaction);
                    }

                    if (categoriesToBeAdded.Any())
                    {
                        var newFAQCategories = categoriesToBeAdded.Select(categoryId => new FAQFAQCategory { Id = Guid.NewGuid(), FAQId = faq.Id, FAQCategoryId = categoryId }).ToList();
                        await _dbConnection.ExecuteAsync("INSERT INTO FAQFAQCategories (Id, FAQId, FAQCategoryId) VALUES (@Id, @FAQId, @FAQCategoryId)", newFAQCategories, transaction);
                    }

                    transaction.Commit();
                    return faq.Id.ToString();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }
        }

        public async Task<bool> DeleteFAQ(DeleteFAQDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing FAQ object from the database
            var faq = await _dbConnection.QuerySingleOrDefaultAsync<FAQ>("SELECT * FROM FAQs WHERE Id = @Id", new { Id = request.Id });
            if (faq == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Perform Database Updates in a Single Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("DELETE FROM FAQFAQCategories WHERE FAQId = @FAQId", new { FAQId = faq.Id }, transaction);
                    await _dbConnection.ExecuteAsync("DELETE FROM FAQs WHERE Id = @Id", new { Id = faq.Id }, transaction);

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

        public async Task<List<FAQDto>> GetListFAQ(ListFAQRequestDto request)
        {
            // Step 1: Validate the request payload
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Set default sorting values if not provided
            request.SortField = string.IsNullOrEmpty(request.SortField) ? "Id" : request.SortField;
            request.SortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

            // Step 3: Fetch the list of FAQs from the database
            var faqs = await _dbConnection.QueryAsync<FAQ>($"SELECT * FROM FAQs ORDER BY {request.SortField} {request.SortOrder} OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY");

            var faqDtos = new List<FAQDto>();

            foreach (var faq in faqs)
            {
                var temporaryFAQCategories = new List<FAQCategory>();
                var faqCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT FAQCategoryId FROM FAQFAQCategories WHERE FAQId = @FAQId", new { FAQId = faq.Id });

                foreach (var categoryId in faqCategoryIds)
                {
                    var categoryRequestDto = new FAQCategoryRequestDto { Id = categoryId };
                    var category = await _faqCategoryService.GetFAQCategory(categoryRequestDto);
                    if (category == null)
                    {
                        throw new BusinessException("DP-404", "Technical Error");
                    }
                    temporaryFAQCategories.Add(category);
                }

                var faqDto = new FAQDto
                {
                    Id = faq.Id,
                    Question = faq.Question,
                    Answer = faq.Answer,
                    FAQCategories = temporaryFAQCategories,
                    Langcode = faq.Langcode,
                    Status = faq.Status,
                    FaqOrder = faq.FaqOrder,
                    Version = faq.Version,
                    Created = faq.Created,
                    Changed = faq.Changed,
                    CreatorId = faq.CreatorId,
                    ChangedUser = faq.ChangedUser
                };

                faqDtos.Add(faqDto);
            }

            return faqDtos;
        }
    }
}