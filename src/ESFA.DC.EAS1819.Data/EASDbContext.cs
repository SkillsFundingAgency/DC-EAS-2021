using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ESFA.DC.EAS1819.Data
{
    public partial class EasdbContext : DbContext
    {
        public virtual DbSet<EasSubmission> EasSubmission { get; set; }
        public virtual DbSet<EasSubmissionValues> EasSubmissionValues { get; set; }
        public virtual DbSet<PaymentTypes> PaymentTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public EasdbContext(DbContextOptions<EasdbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<EasSubmissionValues>(entity =>
            {
                entity.HasKey(e => new { e.SubmissionId, e.CollectionPeriod, e.PaymentId });

                entity.ToTable("EAS_Submission_Values");

                entity.Property(e => e.SubmissionId).HasColumnName("Submission_Id");

                entity.Property(e => e.PaymentId).HasColumnName("Payment_Id");

                entity.Property(e => e.PaymentValue).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<PaymentTypes>(entity =>
            {
                entity.HasKey(e => e.PaymentId);

                entity.ToTable("Payment_Types");

                entity.Property(e => e.PaymentId)
                    .HasColumnName("Payment_Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.PaymentName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.SubSectionHeading).HasMaxLength(250);

                entity.Property(e => e.RowHeading).HasMaxLength(250);

                entity.Property(e => e.PaymentTypeDescription).HasMaxLength(250);
            });
        }
    }
}
