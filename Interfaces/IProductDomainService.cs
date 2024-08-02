
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing product domain operations.
    /// </summary>
    public interface IProductDomainService
    {
        /// <summary>
        /// Creates a new product domain based on the provided data transfer object.
        /// </summary>
        /// <param name="createProductDomainDto">The data transfer object containing the information for the new product domain.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateProductDomain(CreateProductDomainDto createProductDomainDto);

        /// <summary>
        /// Retrieves a product domain based on the provided request data transfer object.
        /// </summary>
        /// <param name="productDomainRequestDto">The data transfer object containing the request information for the product domain.</param>
        /// <returns>A ProductDomain object representing the retrieved product domain.</returns>
        Task<ProductDomain> GetProductDomain(ProductDomainRequestDto productDomainRequestDto);

        /// <summary>
        /// Updates an existing product domain based on the provided data transfer object.
        /// </summary>
        /// <param name="updateProductDomainDto">The data transfer object containing the updated information for the product domain.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateProductDomain(UpdateProductDomainDto updateProductDomainDto);

        /// <summary>
        /// Deletes a product domain based on the provided data transfer object.
        /// </summary>
        /// <param name="deleteProductDomainDto">The data transfer object containing the information for the product domain to be deleted.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteProductDomain(DeleteProductDomainDto deleteProductDomainDto);

        /// <summary>
        /// Retrieves a list of product domains based on the provided request data transfer object.
        /// </summary>
        /// <param name="listProductDomainRequestDto">The data transfer object containing the request information for the list of product domains.</param>
        /// <returns>A list of ProductDomain objects representing the retrieved product domains.</returns>
        Task<List<ProductDomain>> GetListProductDomain(ListProductDomainRequestDto listProductDomainRequestDto);
    }
}
