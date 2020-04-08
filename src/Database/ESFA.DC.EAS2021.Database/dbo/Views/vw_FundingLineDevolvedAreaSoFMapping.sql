CREATE VIEW [dbo].[vw_FundingLineDevolvedAreaSoFMapping]
	AS SELECT 
			f.Id as FundingLineId, 
			f.name as FundingLine,			
			DevolvedAreaSoF 
		FROM 
			FundingLineDevolvedAreaSoFMapping fm
 inner join FundingLine f 
		ON	fm.FundingLineId = f.id

