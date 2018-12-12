using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ESFA.DC.EAS1819.Common.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveWhiteSpacesNonAlphaNumericCharacters(this string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9+-.]");
            str = rgx.Replace(str, $"");
            return str.Trim();
        }
    }
}
