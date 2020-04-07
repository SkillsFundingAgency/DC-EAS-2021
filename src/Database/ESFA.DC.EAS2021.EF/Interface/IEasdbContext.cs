using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ESFA.DC.EAS2021.EF.Interface
{
    public interface IEasdbContext : IDisposable
    {
        DbSet<AdjustmentType> AdjustmentTypes { get; set; }
        DbSet<ContractType> ContractTypes { get; set; }
        DbSet<EasSubmission> EasSubmissions { get; set; }
        DbSet<EasSubmissionValue> EasSubmissionValues { get; set; }
        DbSet<FundingLine> FundingLines { get; set; }
        DbSet<FundingLineContractTypeMapping> FundingLineContractTypeMappings { get; set; }
        DbSet<PaymentType> PaymentTypes { get; set; }
        DbSet<SourceFile> SourceFiles { get; set; }
        DbSet<ValidationError> ValidationErrors { get; set; }
        DbSet<ValidationErrorRule> ValidationErrorRules { get; set; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
