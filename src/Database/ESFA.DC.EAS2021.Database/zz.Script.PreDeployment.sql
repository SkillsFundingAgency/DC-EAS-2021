/*
Pre-Deployment Script 
--------------------------------------------------------------------------------------
*/

SET NOCOUNT ON ;

GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EAS_Submission_Values]') AND type in (N'U'))
BEGIN 
	UPDATE SV
		SET [DevolvedAreaSoF] = -1
	FROM [dbo].[EAS_Submission_Values] SV 
	WHERE SV.[DevolvedAreaSoF] IS NULL
END
GO
