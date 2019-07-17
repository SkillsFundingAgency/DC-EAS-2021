CREATE TABLE [dbo].[FundingLineDevolvedAreaSoFMapping](
	[FundingLineId] [int]  NOT NULL,
	[DevolvedAreaSoF] [int] NOT NULL,
 CONSTRAINT [PK_FundingLineDevolvedAreaSoFMapping] PRIMARY KEY CLUSTERED(	[FundingLineId] , [DevolvedAreaSoF] ),
 CONSTRAINT [FK_FundingLineDevolvedAreaSoFMapping_ToFundingLine] FOREIGN KEY ([FundingLineId]) REFERENCES [FundingLine]([Id]) ON DELETE CASCADE 
 )