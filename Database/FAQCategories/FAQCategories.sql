#Path: FAQCategories
#File: FAQCategories.sql

CREATE TABLE FAQCategories (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Name nvarchar(500) NOT NULL,
    Description nvarchar(500) NULL
);