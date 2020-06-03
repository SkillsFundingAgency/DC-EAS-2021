using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS.DataService.Constants
{
    public static class DataServiceConstants
    {
        public static DateTime AcademicYearStart = new DateTime(2020, 8, 1);

        public static DateTime AcademicYearEnd = new DateTime(2021, 7, 31, 23, 59, 59);

        public static IEnumerable<int> ValidDevolvedSourceOfFundingCodes = new HashSet<int>()
        {
            110, 111, 112, 113, 114, 115, 116, 117
        };
    }
}
