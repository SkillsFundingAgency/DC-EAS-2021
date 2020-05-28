DECLARE @SummaryOfChanges_ValidationErrorRules TABLE ([RuleId] nvarchar(50), [Action] VARCHAR(100));

MERGE INTO [ValidationErrorRules] AS Target
USING (VALUES		
	(N'Filename_01',N'E',N'The UKPRN in the name of the file is wrong. It doesn''t match your account''s UKPRN on Submit learner data. Please check you have the correct file for the provider you are submitting for.',N'E'),
	(N'Filename_02',N'E',N'We don''t recognise the UKPRN. It doesn''t exist in our database. Please check the UKPRN and retry.',N'E'),
	(N'Filename_03',N'E',N'The filename is wrong. It should use the format ''EASDATA-LLLLLLLL-yyyymmdd-hhmmss.csv''. LLLLLLLL = your UKPRN, yyyymmdd-hhmmss = date/time stamp (year, month, day, hours, minutes, seconds). Please amend the filename and try again. ',N'E'),
	(N'Filename_04',N'E',N'This filename can''t be used. A file with this filename has already been processed. Please upload a file with a different filename (this usually means changing the time/date stamp in the filename, which must be after your last submission).',N'E'),
	(N'Filename_05',N'E',N'The filename is wrong. The time/date stamp can''t be in the future. Please amend the filename and try again.',N'E'),
	(N'Filename_07',N'E',N'The file type is wrong. The file must be a .csv file. Please check the file type and make sure the extension at the end of the filename is ''.csv''.',N'E'),
	(N'Filename_08',N'E',N'This filename can''t be used. The date/time stamp in the filename can''t be earlier than the latest file we''ve received for your organisation.',N'E'),
	(N'Fileformat_01',N'E',N'The field headers are wrong. You can''t submit this file as the field headers aren''t in the correct format. Please check EAS guidance.',N'E'),
	(N'CalendarMonth_01',N'E',N'''CalendarMonth'' is wrong. It should be a number between 1 and 12. Please check the CalendarMonth field.',N'E'),
	(N'CalendarYear_01',N'E',N'''CalendarYear'' is wrong. It must either be 2020 or 2021. Please check the CalendarYear field.',N'E'),
	(N'CalendarYearCalendarMonth_01',N'E',N'''CalendarMonth'' is wrong. It can''t be after the month in which the current collection opened. Please check the CalendarMonth field.',N'E'),
	(N'CalendarYearCalendarMonth_02',N'E',N'''CalendarMonth'' and/or ''CalendarYear'' is wrong. The month/year aren''t in this academic funding year. Please check you''ve entered the correct CalendarMonth and CalendarYear for when the activity you''re claiming for happened. The current funding year spans from 2020-08 to 2021-07.',N'E'),
	(N'FundingLine_01',N'E',N'''FundingLine'' is wrong. We don''t recognise this FundingLine. You can see the full list of accepted FundingLine codes in the EAS guidance (available on GOV.UK). (Note - we remove all white spaces when displaying the FundingLine data back to you.)',N'E'),
	(N'FundingLine_02',N'E',N'''FundingLine'' is wrong. You don''t have a valid contract to claim earning adjustments against this FundingLine. Please check the FundingLine field. You can see the full list of accepted FundingLine codes in the EAS guidance (available on GOV.UK).',N'E'),
	(N'DevolvedAreaSourceOfFunding_01',N'E',N'''DevolvedAreaSourceOfFunding'' is missing. For this type of FundingLine, this field must be filled in. Please check ''DevolvedAreaSourceOfFunding'' is returned.',N'E'),
	(N'DevolvedAreaSourceOfFunding_02',N'E',N'''DevolvedAreaSourceOfFunding'' is wrong. For this type of FundingLine, this field must be empty. Please check ''FundingLine''. If correct, delete the information in ''DevolvedAreaSourceOfFunding''.',N'E'),
	(N'DevolvedAreaSourceOfFunding_03',N'E',N'''DevolvedAreaSourceOfFunding'' is wrong. We don''t recognise the code you''ve given us. The ''DevolvedAreaSourceOfFunding'' codes we accept are: 110; 111; 112; 113; 114; 115; 116. You should not use SOF 105 for your non-devolved delivery.',N'E'),
	(N'AdjustmentType_01',N'E',N'''AdjustmentType'' is wrong. We don''t recognise the AdjustmentType you''ve entered. The AdjustmentTypes we accept are: ALLB Excess Support; Authorised Claims; Authorised Claims - Additional payments for apprentice; Authorised Claims - Additional payments for employer; Authorised Claims - Additional payments for provider; Authorised Claims - Training costs exc Maths/Eng; Discretionary Bursary; Excess Learning Support; Free Meals; Learner Support; Princes Trust; Vulnerable Bursary; MCA/GLA Defined Adjustment 1; MCA/GLA Defined Adjustment 2; MCA/GLA Defined Adjustment 3.',N'E'),
	(N'AdjustmentType_02',N'E',N'''AdjustmentType'' is wrong. The AdjustmentType isn''t valid for the FundingLine you''ve entered. Please check AdjustmentType and FundingLine are a valid combination in the EAS guidance (available on GOV.UK).',N'E'),
	(N'Value_01',N'E',N'''Value'' is wrong. It must be more than zero. Please check that you''ve entered a numerical value that''s not zero in ''Value''.',N'E'),
	(N'Value_03',N'E',N'''Value'' is wrong. It must be between -99999999.99 and 99999999.99 (but can''t be zero). Please check the number you''ve entered in ''Value''.',N'E'),
	(N'Duplicate_01',N'E',N'This record is a duplicate. The FundingLine, AdjustmentType, CalendarMonth (and DevolvedAreaSourceofFunding if returned) combination already exists in your file. Please make sure only one record with this combination of data appears in your file each month.',N'E')
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

