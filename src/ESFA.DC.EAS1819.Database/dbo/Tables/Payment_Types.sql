CREATE TABLE [dbo].[Payment_Types](
	[Payment_Id] [int] NOT NULL,
	[PaymentName] [nvarchar](250) NOT NULL,
	[FM36] [bit] NOT NULL,	
	[PaymentTypeDescription] [nvarchar](250) NULL,	
	[FundingLineId] INT NULL,
	[AdjustmentTypeId] INT NULL,
 CONSTRAINT [PK_Payment_Types] PRIMARY KEY CLUSTERED (	[Payment_Id] ASC),
 CONSTRAINT [FK_PaymentTypes_ToFundingLine] FOREIGN KEY (FundingLineId) REFERENCES [FundingLine]([Id]) ON DELETE CASCADE,
 CONSTRAINT [FK_PaymentTypes_ToAdjustmentType] FOREIGN KEY (AdjustmentTypeId) REFERENCES AdjustmentType([Id]) ON DELETE CASCADE)
GO

ALTER TABLE [dbo].[Payment_Types] ADD  CONSTRAINT [DF_Payment_Types_FM36]  DEFAULT ((0)) FOR [FM36]
GO
