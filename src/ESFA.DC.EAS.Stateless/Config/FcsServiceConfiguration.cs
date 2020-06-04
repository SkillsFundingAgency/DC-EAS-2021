using ESFA.DC.EAS.Interface.Config;

namespace ESFA.DC.EAS.Stateless.Config
{
    public class FcsServiceConfiguration : IFcsServiceConfiguration
    {
        public string FcsConnectionString { get; set; }
    }
}
