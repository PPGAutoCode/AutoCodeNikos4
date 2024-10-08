CREATE TABLE Articles (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Title nvarchar(200) NOT NULL,
    Author uniqueidentifier NOT NULL,
    Summary nvarchar(500) NULL,
    Body nvarchar(max) NULL,
    GoogleDriveId nvarchar(50) NULL,
    HideScrollSpy bit NULL,
    Image uniqueidentifier NULL,
    Pdf uniqueidentifier NULL,
    Langcode nvarchar(4) NOT NULL,
    Status bit NULL,
    Sticky bit NULL,
    Promote bit NULL,
    Version int NULL,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7) NOT NULL,
    CreatorId uniqueidentifier NULL,
    ChangedUser uniqueidentifier NULL
);