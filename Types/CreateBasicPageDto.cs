
namespace ProjectName.Types
{
    public class CreateBasicPageDto
    {
        public string Name { get; set; }
        public string? Body { get; set; }
        public List<CreateImageDto>? Images { get; set; }
    }
}
