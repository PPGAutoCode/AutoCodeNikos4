
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing product categories.
    /// </summary>
    public interface IProductCategoryService
    {
        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="createProductCategoryDto">Data transfer object for creating a product category.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateProductCategory(CreateProductCategoryDto createProductCategoryDto);

        /// <summary>
        /// Retrieves a product category based on the provided request data.
        /// </summary>
        /// <param name="productCategoryRequestDto">Data transfer object for requesting a product category.</param>
        /// <returns>A ProductCategory object representing the requested product category.</returns>
        Task<ProductCategory> GetProductCategory(ProductCategoryRequestDto productCategoryRequestDto);

        /// <summary>
        /// Updates an existing product category.
        /// </summary>
        /// <param name="updateProductCategoryDto">Data transfer object for updating a product category.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateProductCategory(UpdateProductCategoryDto updateProductCategoryDto);

        /// <summary>
        /// Deletes a product category.
        /// </summary>
        /// <param name="deleteProductCategoryDto">Data transfer object for deleting a product category.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteProductCategory(DeleteProductCategoryDto deleteProductCategoryDto);

        /// <summary>
        /// Retrieves a list of product categories based on the provided request data.
        /// </summary>
        /// <param name="listProductCategoryRequestDto">Data transfer object for requesting a list of product categories.</param>
        /// <returns>A list of ProductCategory objects.</returns>
        Task<List<ProductCategory>> GetListProductCategory(ListProductCategoryRequestDto listProductCategoryRequestDto);
    }
}
