CREATE VIEW [dbo].[vw_FundingLineContractTypeMapping]
	AS SELECT 
			f.Id as FundingLineId, 
			f.name as FundingLine,
			c.Id as ContractTypeId, 
			c.name as contract 
		FROM 
			FundingLineContractTypeMapping fm
 inner join FundingLine f 
		ON	fm.FundingLineId = f.id
 inner join ContractType c 
		ON fm.contracttypeId = c.id
