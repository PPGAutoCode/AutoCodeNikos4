
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for services that manage basic pages.
    /// </summary>
    public interface IBasicPageService
    {
        /// <summary>
        /// Creates a new basic page based on the provided data transfer object.
        /// </summary>
        /// <param name="createBasicPageDto">The data transfer object containing the information for the new basic page.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateBasicPage(CreateBasicPageDto createBasicPageDto);

        /// <summary>
        /// Retrieves a basic page based on the provided request data transfer object.
        /// </summary>
        /// <param name="basicPageRequestDto">The data transfer object containing the request information for the basic page.</param>
        /// <returns>A data transfer object representing the requested basic page.</returns>
        Task<BasicPageDto> GetBasicPage(BasicPageRequestDto basicPageRequestDto);

        /// <summary>
        /// Updates an existing basic page based on the provided data transfer object.
        /// </summary>
        /// <param name="updateBasicPageDto">The data transfer object containing the updated information for the basic page.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateBasicPage(UpdateBasicPageDto updateBasicPageDto);

        /// <summary>
        /// Deletes a basic page based on the provided data transfer object.
        /// </summary>
        /// <param name="deleteBasicPageDto">The data transfer object containing the information for the basic page to be deleted.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteBasicPage(DeleteBasicPageDto deleteBasicPageDto);

        /// <summary>
        /// Retrieves a list of basic pages based on the provided request data transfer object.
        /// </summary>
        /// <param name="listBasicPageRequestDto">The data transfer object containing the request information for the list of basic pages.</param>
        /// <returns>A list of data transfer objects representing the requested basic pages.</returns>
        Task<List<BasicPageDto>> GetListBasicPage(ListBasicPageRequestDto listBasicPageRequestDto);
    }
}
