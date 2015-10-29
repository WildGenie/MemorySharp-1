using System;
using System.Linq;

namespace MemorySharp.Tools.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="System.DateTime" /> data type.
    /// </summary>
    public static class DateTimeExtensions
    {
        #region  Fields
        /// <summary>
        ///     String array contains avaliable DateTime formats used to coverts string to DateTime.
        /// </summary>
        /// <example>
        ///     <code>
        /// "dd/MM/yyyy",
        /// "dd/MM/yyyy hh:mm:ss",
        /// "dd-MM-yyyy",
        /// "dd-MM-yyyy hh:mm:ss",
        /// "yyyy-MM-dd",
        /// "yyyy-MM-dd hh:mm:ss",
        /// "yyyy/MM/dd",
        /// "yyyy/MM/dd hh:mm:ss",
        /// "yyyy.MM.dd",
        /// "yyyy.MM.dd hh:mm:ss",
        /// "dd.MM.yyyy",
        /// "dd.MM.yyyy hh:mm:ss",
        /// "yyyyMMdd"
        /// </code>
        /// </example>
        public static readonly string[] DateFormats =
        {
            "dd/MM/yyyy",
            "d/M/yyyy",
            "d/MM/yyyy",
            "dd/M/yyyy",
            "dd/MM/yyyy HH:mm",
            "d/M/yyyy HH:mm",
            "d/MM/yyyy HH:mm",
            "dd/M/yyyy HH:mm",
            "dd/MM/yyyy HH:mm:ss",
            "d/M/yyyy HH:mm:ss",
            "d/MM/yyyy HH:mm:ss",
            "dd/M/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm:ss.fff",
            "d/M/yyyy HH:mm:ss.fff",
            "d/MM/yyyy HH:mm:ss.fff",
            "dd/M/yyyy HH:mm:ss.fff",
            "dd-MM-yyyy",
            "d-M-yyyy",
            "d-MM-yyyy",
            "dd-M-yyyy",
            "dd-MM-yyyy HH:mm",
            "d-M-yyyy HH:mm",
            "d-MM-yyyy HH:mm",
            "dd-M-yyyy HH:mm",
            "dd-MM-yyyy HH:mm:ss",
            "d-M-yyyy HH:mm:ss",
            "d-MM-yyyy HH:mm:ss",
            "dd-M-yyyy HH:mm:ss",
            "dd-MM-yyyy HH:mm:ss.fff",
            "d-M-yyyy HH:mm:ss.fff",
            "d-MM-yyyy HH:mm:ss.fff",
            "dd-M-yyyy HH:mm:ss.fff",
            "yyyy-MM-dd",
            "yyyy-M-d",
            "yyyy-MM-d",
            "yyyy-M-dd",
            "yyyy-MM-dd HH:mm",
            "yyyy-M-d HH:mm",
            "yyyy-MM-d HH:mm",
            "yyyy-M-dd HH:mm",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-M-d HH:mm:ss",
            "yyyy-MM-d HH:mm:ss",
            "yyyy-M-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyy-M-d HH:mm:ss.fff",
            "yyyy-MM-d HH:mm:ss.fff",
            "yyyy-M-dd HH:mm:ss.fff",
            "yyyy/MM/dd",
            "yyyy/M/d",
            "yyyy/MM/d",
            "yyyy/M/dd",
            "yyyy/MM/dd HH:mm",
            "yyyy/M/d HH:mm",
            "yyyy/MM/d HH:mm",
            "yyyy/M/dd HH:mm",
            "yyyy/MM/dd HH:mm:ss",
            "yyyy/M/d HH:mm:ss",
            "yyyy/MM/d HH:mm:ss",
            "yyyy/M/dd HH:mm:ss",
            "yyyy/MM/dd HH:mm:ss.fff",
            "yyyy/M/d HH:mm:ss.fff",
            "yyyy/MM/d HH:mm:ss.fff",
            "yyyy/M/dd HH:mm:ss.fff",
            "yyyy.MM.dd",
            "yyyy.M.d",
            "yyyy.MM.d",
            "yyyy.M.dd",
            "yyyy.MM.dd HH:mm",
            "yyyy.M.d HH:mm",
            "yyyy.MM.d HH:mm",
            "yyyy.M.dd HH:mm",
            "yyyy.MM.dd HH:mm:ss",
            "yyyy.M.d HH:mm:ss",
            "yyyy.MM.d HH:mm:ss",
            "yyyy.M.dd HH:mm:ss",
            "yyyy.MM.dd HH:mm:ss.fff",
            "yyyy.M.d HH:mm:ss.fff",
            "yyyy.MM.d HH:mm:ss.fff",
            "yyyy.M.dd HH:mm:ss.fff",
            "dd.MM.yyyy",
            "d.M.yyyy",
            "d.MM.yyyy",
            "dd.M.yyyy",
            "dd.MM.yyyy HH:mm",
            "d.M.yyyy HH:mm",
            "d.MM.yyyy HH:mm",
            "dd.M.yyyy HH:mm",
            "dd.MM.yyyy HH:mm:ss",
            "d.M.yyyy HH:mm:ss",
            "d.MM.yyyy HH:mm:ss",
            "dd.M.yyyy HH:mm:ss",
            "dd.MM.yyyy HH:mm:ss.fff",
            "d.M.yyyy HH:mm:ss.fff",
            "d.MM.yyyy HH:mm:ss.fff",
            "dd.M.yyyy HH:mm:ss.fff",
            "yyyyMMdd",
            "yyyyMd",
            "yyyyMMd",
            "yyyyMdd",
            "yyyy-MM-ddTHH:mm:ss.fffK",
            "yyyy-M-dTHH:mm:ss.fffK",
            "yyyy-MM-dTHH:mm:ss.fffK",
            "yyyy-M-ddTHH:mm:ss.fffK",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-M-dTHH:mm:ss.fffZ",
            "yyyy-MM-dTHH:mm:ss.fffZ",
            "yyyy-M-ddTHH:mm:ss.fffZ"
        };
        #endregion

        #region Methods
        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsToday(this DateTime dt)
        {
            return (dt.Date == DateTime.Today);
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateToCompare"></param>
        /// <returns></returns>
        public static bool IsDateEqual(this DateTime date, DateTime dateToCompare)
        {
            return (date.Date == dateToCompare.Date);
        }

        /// <summary>
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeToCompare"></param>
        /// <returns></returns>
        public static bool IsTimeEqual(this DateTime time, DateTime timeToCompare)
        {
            return (time.TimeOfDay == timeToCompare.TimeOfDay);
        }

        /// <summary>
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static long GetMillisecondsSince1970(this DateTime datetime)
        {
            var ts = datetime.Subtract(new DateTime(1970, 1, 1));
            return (long) ts.TotalMilliseconds;
        }

        /// <summary>
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static int GetDays(this DateTime fromDate, DateTime toDate)
        {
            return Convert.ToInt32(toDate.Subtract(fromDate).TotalDays);
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime Yesterday(this DateTime date)
        {
            return date.AddDays(-1);
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime SetTime(this DateTime value, int hours, int minutes, int seconds)
        {
            return value.Date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime Tomorrow(this DateTime date)
        {
            return date.AddDays(1);
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return date.SetTime(23, 59, 59);
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <param name="workDays"></param>
        /// <returns></returns>
        public static DateTime AddWorkdays(this DateTime date, int workDays)
        {
            var tmpDate = date;

            while (workDays != 0)
            {
                tmpDate = tmpDate.AddDays(workDays > 0 ? 1 : -1);
                if (tmpDate.DayOfWeek < DayOfWeek.Saturday &&
                    tmpDate.DayOfWeek > DayOfWeek.Sunday &&
                    !tmpDate.IsHoliday())
                    if (workDays > 0)
                        workDays--;
                    else
                        workDays++;
            }
            return tmpDate;
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsHoliday(this DateTime date)
        {
            var year = date.Year;

            var holidays = new DateTime[11];
            holidays[0] = new DateTime(year, 01, 01);
            holidays[1] = new DateTime(year, 01, 06);
            holidays[2] = new DateTime(year, 05, 01);
            holidays[3] = new DateTime(year, 05, 03);
            holidays[4] = new DateTime(year, 08, 15);
            holidays[5] = new DateTime(year, 11, 01);
            holidays[6] = new DateTime(year, 11, 11);
            holidays[7] = new DateTime(year, 12, 25);
            holidays[8] = new DateTime(year, 12, 26);
            var easter = CalculateEaster(year);
            holidays[9] = easter.AddDays(1);
            holidays[10] = easter.AddDays(60);

            return holidays.Contains(date.Date);
        }

        /// <summary>
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static DateTime CalculateEaster(int year)
        {
            var a = year%19;
            var b = year%4;
            var c = year%7;
            var d = (a*19 + GetA(year))%30;
            var e = (2*b + 4*c + 6*d + GetB(year))%7;
            if ((d == 29 && e == 6) ||
                (d == 28 && e == 6))
            {
                d -= 7;
            }
            var f = 22 + d + e;
            return f > 31 ? new DateTime(year, 04, f%31) : new DateTime(year, 03, f);
        }

        /// <summary>
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static int GetA(int year)
        {
            if (year <= 1582)
            {
                return 15;
            }
            if (year <= 1699)
            {
                return 22;
            }
            if (year <= 1899)
            {
                return 23;
            }
            if (year <= 2199)
            {
                return 24;
            }
            if (year <= 2299)
            {
                return 25;
            }
            if (year <= 2399)
            {
                return 26;
            }
            if (year <= 2499)
            {
                return 25;
            }

            return 0; /* poza zakresem */
        }

        /// <summary>
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static int GetB(int year)
        {
            if (year <= 1582)
            {
                return 6;
            }
            if (year <= 1699)
            {
                return 2;
            }
            if (year <= 1799)
            {
                return 3;
            }
            if (year <= 1899)
            {
                return 4;
            }
            if (year <= 2099)
            {
                return 5;
            }
            if (year <= 2199)
            {
                return 6;
            }
            if (year <= 2299)
            {
                return 0;
            }
            if (year <= 2499)
            {
                return 1;
            }

            return 0;
        }
        #endregion
    }
}