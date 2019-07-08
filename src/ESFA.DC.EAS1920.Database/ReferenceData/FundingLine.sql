DECLARE @SummaryOfChanges_FundingLines TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [FundingLine] AS Target
USING (VALUES		
	(1,N'16-18 Apprenticeship (From May 2017) Levy Contract'),
	(2,N'16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)'),
	(3,N'16-18 Apprenticeship Non-Levy Contract (procured)'),
	(4,N'16-18 Apprenticeships'),
	(5,N'16-18 Trailblazer Apprenticeships'),
	(6,N'16-18 Traineeships'),
	(7,N'16-19 Traineeships Bursary'),
	(8,N'19+ Apprenticeship (From May 2017) Levy Contract'),
	(9,N'19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)'),
	(10,N'19+ Apprenticeship Non-Levy Contract (procured)'),
	(11,N'19-23 Apprenticeships'),
	(12,N'19-23 Trailblazer Apprenticeships'),
	(13,N'19-24 Traineeships (non-procured)'),
	(14,N'19-24 Traineeships (procured from Nov 2017)'),
	(15,N'24+ Apprenticeships'),
	(16,N'24+ Trailblazer Apprenticeships'),
	(17,N'Advanced Learner Loans Bursary'),
	(18,N'AEB - Other Learning (non-procured)'),
	(19,N'AEB - Other Learning (procured from Nov 2017)')
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
	OUTPUT Inserted.[Id],$action INTO @SummaryOfChanges_FundingLines([Id],[Action])
;


	DECLARE @AddCount_FundingLine INT, @UpdateCount_FundingLine INT, @DeleteCount_FundingLine INT
	SET @AddCount_FundingLine  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FundingLines WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_FundingLine = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FundingLines WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_FundingLine = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FundingLines WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount_FundingLine, @UpdateCount_FundingLine, @DeleteCount_FundingLine) WITH NOWAIT;

