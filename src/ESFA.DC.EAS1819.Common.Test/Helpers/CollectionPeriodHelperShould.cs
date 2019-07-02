using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Common.Helpers;
using Xunit;

namespace ESFA.DC.EAS1819.Common.Test.Helpers
{
    public class CollectionPeriodHelperShould
    {
        [Theory]
        [InlineData(2018, 8, 1)]
        [InlineData(2018, 9, 2)]
        [InlineData(2018, 10, 3)]
        [InlineData(2018, 11, 4)]
        [InlineData(2018, 12, 5)]
        [InlineData(2019, 1, 6)]
        [InlineData(2019, 2, 7)]
        [InlineData(2019, 3, 8)]
        [InlineData(2019, 4, 9)]
        [InlineData(2019, 5, 10)]
        [InlineData(2019, 6, 11)]
        [InlineData(2019, 7, 12)]
        public void Return_CorrectCollectionPeriod_For_Given_Calendar_Year_Month(int calendarYear, int calendarMonth, int expectedCollectionPeriod)
        {
            var actualCollectionPeriod = CollectionPeriodHelper.GetCollectionPeriod(calendarYear, calendarMonth);
            Assert.Equal(expectedCollectionPeriod, actualCollectionPeriod);
        }

        [Theory]
        [InlineData(1, 2018, 8)]
        [InlineData(2, 2018, 9)]
        [InlineData(3, 2018, 10)]
        [InlineData(4, 2018, 11)]
        [InlineData(5, 2018, 12)]
        [InlineData(6, 2019, 1)]
        [InlineData(7, 2019, 2)]
        [InlineData(8, 2019, 3)]
        [InlineData(9, 2019, 4)]
        [InlineData(10, 2019, 5)]
        [InlineData(11, 2019, 6)]
        [InlineData(12, 2019, 7)]
        public void Return_CalendaryearAndMonth_For_Given_CollectionPeriod(int collectionPeriod, int expectedCalendarYear, int expectedCalendarMonth)
        {
            var calendarYearAndMonth = CollectionPeriodHelper.GetCalendarYearAndMonth(collectionPeriod);
            Assert.Equal(expectedCalendarYear, calendarYearAndMonth.Item1);
            Assert.Equal(expectedCalendarMonth, calendarYearAndMonth.Item2);
        }
    }
}
