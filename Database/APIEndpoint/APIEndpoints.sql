#Path: APIEndpoint
#File: APIEndpoints.sql
CREATE TABLE APIEndpoints (
    Id uniqueidentifier NOT NULL UNIQUE,
    ApiName nvarchar(200) NOT NULL,
    ApiScope nvarchar(200) NULL,
    ApiScopeProduction nvarchar(200) NULL,
    Deprecated bit NULL,
    Description nvarchar(max) NULL,
    Documentation uniqueidentifier NULL,
    EndpointUrls nvarchar(200) NULL,
    AppEnvironment uniqueidentifier NOT NULL,
    Swagger uniqueidentifier NULL,
    Tour uniqueidentifier NULL,
    ApiVersion nvarchar(200) NULL,
    Langcode nvarchar(4) NOT NULL,
    Sticky bit NULL,
    Promote bit NULL,
    UrlAlias nvarchar(200) NOT NULL,
    Published bit NULL,
    Version int NULL,
    Created datetime2(7) NULL,
    Changed datetime2(7) NULL,
    CreatorId uniqueidentifier NULL,
    ChangedUser uniqueidentifier NULL
);