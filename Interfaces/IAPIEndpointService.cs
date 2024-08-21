
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing API endpoints.
    /// </summary>
    public interface IAPIEndpointService
    {
        /// <summary>
        /// Creates a new API endpoint.
        /// </summary>
        /// <param name="createAPIEndpointDto">Data transfer object for creating an API endpoint.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateAPIEndpoint(CreateAPIEndpointDto createAPIEndpointDto);

        /// <summary>
        /// Retrieves an API endpoint based on the provided request data.
        /// </summary>
        /// <param name="apiEndpointRequestDto">Data transfer object for requesting an API endpoint.</param>
        /// <returns>An API endpoint data transfer object.</returns>
        Task<APIEndpointDto> GetAPIEndpoint(APIEndpointRequestDto apiEndpointRequestDto);

        /// <summary>
        /// Updates an existing API endpoint.
        /// </summary>
        /// <param name="updateAPIEndpointDto">Data transfer object for updating an API endpoint.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateAPIEndpoint(UpdateAPIEndpointDto updateAPIEndpointDto);

        /// <summary>
        /// Deletes an API endpoint.
        /// </summary>
        /// <param name="deleteAPIEndpointDto">Data transfer object for deleting an API endpoint.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteAPIEndpoint(DeleteAPIEndpointDto deleteAPIEndpointDto);

        /// <summary>
        /// Retrieves a list of API endpoints based on the provided request data.
        /// </summary>
        /// <param name="listAPIEndpointRequestDto">Data transfer object for requesting a list of API endpoints.</param>
        /// <returns>A list of API endpoint data transfer objects.</returns>
        Task<List<APIEndpointDto>> GetListAPIEndpoint(ListAPIEndpointRequestDto listAPIEndpointRequestDto);
    }
}
