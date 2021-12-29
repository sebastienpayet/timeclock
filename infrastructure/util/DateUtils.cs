using System;
using System.Globalization;

namespace TimeClock.infrastructure.util
{
    public class DateUtils
    {
        private static readonly CultureInfo cultureInfo = new CultureInfo("fr-FR");
        private static readonly Calendar calendar = cultureInfo.Calendar;

        public static bool IsInTheSameWeek(DateTime refDate1, DateTime refDate2)
        {
            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = cultureInfo.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = cultureInfo.DateTimeFormat.FirstDayOfWeek;

            return calendar.GetWeekOfYear(refDate1, myCWR, myFirstDOW) == calendar.GetWeekOfYear(refDate2, myCWR, myFirstDOW);
        }
    }
}
