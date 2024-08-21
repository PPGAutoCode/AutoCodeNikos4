// File: APIEndpointDto.cs
namespace ProjectName.Types
{
    public class APIEndpointDto
    {
        public Guid? Id { get; set; }
        public string? ApiName { get; set; }
        public string? ApiScope { get; set; }
        public string? ApiScopeProduction { get; set; }
        public List<ApiTag>? ApiTags { get; set; }
        public bool? Deprecated { get; set; }
        public string? Description { get; set; }
        public Attachment? Documentation { get; set; }
        public string? EndpointUrls { get; set; }
        public Guid? AppEnvironment { get; set; }
        public Attachment? Swagger { get; set; }
        public Attachment? Tour { get; set; }
        public string? ApiVersion { get; set; }
        public string? Langcode { get; set; }
        public bool? Sticky { get; set; }
        public bool? Promote { get; set; }
        public string? UrlAlias { get; set; }
        public bool? Published { get; set; }
        public int? Version { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? ChangedUser { get; set; }
    }
}
