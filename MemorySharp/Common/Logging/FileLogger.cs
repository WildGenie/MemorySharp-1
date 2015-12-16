using System;
using System.IO;
using Binarysharp.MemoryManagement.Common.Builders;

namespace Binarysharp.MemoryManagement.Common.Logging
{
    /// <summary>
    ///     Class for logging text to files.
    /// </summary>
    public class FileLogger : IManagedLog
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The streamwrite to use to write to the file log.
        /// </summary>
        private StreamWriter StreamWriter { get; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileLogger" /> class.
        /// </summary>
        /// <param name="fileLoggerParams">The file logger parameters.</param>
        public FileLogger(FileLoggerParams fileLoggerParams)
        {
            Name = fileLoggerParams.InstanceName;
            MustBeDisposed = fileLoggerParams.MustBeDisposed;
            UseFormattedText = fileLoggerParams.UseFormattedText;
            Directory.CreateDirectory(fileLoggerParams.Directory);
            StreamWriter =
                new StreamWriter(Path.Combine(fileLoggerParams.Directory, fileLoggerParams.FileName,
                    $"{fileLoggerParams.FileName}-{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.txt"))
                {AutoFlush = true};
            IsEnabled = true;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets or sets if the log should use formatted or raw text.
        /// </summary>
        /// <value> If the log should use formatted or raw text.</value>
        public bool UseFormattedText { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the element is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; }

        /// <summary>
        ///     States if the element is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     The name of the element.
        /// </summary>
        public string Name { get; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        /// <param name="message">The message being logged.</param>
        public void Write(string message)
        {
            StreamWriter.WriteLine(UseFormattedText ? FormatText("[" + Name + "] ", message) : message);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Dispose()
        {
            if (IsDisposed || !MustBeDisposed) return;
            StreamWriter.Dispose();
            IsDisposed = true;
        }

        /// <summary>
        ///     Disables the element.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Disable()
        {
            IsEnabled = false;
        }

        /// <summary>
        ///     Enables the element.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Enable()
        {
            IsEnabled = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Creates and returns a new <see cref="FileLogger" /> instance with the file residing with in the base directory of
        ///     the executing application.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="useFormattedText">
        ///     Whether the fiie log instances log entrys must be formatted with
        ///     InstanceName::HH::MM::SS before being written to the file.
        /// </param>
        /// <param name="mustMeDisposed">
        ///     Whether the <see cref="FileLogger" /> must be disposed when the Garbage Collector
        ///     collects the object.
        /// </param>
        /// <returns>A new <see cref="FileLogger" /> instance.</returns>
        public static FileLogger QuickCreate(string folderName, string fileName, string instanceName,
            bool useFormattedText = true, bool mustMeDisposed = true)
            =>
                new FileLogger(new FileLoggerParams
                {
                    Directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName),
                    FileName = fileName,
                    MustBeDisposed = mustMeDisposed,
                    UseFormattedText = useFormattedText,
                    InstanceName = instanceName
                });
        #endregion

        #region Private Methods
        private static string FormatText(string type, string text)
        {
            return $"{"["}{type}{"]"}{"["}{DateTime.Now.ToString("HH:mm:ss")}{"]: "}{text}";
        }
        #endregion
    }

    /// <summary>
    ///     The params needed to create a file logger.
    /// </summary>
    public struct FileLoggerParams
    {
        /// <summary>
        ///     Gets or sets the directory.
        /// </summary>
        /// <value>
        ///     The directory.
        /// </value>
        public string Directory { get; set; }

        /// <summary>
        ///     Gets or sets the name of the file.
        /// </summary>
        /// <value>
        ///     The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the instance.
        /// </summary>
        /// <value>
        ///     The name of the instance.
        /// </value>
        public string InstanceName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [must be disposed].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [must be disposed]; otherwise, <c>false</c>.
        /// </value>
        public bool MustBeDisposed { get; set; }

        /// <summary>
        ///     Gets or sets if the log should use formatted or raw text.
        /// </summary>
        /// <value> If the log should use formatted or raw text.</value>
        public bool UseFormattedText { get; set; }
    }
}