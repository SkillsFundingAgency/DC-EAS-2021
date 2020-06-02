using System.Collections.Generic;

namespace ESFA.DC.EAS.Tests.Base.Builders
{
    public class McaSofDictionaryBuilder
    {
        public static implicit operator Dictionary<int, string>(McaSofDictionaryBuilder instance)
        {
            return instance.Build();
        }

        public Dictionary<int, string> Build()
        {
            return new Dictionary<int, string>
            {
                { 110, "GMCA" },
                { 111, "LCRCA" },
                { 112, "WMCA" },
                { 113, "WECA" },
                { 114, "TVCA" },
                { 115, "CPCA" },
                { 116, "London" },
                { 117, "NTCA" },
            };
        }
    }
}
