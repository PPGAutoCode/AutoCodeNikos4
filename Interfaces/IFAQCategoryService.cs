using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing FAQ categories.
    /// </summary>
    public interface IFAQCategoryService
    {
        /// <summary>
        /// Creates a new FAQ category.
        /// </summary>
        /// <param name="createFAQCategoryDto">Data transfer object for creating a FAQ category.</param>
        /// <returns>The ID of the created FAQ category.</returns>
        Task<string> CreateFAQCategory(CreateFAQCategoryDto createFAQCategoryDto);

        /// <summary>
        /// Retrieves a specific FAQ category by request.
        /// </summary>
        /// <param name="faqCategoryRequestDto">Data transfer object for requesting a FAQ category.</param>
        /// <returns>The requested FAQ category.</returns>
        Task<FAQCategory> GetFAQCategory(FAQCategoryRequestDto faqCategoryRequestDto);

        /// <summary>
        /// Updates an existing FAQ category.
        /// </summary>
        /// <param name="updateFAQCategoryDto">Data transfer object for updating a FAQ category.</param>
        /// <returns>The ID of the updated FAQ category.</returns>
        Task<string> UpdateFAQCategory(UpdateFAQCategoryDto updateFAQCategoryDto);

        /// <summary>
        /// Deletes a specific FAQ category.
        /// </summary>
        /// <param name="deleteFAQCategoryDto">Data transfer object for deleting a FAQ category.</param>
        /// <returns>True if the FAQ category was deleted successfully, otherwise false.</returns>
        Task<bool> DeleteFAQCategory(DeleteFAQCategoryDto deleteFAQCategoryDto);

        /// <summary>
        /// Retrieves a list of FAQ categories based on the request.
        /// </summary>
        /// <param name="listFAQCategoryRequestDto">Data transfer object for requesting a list of FAQ categories.</param>
        /// <returns>A list of FAQ categories.</returns>
        Task<List<FAQCategory>> GetListFAQCategory(ListFAQCategoryRequestDto listFAQCategoryRequestDto);
    }
}
