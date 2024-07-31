
namespace ProjectName.Types
{
    public class UpdateBasicPageDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Body { get; set; }
        public List<UpdateImageDto>? Images { get; set; }
    }
}
