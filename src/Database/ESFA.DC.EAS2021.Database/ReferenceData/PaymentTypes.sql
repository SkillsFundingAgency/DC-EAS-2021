DECLARE @SummaryOfChanges_Payment_Types TABLE ([Payment_Id] INT, [Action] VARCHAR(100));

MERGE INTO [Payment_Types] AS Target
USING (VALUES

	(2,N'Excess Learning Support: 16-18 Apprenticeships',0,N'Excess Learning Support',1,8),
	(5,N'Authorised Claims: 16-18 Apprenticeships',0,N'Authorised Claims',1,2),
	(6,N'Learner Support: 16-18 Apprenticeships',0,N'Learner Support',1,10),
	(7,N'Excess Learning Support: 19-23 Apprenticeships',0,N'Excess Learning Support',2,8),
	(10,N'Authorised Claims: 19-23 Apprenticeships',0,N'Authorised Claims',2,2),
	(11,N'Learner Support: 19-23 Apprenticeships',0,N'Learner Support',2,10),
	(12,N'Excess Learning Support: 24+ Apprenticeships',0,N'Excess Learning Support',3,8),
	(15,N'Authorised Claims: 24+ Apprenticeships',0,N'Authorised Claims',3,2),
	(16,N'Learner Support: 24+ Apprenticeships',0,N'Learner Support',3,10),
	(17,N'Authorised Claims: 16-18 Traineeships',0,N'Authorised Claims',15,2),
	(19,N'Excess Learning Support: 19-24 Traineeships',0,N'Excess Learning Support',17,8),
	(22,N'Authorised Claims: 19-24 Traineeships',0,N'Authorised Claims',17,2),
	(32,N'Vulnerable Bursary: 16-19 Traineeships Bursary',0,N'Vulnerable Bursary',15,12),
	(33,N'Discretionary Bursary: 16-19 Traineeships Bursary',0,N'Discretionary Bursary',15,7),
	(34,N'Free Meals: 16-19 Traineeships Bursary',0,N'Free Meals',15,9),
	(35,N'Excess Support: Advanced Learner Loans Bursary',0,N'Excess Support',20,1),
	(37,N'Excess Learning Support: 16-18 Trailblazer Apprenticeships',0,N'Excess Learning Support',4,8),
	(40,N'Authorised Claims: 16-18 Trailblazer Apprenticeships',0,N'Authorised Claims',4,2),
	(42,N'Excess Learning Support: 19-23 Trailblazer Apprenticeships',0,N'Excess Learning Support',5,8),
	(45,N'Authorised Claims: 19-23 Trailblazer Apprenticeships',0,N'Authorised Claims',5,2),
	(47,N'Excess Learning Support: 24+ Trailblazer Apprenticeships',0,N'Excess Learning Support',6,8),
	(50,N'Authorised Claims: 24+ Trailblazer Apprenticeships',0,N'Authorised Claims: 24+ Trailblazer Apprenticeships',6,2),
	(51,N'Excess Learning Support: 16-18 Traineeships',0,N'Excess Learning Support',15,8),
	(52,N'Excess Learning Support: AEB-Other Learning',0,N'Excess Learning Support',16,8),
	(55,N'Authorised Claims: AEB-Other Learning',0,N'Authorised Claims',16,2),
	(69,N'Authorised Claims: 16-18 Levy Apprenticeships - Employer',1,N'Authorised Claims',9,4),
	(70,N'Authorised Claims: 16-18 Levy Apprenticeships - Provider',1,N'Authorised Claims',9,5),
	(71,N'Authorised Claims: 16-18 Levy Apprenticeships - Training',1,N'Authorised Claims',9,6),
	(72,N'Authorised Claims: 16-18 Non-Levy Apprenticeships - Employer',1,N'Authorised Claims',7,4),
	(73,N'Authorised Claims: 16-18 Non-Levy Apprenticeships - Provider',1,N'Authorised Claims',7,5),
	(74,N'Authorised Claims: 16-18 Non-Levy Apprenticeships - Training',1,N'Authorised Claims',7,6),
	(75,N'Authorised Claims: Adult Levy Apprenticeships - Employer',1,N'Authorised Claims',10,4),
	(76,N'Authorised Claims: Adult Levy Apprenticeships - Provider',1,N'Authorised Claims',10,5),
	(77,N'Authorised Claims: Adult Levy Apprenticeships - Training',1,N'Authorised Claims',10,6),
	(78,N'Authorised Claims: Adult Non-Levy Apprenticeships - Employer',1,N'Authorised Claims',8,4),
	(79,N'Authorised Claims: Adult Non-Levy Apprenticeships - Provider',1,N'Authorised Claims',8,5),
	(80,N'Authorised Claims: Adult Non-Levy Apprenticeships - Training',1,N'Authorised Claims',8,6),
	(81,N'Excess Learning Support: 16-18 Levy Apprenticeships - Provider',1,N'Excess Learning Support',9,8),
	(82,N'Excess Learning Support: 16-18 Non-Levy Apprenticeships - Provider',1,N'Excess Learning Support',7,8),
	(83,N'Excess Learning Support: Adult Levy Apprenticeships - Provider',1,N'Excess Learning Support',10,8),
	(84,N'Excess Learning Support: Adult Non-Levy Apprenticeships - Provider',1,N'Excess Learning Support',8,8),
	(100,N'Excess Learning Support: 16-18 Non-Levy Apprenticeships (procured) - Provider',1,N'Excess Learning Support',13,8),
	(105,N'Authorised Claims: 16-18 Non-Levy Apprenticeships (procured) - Training',1,N'Authorised Claims',13,6),
	(106,N'Authorised Claims: 16-18 Non-Levy Apprenticeships (procured) - Provider',1,N'Authorised Claims',13,5),
	(107,N'Authorised Claims: 16-18 Non-Levy Apprenticeships (procured) - Employer',1,N'Authorised Claims',13,4),
	(108,N'Excess Learning Support: Adult Non-Levy Apprenticeships (procured) - Provider',1,N'Excess Learning Support',14,8),
	(113,N'Authorised Claims: Adult Non-Levy Apprenticeships (procured) - Training',1,N'Authorised Claims',14,6),
	(114,N'Authorised Claims: Adult Non-Levy Apprenticeships (procured) - Provider',1,N'Authorised Claims',14,5),
	(115,N'Authorised Claims: Adult Non-Levy Apprenticeships (procured) - Employer',1,N'Authorised Claims',14,4),
	(116,N'Excess Learning Support: 19-24 Traineeships (From Nov 2017)',0,N'Excess Learning Support',19,8),
	(119,N'Authorised Claims: 19-24 Traineeships (From Nov 2017)',0,N'Authorised Claims',19,2),
	(121,N'Excess Learning Support: AEB-Other Learning (From Nov 2017)',0,N'Excess Learning Support',18,8),
	(124,N'Authorised Claims: AEB-Other Learning (From Nov 2017)',0,N'Authorised Claims',18,2),
	(126,N'Authorised Claims: 16-18 Levy Apprenticeships - Apprentice',1,N'Authorised Claims',9,3),
	(128,N'Authorised Claims: Adult Levy Apprenticeships - Apprentice',1,N'Authorised Claims',10,3),
	(130,N'Authorised Claims: 16-18 Non-Levy Apprenticeships (procured) - Apprentice',1,N'Authorised Claims',13,3),
	(132,N'Authorised Claims: Adult Non-Levy Apprenticeships (procured) - Apprentice',1,N'Authorised Claims',14,3),
	(133,N'Princes Trust: AEB-Other Learning',0,N'Princes Trust',16,11),
	(134,N'Princes Trust: AEB-Other Learning (From Nov 2017)',0,N'Princes Trust',18,11),
	(135,N'Authorised Claims: Advanced Learner Loans Bursary',0,N'Authorised Claims',20,2),
	(136,N'Excess Learning Support: Adult Education - Eligible for MCA/GLA funding (non-procured)',0,N'Excess Learning Support',21,8),
	(137,N'Authorised Claims: Adult Education - Eligible for MCA/GLA funding (non-procured)',0,N'Authorised Claims',21,2),
	(138,N'Princes Trust: Adult Education - Eligible for MCA/GLA funding (non-procured)',0,N'Princes Trust',21,11),
	(139,N'MCA/GLA Defined Adjustment 1: Adult Education - Eligible for MCA/GLA funding (non-procured)',0,N'MCA/GLA Defined Adjustment 1',21,13),
	(140,N'MCA/GLA Defined Adjustment 2: Adult Education - Eligible for MCA/GLA funding (non-procured)',0,N'MCA/GLA Defined Adjustment 2',21,14),
	(141,N'MCA/GLA Defined Adjustment 3: Adult Education - Eligible for MCA/GLA funding (non-procured)',0,N'MCA/GLA Defined Adjustment 3',21,15),
	(142,N'Authorised Claims: Adult Education - Eligible for MCA/GLA funding (procured)',0,N'Authorised Claims',22,2),
	(143,N'Excess Learning Support: Adult Education - Eligible for MCA/GLA funding (procured)',0,N'Excess Learning Support',22,8),
	(144,N'Princes Trust: Adult Education - Eligible for MCA/GLA funding (procured)',0,N'Princes Trust',22,11),
	(145,N'MCA/GLA Defined Adjustment 1: Adult Education - Eligible for MCA/GLA funding (procured)',0,N'MCA/GLA Defined Adjustment 1',22,13),
	(146,N'MCA/GLA Defined Adjustment 2: Adult Education - Eligible for MCA/GLA funding (procured)',0,N'MCA/GLA Defined Adjustment 2',22,14),
	(147,N'MCA/GLA Defined Adjustment 3: Adult Education - Eligible for MCA/GLA funding (procured)',0,N'MCA/GLA Defined Adjustment 3',22,15),
	(148,N'Excess Learning Support: 16-18 Apprenticeship (Employer on App Service) Non-Levy',1,N'Excess Learning Support',11,8),
	(149,N'Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Training',1,N'Authorised Claims',11,6),
	(150,N'Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Provider',1,N'Authorised Claims',11,5),
	(151,N'Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Employer',1,N'Authorised Claims',11,4),
	(152,N'Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Apprentice',1,N'Authorised Claims',11,3),
	(153,N'Excess Learning Support: 19+ Apprenticeship (Employer on App Service) Non-Levy',1,N'Excess Learning Support',12,8),
	(154,N'Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Training',1,N'Authorised Claims',12,6),
	(155,N'Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Provider',1,N'Authorised Claims',12,5),
	(156,N'Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Employer',1,N'Authorised Claims',12,4),
	(157,N'Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Apprentice',1,N'Authorised Claims',12,3),
	(158,N'Authorised Claims: 16-18 Levy Apprenticeships - Training',1,N'Authorised Claims - Maths and English',9,19),
	(159,N'Authorised Claims: 16-18 Non-Levy Apprenticeships - Training',1,N'Authorised Claims - Maths and English',7,19),
	(160,N'Authorised Claims: Adult Levy Apprenticeships - Training',1,N'Authorised Claims - Maths and English',10,19),
	(161,N'Authorised Claims: Adult Non-Levy Apprenticeships - Training',1,N'Authorised Claims - Maths and English',8,19),
	(162,N'Authorised Claims: 16-18 Non-Levy Apprenticeships (procured) - Training',1,N'Authorised Claims - Maths and English',13,19),
	(163,N'Authorised Claims: Adult Non-Levy Apprenticeships (procured) - Training',1,N'Authorised Claims - Maths and English',14,19),
	(164,N'Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Training',1,N'Authorised Claims - Maths and English',11,19),
	(165,N'Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Training',1,N'Authorised Claims - Maths and English',12,19),
	(166,N'MCA/GLA Defined Adjustment 4: Adult Education - Eligible for MCA/GLA funding (non-procured)',0,N'MCA/GLA Defined Adjustment 4',21,16),
	(167,N'MCA/GLA Defined Adjustment 5: Adult Education - Eligible for MCA/GLA funding (non-procured)',0,N'MCA/GLA Defined Adjustment 5',21,17),
	(168,N'MCA/GLA Defined Adjustment 6: Adult Education - Eligible for MCA/GLA funding (non-procured)',0,N'MCA/GLA Defined Adjustment 6',21,18),
	(169,N'MCA/GLA Defined Adjustment 4: Adult Education - Eligible for MCA/GLA funding (procured)',0,N'MCA/GLA Defined Adjustment 4',22,16),
	(170,N'MCA/GLA Defined Adjustment 5: Adult Education - Eligible for MCA/GLA funding (procured)',0,N'MCA/GLA Defined Adjustment 5',22,17),
	(171,N'MCA/GLA Defined Adjustment 6: Adult Education - Eligible for MCA/GLA funding (procured)',0,N'MCA/GLA Defined Adjustment 6',22,18),
	(172,N'Authorised Claims: Short Term Funding Initiative 1',0,N'Authorised Claims',23,2),
	(173,N'Authorised Claims: Short Term Funding Initiative 2',0,N'Authorised Claims',24,2),
	(174,N'Authorised Claims: Short Term Funding Initiative 3',0,N'Authorised Claims',25,2),
	(175,N'Authorised Claims: Short Term Funding Initiative 4',0,N'Authorised Claims',26,2)

)
	AS Source([Payment_Id], [PaymentName], [FM36], [PaymentTypeDescription],[FundinglineId],[AdjustmentTypeId])
	ON Target.[Payment_Id] = Source.[Payment_Id]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[Payment_Id] ,
							Target.[PaymentName] ,
							Target.[FM36] ,							
							Target.[PaymentTypeDescription] ,														  
							Target.[FundinglineId] ,  
							Target.[AdjustmentTypeId] 														  
					EXCEPT 
						SELECT Source.[Payment_Id] ,
							Source.[PaymentName]	,
							Source.[FM36] ,							
							Source.[PaymentTypeDescription] ,														  
							Source.[FundinglineId] ,  
							Source.[AdjustmentTypeId] 					      
				)
		  THEN UPDATE SET 
						Target.[Payment_Id] = Source.[Payment_Id],
						Target.[PaymentName] = Source.[PaymentName],
						Target.[FM36] = Source.[FM36],						
						Target.[PaymentTypeDescription] = Source.[PaymentTypeDescription],
						Target.[FundinglineId] = Source.[FundinglineId],
						Target.[AdjustmentTypeId] = Source.[AdjustmentTypeId]
	WHEN NOT MATCHED BY TARGET THEN INSERT([Payment_Id], [PaymentName], [FM36], [PaymentTypeDescription],[FundinglineId],[AdjustmentTypeId]) 
									VALUES ([Payment_Id], [PaymentName], [FM36], [PaymentTypeDescription],[FundinglineId],[AdjustmentTypeId])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT Inserted.[Payment_Id],$action INTO @SummaryOfChanges_Payment_Types([Payment_Id],[Action])
;


	DECLARE @AddCount_AET INT, @UpdateCount_AET INT, @DeleteCount_AET INT
	SET @AddCount_AET  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Payment_Types WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_AET = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Payment_Types WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_AET = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Payment_Types WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount_AET, @UpdateCount_AET, @DeleteCount_AET) WITH NOWAIT;

