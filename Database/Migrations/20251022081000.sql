IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Recipes')
BEGIN
    CREATE TABLE Recipes (
        Id INT IDENTITY PRIMARY KEY,
        Title NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        Ingredients NVARCHAR(MAX),
        Steps NVARCHAR(MAX),
        AuthorId UNIQUEIDENTIFIER NOT NULL,
        FOREIGN KEY (AuthorId) REFERENCES Users(Id)
    );
END