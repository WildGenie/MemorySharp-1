using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1591

namespace MemorySharp.Tools.Logger
{
    [SuppressMessage("ReSharper", "InvocationIsSkipped")]
    public class DebugLog : ILog
    {
        #region  Interface members
        public void Warning(string message)
        {
            Debug.WriteLine($"{"[Warning]["}{DateTime.Now}{"]: "}{message}");
        }

        public void Normal(string message)
        {
            Debug.WriteLine($"{"[Normal]["}{DateTime.Now}{"]: "}{message}");
        }

        public void Error(string message)
        {
            Debug.WriteLine($"{"[Error]["}{DateTime.Now}{"]: "}{message}");
        }

        public void Info(string message)
        {
            Debug.WriteLine($"{"[Info]["}{DateTime.Now}{"]: "}{message}");
        }
        #endregion
    }
}