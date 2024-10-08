#Path: DevelopersPortal
#File: AppStatuses.sql
CREATE TABLE AppStatuses (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    Version int NULL,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7) NOT NULL,
    CreatorId uniqueidentifier NULL UNIQUE,
    ChangedUser uniqueidentifier NULL UNIQUE
)