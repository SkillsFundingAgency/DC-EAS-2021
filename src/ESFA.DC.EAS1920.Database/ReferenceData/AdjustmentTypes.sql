DECLARE @SummaryOfChanges_AdjustmentTypes TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [AdjustmentType] AS Target
USING (VALUES		
	(1,N'ALLB Excess Support'),
	(2,N'Authorised Claims'),
	(3,N'Authorised Claims - Additional payments for apprentice'),
	(4,N'Authorised Claims - Additional payments for employer'),
	(5,N'Authorised Claims - Additional payments for provider'),
	(6,N'Authorised Claims - Training costs exc Maths/Eng'),
	(7,N'Discretionary Bursary'),
	(8,N'Excess Learning Support'),
	(9,N'Free Meals'),
	(10,N'Learner Support'),
	(11,N'Princes Trust'),
	(12,N'Vulnerable Bursary'),
	(13,N'MCA/GLA Defined Adjustment 1'),
	(14,N'MCA/GLA Defined Adjustment 2'),
	(15,N'MCA/GLA Defined Adjustment 3')
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
	OUTPUT Inserted.[Id],$action INTO @SummaryOfChanges_AdjustmentTypes([Id],[Action])
;


	DECLARE @AddCount_AdjustmentTypes INT, @UpdateCount_AdjustmentTypes INT, @DeleteCount_AdjustmentTypes INT
	SET @AddCount_AdjustmentTypes  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_AdjustmentTypes WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_AdjustmentTypes = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_AdjustmentTypes WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_AdjustmentTypes = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_AdjustmentTypes WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount_AdjustmentTypes, @UpdateCount_AdjustmentTypes, @DeleteCount_AdjustmentTypes) WITH NOWAIT;

