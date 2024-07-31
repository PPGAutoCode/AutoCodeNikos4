
namespace ProjectName.Types
{
    public class ListContactRequestDto
    {
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public int? PageLimit { get; set; }
        public int? PageOffset { get; set; }
    }
}
