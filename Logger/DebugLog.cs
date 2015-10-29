using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1591

namespace MemorySharp.Logger
{
    [SuppressMessage("ReSharper", "InvocationIsSkipped")]
    public class DebugLog : ILog
    {
        #region  Interface members
        public void WriteWarning(string message)
        {
            Debug.WriteLine($"{"[Warning]["}{DateTime.Now}{"]: "}{message}");
        }

        public void WriteNormal(string message)
        {
            Debug.WriteLine($"{"[Normal]["}{DateTime.Now}{"]: "}{message}");
        }

        public void WriteError(string message)
        {
            Debug.WriteLine($"{"[Error]["}{DateTime.Now}{"]: "}{message}");
        }

        public void WriteInfo(string message)
        {
            Debug.WriteLine($"{"[Info]["}{DateTime.Now}{"]: "}{message}");
        }
        #endregion
    }
}