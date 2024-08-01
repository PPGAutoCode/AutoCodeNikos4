using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing application statuses.
    /// </summary>
    public interface IAppStatusService
    {
        /// <summary>
        /// Creates a new application status.
        /// </summary>
        /// <param name="createAppStatusDto">Data transfer object for creating an application status.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateAppStatus(CreateAppStatusDto createAppStatusDto);

        /// <summary>
        /// Retrieves an application status based on the provided request data.
        /// </summary>
        /// <param name="appStatusRequestDto">Data transfer object for requesting an application status.</param>
        /// <returns>An AppStatus object representing the requested application status.</returns>
        Task<AppStatus> GetAppStatus(AppStatusRequestDto appStatusRequestDto);

        /// <summary>
        /// Updates an existing application status.
        /// </summary>
        /// <param name="updateAppStatusDto">Data transfer object for updating an application status.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateAppStatus(UpdateAppStatusDto updateAppStatusDto);

        /// <summary>
        /// Deletes an application status.
        /// </summary>
        /// <param name="deleteAppStatusDto">Data transfer object for deleting an application status.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteAppStatus(DeleteAppStatusDto deleteAppStatusDto);

        /// <summary>
        /// Retrieves a list of application statuses based on the provided request data.
        /// </summary>
        /// <param name="listAppStatusRequestDto">Data transfer object for requesting a list of application statuses.</param>
        /// <returns>A list of AppStatus objects representing the requested application statuses.</returns>
        Task<List<AppStatus>> GetListAppStatus(ListAppStatusRequestDto listAppStatusRequestDto);
    }
}