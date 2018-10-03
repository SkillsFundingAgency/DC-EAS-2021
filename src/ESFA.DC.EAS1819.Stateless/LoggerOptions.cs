using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Stateless.Config.Interfaces;

namespace ESFA.DC.EAS1819.Stateless
{
    public class LoggerOptions : ILoggerOptions
    {
        public string LoggerConnectionString { get; set; }
    }
}
