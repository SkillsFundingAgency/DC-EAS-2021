/*
Pre-Deployment Script 
--------------------------------------------------------------------------------------
*/

SET NOCOUNT ON ;

GO
UPDATE SV
	SET [DevolvedAreaSoF] = -1
FROM [dbo].[EAS_Submission_Values] SV 
WHERE SV.[DevolvedAreaSoF] IS NULL
GO
