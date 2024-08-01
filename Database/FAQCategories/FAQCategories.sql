#Path: FAQCategories
#File: FAQCategories.sql

CREATE TABLE FAQCategories (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Name NVARCHAR(500) NOT NULL,
    Description NVARCHAR(500) NULL
);