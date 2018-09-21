namespace ESFA.DC.EAS1819.EF
{
    using System;

    public partial class SourceFile : BaseEntity
    {
        public int SourceFileId { get; set; }

        public string FileName { get; set; }

        public DateTime FilePreparationDate { get; set; }

        public string UKPRN { get; set; }

        public DateTime? DateTime { get; set; }
    }
}
