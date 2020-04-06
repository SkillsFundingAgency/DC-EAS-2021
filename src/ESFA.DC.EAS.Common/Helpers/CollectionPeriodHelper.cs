using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS.Common.Helpers
{
    public static class CollectionPeriodHelper
    {
        public static readonly Dictionary<Tuple<int, int>, int> CollectionPeriodDictionary = new Dictionary<Tuple<int, int>, int>
        {
            { new Tuple<int, int>(2019, 8), 1 },
            { new Tuple<int, int>(2019, 9), 2 },
            { new Tuple<int, int>(2019, 10), 3 },
            { new Tuple<int, int>(2019, 11), 4 },
            { new Tuple<int, int>(2019, 12), 5 },
            { new Tuple<int, int>(2020, 1), 6 },
            { new Tuple<int, int>(2020, 2), 7 },
            { new Tuple<int, int>(2020, 3), 8 },
            { new Tuple<int, int>(2020, 4), 9 },
            { new Tuple<int, int>(2020, 5), 10 },
            { new Tuple<int, int>(2020, 6), 11 },
            { new Tuple<int, int>(2020, 7), 12 }
        };

        public static int GetCollectionPeriod(int calendarYear, int calendarMonth)
        {
            var collectionPeriod = CollectionPeriodDictionary[new Tuple<int, int>(calendarYear, calendarMonth)];
            return collectionPeriod;
        }

        public static Tuple<int, int> GetCalendarYearAndMonth(int collectionPeriod)
        {
            var tuple = CollectionPeriodDictionary.FirstOrDefault(x => x.Value == collectionPeriod).Key;
            return tuple;
        }
    }
}
