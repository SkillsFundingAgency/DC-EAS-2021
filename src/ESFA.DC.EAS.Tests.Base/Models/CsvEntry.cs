using CsvHelper.Configuration;

namespace ESFA.DC.EAS.Tests.Base.Models
{
    public sealed class CsvEntry
    {
        public CsvEntry(ClassMap mapper, int dataRows)
        {
            Mapper = mapper;
            DataRows = dataRows;
        }

        public ClassMap Mapper { get; }

        public int DataRows { get; }
    }
}
