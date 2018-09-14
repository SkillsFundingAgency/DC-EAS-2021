CREATE TABLE [dbo].[Payment_Types](
	[Payment_Id] [int] NOT NULL,
	[PaymentName] [nvarchar](250) NOT NULL,
	[FM36] [bit] NOT NULL,
	[SubSectionHeading] [nvarchar](250) NULL,
	[RowHeading] [nvarchar](250) NULL,
	[PaymentTypeDescription] [nvarchar](250) NULL,
 CONSTRAINT [PK_Payment_Types] PRIMARY KEY CLUSTERED 
(
	[Payment_Id] ASC
))
GO

ALTER TABLE [dbo].[Payment_Types] ADD  CONSTRAINT [DF_Payment_Types_FM36]  DEFAULT ((0)) FOR [FM36]
GO
