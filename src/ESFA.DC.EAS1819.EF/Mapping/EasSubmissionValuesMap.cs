using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class EasSubmissionValuesMap : EntityTypeConfiguration<EasSubmissionValues>
    {
        public EasSubmissionValuesMap()
        {
            this.ToTable("EAS_Submission_Values");
            this.HasKey(e => new { e.SubmissionId, e.CollectionPeriod, e.PaymentId });
            this.Property(e => e.SubmissionId).HasColumnName("Submission_Id");
            this.Property(e => e.PaymentId).HasColumnName("Payment_Id");
            this.Property(e => e.PaymentValue).HasPrecision(10, 2);
        }
    }
}
