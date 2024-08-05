// File: FAQDto.cs
namespace ProjectName.Types
{
    public class FAQDto
    {
        public Guid? Id { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public List<FAQCategory>? FAQCategories { get; set; }
        public string? Langcode { get; set; }
        public bool? Status { get; set; }
        public int? FaqOrder { get; set; }
        public int? Version { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? ChangedUser { get; set; }
    }
}
