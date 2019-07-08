DECLARE @SummaryOfChanges_ContractTypes TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [ContractType] AS Target
USING (VALUES		
	(1,N'16-18NLAP2018'),
	(2,N'16-18TRN1819'),
	(3,N'AEBC1819'),
	(4,N'AEB-TOL1819'),
	(5,N'AEBTO-TOL1819'),
	(6,N'ALLB1819'),
	(7,N'ALLBC1819'),
	(8,N'ANLAP2018'),
	(9,N'APPS1819'),
	(10,N'LEVY1799')
)
	AS Source([Id], [Name])
	ON Target.[Id] = Source.[Id]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[Id] ,
							Target.[Name] 																					  
					EXCEPT 
						SELECT Source.[Id] ,
							Source.[Name]													      
				)
		  THEN UPDATE SET 
						Target.[Id] = Source.[Id],
						Target.[Name] = Source.[Name]					
	WHEN NOT MATCHED BY TARGET THEN INSERT([Id], [Name]) VALUES ([Id], [Name])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT Inserted.[Id],$action INTO @SummaryOfChanges_ContractTypes([Id],[Action])
;


	DECLARE @AddCount_ContractType INT, @UpdateCount_ContractType INT, @DeleteCount_ContractType INT
	SET @AddCount_ContractType  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ContractTypes WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_ContractType = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ContractTypes WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_ContractType = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ContractTypes WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount_ContractType, @UpdateCount_ContractType, @DeleteCount_ContractType) WITH NOWAIT;

