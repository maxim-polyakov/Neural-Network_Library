using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    internal class TimeSpanUtil
    {
        private DateTime _from;
        private DateTime _to;

        public TimeSpanUtil(DateTime from, DateTime to)
        {
            _from = from;
            _to = to;
        }

        public DateTime From
        {
            get { return _from; }
        }

        public DateTime To
        {
            get { return _to; }
        }


        public long GetSpan(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Ticks:
                    return GetSpanTicks();
                case TimeUnit.Seconds:
                    return GetSpanSeconds();
                case TimeUnit.Minutes:
                    return GetSpanMinutes();
                case TimeUnit.Hours:
                    return GetSpanHours();
                case TimeUnit.Days:
                    return GetSpanDays();
                case TimeUnit.Weeks:
                    return GetSpanWeeks();
                case TimeUnit.Fortnights:
                    return GetSpanFortnights();
                case TimeUnit.Months:
                    return GetSpanMonths();
                case TimeUnit.Years:
                    return GetSpanYears();
                case TimeUnit.Scores:
                    return GetSpanScores();
                case TimeUnit.Centuries:
                    return GetSpanCenturies();
                case TimeUnit.Millennia:
                    return GetSpanMillennia();
                default:
                    return 0;
            }
        }

        private long GetSpanTicks()
        {
            TimeSpan span = _to.Subtract(_from);
            return span.Ticks;
        }

        private long GetSpanSeconds()
        {
            TimeSpan span = _to.Subtract(_from);
            return span.Ticks / TimeSpan.TicksPerSecond;
        }

        private long GetSpanMinutes()
        {
            return GetSpanSeconds() / 60;
        }

        private long GetSpanHours()
        {
            return GetSpanMinutes() / 60;
        }

        private long GetSpanDays()
        {
            return GetSpanHours() / 24;
        }

        private long GetSpanWeeks()
        {
            return GetSpanDays() / 7;
        }

        private long GetSpanFortnights()
        {
            return GetSpanWeeks() / 2;
        }

        private long GetSpanMonths()
        {
            return (_to.Month - _from.Month) + (_to.Year - _from.Year) * 12;
        }

        private long GetSpanYears()
        {
            return GetSpanMonths() / 12;
        }

        private long GetSpanScores()
        {
            return GetSpanYears() / 20;
        }

        private long GetSpanCenturies()
        {
            return GetSpanYears() / 100;
        }

        private long GetSpanMillennia()
        {
            return GetSpanYears() / 1000;
        }
    }
}
