
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing product tags.
    /// </summary>
    public interface IProductTagService
    {
        /// <summary>
        /// Creates a new product tag.
        /// </summary>
        /// <param name="createProductTagDto">Data transfer object for creating a product tag.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateProductTag(CreateProductTagDto createProductTagDto);

        /// <summary>
        /// Retrieves a product tag based on the provided request data.
        /// </summary>
        /// <param name="requestDto">Data transfer object containing request parameters for retrieving a product tag.</param>
        /// <returns>A ProductTag object matching the request.</returns>
        Task<ProductTag> GetProductTag(ProductTagRequestDto requestDto);

        /// <summary>
        /// Updates an existing product tag.
        /// </summary>
        /// <param name="updateProductTagDto">Data transfer object for updating a product tag.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateProductTag(UpdateProductTagDto updateProductTagDto);

        /// <summary>
        /// Deletes a product tag based on the provided request data.
        /// </summary>
        /// <param name="deleteProductTagDto">Data transfer object for deleting a product tag.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteProductTag(DeleteProductTagDto deleteProductTagDto);

        /// <summary>
        /// Retrieves a list of product tags based on the provided request data.
        /// </summary>
        /// <param name="listProductTagRequestDto">Data transfer object containing request parameters for retrieving a list of product tags.</param>
        /// <returns>A list of ProductTag objects matching the request.</returns>
        Task<List<ProductTag>> GetListProductTag(ListProductTagRequestDto listProductTagRequestDto);
    }
}
