using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class DateFormatter
    {
        /// <summary>
        /// Truncate the milliseconds from date time this
        /// is to allow access to work with date time
        /// FROM: http://stackoverflow.com/questions/16217464/trying-to-insert-datetime-now-into-date-time-field-gives-data-type-mismatch-er
        /// </summary>
        /// <param name="d">Date time</param>
        /// <returns>Formatted date time</returns>
        public static DateTime GetDateWithoutMilliseconds(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
        }
    }
}
