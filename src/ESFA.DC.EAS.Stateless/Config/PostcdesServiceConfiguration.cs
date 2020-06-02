using ESFA.DC.EAS.Stateless.Config.Interfaces;

namespace ESFA.DC.EAS.Stateless.Config
{
    public class PostcodesServiceConfiguration : IPostcodesServiceConfiguration
    {
        public string PostcodesConnectionString { get; set; }
    }
}
