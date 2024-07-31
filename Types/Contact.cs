
namespace ProjectName.Types
{
    public class Contact
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int? Version { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? ChangedUser { get; set; }
    }
}
