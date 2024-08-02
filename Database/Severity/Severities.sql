#Path: Severity
#File: Severities.sql
CREATE TABLE Severities (
    Id uniqueidentifier PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    Version int NULL,
    Created datetime2(7) NULL,
    Changed datetime2(7) NULL,
    CreatorId uniqueidentifier NULL,
    ChangedUser uniqueidentifier NULL
);