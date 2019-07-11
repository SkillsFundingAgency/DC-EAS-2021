using ESFA.DC.EAS.Stateless.Config.Interfaces;

namespace ESFA.DC.EAS.Stateless.Config
{
    public class FcsServiceConfiguration : IFcsServiceConfiguration
    {
        public string FcsConnectionString { get; set; }
    }
}
