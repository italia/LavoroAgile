SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (
    SELECT 1
    FROM sys.schemas
    WHERE name = 'cap'
)
BEGIN
    EXEC('CREATE SCHEMA cap');
END
GO
IF OBJECT_ID(N'[cap].[Received]') IS NULL
BEGIN
CREATE TABLE [cap].[Received](
	[Id] [bigint] NOT NULL,
	[Version] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Group] [nvarchar](200) NULL,
	[Content] [nvarchar](max) NULL,
	[Retries] [int] NOT NULL,
	[Added] [datetime2](7) NOT NULL,
	[ExpiresAt] [datetime2](7) NULL,
	[StatusName] [nvarchar](50) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
ALTER TABLE [cap].[Received] ADD  CONSTRAINT [PK_cap.Received] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO


IF OBJECT_ID(N'[cap].[Published]') IS NULL
BEGIN
CREATE TABLE [cap].[Published](
	[Id] [bigint] NOT NULL,
	[Version] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Retries] [int] NOT NULL,
	[Added] [datetime2](7) NOT NULL,
	[ExpiresAt] [datetime2](7) NULL,
	[StatusName] [nvarchar](50) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
ALTER TABLE [cap].[Published] ADD  CONSTRAINT [PK_cap.Published] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO