SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (
    SELECT 1
    FROM sys.schemas
    WHERE name = 'Elsa'
)
BEGIN
    EXEC('CREATE SCHEMA Elsa');
END
GO

IF OBJECT_ID(N'[Elsa].[Bookmarks]') IS NULL
BEGIN
CREATE TABLE [Elsa].[Bookmarks](
	[Id] [nvarchar](450) NOT NULL,
	[TenantId] [nvarchar](450) NULL,
	[Hash] [nvarchar](450) NOT NULL,
	[Model] [nvarchar](max) NOT NULL,
	[ModelType] [nvarchar](max) NOT NULL,
	[ActivityType] [nvarchar](450) NOT NULL,
	[ActivityId] [nvarchar](450) NOT NULL,
	[WorkflowInstanceId] [nvarchar](450) NOT NULL,
	[CorrelationId] [nvarchar](450) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [Elsa].[Bookmarks] ADD  CONSTRAINT [PK_Bookmarks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Bookmark_ActivityId] ON [Elsa].[Bookmarks]
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Bookmark_ActivityType] ON [Elsa].[Bookmarks]
(
	[ActivityType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Bookmark_ActivityType_TenantId_Hash] ON [Elsa].[Bookmarks]
(
	[ActivityType] ASC,
	[TenantId] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Bookmark_CorrelationId] ON [Elsa].[Bookmarks]
(
	[CorrelationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Bookmark_Hash] ON [Elsa].[Bookmarks]
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Bookmark_Hash_CorrelationId_TenantId] ON [Elsa].[Bookmarks]
(
	[Hash] ASC,
	[CorrelationId] ASC,
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Bookmark_TenantId] ON [Elsa].[Bookmarks]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Bookmark_WorkflowInstanceId] ON [Elsa].[Bookmarks]
(
	[WorkflowInstanceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
ALTER TABLE [Elsa].[Bookmarks] ADD  DEFAULT (N'') FOR [CorrelationId]
END
GO

IF OBJECT_ID(N'[Elsa].[Triggers]') IS NULL
BEGIN
CREATE TABLE [Elsa].[Triggers](
	[Id] [nvarchar](450) NOT NULL,
	[TenantId] [nvarchar](450) NULL,
	[Hash] [nvarchar](450) NOT NULL,
	[Model] [nvarchar](max) NOT NULL,
	[ModelType] [nvarchar](max) NOT NULL,
	[ActivityType] [nvarchar](450) NOT NULL,
	[ActivityId] [nvarchar](450) NOT NULL,
	[WorkflowDefinitionId] [nvarchar](450) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [Elsa].[Triggers] ADD  CONSTRAINT [PK_Triggers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Trigger_ActivityId] ON [Elsa].[Triggers]
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Trigger_ActivityType] ON [Elsa].[Triggers]
(
	[ActivityType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Trigger_ActivityType_TenantId_Hash] ON [Elsa].[Triggers]
(
	[ActivityType] ASC,
	[TenantId] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Trigger_Hash] ON [Elsa].[Triggers]
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Trigger_Hash_TenantId] ON [Elsa].[Triggers]
(
	[Hash] ASC,
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Trigger_TenantId] ON [Elsa].[Triggers]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_Trigger_WorkflowDefinitionId] ON [Elsa].[Triggers]
(
	[WorkflowDefinitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END
GO


IF OBJECT_ID(N'[Elsa].[WorkflowDefinitions]') IS NULL
BEGIN
CREATE TABLE [Elsa].[WorkflowDefinitions](
	[Id] [nvarchar](450) NOT NULL,
	[DefinitionId] [nvarchar](450) NOT NULL,
	[TenantId] [nvarchar](450) NULL,
	[Name] [nvarchar](450) NULL,
	[DisplayName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Version] [int] NOT NULL,
	[IsSingleton] [bit] NOT NULL,
	[PersistenceBehavior] [int] NOT NULL,
	[DeleteCompletedInstances] [bit] NOT NULL,
	[IsPublished] [bit] NOT NULL,
	[IsLatest] [bit] NOT NULL,
	[Tag] [nvarchar](450) NULL,
	[Data] [nvarchar](max) NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [Elsa].[WorkflowDefinitions] ADD  CONSTRAINT [PK_WorkflowDefinitions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE UNIQUE NONCLUSTERED INDEX [IX_WorkflowDefinition_DefinitionId_VersionId] ON [Elsa].[WorkflowDefinitions]
(
	[DefinitionId] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WorkflowDefinition_IsLatest] ON [Elsa].[WorkflowDefinitions]
(
	[IsLatest] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WorkflowDefinition_IsPublished] ON [Elsa].[WorkflowDefinitions]
(
	[IsPublished] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowDefinition_Name] ON [Elsa].[WorkflowDefinitions]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowDefinition_Tag] ON [Elsa].[WorkflowDefinitions]
(
	[Tag] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowDefinition_TenantId] ON [Elsa].[WorkflowDefinitions]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WorkflowDefinition_Version] ON [Elsa].[WorkflowDefinitions]
(
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
ALTER TABLE [Elsa].[WorkflowDefinitions] ADD  DEFAULT ('0001-01-01T00:00:00.0000000+00:00') FOR [CreatedAt]
END
GO

IF OBJECT_ID(N'[Elsa].[WorkflowExecutionLogRecords]') IS NULL
BEGIN
CREATE TABLE [Elsa].[WorkflowExecutionLogRecords](
	[Id] [nvarchar](450) NOT NULL,
	[TenantId] [nvarchar](450) NULL,
	[WorkflowInstanceId] [nvarchar](450) NOT NULL,
	[ActivityId] [nvarchar](450) NOT NULL,
	[ActivityType] [nvarchar](450) NOT NULL,
	[Timestamp] [datetimeoffset](7) NOT NULL,
	[EventName] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[Source] [nvarchar](max) NULL,
	[Data] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [Elsa].[WorkflowExecutionLogRecords] ADD  CONSTRAINT [PK_WorkflowExecutionLogRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowExecutionLogRecord_ActivityId] ON [Elsa].[WorkflowExecutionLogRecords]
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowExecutionLogRecord_ActivityType] ON [Elsa].[WorkflowExecutionLogRecords]
(
	[ActivityType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowExecutionLogRecord_TenantId] ON [Elsa].[WorkflowExecutionLogRecords]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WorkflowExecutionLogRecord_Timestamp] ON [Elsa].[WorkflowExecutionLogRecords]
(
	[Timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowExecutionLogRecord_WorkflowInstanceId] ON [Elsa].[WorkflowExecutionLogRecords]
(
	[WorkflowInstanceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO

IF OBJECT_ID(N'[Elsa].[WorkflowInstances]') IS NULL
BEGIN
CREATE TABLE [Elsa].[WorkflowInstances](
	[Id] [nvarchar](450) NOT NULL,
	[DefinitionId] [nvarchar](450) NOT NULL,
	[TenantId] [nvarchar](450) NULL,
	[Version] [int] NOT NULL,
	[WorkflowStatus] [int] NOT NULL,
	[CorrelationId] [nvarchar](450) NOT NULL,
	[ContextType] [nvarchar](450) NULL,
	[ContextId] [nvarchar](450) NULL,
	[Name] [nvarchar](450) NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[LastExecutedAt] [datetimeoffset](7) NULL,
	[FinishedAt] [datetimeoffset](7) NULL,
	[CancelledAt] [datetimeoffset](7) NULL,
	[FaultedAt] [datetimeoffset](7) NULL,
	[Data] [nvarchar](max) NULL,
	[LastExecutedActivityId] [nvarchar](max) NULL,
	[DefinitionVersionId] [nvarchar](450) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [Elsa].[WorkflowInstances] ADD  CONSTRAINT [PK_WorkflowInstances] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_ContextId] ON [Elsa].[WorkflowInstances]
(
	[ContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_ContextType] ON [Elsa].[WorkflowInstances]
(
	[ContextType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_CorrelationId] ON [Elsa].[WorkflowInstances]
(
	[CorrelationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_CreatedAt] ON [Elsa].[WorkflowInstances]
(
	[CreatedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_DefinitionId] ON [Elsa].[WorkflowInstances]
(
	[DefinitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_DefinitionVersionId] ON [Elsa].[WorkflowInstances]
(
	[DefinitionVersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_FaultedAt] ON [Elsa].[WorkflowInstances]
(
	[FaultedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_FinishedAt] ON [Elsa].[WorkflowInstances]
(
	[FinishedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_LastExecutedAt] ON [Elsa].[WorkflowInstances]
(
	[LastExecutedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_Name] ON [Elsa].[WorkflowInstances]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_TenantId] ON [Elsa].[WorkflowInstances]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_WorkflowStatus] ON [Elsa].[WorkflowInstances]
(
	[WorkflowStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_WorkflowStatus_DefinitionId] ON [Elsa].[WorkflowInstances]
(
	[WorkflowStatus] ASC,
	[DefinitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_PADDING ON
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_WorkflowStatus_DefinitionId_Version] ON [Elsa].[WorkflowInstances]
(
	[WorkflowStatus] ASC,
	[DefinitionId] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
ALTER TABLE [Elsa].[WorkflowInstances] ADD  DEFAULT (N'') FOR [CorrelationId]
END
GO

IF OBJECT_ID(N'[Elsa].[__EFMigrationsHistory]') IS NULL
BEGIN
CREATE TABLE [Elsa].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL
) ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [Elsa].[__EFMigrationsHistory] ADD  CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO

IF NOT EXISTS(SELECT * FROM [Elsa].[__EFMigrationsHistory] WHERE [MigrationId] = N'20210523093504_Initial')
BEGIN
    INSERT INTO [Elsa].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210523093504_Initial', N'5.0.9');
END;
IF NOT EXISTS(SELECT * FROM [Elsa].[__EFMigrationsHistory] WHERE [MigrationId] = N'20210611200049_Update21')
BEGIN
    INSERT INTO [Elsa].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210611200049_Update21', N'5.0.9');
END;
IF NOT EXISTS(SELECT * FROM [Elsa].[__EFMigrationsHistory] WHERE [MigrationId] = N'20210923112224_Update23')
BEGIN
    INSERT INTO [Elsa].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210923112224_Update23', N'5.0.10');
END;
IF NOT EXISTS(SELECT * FROM [Elsa].[__EFMigrationsHistory] WHERE [MigrationId] = N'20211215100215_Update24')
BEGIN
    INSERT INTO [Elsa].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211215100215_Update24', N'8.0.6');
END;
IF NOT EXISTS(SELECT * FROM [Elsa].[__EFMigrationsHistory] WHERE [MigrationId] = N'20220120170305_Update241')
BEGIN
    INSERT INTO [Elsa].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220120170305_Update241', N'8.0.6');
END;
IF NOT EXISTS(SELECT * FROM [Elsa].[__EFMigrationsHistory] WHERE [MigrationId] = N'20220120204205_Update25')
BEGIN
    INSERT INTO [Elsa].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220120204205_Update25', N'8.0.6');
END;
IF NOT EXISTS(SELECT * FROM [Elsa].[__EFMigrationsHistory] WHERE [MigrationId] = N'20220512203701_Update28')
BEGIN
    INSERT INTO [Elsa].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220512203701_Update28', N'8.0.6');
END;