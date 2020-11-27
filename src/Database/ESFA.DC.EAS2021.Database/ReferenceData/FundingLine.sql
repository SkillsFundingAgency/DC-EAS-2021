DECLARE @SummaryOfChanges_FundingLines TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [FundingLine] AS Target
USING (VALUES
(1,N'16-18 Apprenticeships'),
(2,N'19-23 Apprenticeships'),
(3,N'24+ Apprenticeships'),
(4,N'16-18 Trailblazer Apprenticeships'),
(5,N'19-23 Trailblazer Apprenticeships'),
(6,N'24+ Trailblazer Apprenticeships'),
(7,N'16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)'),
(8,N'19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)'),
(9,N'16-18 Apprenticeship (Employer on App Service) Levy funding'),
(10,N'19+ Apprenticeship (Employer on App Service) Levy funding'),
(11,N'16-18 Apprenticeship (Employer on App Service) Non-Levy funding'),
(12,N'19+ Apprenticeship (Employer on App Service) Non-Levy funding'),
(13,N'16-18 Apprenticeship Non-Levy Contract (procured)'),
(14,N'19+ Apprenticeship Non-Levy Contract (procured)'),
(15,N'16-18 Traineeships'),
(16,N'ESFA AEB - Adult Skills (non-procured)'),
(17,N'19-24 Traineeships (non-procured)'),
(18,N'ESFA AEB - Adult Skills (procured from Nov 2017)'),
(19,N'19-24 Traineeships (procured from Nov 2017)'),
(20,N'Advanced Learner Loans Bursary'),
(21,N'Adult Education - Eligible for MCA/GLA funding (non-procured)'),
(22,N'Adult Education - Eligible for MCA/GLA funding (procured)'),
(23,N'Short Term Funding Initiative 1'),
(24,N'Short Term Funding Initiative 2'),
(25,N'Short Term Funding Initiative 3'),
(26,N'Short Term Funding Initiative 4'),
(27, N'ESFA AEB - COVID-19 Skills Offer (non-procured)'),
(28, N'ESFA AEB - COVID-19 Skills Offer (procured)')
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

