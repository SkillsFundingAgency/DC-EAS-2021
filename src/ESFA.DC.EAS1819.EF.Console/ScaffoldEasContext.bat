dotnet.exe ef dbcontext scaffold "Server=.\;Database=EasDb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -c EasContext --schema dbo --force --startup-project . --project ..\ESFA.DC.EAS1819.EF --verbose
pause