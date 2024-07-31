
CREATE TABLE PhpSdkSettings (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    PhpSdkSettingsProfileName nvarchar(500) NULL,
    IDSPortalEnabled bit NULL,
    TokenUrl nvarchar(500) NULL,
    AuthorizationUrl nvarchar(500) NULL,
    UserInfoUrl nvarchar(500) NULL,
    PortalUrl nvarchar(max) NULL,
    ClientId nvarchar(500) NULL,
    ClientSecret nvarchar(500) NULL,
    GrantType nvarchar(500) NULL,
    Scope nvarchar(500) NULL,
    HeaderIBMClientId nvarchar(500) NULL,
    HeaderIBMClientSecret nvarchar(500) NULL,
    IDSConnectLogoutUrl nvarchar(500) NULL,
    ExternalNbgApisClientId nvarchar(500) NULL,
    ExternalNbgApisClientSecret nvarchar(500) NULL,
    ExternalNbgApisrGandType nvarchar(500) NULL,
    ExternalNbgApisScope nvarchar(500) NULL,
    NbgAnalyticsApiDisplayApplicationAnalytics bit NULL,
    NbgAnalyticsApiDevelopment bit NULL,
    NbgAnalyticsApiDevelopmentSandboxId nvarchar(500) NULL,
    NbgAnalyticsApiDevelopmentSumAggregatesPerEndpointUrl nvarchar(500) NULL,
    NbgAnalyticsApiDevelopmentSumAggregatesPerEndpointPerDayUrl nvarchar(500) NULL,
    NbgAnalyticsApiDevelopmentScopes nvarchar(500) NULL,
    NbgAnalyticsApiProductionSumAggregatesPerEndpointUrl nvarchar(500) NULL,
    NbgAnalyticsApiProductionSumAggregatesPerEndpointPerDayUrl nvarchar(500) NULL,
    NbgAnalyticsApiProductionScopes nvarchar(500) NULL,
    NbgCertificateValidationApiDevelopment bit NULL,
    NbgCertificateValidationApiDevelopmentSandboxId nvarchar(500) NULL,
    NbgCertificateValidationApiDevelopmentCertificateValidationUrl nvarchar(500) NULL,
    NbgCertificateValidationApiDevelopmentScopes nvarchar(500) NULL,
    NbgCertificateValidationApiProductionCertificateValidationUrl bit NULL,
    NbgCertificateValidationApiProductionScopes nvarchar(500) NULL,
    EmailUrlsReactUrl nvarchar(500) NULL,
    EmailUrlsDrupalUrl nvarchar(500) NULL,
    EmailUrlsDrupalUrlAdmin nvarchar(500) NULL,
    AdminEmailAddress nvarchar(500) NULL,
    GeneralConfigurationsMaxAppsPerUser nvarchar(500) NULL,
    GeneralConfigurationsDisableSnippetGeneration bit NULL,
    GeneralConfigurationsDisableDocumentationGeneration bit NULL,
    JsonApiRoles nvarchar(500) NULL
);
