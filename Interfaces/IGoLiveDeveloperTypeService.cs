
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing GoLiveDeveloperType entities.
    /// </summary>
    public interface IGoLiveDeveloperTypeService
    {
        /// <summary>
        /// Creates a new GoLiveDeveloperType based on the provided data.
        /// </summary>
        /// <param name="createGoLiveDeveloperTypeDto">Data transfer object for creating a GoLiveDeveloperType.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateGoLiveDeveloperType(CreateGoLiveDeveloperTypeDto createGoLiveDeveloperTypeDto);

        /// <summary>
        /// Retrieves a GoLiveDeveloperType based on the provided request data.
        /// </summary>
        /// <param name="requestDto">Data transfer object containing request parameters for retrieving a GoLiveDeveloperType.</param>
        /// <returns>A GoLiveDeveloperType object matching the request.</returns>
        Task<GoLiveDeveloperType> GetGoLiveDeveloperType(GoLiveDeveloperTypeRequestDto requestDto);

        /// <summary>
        /// Updates an existing GoLiveDeveloperType based on the provided data.
        /// </summary>
        /// <param name="updateGoLiveDeveloperTypeDto">Data transfer object for updating a GoLiveDeveloperType.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateGoLiveDeveloperType(UpdateGoLiveDeveloperTypeDto updateGoLiveDeveloperTypeDto);

        /// <summary>
        /// Deletes a GoLiveDeveloperType based on the provided data.
        /// </summary>
        /// <param name="deleteGoLiveDeveloperTypeDto">Data transfer object for deleting a GoLiveDeveloperType.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteGoLiveDeveloperType(DeleteGoLiveDeveloperTypeDto deleteGoLiveDeveloperTypeDto);

        /// <summary>
        /// Retrieves a list of GoLiveDeveloperType based on the provided request data.
        /// </summary>
        /// <param name="listGoLiveDeveloperTypeRequestDto">Data transfer object containing request parameters for retrieving a list of GoLiveDeveloperType.</param>
        /// <returns>A list of GoLiveDeveloperType objects matching the request.</returns>
        Task<List<GoLiveDeveloperType>> GetListGoLiveDeveloperType(ListGoLiveDeveloperTypeRequestDto listGoLiveDeveloperTypeRequestDto);
    }
}
