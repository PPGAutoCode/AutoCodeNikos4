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
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateFAQCategory(CreateFAQCategoryDto createFAQCategoryDto);

        /// <summary>
        /// Retrieves a FAQ category based on the provided request.
        /// </summary>
        /// <param name="faqCategoryRequestDto">Data transfer object for requesting a FAQ category.</param>
        /// <returns>A FAQ category object.</returns>
        Task<FAQCategory> GetFAQCategory(FAQCategoryRequestDto faqCategoryRequestDto);

        /// <summary>
        /// Updates an existing FAQ category.
        /// </summary>
        /// <param name="updateFAQCategoryDto">Data transfer object for updating a FAQ category.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateFAQCategory(UpdateFAQCategoryDto updateFAQCategoryDto);

        /// <summary>
        /// Deletes a FAQ category.
        /// </summary>
        /// <param name="deleteFAQCategoryDto">Data transfer object for deleting a FAQ category.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteFAQCategory(DeleteFAQCategoryDto deleteFAQCategoryDto);

        /// <summary>
        /// Retrieves a list of FAQ categories based on the provided request.
        /// </summary>
        /// <param name="listFAQCategoryRequestDto">Data transfer object for requesting a list of FAQ categories.</param>
        /// <returns>A list of FAQ category objects.</returns>
        Task<List<FAQCategory>> GetListFAQCategory(ListFAQCategoryRequestDto listFAQCategoryRequestDto);
    }
}