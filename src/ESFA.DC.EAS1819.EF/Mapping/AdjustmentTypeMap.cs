using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class AdjustmentTypeMap : EntityTypeConfiguration<AdjustmentType>
    {
        public AdjustmentTypeMap()
        {
            this.ToTable("AdjustmentType");
            this.HasKey(e => new { e.Id });
            this.Property(e => e.Name).IsRequired()
                .HasMaxLength(250);
            this.HasMany(e => e.PaymentTypes);
        }
    }
}
