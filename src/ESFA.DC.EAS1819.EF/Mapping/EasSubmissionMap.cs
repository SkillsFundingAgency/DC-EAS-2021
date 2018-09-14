using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class EasSubmissionMap : EntityTypeConfiguration<EasSubmission>
    {
        public EasSubmissionMap()
        {
            this.ToTable("EAS_Submission");
            this.HasKey(e => new { e.SubmissionId, e.CollectionPeriod });
            this.Property(e => e.SubmissionId).HasColumnName("Submission_Id");
            this.Property(e => e.ProviderName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(e => e.Ukprn)
                .IsRequired()
                .HasColumnName("UKPRN")
                .HasMaxLength(10);
            this.Property(e => e.UpdatedBy).HasMaxLength(250);
        }
    }
}
