using Microsoft.EntityFrameworkCore.Design;

namespace ESFA.DC.EAS2021.EF.Console.DesignTime
{
    public class Pluralizer : IPluralizer
    {
        public string Pluralize(string name)
        {
            return name.Pluralize() ?? name;
        }

        public string Singularize(string name)
        {
            return name.Singularize() ?? name;
        }
    }
}
