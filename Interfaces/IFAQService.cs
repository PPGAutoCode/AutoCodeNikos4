
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing FAQ entries.
    /// </summary>
    public interface IFAQService
    {
        /// <summary>
        /// Creates a new FAQ entry.
        /// </summary>
        /// <param name="createFAQDto">The data transfer object containing information for creating a new FAQ entry.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateFAQ(CreateFAQDto createFAQDto);

        /// <summary>
        /// Retrieves a specific FAQ entry.
        /// </summary>
        /// <param name="faqRequestDto">The data transfer object containing the request information for retrieving an FAQ entry.</param>
        /// <returns>A data transfer object representing the retrieved FAQ entry.</returns>
        Task<FAQDto> GetFAQ(FAQRequestDto faqRequestDto);

        /// <summary>
        /// Updates an existing FAQ entry.
        /// </summary>
        /// <param name="updateFAQDto">The data transfer object containing the updated information for the FAQ entry.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateFAQ(UpdateFAQDto updateFAQDto);

        /// <summary>
        /// Deletes a specific FAQ entry.
        /// </summary>
        /// <param name="deleteFAQDto">The data transfer object containing the information for deleting an FAQ entry.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteFAQ(DeleteFAQDto deleteFAQDto);

        /// <summary>
        /// Retrieves a list of FAQ entries based on the request.
        /// </summary>
        /// <param name="listFAQRequestDto">The data transfer object containing the request information for retrieving a list of FAQ entries.</param>
        /// <returns>A list of data transfer objects representing the retrieved FAQ entries.</returns>
        Task<List<FAQDto>> GetListFAQ(ListFAQRequestDto listFAQRequestDto);
    }
}
