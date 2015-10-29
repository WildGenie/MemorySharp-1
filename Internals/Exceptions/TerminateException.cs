using System;

namespace MemorySharp.Internals.Exceptions
{
    public class TerminateException : Exception
    {
        #region Constructors
        public TerminateException()
        {
        }

        public TerminateException(string reason)
        {
            Reason = reason;
        }
        #endregion

        #region  Properties
        public string Reason { get; private set; }
        #endregion
    }
}