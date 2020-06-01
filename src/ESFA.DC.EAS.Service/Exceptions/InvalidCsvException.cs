using System;

namespace ESFA.DC.EAS.Service.Exceptions
{
    public class InvalidCsvException : System.Exception
    {
        public InvalidCsvException()
        {
        }

        public InvalidCsvException(string errorMessage)
            : base(errorMessage)
        {
        }

        public InvalidCsvException(string errorMessage, Exception innerException)
            : base(errorMessage, innerException)
        {
        }
    }
}
