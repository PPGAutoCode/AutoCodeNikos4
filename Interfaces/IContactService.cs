
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing contact-related operations.
    /// </summary>
    public interface IContactService
    {
        /// <summary>
        /// Asynchronously creates a new contact.
        /// </summary>
        /// <param name="createContactDto">The data transfer object containing information for creating a new contact.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ID of the created contact as a string.</returns>
        Task<string> CreateContact(CreateContactDto createContactDto);

        /// <summary>
        /// Asynchronously retrieves a contact by the given request.
        /// </summary>
        /// <param name="contactRequestDto">The data transfer object containing the request for retrieving a contact.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved contact.</returns>
        Task<Contact> GetContact(ContactRequestDto contactRequestDto);

        /// <summary>
        /// Asynchronously updates an existing contact.
        /// </summary>
        /// <param name="updateContactDto">The data transfer object containing information for updating an existing contact.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ID of the updated contact as a string.</returns>
        Task<string> UpdateContact(UpdateContactDto updateContactDto);

        /// <summary>
        /// Asynchronously deletes a contact by the given request.
        /// </summary>
        /// <param name="deleteContactDto">The data transfer object containing the request for deleting a contact.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteContact(DeleteContactDto deleteContactDto);

        /// <summary>
        /// Asynchronously retrieves a list of contacts based on the given request.
        /// </summary>
        /// <param name="listContactRequestDto">The data transfer object containing the request for retrieving a list of contacts.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of retrieved contacts.</returns>
        Task<List<Contact>> GetListContact(ListContactRequestDto listContactRequestDto);
    }
}
