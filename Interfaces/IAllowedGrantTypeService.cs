using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing allowed grant types.
    /// </summary>
    public interface IAllowedGrantTypeService
    {
        /// <summary>
        /// Creates a new allowed grant type.
        /// </summary>
        /// <param name="createAllowedGrantTypeDto">Data transfer object for creating an allowed grant type.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateAllowedGrantType(CreateAllowedGrantTypeDto createAllowedGrantTypeDto);

        /// <summary>
        /// Retrieves an allowed grant type based on the provided request data.
        /// </summary>
        /// <param name="allowedGrantTypeRequestDto">Data transfer object for requesting an allowed grant type.</param>
        /// <returns>An instance of AllowedGrantType representing the requested grant type.</returns>
        Task<AllowedGrantType> GetAllowedGrantType(AllowedGrantTypeRequestDto allowedGrantTypeRequestDto);

        /// <summary>
        /// Updates an existing allowed grant type.
        /// </summary>
        /// <param name="updateAllowedGrantTypeDto">Data transfer object for updating an allowed grant type.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateAllowedGrantType(UpdateAllowedGrantTypeDto updateAllowedGrantTypeDto);

        /// <summary>
        /// Deletes an allowed grant type.
        /// </summary>
        /// <param name="deleteAllowedGrantTypeDto">Data transfer object for deleting an allowed grant type.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteAllowedGrantType(DeleteAllowedGrantTypeDto deleteAllowedGrantTypeDto);

        /// <summary>
        /// Retrieves a list of allowed grant types based on the provided request data.
        /// </summary>
        /// <param name="listAllowedGrantTypeRequestDto">Data transfer object for requesting a list of allowed grant types.</param>
        /// <returns>A list of AllowedGrantType instances representing the requested grant types.</returns>
        Task<List<AllowedGrantType>> GetListAllowedGrantType(ListAllowedGrantTypeRequestDto listAllowedGrantTypeRequestDto);
    }
}