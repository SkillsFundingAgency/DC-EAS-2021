using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.EF.Mapping
{
    public class ValidationErrorMap : EntityTypeConfiguration<ValidationError>
    {
        public ValidationErrorMap()
        {
            this.ToTable("ValidationError");
            this.HasKey(e => new { e.SourceFileId, e.ValidationErrorId });
            this.Property(e => e.ValidationErrorId).HasColumnName("ValidationError_Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); ;
            this.Property(e => e.RowId);
            this.Property(e => e.RuleId).HasMaxLength(50);
            this.Property(e => e.CalendarYear);
            this.Property(e => e.CalendarMonth);
            this.Property(e => e.Severity).HasMaxLength(2);
            this.Property(e => e.ErrorMessage);
            this.Property(e => e.Value);
            this.Property(e => e.CreatedOn);
        }
    }
}
