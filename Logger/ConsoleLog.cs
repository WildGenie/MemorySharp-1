using System;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;

namespace Binarysharp.MemoryManagement.Logger
{
    /// <summary>
    ///     A class to handle writing logs to the system <see cref="Console" />.
    /// </summary>
    public class ConsoleLog : ILog
    {
        #region  Interface members
        public void LogWarning(string message)
        {
            Console.WriteLine($"{"[LogWarning]["}{DateTime.Now}{"]: "}{message}");
        }

        public void LogNormal(string message)
        {
            Console.WriteLine($"{"[LogNormal]["}{DateTime.Now}{"]: "}{message}");
        }

        public void LogError(string message)
        {
            Console.WriteLine($"{"[LogError]["}{DateTime.Now}{"]: "}{message}");
        }

        public void LogInfo(string message)
        {
            Console.WriteLine($"{"[LogInfo]["}{DateTime.Now}{"]: "}{message}");
        }
        #endregion
    }
}