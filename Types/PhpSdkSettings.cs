
namespace ProjectName.Types
{
    public class PhpSdkSettings
    {
        public Guid? Id { get; set; }
        public string? PhpSdkSettingsProfileName { get; set; }
        public bool? IDSPortalEnabled { get; set; }
        public string? TokenUrl { get; set; }
        public string? AuthorizationUrl { get; set; }
        public string? UserInfoUrl { get; set; }
        public string? PortalUrl { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? GrantType { get; set; }
        public string? Scope { get; set; }
        public string? HeaderIBMClientId { get; set; }
        public string? HeaderIBMClientSecret { get; set; }
        public string? IDSConnectLogoutUrl { get; set; }
        public string? ExternalNbgApisClientId { get; set; }
        public string? ExternalNbgApisClientSecret { get; set; }
        public string? ExternalNbgApisrGandType { get; set; }
        public string? ExternalNbgApisScope { get; set; }
        public bool? NbgAnalyticsApiDisplayApplicationAnalytics { get; set; }
        public bool? NbgAnalyticsApiDevelopment { get; set; }
        public string? NbgAnalyticsApiDevelopmentSandboxId { get; set; }
        public string? NbgAnalyticsApiDevelopmentSumAggregatesPerEndpointUrl { get; set; }
        public string? NbgAnalyticsApiDevelopmentSumAggregatesPerEndpointPerDayUrl { get; set; }
        public string? NbgAnalyticsApiDevelopmentScopes { get; set; }
        public string? NbgAnalyticsApiProductionSumAggregatesPerEndpointUrl { get; set; }
        public string? NbgAnalyticsApiProductionSumAggregatesPerEndpointPerDayUrl { get; set; }
        public string? NbgAnalyticsApiProductionScopes { get; set; }
        public bool? NbgCertificateValidationApiDevelopment { get; set; }
        public string? NbgCertificateValidationApiDevelopmentSandboxId { get; set; }
        public string? NbgCertificateValidationApiDevelopmentCertificateValidationUrl { get; set; }
        public string? NbgCertificateValidationApiDevelopmentScopes { get; set; }
        public bool? NbgCertificateValidationApiProductionCertificateValidationUrl { get; set; }
        public string? NbgCertificateValidationApiProductionScopes { get; set; }
        public string? EmailUrlsReactUrl { get; set; }
        public string? EmailUrlsDrupalUrl { get; set; }
        public string? EmailUrlsDrupalUrlAdmin { get; set; }
        public string? AdminEmailAddress { get; set; }
        public string? GeneralConfigurationsMaxAppsPerUser { get; set; }
        public bool? GeneralConfigurationsDisableSnippetGeneration { get; set; }
        public bool? GeneralConfigurationsDisableDocumentationGeneration { get; set; }
        public string? JsonApiRoles { get; set; }
    }
}
