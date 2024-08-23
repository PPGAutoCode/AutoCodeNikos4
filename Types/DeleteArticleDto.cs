// File: DeleteArticleDto.cs
namespace ProjectName.Types
{
    public class DeleteArticleDto
    {
        public Guid? Id { get; set; }
        public List<string>? FieldsToDelete { get; set; }
    }
}
