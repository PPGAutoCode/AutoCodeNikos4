// File: ListArticleRequestDto.cs
namespace ProjectName.Types
{
    public class ListArticleRequestDto
    {
        public int? PageLimit { get; set; }
        public int? PageOffset { get; set; }
        public string? SortField { get; set; }
        public string? SortOrder { get; set; }
        public Guid? Author { get; set; }
        public Guid? BlogCategory { get; set; }
        public Guid? BlogTag { get; set; }
    }
}