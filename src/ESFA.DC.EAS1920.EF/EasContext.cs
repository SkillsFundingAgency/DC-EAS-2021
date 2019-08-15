using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ESFA.DC.EAS1920.EF
{
    public partial class EasContext : DbContext
    {
        public EasContext()
        {
        }

        public EasContext(DbContextOptions<EasContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdjustmentType> AdjustmentTypes { get; set; }
        public virtual DbSet<ContractType> ContractTypes { get; set; }
        public virtual DbSet<EasSubmission> EasSubmissions { get; set; }
        public virtual DbSet<EasSubmissionValue> EasSubmissionValues { get; set; }
        public virtual DbSet<FundingLine> FundingLines { get; set; }
        public virtual DbSet<FundingLineContractTypeMapping> FundingLineContractTypeMappings { get; set; }
        public virtual DbSet<FundingLineDevolvedAreaSoFmapping> FundingLineDevolvedAreaSoFmappings { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<SourceFile> SourceFiles { get; set; }
        public virtual DbSet<ValidationError> ValidationErrors { get; set; }
        public virtual DbSet<ValidationErrorRule> ValidationErrorRules { get; set; }

        // Unable to generate entity type for table 'dbo.VersionInfo'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\;Database=EAS1920;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<AdjustmentType>(entity =>
            {
                entity.ToTable("AdjustmentType");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<ContractType>(entity =>
            {
                entity.ToTable("ContractType");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<EasSubmission>(entity =>
            {
                entity.HasKey(e => new { e.SubmissionId, e.CollectionPeriod });

                entity.ToTable("EAS_Submission");

                entity.Property(e => e.SubmissionId).HasColumnName("Submission_Id");

                entity.Property(e => e.ProviderName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Ukprn)
                    .IsRequired()
                    .HasColumnName("UKPRN")
                    .HasMaxLength(10);

                entity.Property(e => e.UpdatedBy).HasMaxLength(250);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<EasSubmissionValue>(entity =>
            {
                entity.HasKey(e => new { e.SubmissionId, e.CollectionPeriod, e.PaymentId, e.DevolvedAreaSoF });

                entity.ToTable("EAS_Submission_Values");

                entity.Property(e => e.SubmissionId).HasColumnName("Submission_Id");

                entity.Property(e => e.PaymentId).HasColumnName("Payment_Id");

                entity.Property(e => e.DevolvedAreaSoF).HasDefaultValueSql("((-1))");

                entity.Property(e => e.PaymentValue).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.EasSubmissionValues)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EAS_Submission_Values_Payment_Types");

                entity.HasOne(d => d.EasSubmission)
                    .WithMany(p => p.EasSubmissionValues)
                    .HasForeignKey(d => new { d.SubmissionId, d.CollectionPeriod })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EAS_Submission_Values_EAS_Submission");
            });

            modelBuilder.Entity<FundingLine>(entity =>
            {
                entity.ToTable("FundingLine");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<FundingLineContractTypeMapping>(entity =>
            {
                entity.HasKey(e => new { e.FundingLineId, e.ContractTypeId });

                entity.ToTable("FundingLineContractTypeMapping");

                entity.HasOne(d => d.ContractType)
                    .WithMany(p => p.FundingLineContractTypeMappings)
                    .HasForeignKey(d => d.ContractTypeId)
                    .HasConstraintName("FK_FundingLineContractTypeMapping_ToContractType");

                entity.HasOne(d => d.FundingLine)
                    .WithMany(p => p.FundingLineContractTypeMappings)
                    .HasForeignKey(d => d.FundingLineId)
                    .HasConstraintName("FK_FundingLineContractTypeMapping_ToFundingLine");
            });

            modelBuilder.Entity<FundingLineDevolvedAreaSoFmapping>(entity =>
            {
                entity.HasKey(e => new { e.FundingLineId, e.DevolvedAreaSoF });

                entity.ToTable("FundingLineDevolvedAreaSoFMapping");

                entity.HasOne(d => d.FundingLine)
                    .WithMany(p => p.FundingLineDevolvedAreaSoFmappings)
                    .HasForeignKey(d => d.FundingLineId)
                    .HasConstraintName("FK_FundingLineDevolvedAreaSoFMapping_ToFundingLine");
            });

            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.HasKey(e => e.PaymentId);

                entity.ToTable("Payment_Types");

                entity.Property(e => e.PaymentId)
                    .HasColumnName("Payment_Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Fm36).HasColumnName("FM36");

                entity.Property(e => e.PaymentName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.PaymentTypeDescription).HasMaxLength(250);

                entity.HasOne(d => d.AdjustmentType)
                    .WithMany(p => p.PaymentTypes)
                    .HasForeignKey(d => d.AdjustmentTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PaymentTypes_ToAdjustmentType");

                entity.HasOne(d => d.FundingLine)
                    .WithMany(p => p.PaymentTypes)
                    .HasForeignKey(d => d.FundingLineId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PaymentTypes_ToFundingLine");
            });

            modelBuilder.Entity<SourceFile>(entity =>
            {
                entity.ToTable("SourceFile");

                entity.Property(e => e.DateTime).HasColumnType("datetime");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.FilePreparationDate).HasColumnType("datetime");

                entity.Property(e => e.Ukprn)
                    .IsRequired()
                    .HasColumnName("UKPRN")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<ValidationError>(entity =>
            {
                entity.HasKey(e => new { e.SourceFileId, e.ValidationErrorId })
                    .HasName("PK__Validati__97356EBC2B689109");

                entity.ToTable("ValidationError");

                entity.Property(e => e.ValidationErrorId)
                    .HasColumnName("ValidationError_Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AdjustmentType).IsUnicode(false);

                entity.Property(e => e.CalendarMonth).IsUnicode(false);

                entity.Property(e => e.CalendarYear).IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DevolvedAreaSoF).IsUnicode(false);

                entity.Property(e => e.ErrorMessage).IsUnicode(false);

                entity.Property(e => e.FundingLine).IsUnicode(false);

                entity.Property(e => e.RuleId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Severity)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Value).IsUnicode(false);

                entity.HasOne(d => d.SourceFile)
                    .WithMany(p => p.ValidationErrors)
                    .HasForeignKey(d => d.SourceFileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ValidationError_SourceFile");
            });

            modelBuilder.Entity<ValidationErrorRule>(entity =>
            {
                entity.HasKey(e => e.RuleId);

                entity.Property(e => e.RuleId)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Severity)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.SeverityFis)
                    .IsRequired()
                    .HasColumnName("SeverityFIS")
                    .HasMaxLength(1);
            });
        }
    }
}
