using ESFA.DC.EAS1819.Stateless.Config.Interfaces;

namespace ESFA.DC.EAS1819.Stateless.Config
{
    public class FcsServiceConfiguration : IFcsServiceConfiguration
    {
        public string FcsConnectionString { get; set; }
    }
}
