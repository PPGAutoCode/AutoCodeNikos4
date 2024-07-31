
CREATE TABLE BasicPageImages (
    Id uniqueidentifier PRIMARY KEY,
    BasicPageId uniqueidentifier NOT NULL,
    ImageId uniqueidentifier NOT NULL,
    FOREIGN KEY (BasicPageId) REFERENCES BasicPages(Id),
    FOREIGN KEY (ImageId) REFERENCES Images(Id)
);
