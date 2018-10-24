using System.Data.Entity.ModelConfiguration;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class ContractTypeMap : EntityTypeConfiguration<ContractType>
    {
        public ContractTypeMap()
        {
            this.ToTable("ContractType");
            this.HasKey(e => new { e.Id });
            this.Property(e => e.Name).IsRequired()
                .HasMaxLength(250);
        }
    }
}
