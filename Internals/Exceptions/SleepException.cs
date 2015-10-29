using System;

namespace MemorySharp.Internals.Exceptions
{
    public class SleepException : Exception
    {
        #region Constructors
        public SleepException(int ms)
        {
            Time = TimeSpan.FromMilliseconds(ms);
        }

        public SleepException(TimeSpan time)
        {
            Time = time;
        }
        #endregion

        #region  Properties
        public TimeSpan Time { get; private set; }
        #endregion
    }
}