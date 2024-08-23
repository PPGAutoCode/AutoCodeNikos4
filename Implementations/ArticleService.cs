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
    public class ArticleService : IArticleService
    {
        private readonly IDbConnection _dbConnection;
        private readonly IAuthorService _authorService;
        private readonly IBlogCategoryService _blogCategoryService;
        private readonly IBlogTagService _blogTagService;
        private readonly IAttachmentService _attachmentService;
        private readonly IImageService _imageService;

        public ArticleService(IDbConnection dbConnection, IAuthorService authorService, IBlogCategoryService blogCategoryService, IBlogTagService blogTagService, IAttachmentService attachmentService, IImageService imageService)
        {
            _dbConnection = dbConnection;
            _authorService = authorService;
            _blogCategoryService = blogCategoryService;
            _blogTagService = blogTagService;
            _attachmentService = attachmentService;
            _imageService = imageService;
        }

        public async Task<string> CreateArticle(CreateArticleDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.Title) || request.Author == Guid.Empty || string.IsNullOrEmpty(request.Langcode) || request.BlogCategories == null || !request.BlogCategories.Any())
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch and Validate Author
            var authorRequest = new AuthorRequestDto { Id = request.Author };
            var author = await _authorService.GetAuthor(authorRequest);
            if (author == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch and Validate that BlogCategories exist
            var blogCategories = new List<BlogCategory>();
            foreach (var blogCategoryId in request.BlogCategories)
            {
                var blogCategoryRequest = new BlogCategoryRequestDto { Id = blogCategoryId };
                var blogCategory = await _blogCategoryService.GetBlogCategory(blogCategoryRequest);
                if (blogCategory == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                blogCategories.Add(blogCategory);
            }

            // Step 4: Fetch or Create BlogTags
            var blogTags = new List<Guid>();
            if (request.BlogTags != null && request.BlogTags.Any())
            {
                var allBlogTags = await _dbConnection.QueryAsync<BlogTag>("SELECT Id, Name FROM BlogTags");
                var newBlogTags = request.BlogTags.Except(allBlogTags.Select(bt => bt.Name)).ToList();

                foreach (var newBlogTag in newBlogTags)
                {
                    var createBlogTagDto = new CreateBlogTagDto { Name = newBlogTag };
                    var createdBlogTagId = await _blogTagService.CreateBlogTag(createBlogTagDto);
                    blogTags.Add(Guid.Parse(createdBlogTagId));
                }

                blogTags.AddRange(allBlogTags.Where(bt => request.BlogTags.Contains(bt.Name)).Select(bt => bt.Id));
            }

            // Step 5: Create Attachment File
            Guid? pdfId = null;
            if (request.Pdf != null)
            {
                var createAttachmentDto = new CreateAttachmentDto
                {
                    FileName = request.Pdf.FileName,
                    FileData = request.Pdf.FileData
                };
                var attachmentId = await _attachmentService.CreateAttachment(createAttachmentDto);
                pdfId = Guid.Parse(attachmentId);
            }

            // Step 6: Create Image File
            Guid? imageId = null;
            if (request.Image != null)
            {
                var createImageDto = new CreateImageDto
                {
                    ImageName = request.Image.ImageName,
                    ImageFile = request.Image.ImageFile,
                    AltText = request.Image.AltText
                };
                var imageIdStr = await _imageService.CreateImage(createImageDto);
                imageId = Guid.Parse(imageIdStr);
            }

            // Step 7: Create new Article object
            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Author = request.Author,
                Summary = request.Summary,
                Body = request.Body,
                GoogleDriveId = request.GoogleDriveId,
                HideScrollSpy = request.HideScrollSpy,
                Image = imageId,
                Pdf = pdfId,
                Langcode = request.Langcode,
                Status = request.Status,
                Sticky = request.Sticky,
                Promote = request.Promote,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.Author,
                ChangedUser = request.Author
            };

            // Step 8: Create new list of ArticleBlogCategories objects
            var articleBlogCategories = request.BlogCategories.Select(blogCategoryId => new ArticleBlogCategory
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                BlogCategoryId = blogCategoryId
            }).ToList();

            // Step 9: Create new list of ArticleBlogTags objects
            var articleBlogTags = blogTags.Select(blogTagId => new ArticleBlogTag
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                BlogTagId = blogTagId
            }).ToList();

            // Step 10: Perform Database Operations in a Single Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("INSERT INTO Articles (Id, Title, Author, Summary, Body, GoogleDriveId, HideScrollSpy, Image, Pdf, Langcode, Status, Sticky, Promote, Version, Created, Changed, CreatorId, ChangedUser) VALUES (@Id, @Title, @Author, @Summary, @Body, @GoogleDriveId, @HideScrollSpy, @Image, @Pdf, @Langcode, @Status, @Sticky, @Promote, @Version, @Created, @Changed, @CreatorId, @ChangedUser)", article, transaction);

                    await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogCategories (Id, ArticleId, BlogCategoryId) VALUES (@Id, @ArticleId, @BlogCategoryId)", articleBlogCategories, transaction);

                    await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogTags (Id, ArticleId, BlogTagId) VALUES (@Id, @ArticleId, @BlogTagId)", articleBlogTags, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 11: Return the ArticleId.ToString() from the database
            return article.Id.ToString();
        }
        public async Task<ArticleDto> GetArticle(ArticleRequestDto request)
    {
        // Step 1: Validate Request Payload
        if (request.Id == null && string.IsNullOrEmpty(request.Title))
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        Article article;
        if (request.Id != null)
        {
            // Step 2.1: Fetch Article by Id
            article = await _dbConnection.QuerySingleOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = request.Id });
        }
        else
        {
            // Step 2.2: Fetch Article by Title
            article = await _dbConnection.QuerySingleOrDefaultAsync<Article>("SELECT TOP 1 * FROM Articles WHERE Title = @Title", new { Title = request.Title });
        }

        if (article == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 3: Retrieve Author
        var authorRequest = new AuthorRequestDto { Id = article.Author };
        var author = await _authorService.GetAuthor(authorRequest);
        if (author == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 4: Retrieve Attachment
        Attachment attachment = null;
        if (article.Pdf != null && article.Pdf != Guid.Empty)
        {
            var attachmentRequest = new AttachmentRequestDto { Id = article.Pdf.Value };
            attachment = await _attachmentService.GetAttachment(attachmentRequest);
            if (attachment == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }
        }

        // Step 5: Retrieve Image
        Image image = null;
        if (article.Image != null && article.Image != Guid.Empty)
        {
            var imageRequest = new ImageRequestDto { Id = article.Image.Value };
            image = await _imageService.GetImage(imageRequest);
            if (image == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }
        }

        // Step 6: Fetch Associated BlogCategories
        var blogCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT BlogCategoryId FROM ArticleBlogCategories WHERE ArticleId = @ArticleId", new { ArticleId = article.Id });
        var temporaryBlogCategories = new List<BlogCategory>();
        foreach (var blogCategoryId in blogCategoryIds)
        {
            var blogCategoryRequest = new BlogCategoryRequestDto { Id = blogCategoryId };
            var blogCategory = await _blogCategoryService.GetBlogCategory(blogCategoryRequest);
            if (blogCategory == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }
            temporaryBlogCategories.Add(blogCategory);
        }

        // Step 7: Fetch Associated BlogTags
        var blogTagIds = await _dbConnection.QueryAsync<Guid>("SELECT BlogTagId FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = article.Id });
        List<BlogTag> temporaryBlogTags = null;
        if (blogTagIds.Any())
        {
            temporaryBlogTags = new List<BlogTag>();
            foreach (var blogTagId in blogTagIds)
            {
                var blogTagRequest = new BlogTagRequestDto { Id = blogTagId };
                var blogTag = await _blogTagService.GetBlogTag(blogTagRequest);
                if (blogTag == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                temporaryBlogTags.Add(blogTag);
            }
        }

        // Step 8: Map db object to ArticleDto and return
        var articleDto = new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Author = author,
            Summary = article.Summary,
            Body = article.Body,
            GoogleDriveId = article.GoogleDriveId,
            HideScrollSpy = article.HideScrollSpy,
            Image = image,
            Pdf = attachment,
            Langcode = article.Langcode,
            Status = article.Status,
            Sticky = article.Sticky,
            Promote = article.Promote,
            BlogCategories = temporaryBlogCategories,
            BlogTags = temporaryBlogTags,
            Version = article.Version,
            Created = article.Created,
            Changed = article.Changed,
            CreatorId = article.CreatorId,
            ChangedUser = article.ChangedUser
        };

        return articleDto;
    }
        public async Task<string> UpdateArticle(UpdateArticleDto request)
        {
            // Step 1: Validate the request payload
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch the existing article
            var article = await _dbConnection.QuerySingleOrDefaultAsync<Article>("SELECT * FROM Articles WHERE Id = @Id", new { Id = request.Id });
            if (article == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch and Validate Author
            if (request.Author != null)
            {
                var authorRequestDto = new AuthorRequestDto { Id = request.Author.Value };
                var authorDto = await _authorService.GetAuthor(authorRequestDto);
                if (authorDto == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 4: Update Blog Categories
            var existingBlogCategoriesIds = (await _dbConnection.QueryAsync<Guid>("SELECT BlogCategoryId FROM ArticleBlogCategories WHERE ArticleId = @ArticleId", new { ArticleId = request.Id })).ToList();
            var temporaryBlogCategoriesIds = request.BlogCategories?.ToList() ?? existingBlogCategoriesIds;
            var categoriesToRemove = existingBlogCategoriesIds.Except(temporaryBlogCategoriesIds).ToList();
            var categoriesToAdd = temporaryBlogCategoriesIds.Except(existingBlogCategoriesIds).ToList();

            foreach (var categoryToAdd in categoriesToAdd)
            {
                var blogCategoryRequestDto = new BlogCategoryRequestDto { Id = categoryToAdd };
                var blogCategory = await _blogCategoryService.GetBlogCategory(blogCategoryRequestDto);
                if (blogCategory == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
            }

            // Step 5: Fetch or Create BlogTags
            List<Guid> blogTagsToRemove = new List<Guid>();
            List<Guid> blogTagsToAdd = new List<Guid>();

            if (request.BlogTags != null)
            {
                var allBlogTags = (await _dbConnection.QueryAsync<BlogTag>("SELECT * FROM BlogTags")).ToList();
                var existingBlogTagsIds = (await _dbConnection.QueryAsync<Guid>("SELECT BlogTagId FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = request.Id })).ToList();
                var existingBlogTags = allBlogTags.Where(bt => existingBlogTagsIds.Contains(bt.Id)).ToList();
                blogTagsToRemove = existingBlogTags.Where(bt => !request.BlogTags.Contains(bt.Name)).Select(bt => bt.Id).ToList();
                var newBlogTagNames = request.BlogTags.Except(existingBlogTags.Select(bt => bt.Name)).ToList();
                var existedNewBlogTags = allBlogTags.Where(bt => newBlogTagNames.Contains(bt.Name)).ToList();
                var blogTagsToCreate = newBlogTagNames.Except(existedNewBlogTags.Select(bt => bt.Name)).ToList();

                if (blogTagsToCreate.Any())
                {
                    foreach (var blogTagToCreate in blogTagsToCreate)
                    {
                        var createBlogTagDto = new CreateBlogTagDto { Name = blogTagToCreate };
                        await _blogTagService.CreateBlogTag(createBlogTagDto);
                    }
                }

                blogTagsToAdd = existedNewBlogTags.Select(bt => bt.Id).ToList();
            }

            // Step 6: Handle Image and Pdf
            Guid? imageId = null;
            if (request.Image != null)
            {
                if (request.Image.Id == null)
                {
                    var createImageDto = new CreateImageDto
                    {
                        ImageName = request.Image.ImageName,
                        ImageFile = request.Image.ImageFile,
                        AltText = request.Image.AltText
                    };
                    imageId = Guid.Parse(await _imageService.CreateImage(createImageDto));
                }
                else
                {
                    var updateImageDto = new UpdateImageDto
                    {
                        Id = request.Image.Id.Value,
                        ImageName = request.Image.ImageName,
                        ImageFile = request.Image.ImageFile,
                        AltText = request.Image.AltText
                    };
                    imageId = Guid.Parse(await _imageService.UpdateImage(updateImageDto));
                }
            }
            else
            {
                imageId = article.Image;
            }

            Guid? pdfId = null;
            if (request.Pdf != null)
            {
                if (request.Pdf.Id == null)
                {
                    var createAttachmentDto = new CreateAttachmentDto
                    {
                        FileName = request.Pdf.FileName,
                        FileData = request.Pdf.FileData
                    };
                    pdfId = Guid.Parse(await _attachmentService.CreateAttachment(createAttachmentDto));
                }
                else
                {
                    var updateAttachmentDto = new UpdateAttachmentDto
                    {
                        Id = request.Pdf.Id.Value,
                        FileName = request.Pdf.FileName,
                        FileData = request.Pdf.FileData
                    };
                    pdfId = Guid.Parse(await _attachmentService.UpdateAttachment(updateAttachmentDto));
                }
            }
            else
            {
                pdfId = article.Pdf;
            }

            // Step 7: Update the Article object
            article.Title = request.Title ?? article.Title;
            article.Author = request.Author ?? article.Author;
            article.Summary = request.Summary ?? article.Summary;
            article.Body = request.Body ?? article.Body;
            article.GoogleDriveId = request.GoogleDriveId ?? article.GoogleDriveId;
            article.HideScrollSpy = request.HideScrollSpy ?? article.HideScrollSpy;
            article.Image = imageId ?? article.Image;
            article.Pdf = pdfId ?? article.Pdf;
            article.Langcode = request.Langcode ?? article.Langcode;
            article.Status = request.Status ?? article.Status;
            article.Sticky = request.Sticky ?? article.Sticky;
            article.Promote = request.Promote ?? article.Promote;
            article.Version += 1;
            article.Changed = DateTime.UtcNow;

            // Step 8: Create new lists for ArticleBlogCategories and ArticleBlogTags
            var articleBlogCategories = categoriesToAdd.Select(blogCategoryId => new ArticleBlogCategory
            {
                Id = Guid.NewGuid(),
                ArticleId = request.Id,
                BlogCategoryId = blogCategoryId
            }).ToList();

            var articleBlogTags = blogTagsToAdd.Select(blogTagId => new ArticleBlogTag
            {
                Id = Guid.NewGuid(),
                ArticleId = request.Id,
                BlogTagId = blogTagId
            }).ToList();

            // Step 9: In a single SQL Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync("UPDATE Articles SET Title = @Title, Author = @Author, Summary = @Summary, Body = @Body, GoogleDriveId = @GoogleDriveId, HideScrollSpy = @HideScrollSpy, Image = @Image, Pdf = @Pdf, Langcode = @Langcode, Status = @Status, Sticky = @Sticky, Promote = @Promote, Version = @Version, Changed = @Changed WHERE Id = @Id", article, transaction);

                    if (categoriesToRemove.Any())
                    {
                        await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogCategories WHERE ArticleId = @ArticleId AND BlogCategoryId IN @BlogCategoryIds", new { ArticleId = request.Id, BlogCategoryIds = categoriesToRemove }, transaction);
                    }
                    if (articleBlogCategories.Any())
                    {
                        await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogCategories (Id, ArticleId, BlogCategoryId) VALUES (@Id, @ArticleId, @BlogCategoryId)", articleBlogCategories, transaction);
                    }

                    if (blogTagsToRemove.Any())
                    {
                        await _dbConnection.ExecuteAsync("DELETE FROM ArticleBlogTags WHERE ArticleId = @ArticleId AND BlogTagId IN @BlogTagIds", new { ArticleId = request.Id, BlogTagIds = blogTagsToRemove }, transaction);
                    }
                    if (articleBlogTags.Any())
                    {
                        await _dbConnection.ExecuteAsync("INSERT INTO ArticleBlogTags (Id, ArticleId, BlogTagId) VALUES (@Id, @ArticleId, @BlogTagId)", articleBlogTags, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            return article.Id.ToString();
        }
        public async Task<bool> DeleteArticle(DeleteArticleDto deleteArticleDto)
        {
            // Step 1: Validate Request Payload
            if (deleteArticleDto.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing Article
            var article = await _dbConnection.QuerySingleOrDefaultAsync<Article>(
                "SELECT * FROM Articles WHERE Id = @Id", new { Id = deleteArticleDto.Id });

            if (article == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Determine Fields to Delete
            if (deleteArticleDto.FieldsToDelete != null)
            {
                foreach (var field in deleteArticleDto.FieldsToDelete)
                {
                    // Nullifying the Corresponding Column
                    await _dbConnection.ExecuteAsync(
                        $"UPDATE Articles SET {field} = NULL WHERE Id = @Id", new { Id = deleteArticleDto.Id });
                }
            }
            else
            {
                // Step 4: Delete Related Attachment
                if (article.Pdf != null)
                {
                    await _attachmentService.DeleteAttachment(new DeleteAttachmentDto { Id = article.Pdf });
                }

                // Step 5: Delete Related Image
                if (article.Image != null)
                {
                    await _imageService.DeleteImage(new DeleteImageDto { Id = article.Image });
                }

                // Step 6: Perform Database Updates in a Single Transaction
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    try
                    {
                        // Step 7: Delete ArticleBlogCategories
                        await _dbConnection.ExecuteAsync(
                            "DELETE FROM ArticleBlogCategories WHERE ArticleId = @ArticleId",
                            new { ArticleId = deleteArticleDto.Id }, transaction);

                        // Step 8: Delete ArticleBlogTags
                        await _dbConnection.ExecuteAsync(
                            "DELETE FROM ArticleBlogTags WHERE ArticleId = @ArticleId",
                            new { ArticleId = deleteArticleDto.Id }, transaction);

                        // Step 9: Delete Article
                        await _dbConnection.ExecuteAsync(
                            "DELETE FROM Articles WHERE Id = @Id", new { Id = deleteArticleDto.Id }, transaction);

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw new TechnicalException("DP-500", "Technical Error");
                    }
                }
            }

            return true;
        }

        public async Task<List<ArticleDto>> GetListArticle(ListArticleRequestDto requestDto)
    {
        // Step 1: Validate the requestDto
        if (requestDto.PageLimit <= 0 || requestDto.PageOffset < 0)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Fetch Articles
        string sortField = requestDto.SortField ?? "Id";
        string sortOrder = requestDto.SortOrder ?? "asc";
        string sql = $"SELECT * FROM Articles ORDER BY {sortField} {sortOrder} OFFSET {requestDto.PageOffset} ROWS FETCH NEXT {requestDto.PageLimit} ROWS ONLY";
        var articles = await _dbConnection.QueryAsync<Article>(sql);

        var articleDtos = new List<ArticleDto>();

        foreach (var article in articles)
        {
            // Step 3: Fetch and Map Associated Author
            var authorRequestDto = new AuthorRequestDto { Id = article.Author };
            var authorDto = await _authorService.GetAuthor(authorRequestDto);
            if (authorDto == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: Fetch and Map Associated Pdf Attachment
            Attachment pdf = null;
            if (article.Pdf != null)
            {
                var attachmentRequestDto = new AttachmentRequestDto { Id = article.Pdf.Value };
                pdf = await _attachmentService.GetAttachment(attachmentRequestDto);
            }

            // Step 5: Fetch and Map Associated Image
            Image image = null;
            if (article.Image != null)
            {
                var imageRequestDto = new ImageRequestDto { Id = article.Image.Value };
                image = await _imageService.GetImage(imageRequestDto);
            }

            // Step 6: Fetch and Map Associated BlogCategories
            var blogCategoryIds = await _dbConnection.QueryAsync<Guid>("SELECT BlogCategoryId FROM ArticleBlogCategories WHERE ArticleId = @ArticleId", new { ArticleId = article.Id });
            var blogCategories = new List<BlogCategory>();
            foreach (var blogCategoryId in blogCategoryIds)
            {
                var blogCategoryRequestDto = new BlogCategoryRequestDto { Id = blogCategoryId };
                var blogCategory = await _blogCategoryService.GetBlogCategory(blogCategoryRequestDto);
                if (blogCategory == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                blogCategories.Add(blogCategory);
            }

            // Step 7: Fetch and Map Related BlogTags
            List<Guid> blogTagIds = await _dbConnection.QueryAsync<Guid>("SELECT BlogTagId FROM ArticleBlogTags WHERE ArticleId = @ArticleId", new { ArticleId = article.Id }).ConfigureAwait(false) as List<Guid>;
            var blogTags = new List<BlogTag>();
            if (blogTagIds != null)
            {
                foreach (var blogTagId in blogTagIds)
                {
                    var blogTagRequestDto = new BlogTagRequestDto { Id = blogTagId };
                    var blogTag = await _blogTagService.GetBlogTag(blogTagRequestDto);
                    blogTags.Add(blogTag);
                }
            }

            // Map the Article details to the ArticleDto object
            var articleDto = new ArticleDto
            {
                Id = article.Id,
                Title = article.Title,
                Author = authorDto,
                Summary = article.Summary,
                Body = article.Body,
                GoogleDriveId = article.GoogleDriveId,
                HideScrollSpy = article.HideScrollSpy,
                Image = image,
                Pdf = pdf,
                Langcode = article.Langcode,
                Status = article.Status,
                Sticky = article.Sticky,
                Promote = article.Promote,
                BlogCategories = blogCategories,
                BlogTags = blogTags,
                Version = article.Version,
                Created = article.Created,
                Changed = article.Changed,
                CreatorId = article.CreatorId,
                ChangedUser = article.ChangedUser
            };

            articleDtos.Add(articleDto);
        }

        return articleDtos;
    }
    }
}
