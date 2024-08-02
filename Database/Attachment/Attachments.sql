CREATE TABLE Attachments (
    Id uniqueidentifier PRIMARY KEY,
    FileName nvarchar(100),
    FileData nvarchar(max) NOT NULL,
    Version int,
    Created datetime2(7),
    Changed datetime2(7),
    CreatorId uniqueidentifier,
    ChangedUser uniqueidentifier
);