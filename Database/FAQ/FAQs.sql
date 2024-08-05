CREATE TABLE FAQs (
    Id uniqueidentifier PRIMARY KEY,
    Question nvarchar(500) NOT NULL,
    Answer nvarchar(max) NOT NULL,
    Langcode nvarchar(4) NOT NULL CHECK (Langcode IN ('el', 'en')),
    Status bit NULL,
    FaqOrder int NULL,
    Version int NULL,
    Created datetime2(7) NULL,
    Changed datetime2(7) NULL,
    CreatorId uniqueidentifier NULL,
    ChangedUser uniqueidentifier NULL
);