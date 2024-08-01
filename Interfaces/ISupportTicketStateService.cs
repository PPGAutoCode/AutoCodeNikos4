
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing support ticket states.
    /// </summary>
    public interface ISupportTicketStateService
    {
        /// <summary>
        /// Creates a new support ticket state.
        /// </summary>
        /// <param name="createSupportTicketStateDto">Data transfer object for creating a support ticket state.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateSupportTicketState(CreateSupportTicketStateDto createSupportTicketStateDto);

        /// <summary>
        /// Retrieves a support ticket state based on the provided request.
        /// </summary>
        /// <param name="supportTicketStateRequestDto">Data transfer object for requesting a support ticket state.</param>
        /// <returns>A SupportTicketState object representing the requested state.</returns>
        Task<SupportTicketState> GetSupportTicketState(SupportTicketStateRequestDto supportTicketStateRequestDto);

        /// <summary>
        /// Updates an existing support ticket state.
        /// </summary>
        /// <param name="updateSupportTicketStateDto">Data transfer object for updating a support ticket state.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateSupportTicketState(UpdateSupportTicketStateDto updateSupportTicketStateDto);

        /// <summary>
        /// Deletes a support ticket state.
        /// </summary>
        /// <param name="deleteSupportTicketStateDto">Data transfer object for deleting a support ticket state.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteSupportTicketState(DeleteSupportTicketStateDto deleteSupportTicketStateDto);

        /// <summary>
        /// Retrieves a list of support ticket states based on the provided request.
        /// </summary>
        /// <param name="listSupportTicketStateRequestDto">Data transfer object for requesting a list of support ticket states.</param>
        /// <returns>A list of SupportTicketState objects.</returns>
        Task<List<SupportTicketState>> GetListSupportTicketState(ListSupportTicketStateRequestDto listSupportTicketStateRequestDto);
    }
}
