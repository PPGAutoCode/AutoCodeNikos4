using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

public class AuthorService : IAuthorService
{
    private readonly IDbConnection _dbConnection;
    private readonly IImageService _imageService;

    public AuthorService(IDbConnection dbConnection, IImageService imageService)
    {
        _dbConnection = dbConnection;
        _imageService = imageService;
    }

    public async Task<string> CreateAuthor(CreateAuthorDto request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        Guid? imageId = null;
        if (request.Image != null)
        {
            imageId = Guid.Parse(await _imageService.CreateImage(request.Image));
        }

        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Image = imageId,
            Details = request.Details
        };

        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                string sql = "INSERT INTO Authors (Id, Name, Image, Details) VALUES (@Id, @Name, @Image, @Details)";
                await _dbConnection.ExecuteAsync(sql, author, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        return author.Id.ToString();
    }

    public async Task<AuthorDto> GetAuthor(AuthorRequestDto request)
    {
        if (request.Id == null)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        string sql = "SELECT * FROM Authors WHERE Id = @Id";
        var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(sql, new { Id = request.Id });

        if (author == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        Image image = null;
        if (author.Image != null)
        {
            image = await _imageService.GetImage(new ImageRequestDto { Id = author.Image.Value });
        }

        return new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            Image = image,
            Details = author.Details
        };
    }

    public async Task<string> UpdateAuthor(UpdateAuthorDto request)
    {
        if (request.Id == null)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        string sql = "SELECT * FROM Authors WHERE Id = @Id";
        var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(sql, new { Id = request.Id });

        if (author == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        Guid? imageId = null;
        if (request.Image != null)
        {
            if (request.Image.Id == null)
            {
                imageId = Guid.Parse(await _imageService.CreateImage(new CreateImageDto
                {
                    ImageName = request.Image.ImageName,
                    ImageFile = request.Image.ImageFile,
                    AltText = request.Image.AltText
                }));
            }
            else
            {
                imageId = Guid.Parse(await _imageService.UpdateImage(request.Image));
            }
        }

        author.Name = request.Name;
        author.Details = request.Details;
        author.Image = imageId;

        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                sql = "UPDATE Authors SET Name = @Name, Details = @Details, Image = @Image WHERE Id = @Id";
                await _dbConnection.ExecuteAsync(sql, author, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        return author.Id.ToString();
    }

    public async Task<bool> DeleteAuthor(DeleteAuthorDto request)
    {
        if (request.Id == null)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        string sql = "SELECT * FROM Authors WHERE Id = @Id";
        var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(sql, new { Id = request.Id });

        if (author == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        if (author.Image != null)
        {
            await _imageService.DeleteImage(new DeleteImageDto { Id = author.Image.Value });
        }

        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                sql = "DELETE FROM Authors WHERE Id = @Id";
                await _dbConnection.ExecuteAsync(sql, new { Id = request.Id }, transaction);
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

    public async Task<List<AuthorDto>> GetListAuthor(ListAuthorRequestDto request)
    {
        if (request.PageLimit <= 0 || request.PageOffset < 0)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        if (string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
        {
            request.SortField = "Id";
            request.SortOrder = "asc";
        }

        string sql = $"SELECT * FROM Authors ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
        var authors = await _dbConnection.QueryAsync<Author>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });

        var authorDtos = new List<AuthorDto>();
        foreach (var author in authors)
        {
            Image image = null;
            if (author.Image != null)
            {
                image = await _imageService.GetImage(new ImageRequestDto { Id = author.Image.Value });
            }

            authorDtos.Add(new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Image = image,
                Details = author.Details
            });
        }

        return authorDtos;
    }
}