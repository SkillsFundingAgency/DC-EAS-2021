using System.Collections.Generic;
using ESFA.DC.EAS1819.Model.Enums;

namespace ESFA.DC.EAS1819.Model
{
    public class Section
    {
        public string SectionHeading { get; set; }

        public List<SubSection> SubSections { get; set; }

        public SectionType Type { get; set; }
    }
}