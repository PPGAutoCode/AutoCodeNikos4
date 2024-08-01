
namespace ProjectName.Types
{
    public class ProductCategory
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public bool? UserQuestionnaire { get; set; }
        public string? Description { get; set; }
        public Guid? Parent { get; set; }
        public string? UrlAlias { get; set; }
        public int? Weight { get; set; }
    }
}
