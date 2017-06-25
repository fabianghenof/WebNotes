IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [AccessFailedCount] int NOT NULL,
        [ConcurrencyStamp] nvarchar(max),
        [Email] nvarchar(256),
        [EmailConfirmed] bit NOT NULL,
        [LockoutEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset,
        [NormalizedEmail] nvarchar(256),
        [NormalizedUserName] nvarchar(256),
        [PasswordHash] nvarchar(max),
        [PhoneNumber] nvarchar(max),
        [PhoneNumberConfirmed] bit NOT NULL,
        [SecurityStamp] nvarchar(max),
        [TwoFactorEnabled] bit NOT NULL,
        [UserName] nvarchar(256),
        [WebnoteSortingOption] nvarchar(max),
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [ConcurrencyStamp] nvarchar(max),
        [Name] nvarchar(256),
        [NormalizedName] nvarchar(256),
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max),
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE TABLE [FileAttachments] (
        [Id] int NOT NULL IDENTITY,
        [FileData] varbinary(max),
        [Name] nvarchar(max),
        [Type] nvarchar(max),
        [UserId] nvarchar(450),
        CONSTRAINT [PK_FileAttachments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FileAttachments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [ClaimType] nvarchar(max),
        [ClaimValue] nvarchar(max),
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max),
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [ClaimType] nvarchar(max),
        [ClaimValue] nvarchar(max),
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE TABLE [WebNotes] (
        [Id] int NOT NULL IDENTITY,
        [Color] nvarchar(max),
        [Content] nvarchar(max),
        [FileId] int NOT NULL,
        [OrderIndex] int NOT NULL,
        [Title] nvarchar(max),
        [UserId] nvarchar(450),
        CONSTRAINT [PK_WebNotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WebNotes_FileAttachments_FileId] FOREIGN KEY ([FileId]) REFERENCES [FileAttachments] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_WebNotes_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE INDEX [IX_FileAttachments_UserId] ON [FileAttachments] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE INDEX [IX_WebNotes_FileId] ON [WebNotes] ([FileId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE INDEX [IX_WebNotes_UserId] ON [WebNotes] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625113128_finalFiles')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20170625113128_finalFiles', N'1.1.2');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625120913_finalFiles1')
BEGIN
    ALTER TABLE [WebNotes] ADD [hasFile] bit NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625120913_finalFiles1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20170625120913_finalFiles1', N'1.1.2');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625185718_finalFiles2')
BEGIN
    ALTER TABLE [FileAttachments] ADD [WebNoteId] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625185718_finalFiles2')
BEGIN
    CREATE INDEX [IX_FileAttachments_WebNoteId] ON [FileAttachments] ([WebNoteId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625185718_finalFiles2')
BEGIN
    ALTER TABLE [FileAttachments] ADD CONSTRAINT [FK_FileAttachments_WebNotes_WebNoteId] FOREIGN KEY ([WebNoteId]) REFERENCES [WebNotes] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625185718_finalFiles2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20170625185718_finalFiles2', N'1.1.2');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625190112_finalFiles3')
BEGIN
    ALTER TABLE [FileAttachments] DROP CONSTRAINT [FK_FileAttachments_WebNotes_WebNoteId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625190112_finalFiles3')
BEGIN
    DROP INDEX [IX_FileAttachments_WebNoteId] ON [FileAttachments];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625190112_finalFiles3')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'FileAttachments') AND [c].[name] = N'WebNoteId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [FileAttachments] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [FileAttachments] DROP COLUMN [WebNoteId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20170625190112_finalFiles3')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20170625190112_finalFiles3', N'1.1.2');
END;

GO

