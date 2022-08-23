using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Webshop.Core.Conventors
{
    public static class FixedDateTime
    {
        public static string PersianDateTime(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();

            string dateTime = pc.GetYear(value) + "/" + pc.GetMonth(value).ToString("00") + "/" + pc.GetDayOfMonth(value).ToString("00");
            return dateTime;
        }
    }
}
