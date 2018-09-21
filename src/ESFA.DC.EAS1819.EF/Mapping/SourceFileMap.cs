using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class SourceFileMap : EntityTypeConfiguration<SourceFile>
    {
        public SourceFileMap()
        {
            this.ToTable("SourceFile");
            this.HasKey(e => new { e.SourceFileId });
            this.Property(e => e.SourceFileId).HasColumnName("SourceFileId");
            this.Property(e => e.FileName).HasMaxLength(60);
            this.Property(e => e.FilePreparationDate);
            this.Property(e => e.FilePreparationDate);
            this.Property(e => e.UKPRN)
                .IsRequired()
                .HasColumnName("UKPRN")
                .HasMaxLength(20);
            this.Property(e => e.DateTime);
        }
    }
}
