using System.Data.Entity.ModelConfiguration;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class FundingLineMap : EntityTypeConfiguration<FundingLine>
    {
        public FundingLineMap()
        {
            this.ToTable("FundingLine");
            this.HasKey(e => new { e.Id });
            this.Property(e => e.Name).IsRequired()
                .HasMaxLength(250);
        }
    }
}
