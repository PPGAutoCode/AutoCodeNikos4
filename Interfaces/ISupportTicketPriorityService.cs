
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing support ticket priorities.
    /// </summary>
    public interface ISupportTicketPriorityService
    {
        /// <summary>
        /// Creates a new support ticket priority.
        /// </summary>
        /// <param name="createSupportTicketPriorityDto">Data transfer object for creating a support ticket priority.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateSupportTicketPriority(CreateSupportTicketPriorityDto createSupportTicketPriorityDto);

        /// <summary>
        /// Retrieves a support ticket priority based on the provided request data.
        /// </summary>
        /// <param name="requestDto">Data transfer object containing request parameters for retrieving a support ticket priority.</param>
        /// <returns>A SupportTicketPriority object representing the retrieved priority.</returns>
        Task<SupportTicketPriority> GetSupportTicketPriority(SupportTicketPriorityRequestDto requestDto);

        /// <summary>
        /// Updates an existing support ticket priority.
        /// </summary>
        /// <param name="updateSupportTicketPriorityDto">Data transfer object for updating a support ticket priority.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateSupportTicketPriority(UpdateSupportTicketPriorityDto updateSupportTicketPriorityDto);

        /// <summary>
        /// Deletes a support ticket priority.
        /// </summary>
        /// <param name="deleteSupportTicketPriorityDto">Data transfer object for deleting a support ticket priority.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteSupportTicketPriority(DeleteSupportTicketPriorityDto deleteSupportTicketPriorityDto);

        /// <summary>
        /// Retrieves a list of support ticket priorities based on the provided request data.
        /// </summary>
        /// <param name="listSupportTicketPriorityRequestDto">Data transfer object containing request parameters for retrieving a list of support ticket priorities.</param>
        /// <returns>A list of SupportTicketPriority objects representing the retrieved priorities.</returns>
        Task<List<SupportTicketPriority>> GetListSupportTicketPriority(ListSupportTicketPriorityRequestDto listSupportTicketPriorityRequestDto);
    }
}
