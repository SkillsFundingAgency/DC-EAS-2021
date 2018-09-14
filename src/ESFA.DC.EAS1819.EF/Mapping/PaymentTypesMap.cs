using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class PaymentTypesMap : EntityTypeConfiguration<PaymentTypes>
    {
        public PaymentTypesMap()
        {
            this.HasKey(e => e.PaymentId);

            this.ToTable("Payment_Types");

            this.Property(e => e.PaymentId).HasColumnName("Payment_Id");

            this.Property(e => e.PaymentName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(e => e.SubSectionHeading).HasMaxLength(250);

            this.Property(e => e.RowHeading).HasMaxLength(250);

            this.Property(e => e.PaymentTypeDescription).HasMaxLength(250);
        }
    }
}
