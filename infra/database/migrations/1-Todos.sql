IF SCHEMA_ID(N'Todos') IS NULL EXEC(N'CREATE SCHEMA [Todos];');

CREATE TABLE [Todos].[todos] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Completed] BIT NOT NULL,
    CONSTRAINT [PK_todos] PRIMARY KEY ([Id])
);

