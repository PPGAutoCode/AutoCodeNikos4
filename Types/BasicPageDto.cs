
namespace ProjectName.Types
{
    public class BasicPageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Body { get; set; }
        public List<Image>? Images { get; set; }
    }
}
