using ESFA.DC.EAS.Interface.Config;

namespace ESFA.DC.EAS.Stateless.Config
{
    public class LoggerOptions : ILoggerOptions
    {
        public string LoggerConnectionString { get; set; }
    }
}
