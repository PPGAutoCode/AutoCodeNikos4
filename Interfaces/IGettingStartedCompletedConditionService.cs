
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing the conditions under which getting started tasks are considered completed.
    /// </summary>
    public interface IGettingStartedCompletedConditionService
    {
        /// <summary>
        /// Creates a new getting started completed condition based on the provided data transfer object.
        /// </summary>
        /// <param name="createGettingStartedCompletedConditionDto">The data transfer object containing the information for creating the condition.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateGettingStartedCompletedCondition(CreateGettingStartedCompletedConditionDto createGettingStartedCompletedConditionDto);

        /// <summary>
        /// Retrieves a getting started completed condition based on the provided request data transfer object.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the request information for retrieving the condition.</param>
        /// <returns>A GettingStartedCompletedCondition object representing the retrieved condition.</returns>
        Task<GettingStartedCompletedCondition> GetGettingStartedCompletedCondition(GettingStartedCompletedConditionRequestDto requestDto);

        /// <summary>
        /// Updates an existing getting started completed condition based on the provided data transfer object.
        /// </summary>
        /// <param name="updateGettingStartedCompletedConditionDto">The data transfer object containing the updated information for the condition.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateGettingStartedCompletedCondition(UpdateGettingStartedCompletedConditionDto updateGettingStartedCompletedConditionDto);

        /// <summary>
        /// Deletes a getting started completed condition based on the provided data transfer object.
        /// </summary>
        /// <param name="deleteGettingStartedCompletedConditionDto">The data transfer object containing the information for deleting the condition.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteGettingStartedCompletedCondition(DeleteGettingStartedCompletedConditionDto deleteGettingStartedCompletedConditionDto);

        /// <summary>
        /// Retrieves a list of getting started completed conditions based on the provided request data transfer object.
        /// </summary>
        /// <param name="listGettingStartedCompletedConditionRequestDto">The data transfer object containing the request information for retrieving the list of conditions.</param>
        /// <returns>A list of GettingStartedCompletedCondition objects representing the retrieved conditions.</returns>
        Task<List<GettingStartedCompletedCondition>> GetListGettingStartedCompletedCondition(ListGettingStartedCompletedConditionRequestDto listGettingStartedCompletedConditionRequestDto);
    }
}
