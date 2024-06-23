IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [DataProtectionKeys] (
    [Id] int NOT NULL IDENTITY,
    [FriendlyName] nvarchar(max) NULL,
    [Xml] nvarchar(max) NULL,
    CONSTRAINT [PK_DataProtectionKeys] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240623082301_DataProtection', N'8.0.6');
GO

COMMIT;
GO

