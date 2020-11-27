DECLARE @SummaryOfChanges_FundingLineDevolvedAreaSoFMapping TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [FundingLineDevolvedAreaSoFMapping] AS Target
USING (VALUES		
	(21,110),
	(21,111),
	(21,112),
	(21,113),
	(21,114),
	(21,115),
	(21,116),
	(21,117),
	(22,110),
	(22,111),
	(22,112),
	(22,113),
	(22,114),
	(22,115),
	(22,116),
	(22,117)
)
	AS Source([FundingLineId], [DevolvedAreaSoF])
	ON Target.[FundingLineId] = Source.[FundingLineId] 
		and  Target.[DevolvedAreaSoF] = Source.[DevolvedAreaSoF]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[FundingLineId] ,
							Target.[DevolvedAreaSoF] 																					  
					EXCEPT 
						SELECT Source.[FundingLineId] ,
							Source.[DevolvedAreaSoF]													      
				)
		  THEN UPDATE SET 
						Target.[FundingLineId] = Source.[FundingLineId],
						Target.[DevolvedAreaSoF] = Source.[DevolvedAreaSoF]					
	WHEN NOT MATCHED BY TARGET THEN INSERT([FundingLineId], [DevolvedAreaSoF]) VALUES ([FundingLineId], [DevolvedAreaSoF])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT Inserted.[FundingLineId],$action INTO @SummaryOfChanges_FundingLineDevolvedAreaSoFMapping([Id],[Action])
;


	DECLARE @AddCount_FundingLineDevolvedAreaSoFMapping INT, @UpdateCount_FundingLineDevolvedAreaSoFMapping INT, @DeleteCount_FundingLineDevolvedAreaSoFMapping INT
	SET @AddCount_FundingLineDevolvedAreaSoFMapping  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FundingLineDevolvedAreaSoFMapping WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_FundingLineDevolvedAreaSoFMapping = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FundingLineDevolvedAreaSoFMapping WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_FundingLineDevolvedAreaSoFMapping = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FundingLineDevolvedAreaSoFMapping WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount_FundingLineDevolvedAreaSoFMapping, @UpdateCount_FundingLineDevolvedAreaSoFMapping, @DeleteCount_FundingLineDevolvedAreaSoFMapping) WITH NOWAIT;

