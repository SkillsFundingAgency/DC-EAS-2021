using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Service.Helpers;
using Xunit;

namespace ESFA.DC.EAS1819.Service.Test.Helpers
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
    }
}
