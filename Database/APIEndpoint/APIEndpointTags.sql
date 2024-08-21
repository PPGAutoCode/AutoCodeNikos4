#Path: APIEndpoint
#File: APIEndpointTags.sql
CREATE TABLE APIEndpointTags (
    Id uniqueidentifier NOT NULL UNIQUE,
    APIEndpointId uniqueidentifier NOT NULL,
    APITagId uniqueidentifier NOT NULL
);