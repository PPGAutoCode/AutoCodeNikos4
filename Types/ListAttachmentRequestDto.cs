
namespace ProjectName.Types
{
    public class ListAttachmentRequestDto
    {
        public int? PageLimit { get; set; }
        public int? PageOffset { get; set; }
        public string? SortField { get; set; }
        public string? SortOrder { get; set; }
    }
}
