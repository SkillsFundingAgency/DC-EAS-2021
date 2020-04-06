CREATE VIEW [dbo].[vw_Payment_Types]
AS 
	SELECT 
			Payment_Id,
			PaymentName,
			PaymentTypeDescription,
			FM36,
			FundingLineId,						
			f.name FundingLine,
			AdjustmentTypeId,	
			a.name AdjustmentType								
		FROM 
			Payment_Types pt
 LEFT OUTER JOIN FundingLine f 
		ON	pt.FundingLineId = f.id
 LEFT OUTER JOIN AdjustmentType a
		ON pt.AdjustmentTypeId = a.id

