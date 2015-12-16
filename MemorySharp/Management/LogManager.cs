using System;
using System.Collections.Generic;
using Binarysharp.MemoryManagement.Common.Builders;
using Binarysharp.MemoryManagement.Common.Logging;

namespace Binarysharp.MemoryManagement.Management
{
    /// <summary>
    ///     A lazy thread-safe implementation of singleton for the <see cref="LogManager" /> class.
    ///     The singleton pattern ensures a class has only one instance, and provide a global point of access to it.
    /// </summary>
    public sealed class LogManager : Manager<IManagedLog>, IFactory
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The thread-safe singleton. The object is created only when <see cref="Instance" /> is accessed.
        /// </summary>
        private static readonly Lazy<LogManager> LazyInstance = new Lazy<LogManager>(() => new LogManager());
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     The constructor must be marked as private in order to prevent people to instance it using the new keyword.
        /// </summary>
        private LogManager()
        {
            AddFileLogger("Logs", "ExceptionLog", "Debug");
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Public accessor to get the singleton.
        /// </summary>
        public static LogManager Instance => LazyInstance.Value;

        /// <summary>
        ///     Key => Value Indexor for this instances <see cref="IManagedLog" /> items it is currently managing.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        public IManagedLog this[string key] => InternalItems[key];
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            RemoveAll();
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Adds a <see cref="FileLogger" /> instance to the log manager.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="useFormattedText">if set to <c>true</c> [use formatted text].</param>
        /// <param name="mustBeDisposed">if set to <c>true</c> [must be disposed].</param>
        /// <returns>
        ///     The <see cref="FileLogger" /> instance contained in the log managers <see cref="Dictionary{TKey,TValue}" />
        ///     matching the instance name given.
        /// </returns>
        public FileLogger AddFileLogger(string directoryName, string fileName, string instanceName,
            bool useFormattedText = true, bool mustBeDisposed = true)
        {
            var log = FileLogger.QuickCreate(directoryName, fileName, instanceName, useFormattedText, mustBeDisposed);
            return AddLogger(log);
        }

        /// <summary>
        ///     Starts the logging of unhandled exceptions.
        /// </summary>
        public void StartLoggingOfUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
        }

        /// <summary>
        ///     Adds the logger to the manager.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="logger">The logger.</param>
        /// <returns>A <see cref="IManagedLog" /> instance casted to <code>(T);</code>.</returns>
        public T AddLogger<T>(T logger) where T : IManagedLog
        {
            InternalItems[logger.Name] = logger;
            return (T) InternalItems[logger.Name];
        }
        #endregion

        #region Private Methods
        private void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this["Debug"].Write("Unhandled exception.");
            var ex = e.ExceptionObject as Exception;
            do
            {
                this["Debug"].Write(ex?.Message);
                ex = ex?.InnerException;
            } while (ex?.InnerException != null);
        }
        #endregion
    }
}