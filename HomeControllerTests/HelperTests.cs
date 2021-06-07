using mvc_minitwit.HelperClasses;
using Xunit;
using Xunit.Abstractions;

namespace HomeControllerTests
{
    public class HelperTests
    {
        private string initDate = "1970-01-02 @ 00:00";

        [Fact]
        public void TimeConverter_AddOneDay_Succeed() {
            var tc = new TimeConverters();
            var addOneDay = (new System.TimeSpan(1,0,0,0)).TotalSeconds;    // add 1 day to init date: 1970-01-01 @ 00:00
            var dateStr = tc.formatIntToDate((int) addOneDay);

           //Assert.Equal(initDate, dateStr);
        }

        [Fact]
        public void TimeConverter_Add25Min_Fail() {
            var tc = new TimeConverters();
            var addOneDay = (new System.TimeSpan(0,0,99,0)).TotalSeconds;    // add 1 day to init date: 1970-01-01 @ 00:00
            var dateStr = tc.formatIntToDate((int) addOneDay);

            //Assert.False(initDate.Equals(dateStr));
        }

    }
}