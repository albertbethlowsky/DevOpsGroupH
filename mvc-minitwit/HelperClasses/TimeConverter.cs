using System;

namespace mvc_minitwit.HelperClasses
{
        public class TimeConverters {

        public string formatIntToDate(int date)
        {
            DateTime newDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            newDate = newDate.AddSeconds(date);
            return newDate.ToString("yyyy-MM-dd" + " @ " + "HH:mm");
        }
    }
}