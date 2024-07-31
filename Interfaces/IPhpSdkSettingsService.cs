
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing PHP SDK settings.
    /// </summary>
    public interface IPhpSdkSettingsService
    {
        /// <summary>
        /// Creates a new PHP SDK settings entry.
        /// </summary>
        /// <param name="phpSdkSettings">The PHP SDK settings to create.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreatePhpSdkSettings(PhpSdkSettings phpSdkSettings);

        /// <summary>
        /// Retrieves PHP SDK settings based on the provided request.
        /// </summary>
        /// <param name="phpSdkSettingsRequestDto">The request DTO containing criteria for retrieving PHP SDK settings.</param>
        /// <returns>The PHP SDK settings that match the request criteria.</returns>
        Task<PhpSdkSettings> GetPhpSdkSettings(PhpSdkSettingsRequestDto phpSdkSettingsRequestDto);

        /// <summary>
        /// Updates an existing PHP SDK settings entry.
        /// </summary>
        /// <param name="phpSdkSettings">The PHP SDK settings to update.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdatePhpSdkSettings(PhpSdkSettings phpSdkSettings);

        /// <summary>
        /// Deletes a PHP SDK settings entry based on the provided DTO.
        /// </summary>
        /// <param name="deletePhpSdkSettingsDto">The DTO containing information for deleting PHP SDK settings.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeletePhpSdkSettings(DeletePhpSdkSettingsDto deletePhpSdkSettingsDto);
    }
}
