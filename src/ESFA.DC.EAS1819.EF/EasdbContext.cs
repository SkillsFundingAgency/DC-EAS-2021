using ESFA.DC.EAS1819.EF.Interface;
using ESFA.DC.EAS1819.EF.Mapping;

namespace ESFA.DC.EAS1819.EF
{
    using System.Data.Entity;

    public partial class EasdbContext : DbContext, IEasdbContext
    {
        public EasdbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public virtual DbSet<EasSubmission> EasSubmission { get; set; }

        public virtual DbSet<EasSubmissionValues> EasSubmissionValues { get; set; }

        public virtual DbSet<PaymentTypes> PaymentTypes { get; set; }

        public virtual DbSet<SourceFile> SourceFiles { get; set; }

        public virtual DbSet<ValidationError> ValidationErrors { get; set; }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        public new IDbSet<TEntity> Set<TEntity>()
            where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new EasSubmissionMap());
            modelBuilder.Configurations.Add(new EasSubmissionValuesMap());
            modelBuilder.Configurations.Add(new PaymentTypesMap());
            modelBuilder.Configurations.Add(new SourceFileMap());
            modelBuilder.Configurations.Add(new ValidationErrorMap());
        }
    }
}
