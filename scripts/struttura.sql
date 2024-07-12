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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701173246_InitDB'
)
BEGIN
    CREATE TABLE [SegreteriaTecnica] (
        [Id] uniqueidentifier NOT NULL,
        [NomeCompleto] nvarchar(max) NULL,
        [EmailUtente] nvarchar(max) NULL,
        [UserProfileId] uniqueidentifier NOT NULL,
        [CREATION_DATE] datetime2 NOT NULL,
        [EDIT_TIME] datetime2 NOT NULL,
        [ParentId] uniqueidentifier NOT NULL,
        [ChildId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SegreteriaTecnica] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701173246_InitDB'
)
BEGIN
    CREATE TABLE [Strutture] (
        [Id] uniqueidentifier NOT NULL,
        [StrutturaCompleta] nvarchar(max) NULL,
        [NumeroLivelli] int NOT NULL,
        [StrutturaLiv1] nvarchar(max) NULL,
        [StrutturaLiv2] nvarchar(max) NULL,
        [StrutturaLiv3] nvarchar(max) NULL,
        [DirettaCollaborazione] bit NOT NULL,
        [IdCapoStruttura] uniqueidentifier NULL,
        [CapoStruttura] nvarchar(max) NULL,
        [EmailCapoStruttura] nvarchar(max) NULL,
        [IdCapoIntermedio] uniqueidentifier NULL,
        [CapoIntermedio] nvarchar(max) NULL,
        [EmailCapoIntermedio] nvarchar(max) NULL,
        [IdDirigenteResponsabile] uniqueidentifier NULL,
        [DirigenteResponsabile] nvarchar(max) NULL,
        [EmailDirigenteResponsabile] nvarchar(max) NULL,
        [IdResponsabileAccordo] uniqueidentifier NULL,
        [ResponsabileAccordo] nvarchar(max) NULL,
        [EmailResponsabileAccordo] nvarchar(max) NULL,
        [IdReferenteInterno] uniqueidentifier NULL,
        [ReferenteInterno] nvarchar(max) NULL,
        [EmailReferenteInterno] nvarchar(max) NULL,
        [CREATION_DATE] datetime2 NOT NULL,
        [EDIT_TIME] datetime2 NOT NULL,
        [ParentId] uniqueidentifier NOT NULL,
        [ChildId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Strutture] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240701173246_InitDB'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240701173246_InitDB', N'8.0.6');
END;
GO

COMMIT;
GO

