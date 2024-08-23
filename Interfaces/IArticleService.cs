
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing article-related operations.
    /// </summary>
    public interface IArticleService
    {
        /// <summary>
        /// Creates a new article based on the provided data.
        /// </summary>
        /// <param name="createArticleDto">Data transfer object containing information for creating a new article.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateArticle(CreateArticleDto createArticleDto);

        /// <summary>
        /// Retrieves an article based on the provided request data.
        /// </summary>
        /// <param name="articleRequestDto">Data transfer object containing request information for retrieving an article.</param>
        /// <returns>An ArticleDto object representing the requested article.</returns>
        Task<ArticleDto> GetArticle(ArticleRequestDto articleRequestDto);

        /// <summary>
        /// Updates an existing article based on the provided data.
        /// </summary>
        /// <param name="updateArticleDto">Data transfer object containing information for updating an article.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateArticle(UpdateArticleDto updateArticleDto);

        /// <summary>
        /// Deletes an article based on the provided data.
        /// </summary>
        /// <param name="deleteArticleDto">Data transfer object containing information for deleting an article.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteArticle(DeleteArticleDto deleteArticleDto);

        /// <summary>
        /// Retrieves a list of articles based on the provided request data.
        /// </summary>
        /// <param name="listArticleRequestDto">Data transfer object containing request information for retrieving a list of articles.</param>
        /// <returns>A list of ArticleDto objects representing the requested articles.</returns>
        Task<List<ArticleDto>> GetListArticle(ListArticleRequestDto listArticleRequestDto);
    }
}
