
CREATE TABLE [dbo].[ValidationError](
	[SourceFileId] [int] NOT NULL,
	[ValidationError_Id] [int] IDENTITY(1,1) NOT NULL,
	[RowId] [uniqueidentifier] NULL,
	[RuleId] [varchar](50) NULL,
	[FundingLine]  [varchar](max) NULL,
	[AdjustmentType]  [varchar](max) NULL,	
	[CalendarYear] [varchar](max) NULL,
	[CalendarMonth] [varchar](max) NULL,	
	[Severity] [varchar](2) NULL,
	[ErrorMessage] [varchar](max) NULL,	
	[Value] [varchar](max) NULL,	
	[CreatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SourceFileId] ASC,
	[ValidationError_Id] ASC
))