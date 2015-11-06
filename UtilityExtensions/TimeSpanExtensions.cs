using System;

namespace Binarysharp.MemoryManagement.UtilityExtensions
{
    /// <summary>
    ///     A class containing extension methods for time spans.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        ///     To the date time.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.DateTime.</returns>
        public static DateTime ToDateTime(this TimeSpan self)
        {
            return new DateTime(1, 1, 1).Add(self);
        }

        /// <summary>
        ///     Trims the milliseconds.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.TimeSpan.</returns>
        public static TimeSpan TrimMilliseconds(this TimeSpan self)
        {
            return self.Add(TimeSpan.FromMilliseconds(-self.Milliseconds));
        }
    }
}