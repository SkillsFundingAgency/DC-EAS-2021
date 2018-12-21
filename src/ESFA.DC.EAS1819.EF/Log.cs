using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1819.EF
{
    public partial class Log
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTime TimeStampUtc { get; set; }
        public string Exception { get; set; }
        public string MachineName { get; set; }
        public string ProcessName { get; set; }
        public string ThreadId { get; set; }
        public string CallerName { get; set; }
        public string SourceFile { get; set; }
        public int? LineNumber { get; set; }
        public string JobId { get; set; }
        public string TaskKey { get; set; }
    }
}
