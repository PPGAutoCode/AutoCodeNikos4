using System;
using System.Data;
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
        // Step 1: Validation
        if (string.IsNullOrEmpty(request.Name))
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        Guid? imageId = null;

        // Step 3: Handle Image Creation
        if (request.Image != null)
        {
            var imageResponse = await _imageService.CreateImage(request.Image);
            imageId = Guid.Parse(imageResponse);
        }

        // Step 4: Create Author Object
        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Image = imageId,
            Details = request.Details
        };

        // Step 5: Database Insertion in a Transaction
        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                const string sql = "INSERT INTO Authors (Id, Name, Image, Details) VALUES (@Id, @Name, @Image, @Details)";
                await _dbConnection.ExecuteAsync(sql, author, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        // Step 6: Return Author Id
        return author.Id.ToString();
    }

    public async Task<AuthorDto> GetAuthor(AuthorRequestDto request)
    {
        // Step 1: Validation
        if (request.Id == Guid.Empty)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Fetch Author from Database
        const string sql = "SELECT * FROM Authors WHERE Id = @Id";
        var author = await _dbConnection.QuerySingleOrDefaultAsync<Author>(sql, new { request.Id });

        if (author == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 3: Fetch Image if Available
        if (author.Image.HasValue)
        {
            var imageRequest = new ImageRequestDto { Id = author.Image.Value };
            var image = await _imageService.GetImage(imageRequest);
            return new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Image = image,
                Details = author.Details
            };
        }

        // Step 4: Return AuthorDto without Image
        return new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            Details = author.Details
        };
    }

    public async Task<string> UpdateAuthor(UpdateAuthorDto request)
    {
        // Step 1: Validation
        if (request.Id == null)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Fetch Existing Author
        var existingAuthor = await GetAuthorById(request.Id.Value);
        if (existingAuthor == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        Guid? imageId = null;

        // Step 3: Handle Image Update or Creation
        if (request.Image != null)
        {
            if (request.Image.Id.HasValue)
            {
                await _imageService.UpdateImage(request.Image);
                imageId = request.Image.Id;
            }
            else
            {
                var imageResponse = await _imageService.CreateImage(new CreateImageDto
                {
                    ImageName = request.Image.ImageName,
                    ImageFile = request.Image.ImageFile,
                    AltText = request.Image.AltText
                });
                imageId = Guid.Parse(imageResponse);
            }
        }

        // Step 4: Update Author Object
        existingAuthor.Name = request.Name ?? existingAuthor.Name;
        existingAuthor.Details = request.Details ?? existingAuthor.Details;
        existingAuthor.Image = imageId ?? existingAuthor.Image;

        // Step 5: Database Update in a Transaction
        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                const string sql = "UPDATE Authors SET Name = @Name, Image = @Image, Details = @Details WHERE Id = @Id";
                await _dbConnection.ExecuteAsync(sql, existingAuthor, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        // Step 6: Return Author Id
        return existingAuthor.Id.ToString();
    }

    public async Task<bool> DeleteAuthor(DeleteAuthorDto request)
    {
        // Step 1: Validation
        if (request.Id == Guid.Empty)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Fetch Existing Author
        var existingAuthor = await GetAuthorById(request.Id);
        if (existingAuthor == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 3: Delete Related Image
        if (existingAuthor.Image.HasValue)
        {
            await _imageService.DeleteImage(new DeleteImageDto { Id = existingAuthor.Image.Value });
        }

        // Step 4: Database Deletion in a Transaction
        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                const string sql = "DELETE FROM Authors WHERE Id = @Id";
                await _dbConnection.ExecuteAsync(sql, new { request.Id }, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        // Step 5: Return Success
        return true;
    }

    private async Task<Author> GetAuthorById(Guid id)
    {
        const string sql = "SELECT * FROM Authors WHERE Id = @Id";
        return await _dbConnection.QuerySingleOrDefaultAsync<Author>(sql, new { Id = id });
    }
}
