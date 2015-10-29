using System;

namespace MemorySharp.Helpers.Logger
{
    /// <summary>
    ///     A class to handle writing logs to the system <see cref="Console" />.
    /// </summary>
    public class ConsoleLog : ILog
    {
        #region  Interface members
        public void WriteWarning(string message)
        {
            Console.WriteLine($"{"[Warning]["}{DateTime.Now}{"]: "}{message}");
        }

        public void WriteNormal(string message)
        {
            Console.WriteLine($"{"[Normal]["}{DateTime.Now}{"]: "}{message}");
        }

        public void WriteError(string message)
        {
            Console.WriteLine($"{"[Error]["}{DateTime.Now}{"]: "}{message}");
        }

        public void WriteInfo(string message)
        {
            Console.WriteLine($"{"[Info]["}{DateTime.Now}{"]: "}{message}");
        }
        #endregion
    }
}