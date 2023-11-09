using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    internal class DateUtil
    {
        /// <summary>
        /// January is 1.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime CreateDate(int month, int day, int year)
        {
            var result = new DateTime(year, month, day);
            return result;
        }

        /// <summary>
        /// Truncate a date, remove the time.
        /// </summary>
        /// <param name="date">The date to truncate.</param>
        /// <returns>The date without the time.</returns>
        public static DateTime TruncateDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }
    }
}
