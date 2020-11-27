CREATE TABLE [dbo].[ValidationErrorRules](
	[RuleId] [nvarchar](50) NOT NULL,
	[Severity] [nvarchar](1) NOT NULL,	
	[Message] [nvarchar](2000) NOT NULL,
	[SeverityFIS] [nvarchar](1) NOT NULL
 CONSTRAINT [PK_ValidationErrorRules] PRIMARY KEY CLUSTERED([RuleId] ASC))