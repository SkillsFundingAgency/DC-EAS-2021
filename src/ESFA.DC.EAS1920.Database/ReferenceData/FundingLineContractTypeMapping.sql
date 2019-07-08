DECLARE @SummaryOfChanges_FundingLineContractTypeMapping TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [FundingLineContractTypeMapping] AS Target
USING (VALUES		
	(4,9),
	(11,9),
	(15,9),
	(5,9),
	(12,9),
	(16,9),
	(2,9),
	(9,9),
	(1,10),
	(8,10),
	(3,1),
	(10,8),
	(6,2),
	(7,2),
	(18,3),
	(13,3),
	(19,4),
	(14,4),
	(17,6),
	(18,5),
	(13,5),
	(17,7)
)
	AS Source([FundingLineId], [ContractTypeId])
	ON Target.[FundingLineId] = Source.[FundingLineId] 
		and  Target.[ContractTypeId] = Source.[ContractTypeId]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[FundingLineId] ,
							Target.[ContractTypeId] 																					  
					EXCEPT 
						SELECT Source.[FundingLineId] ,
							Source.[ContractTypeId]													      
				)
		  THEN UPDATE SET 
						Target.[FundingLineId] = Source.[FundingLineId],
						Target.[ContractTypeId] = Source.[ContractTypeId]					
	WHEN NOT MATCHED BY TARGET THEN INSERT([FundingLineId], [ContractTypeId]) VALUES ([FundingLineId], [ContractTypeId])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT Inserted.[FundingLineId],$action INTO @SummaryOfChanges_FundingLineContractTypeMapping([Id],[Action])
;


	DECLARE @AddCount_FundingLineContractTypeMappijng INT, @UpdateCount_FundingLineContractTypeMappijng INT, @DeleteCount_FundingLineContractTypeMappijng INT
	SET @AddCount_FundingLineContractTypeMappijng  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FundingLineContractTypeMapping WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_FundingLineContractTypeMappijng = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FundingLineContractTypeMapping WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_FundingLineContractTypeMappijng = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FundingLineContractTypeMapping WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount_FundingLineContractTypeMappijng, @UpdateCount_FundingLineContractTypeMappijng, @DeleteCount_FundingLineContractTypeMappijng) WITH NOWAIT;

