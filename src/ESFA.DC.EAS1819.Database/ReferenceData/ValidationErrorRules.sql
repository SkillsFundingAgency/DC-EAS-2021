DECLARE @SummaryOfChanges_ValidationErrorRules TABLE ([RuleId] nvarchar(50), [Action] VARCHAR(100));

MERGE INTO [ValidationErrorRules] AS Target
USING (VALUES		
	(N'Filename_01',N'E',N'The UKPRN in the filename does not match the UKPRN in the Hub.',N'E'),
	(N'Filename_02',N'E',N'The UKPRN in the filename is invalid.',N'E'),
	(N'Filename_03',N'E',N'The filename is not in the correct format.',N'E'),
	(N'Filename_04',N'E',N'A file with this filename has already been processed.',N'E'),
	(N'Filename_05',N'E',N'The date/time in the filename cannot be after the current date/time.',N'E'),
	(N'Filename_07',N'E',N'The file extension must be csv.',N'E'),
	(N'Filename_08',N'E',N'The date/time of the file is not greater than a previous transmission.',N'E'),
	(N'Fileformat_01',N'E',N'The file format is incorrect.  Please check the field headers are as per the Guidance document.',N'E'),
	(N'CalendarMonth_01',N'E',N'The CalendarMonth is not valid.',N'E'),
	(N'CalendarYear_01',N'E',N'The CalendarYear is not valid.',N'E'),
	(N'CalendarYearCalendarMonth_01',N'E',N'The CalendarMonth you have submitted data for cannot be in the future.',N'E'),
	(N'CalendarYearCalendarMonth_02',N'E',N'The CalendarMonth / year you have submitted data for is not within this accademic year.',N'E'),
	(N'FundingLine_01',N'E',N'The FundingLine is not valid.',N'E'),
	(N'FundingLine_02',N'E',N'To claim earning adjustments against funding lines, an appropriate contract type must be held.',N'E'),
	(N'AdjustmentType_01',N'E',N'The AdjustmentType is not valid.',N'E'),
	(N'AdjustmentType_02',N'E',N'The claimed adjustment must be valid for the funding line.',N'E'),
	(N'Value_01',N'E',N'The value field must be returned.',N'E'),
	(N'Value_03',N'E',N'Value must be >=-99999999.99 and <=99999999.99',N'E'),
	(N'Duplicate_01',N'E',N'This record is a duplicate.',N'E')
)
	AS Source([RuleId], [Severity], [Message], [SeverityFIS])
	ON Target.[RuleId] = Source.[RuleId]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT 
							Target.[RuleId] ,
							Target.[Severity] ,
							Target.[Message] ,
							Target.[SeverityFIS] 																					  
					EXCEPT 
						SELECT 
							Source.[RuleId] ,
							Source.[Severity] ,
							Source.[Message] ,
							Source.[SeverityFIS]													      
				)
		  THEN UPDATE SET 
						Target.[RuleId] = Source.[RuleId],
						Target.[Severity] = Source.[Severity],
						Target.[Message] = Source.[Message],
						Target.[SeverityFIS] = Source.[SeverityFIS]					
	WHEN NOT MATCHED BY TARGET THEN INSERT([RuleId], [Severity],[Message],[SeverityFIS]) VALUES ([RuleId], [Severity],[Message],[SeverityFIS])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT Inserted.[RuleId],$action INTO @SummaryOfChanges_ValidationErrorRules([RuleId],[Action]);


	DECLARE @AddCount__ValidationErrorRules INT, @UpdateCount__ValidationErrorRules INT, @DeleteCount__ValidationErrorRules INT
	SET @AddCount__ValidationErrorRules  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ValidationErrorRules WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount__ValidationErrorRules = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ValidationErrorRules WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount__ValidationErrorRules = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_ValidationErrorRules WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount__ValidationErrorRules, @UpdateCount__ValidationErrorRules, @DeleteCount__ValidationErrorRules) WITH NOWAIT;

