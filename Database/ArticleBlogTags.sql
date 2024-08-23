CREATE TABLE ArticleBlogTags (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    ArticleId uniqueidentifier NOT NULL,
    BlogTagId uniqueidentifier NOT NULL
);