using System;
using Binarysharp.MemoryManagement.Common.Builders;

namespace Binarysharp.MemoryManagement.Common.Logging
{
    /// <summary>
    ///     A class to handle writing logs to the system <see cref="Console" />.
    /// </summary>
    public class ConsoleLogger : IManagedLog
    {
        #region Public Properties, Indexers
        /// <summary>
        ///     Gets a value indicating whether the element is disposed.
        /// </summary>
        public bool IsDisposed { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; set; }

        /// <summary>
        ///     States if the element is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     The name of the element.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (!MustBeDisposed)
            {
                return;
            }
            IsEnabled = false;
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disables the element.
        /// </summary>
        public void Disable()
        {
            IsEnabled = false;
        }

        /// <summary>
        ///     Enables the element.
        /// </summary>
        public void Enable()
        {
            IsEnabled = true;
        }

        /// <summary>
        ///     Logs the specified message to the console.
        /// </summary>
        /// <param name="message">The message being logged.</param>
        public void Write(string message)
        {
            Console.WriteLine($"{Name}{" "}{DateTime.Now}{"]: "}{message}");
        }
        #endregion
    }
}