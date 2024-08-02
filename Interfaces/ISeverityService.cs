
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing severity levels within the system.
    /// </summary>
    public interface ISeverityService
    {
        /// <summary>
        /// Creates a new severity level based on the provided data.
        /// </summary>
        /// <param name="createSeverityDto">Data transfer object containing the details of the severity to be created.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateSeverity(CreateSeverityDto createSeverityDto);

        /// <summary>
        /// Retrieves a severity level based on the provided request data.
        /// </summary>
        /// <param name="severityRequestDto">Data transfer object containing the request details for the severity.</param>
        /// <returns>A Severity object representing the requested severity level.</returns>
        Task<Severity> GetSeverity(SeverityRequestDto severityRequestDto);

        /// <summary>
        /// Updates an existing severity level based on the provided update data.
        /// </summary>
        /// <param name="updateSeverityDto">Data transfer object containing the details of the severity to be updated.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateSeverity(UpdateSeverityDto updateSeverityDto);

        /// <summary>
        /// Deletes a severity level based on the provided delete data.
        /// </summary>
        /// <param name="deleteSeverityDto">Data transfer object containing the details of the severity to be deleted.</param>
        /// <returns>A boolean indicating the success or failure of the delete operation.</returns>
        Task<bool> DeleteSeverity(DeleteSeverityDto deleteSeverityDto);

        /// <summary>
        /// Retrieves a list of severity levels based on the provided request data.
        /// </summary>
        /// <param name="listSeverityRequestDto">Data transfer object containing the request details for the list of severities.</param>
        /// <returns>A list of Severity objects representing the requested severity levels.</returns>
        Task<List<Severity>> GetListSeverity(ListSeverityRequestDto listSeverityRequestDto);
    }
}
