using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;

#pragma warning disable 1591

namespace Binarysharp.MemoryManagement.Logger
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