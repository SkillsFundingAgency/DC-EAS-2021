CREATE VIEW [dbo].[vw_EasCalculation] AS
	SELECT ES.UKPRN
		,ES.CollectionPERIOD AS [Period_Id]
		,EPT.PaymentName
		,ISNULL(ESV.PaymentValue, 0) AS PaymentValue
	FROM [dbo].[EAS_SUBMISSION] ES
	INNER JOIN		[dbo].[EAS_SUBMISSION_VALUES] ESV  
	ON		        ES.[SUBMISSION_ID]=ESV.[SUBMISSION_ID] AND ES.[CollectionPeriod]=ESV.[CollectionPeriod]
	INNER JOIN		[dbo].[PAYMENT_TypeS] EPT
	ON				EPT.PAYMENT_ID = ESV.PAYMENT_ID