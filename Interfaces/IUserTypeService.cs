
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing user types.
    /// </summary>
    public interface IUserTypeService
    {
        /// <summary>
        /// Creates a new user type.
        /// </summary>
        /// <param name="createUserTypeDto">Data transfer object for creating a user type.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateUserType(CreateUserTypeDto createUserTypeDto);

        /// <summary>
        /// Retrieves a user type based on the provided request data.
        /// </summary>
        /// <param name="requestDto">Data transfer object containing request parameters for retrieving a user type.</param>
        /// <returns>A UserType object matching the request.</returns>
        Task<UserType> GetUserType(UserTypeRequestDto requestDto);

        /// <summary>
        /// Updates an existing user type.
        /// </summary>
        /// <param name="updateUserTypeDto">Data transfer object for updating a user type.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateUserType(UpdateUserTypeDto updateUserTypeDto);

        /// <summary>
        /// Deletes a user type based on the provided data.
        /// </summary>
        /// <param name="deleteUserTypeDto">Data transfer object for deleting a user type.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteUserType(DeleteUserTypeDto deleteUserTypeDto);

        /// <summary>
        /// Retrieves a list of user types based on the provided request data.
        /// </summary>
        /// <param name="listUserTypeRequestDto">Data transfer object containing request parameters for retrieving a list of user types.</param>
        /// <returns>A list of UserType objects matching the request.</returns>
        Task<List<UserType>> GetListUserType(ListUserTypeRequestDto listUserTypeRequestDto);
    }
}
