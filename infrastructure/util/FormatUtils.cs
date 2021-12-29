using System;

namespace TimeClock.infrastructure.util
{
    public class FormatUtils
    {
        public static string BuildTimerString(TimeSpan interval)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", interval.Hours, interval.Minutes, interval.Seconds);
        }
    }
}
