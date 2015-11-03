using System;

namespace Binarysharp.MemoryManagement.Extensions
{
    /// <summary>
    ///     A class containing extension methods for time spans.
    /// </summary
    public static class TimeSpanExtensions
    {
        #region Methods

        public static DateTime ToDateTime(this TimeSpan self) => new DateTime(1, 1, 1).Add(self);

        public static TimeSpan TrimMilliseconds(this TimeSpan self)
            => self.Add(TimeSpan.FromMilliseconds(-self.Milliseconds));

        #endregion
    }
}