using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saperdun
{
    public static class DateComparator
    {
        public static bool IsOneDayAhead(DateTime date1, DateTime date2)
        {
            TimeSpan difference = date1 - date2;
            return difference == TimeSpan.FromDays(1);
        }

        public static bool IsOneDayAhaedIgnoringTime(DateTime date1, DateTime date2)
        {
            DateTime dateOnly1 = date1.Date;
            DateTime dateOnly2 = date2.Date;
            TimeSpan difference = dateOnly1 - dateOnly2;
            return difference == TimeSpan.FromDays(1);
        }
    }
}
