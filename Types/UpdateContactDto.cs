
namespace ProjectName.Types
{
    public class UpdateContactDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Mail { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}
