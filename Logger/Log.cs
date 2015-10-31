using System;
using System.Collections.Generic;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;

namespace Binarysharp.MemoryManagement.Logger
{
    /// <summary>
    ///     A log manager class. TODO: Make this much more generic/flexible.
    /// </summary>
    public static class Log
    {
        #region  Fields
        private static readonly LinkedList<ILog> _logReaders = new LinkedList<ILog>();
        #endregion

        #region Constructors
        static Log()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Writes an information log to all <see cref="ILog" /> members added to the list via the <see cref="AddReader" />
        ///     method.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public static void Info(string text)
        {
            foreach (var logReader in _logReaders)
            {
                logReader.LogInfo(text);
            }
        }

        /// <summary>
        ///     Offsets a method to subsribe to that will log all unhandled exceptions.
        /// </summary>
        public static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Error("Unhandled exception:");
            var ex = e.ExceptionObject as Exception;
            do
            {
                Error(ex.Message);
                ex = ex.InnerException;
            } while (ex.InnerException != null);
        }

        /// <summary>
        ///     Writes a warning log to all <see cref="ILog" /> members added to the list via the <see cref="AddReader" /> method.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public static void Warning(string text)
        {
            foreach (var logReader in _logReaders)
            {
                logReader.LogWarning(text);
            }
        }

        /// <summary>
        ///     Writes an error log to all <see cref="ILog" /> members added to the list via the <see cref="AddReader" /> method.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public static void Error(string text)
        {
            ;
            foreach (var logReader in _logReaders)
            {
                logReader.LogError(text);
            }
        }

        /// <summary>
        ///     Writes a normal log to all <see cref="ILog" /> members added to the list via the <see cref="AddReader" /> method.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public static void Normal(string text)
        {
            foreach (var logReader in _logReaders)
            {
                logReader.LogNormal(text);
            }
        }

        /// <summary>
        ///     Adds a reader to the linked list of <see cref="ILog" /> Instances.
        /// </summary>
        /// <param name="logReader">The <see cref="ILog" /> member to add.</param>
        public static void AddReader(ILog logReader)
        {
            _logReaders.AddLast(logReader);
        }

        /// <summary>
        ///     Removes a reader to the linked list of <see cref="ILog" /> Instances.
        /// </summary>
        /// <param name="logReader">The <see cref="ILog" /> member to remove.</param>
        public static void RemoveReader(ILog logReader)
        {
            _logReaders.Remove(logReader);
        }
        #endregion
    }
}