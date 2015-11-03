using System;
using System.Collections.Generic;
using Binarysharp.MemoryManagement.Logging;

namespace Binarysharp.MemoryManagement.Managment
{
    /// <summary>
    ///     A static log manager class implementing <see cref="ILog" /> members.. TODO: Make this much more generic/flexible.
    /// </summary>
    public static class LogManager
    {
        #region  Fields

        /// <summary>
        ///     The linked list of <see cref="ILog" /> members.
        /// </summary>
        private static readonly LinkedList<ILog> _logMembers = new LinkedList<ILog>();

        #endregion

        #region Constructors

        static LogManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Logs the specified text as a <code>LogInfo</code> to all registered <see cref="ILog" /> interfaces.
        /// </summary>
        /// <param name="text">The text.</param>
        public static void LogInfo(string text)
        {
            foreach (var logReader in _logMembers)
            {
                logReader.LogInfo(text);
            }
        }

        /// <summary>
        ///     Logs the specified text as a <code>LogWarning</code> to all registered <see cref="ILog" /> interfaces.
        /// </summary>
        /// <param name="text">The text.</param>
        public static void LogWarning(string text)
        {
            foreach (var logReader in _logMembers)
            {
                logReader.LogWarning(text);
            }
        }

        /// <summary>
        ///     Logs the specified text as an <code>LogError</code> to all registered <see cref="ILog" /> interfaces.
        /// </summary>
        /// <param name="text">The text.</param>
        public static void LogError(string text)
        {
            ;
            foreach (var logReader in _logMembers)
            {
                logReader.LogError(text);
            }
        }

        /// <summary>
        ///     Logs the specified text as an <code>LogNormal</code> to all registered <see cref="ILog" /> interfaces.
        /// </summary>
        /// <param name="text">The text.</param>
        public static void LogNormal(string text)
        {
            foreach (var logReader in _logMembers)
            {
                logReader.LogNormal(text);
            }
        }

        /// <summary>
        ///     Adds a reader to the linked list of <see cref="ILog" /> Instances.
        /// </summary>
        /// <param name="logReader">The <see cref="ILog" /> member to add.</param>
        public static void AddLogger(ILog logReader)
        {
            _logMembers.AddLast(logReader);
        }

        /// <summary>
        ///     Removes a reader to the linked list of <see cref="ILog" /> Instances.
        /// </summary>
        /// <param name="logReader">The <see cref="ILog" /> member to remove.</param>
        public static void RemoveLogger(ILog logReader)
        {
            _logMembers.Remove(logReader);
        }

        /// <summary>
        ///     Logs the unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        public static void HandleLogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError("Unhandled exception:");
            var ex = e.ExceptionObject as Exception;
            do
            {
                if (ex == null) continue;
                LogError(ex.Message);
                ex = ex.InnerException;
            } while (ex?.InnerException != null);
        }

        #endregion
    }
}