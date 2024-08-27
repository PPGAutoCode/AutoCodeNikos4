// File: DeleteAPIEndpointDto.cs
namespace ProjectName.Types
{
    public class DeleteAPIEndpointDto
    {
        public Guid? Id { get; set; }
        public List<string>? FieldsToDelete { get; set; }
    }
}
