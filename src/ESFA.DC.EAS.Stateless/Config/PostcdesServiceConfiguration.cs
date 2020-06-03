using ESFA.DC.EAS.Interface.Config;

namespace ESFA.DC.EAS.Stateless.Config
{
    public class PostcodesServiceConfiguration : IPostcodesServiceConfiguration
    {
        public string PostcodesConnectionString { get; set; }
    }
}
