CREATE TABLE ArticleBlogCategories (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ArticleId uniqueidentifier NOT NULL,
    BlogCategoryId uniqueidentifier NOT NULL
);