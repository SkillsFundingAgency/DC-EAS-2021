using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class ValidationErrorRuleMap : EntityTypeConfiguration<ValidationErrorRule>
    {
        public ValidationErrorRuleMap()
        {
            this.ToTable("ValidationErrorRules");
            this.HasKey(e => new { e.RuleId });
            this.Property(e => e.RuleId).HasMaxLength(50);
            this.Property(e => e.Severity).HasMaxLength(1);
            this.Property(e => e.Message).HasMaxLength(2000);
            this.Property(e => e.SeverityFIS).HasMaxLength(1);
        }
    }
}
