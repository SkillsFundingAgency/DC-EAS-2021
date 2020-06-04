DECLARE @SummaryOfChanges_FundingLineContractTypeMapping TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [FundingLineContractTypeMapping] AS Target
USING (VALUES		
	(1,1),
	(2,1),
	(3,1),
	(4,1),
	(5,1),
	(6,1),
	(7,1),
	(8,1),
	(9,2),
	(10,2),
	(11,3),
	(12,3),
	(13,4),
	(14,5),
	(15,6),
	(16,7),
	(17,8),
	(18,9),
	(19,10),
	(20,11),
	(20,12),
	(23,13),
	(24,13),
	(25,13),
	(26,13)
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

