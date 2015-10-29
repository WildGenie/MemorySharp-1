using System;

namespace MemorySharp.Tools.Logger
{
    /// <summary>
    ///     A class to handle writing logs to the system <see cref="Console" />.
    /// </summary>
    public class ConsoleLog : ILog
    {
        #region  Interface members
        public void Warning(string message)
        {
            Console.WriteLine($"{"[Warning]["}{DateTime.Now}{"]: "}{message}");
        }

        public void Normal(string message)
        {
            Console.WriteLine($"{"[Normal]["}{DateTime.Now}{"]: "}{message}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"{"[Error]["}{DateTime.Now}{"]: "}{message}");
        }

        public void Info(string message)
        {
            Console.WriteLine($"{"[Info]["}{DateTime.Now}{"]: "}{message}");
        }
        #endregion
    }
}