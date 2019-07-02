CREATE TABLE [dbo].[SourceFile](
	[SourceFileId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](60) NOT NULL,
	[FilePreparationDate] [datetime] NOT NULL,
	[UKPRN] [nvarchar](20) NOT NULL,
	[DateTime] [datetime] NULL,
 CONSTRAINT [PK_SourceFile] PRIMARY KEY CLUSTERED 
(
	[SourceFileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]