
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing support categories.
    /// </summary>
    public interface ISupportCategoryService
    {
        /// <summary>
        /// Creates a new support category.
        /// </summary>
        /// <param name="createSupportCategoryDto">Data transfer object for creating a support category.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateSupportCategory(CreateSupportCategoryDto createSupportCategoryDto);

        /// <summary>
        /// Retrieves a support category based on the provided request.
        /// </summary>
        /// <param name="supportCategoryRequestDto">Data transfer object for requesting a support category.</param>
        /// <returns>A SupportCategory object representing the found support category.</returns>
        Task<SupportCategory> GetSupportCategory(SupportCategoryRequestDto supportCategoryRequestDto);

        /// <summary>
        /// Updates an existing support category.
        /// </summary>
        /// <param name="updateSupportCategoryDto">Data transfer object for updating a support category.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateSupportCategory(UpdateSupportCategoryDto updateSupportCategoryDto);

        /// <summary>
        /// Deletes a support category.
        /// </summary>
        /// <param name="deleteSupportCategoryDto">Data transfer object for deleting a support category.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteSupportCategory(DeleteSupportCategoryDto deleteSupportCategoryDto);

        /// <summary>
        /// Retrieves a list of support categories based on the provided request.
        /// </summary>
        /// <param name="listSupportCategoryRequestDto">Data transfer object for requesting a list of support categories.</param>
        /// <returns>A list of SupportCategory objects.</returns>
        Task<List<SupportCategory>> GetListSupportCategory(ListSupportCategoryRequestDto listSupportCategoryRequestDto);
    }
}
