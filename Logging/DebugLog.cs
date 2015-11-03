using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1591

namespace Binarysharp.MemoryManagement.Logging
{
    [SuppressMessage("ReSharper", "InvocationIsSkipped")]
    public class DebugLog : ILog
    {
        #region  Interface members

        public void LogWarning(string message)
        {
            Debug.WriteLine($"{"[LogWarning]["}{DateTime.Now}{"]: "}{message}");
        }

        public void LogNormal(string message)
        {
            Debug.WriteLine($"{"[LogNormal]["}{DateTime.Now}{"]: "}{message}");
        }

        public void LogError(string message)
        {
            Debug.WriteLine($"{"[LogError]["}{DateTime.Now}{"]: "}{message}");
        }

        public void LogInfo(string message)
        {
            Debug.WriteLine($"{"[LogInfo]["}{DateTime.Now}{"]: "}{message}");
        }

        #endregion
    }
}