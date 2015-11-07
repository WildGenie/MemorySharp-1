using System;
using System.Linq;

namespace Binarysharp.MemoryManagement.Common.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="System.DateTime" /> data type.
    /// </summary>
    public static class DateTimeExtensions
    {
        #region Fields, Private Properties
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

        /// <summary>
        ///     Determines whether the specified dt is today.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns><c>true</c> if the specified dt is today; otherwise, <c>false</c>.</returns>
        public static bool IsToday(this DateTime dt)
        {
            return (dt.Date == DateTime.Today);
        }

        /// <summary>
        ///     Determines whether [is date equal] [the specified date to compare].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="dateToCompare">The date to compare.</param>
        /// <returns><c>true</c> if [is date equal] [the specified date to compare]; otherwise, <c>false</c>.</returns>
        public static bool IsDateEqual(this DateTime date, DateTime dateToCompare)
        {
            return (date.Date == dateToCompare.Date);
        }

        /// <summary>
        ///     Determines whether [is time equal] [the specified time to compare].
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="timeToCompare">The time to compare.</param>
        /// <returns><c>true</c> if [is time equal] [the specified time to compare]; otherwise, <c>false</c>.</returns>
        public static bool IsTimeEqual(this DateTime time, DateTime timeToCompare)
        {
            return (time.TimeOfDay == timeToCompare.TimeOfDay);
        }

        /// <summary>
        ///     Gets the milliseconds since1970.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>System.Int64.</returns>
        public static long GetMillisecondsSince1970(this DateTime datetime)
        {
            var ts = datetime.Subtract(new DateTime(1970, 1, 1));
            return (long) ts.TotalMilliseconds;
        }

        /// <summary>
        ///     Gets the days.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>System.Int32.</returns>
        public static int GetDays(this DateTime fromDate, DateTime toDate)
        {
            return Convert.ToInt32(toDate.Subtract(fromDate).TotalDays);
        }

        /// <summary>
        ///     Yesterdays the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Yesterday(this DateTime date)
        {
            return date.AddDays(-1);
        }

        /// <summary>
        ///     Sets the time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="hours">The hours.</param>
        /// <param name="minutes">The minutes.</param>
        /// <param name="seconds">The seconds.</param>
        /// <returns>DateTime.</returns>
        public static DateTime SetTime(this DateTime value, int hours, int minutes, int seconds)
        {
            return value.Date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
        }

        /// <summary>
        ///     Tomorrows the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Tomorrow(this DateTime date)
        {
            return date.AddDays(1);
        }

        /// <summary>
        ///     Ends the of day.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return date.SetTime(23, 59, 59);
        }

        /// <summary>
        ///     Adds the workdays.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="workDays">The work days.</param>
        /// <returns>DateTime.</returns>
        public static DateTime AddWorkdays(this DateTime date, int workDays)
        {
            var tmpDate = date;

            while (workDays != 0)
            {
                tmpDate = tmpDate.AddDays(workDays > 0 ? 1 : -1);
                if (tmpDate.DayOfWeek < DayOfWeek.Saturday &&
                    tmpDate.DayOfWeek > DayOfWeek.Sunday &&
                    !tmpDate.IsAHoliday())
                    if (workDays > 0)
                        workDays--;
                    else
                        workDays++;
            }
            return tmpDate;
        }

        /// <summary>
        ///     Determines whether the specified date is a holiday.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if the specified date is a holiday; otherwise, <c>false</c>.</returns>
        public static bool IsAHoliday(this DateTime date)
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
        ///     Calculates the easter.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>DateTime.</returns>
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
        ///     Gets a.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>System.Int32.</returns>
        private static int GetA(int year)
        {
            var curyear = year;
            if (year <= 1582)
                return 15;
            if (year <= 1699)
                return 22;
            if (year <= 1899)
                return 23;
            if (year <= 2199)
                return 24;
            if (year <= 2299)
                return 25;
            if (year <= 2399)
                return 26;
            return year <= 2499 ? 25 : 0;
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
            return year <= 2499 ? 1 : 0;
        }
    }
}