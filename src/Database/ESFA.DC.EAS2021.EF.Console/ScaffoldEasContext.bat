dotnet.exe ef dbcontext scaffold "Server=.\;Database=ESFA.DC.EAS2021.Database;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -c EasContext --schema dbo --force --startup-project . --project ..\ESFA.DC.EAS2021.EF --verbose
pause