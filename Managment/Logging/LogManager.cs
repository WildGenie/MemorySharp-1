using System;
using Binarysharp.MemoryManagement.Managment.Logging.Core;
using Binarysharp.MemoryManagement.Managment.Logging.Enums;

namespace Binarysharp.MemoryManagement.Managment.Logging
{
    /// <summary>
    ///     An very simple thread-safe implementation of singleton.
    ///     The singleton pattern ensures a class has only one instance, and provide a global point of access to it.
    /// </summary>
    /// <remarks>
    ///     For further details, read the article 'Implementing Singleton in C#' written by Microsoft:
    ///     http://msdn.microsoft.com/en-us/library/ff650316.aspx
    /// </remarks>
    public sealed class LogManager : SafeManager<IManagedLog>
    {
        /// <summary>
        ///     The thread-safe singleton. The object is created when any static members/functions of this class is called.
        /// </summary>
        public static readonly LogManager Instance = new LogManager();

        /// <summary>
        ///     Prevents a default instance of the <see cref="LogManager" /> class from being created.
        /// </summary>
        private LogManager() : base(new DebugLog())
        {
        }

        /// <summary>
        ///     Gets the <see cref="IManagedLog" /> with the specified updater name.
        /// </summary>
        /// <param name="updaterName">Name of the ManagedLog .</param>
        /// <returns>A ManagedLog.</returns>
        public IManagedLog this[string updaterName] => InternalItems[updaterName];

        /// <summary>
        ///     Logs the information.
        /// </summary>
        /// <param name="text">The text.</param>
        public void LogInfo(string text)
        {
            foreach (var logReader in InternalItems.Values)
                logReader.LogInfo(text);
        }

        /// <summary>
        ///     Logs the warning.
        /// </summary>
        /// <param name="text">The text.</param>
        public void LogWarning(string text)
        {
            foreach (var logReader in InternalItems.Values)
                logReader.LogWarning(text);
        }

        /// <summary>
        ///     Logs the error.
        /// </summary>
        /// <param name="text">The text.</param>
        public void LogError(string text)
        {
            foreach (var logReader in InternalItems.Values)
                logReader.LogError(text);
        }

        /// <summary>
        ///     Logs the normal.
        /// </summary>
        /// <param name="text">The text.</param>
        public void LogNormal(string text)
        {
            foreach (var logReader in InternalItems.Values)
                logReader.LogNormal(text);
        }

        /// <summary>
        ///     Gets the logType instance of a given type and adds it to the log manager.
        /// </summary>
        /// <param name="logType">The type of log to get.</param>
        /// <param name="name">The name.</param>
        /// <param name="enableRightAway">If the log should be enabled right away automatically.</param>
        /// <returns>ILog.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void AddLogger(LoggerType logType, string name, bool enableRightAway)
        {
            switch (logType)
            {
                case LoggerType.Console:
                    InternalItems[name] = new ConsoleLog();
                    break;
                case LoggerType.Debug:
                    InternalItems[name] = new DebugLog();
                    break;
                case LoggerType.File:
                    InternalItems[name] = FileLog.Create(name, name, "Logs", true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (enableRightAway)
            {
                InternalItems[name].Enable();
            }
        }
    }
}