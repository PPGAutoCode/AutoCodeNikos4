
#Path: AllowedGrantType
#File: AllowedGrantTypes.sql

CREATE TABLE AllowedGrantTypes (
    Id uniqueidentifier PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    Version int NULL,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7) NOT NULL,
    CreatorId uniqueidentifier NULL,
    ChangedUser uniqueidentifier NULL
);
