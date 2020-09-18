using System;

namespace ESFA.DC.EAS.Service.Exceptions
{
    public class InvalidCsvHeaderException : System.Exception
    {
        public InvalidCsvHeaderException()
        {
        }

        public InvalidCsvHeaderException(string errorMessage)
            : base(errorMessage)
        {
        }

        public InvalidCsvHeaderException(string errorMessage, Exception innerException)
            : base(errorMessage, innerException)
        {
        }
    }
}
