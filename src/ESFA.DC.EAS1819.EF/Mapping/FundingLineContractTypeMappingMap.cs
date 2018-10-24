using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class FundingLineContractTypeMappingMap : EntityTypeConfiguration<FundingLineContractTypeMapping>
    {
        public FundingLineContractTypeMappingMap()
        {
            this.ToTable("FundingLineContractTypeMapping")
                .HasKey(fc => new { fc.FundingLineId, fc.ContractTypeId });

            this.HasRequired(pc => pc.ContractType)
                .WithMany()
                .HasForeignKey(pc => pc.ContractTypeId);

            this.HasRequired(pc => pc.FundingLine)
                .WithMany(p => p.FundingLineContractTypeMappings)
                .HasForeignKey(pc => pc.FundingLineId);
        }
    }
}
