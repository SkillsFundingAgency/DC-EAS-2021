using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.EF.Interface
{
    public interface IEasdbContext
    {
        DbSet<EasSubmission> EasSubmission { get; set; }

        DbSet<EasSubmissionValues> EasSubmissionValues { get; set; }

        DbSet<PaymentTypes> PaymentTypes { get; set; }

        DbSet<SourceFile> SourceFiles { get; set; }

        DbSet<ValidationError> ValidationErrors { get; set; }

        Database Database { get; }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        IDbSet<TEntity> Set<TEntity>()
            where TEntity : BaseEntity;

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
