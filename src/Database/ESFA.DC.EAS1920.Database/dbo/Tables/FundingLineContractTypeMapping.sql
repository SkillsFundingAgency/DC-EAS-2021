CREATE TABLE [dbo].[FundingLineContractTypeMapping](
	[FundingLineId] [int]  NOT NULL,
	[ContractTypeId] [int] NOT NULL,
 CONSTRAINT [PK_FundingLineContractTypeMapping] PRIMARY KEY CLUSTERED(	[FundingLineId] , [ContractTypeId] ),
 CONSTRAINT [FK_FundingLineContractTypeMapping_ToFundingLine] FOREIGN KEY ([FundingLineId]) REFERENCES [FundingLine]([Id]) ON DELETE CASCADE,
 CONSTRAINT [FK_FundingLineContractTypeMapping_ToContractType] FOREIGN KEY ([ContractTypeId]) REFERENCES [ContractType]([Id]) ON DELETE CASCADE
 )