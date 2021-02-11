using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc_minitwit.Models;
using mvc_minitwit.Data;


namespace mvc_minitwit.HelperClasses
{
        public class TimeConverters {

            public TimeConverters(){}
        
        public string formatIntToDate(int date) 
        {
            DateTime newDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            newDate = newDate.AddSeconds(date);
            return newDate.ToString("yyyy-MM-dd" + " @ " + "HH:mm");
        } 
    }
}