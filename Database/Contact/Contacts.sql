
CREATE TABLE Contacts (
    Id uniqueidentifier PRIMARY KEY,
    Name nvarchar(500) NOT NULL,
    Mail nvarchar(500) NOT NULL,
    Subject nvarchar(500) NOT NULL,
    Message nvarchar(max) NOT NULL,
    Version int NULL,
    Created datetime2(7) NULL,
    Changed datetime2(7) NULL,
    CreatorId uniqueidentifier NULL,
    ChangedUser uniqueidentifier NULL
);
