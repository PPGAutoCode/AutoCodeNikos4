#File: FAQFAQCategories.sql
CREATE TABLE FAQFAQCategories (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    FAQId UNIQUEIDENTIFIER NOT NULL,
    FAQCategoryId UNIQUEIDENTIFIER NOT NULL
);