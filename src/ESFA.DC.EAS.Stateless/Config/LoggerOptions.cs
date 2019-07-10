using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS.Stateless.Config.Interfaces;

namespace ESFA.DC.EAS.Stateless.Config
{
    public class LoggerOptions : ILoggerOptions
    {
        public string LoggerConnectionString { get; set; }
    }
}
