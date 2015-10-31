using System;
using System.IO;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;

namespace Binarysharp.MemoryManagement.Logger
{
    /// <summary>
    ///     A class class to handle writing logs to text files.
    /// </summary>
    public class FileLog : IDisposable, ILog
    {
        #region Constructors
        /// <summary>
        ///     The constructor for <see cref="FileLog" /> must be marked as private in order to prevent users initializing it
        ///     using the new keyword.
        /// </summary>
        /// <param name="folderName">The name of the log folder to create/use.</param>
        /// <param name="logFileName">The name of the log file to create/use.</param>
        /// <param name="useFormattedText">State if the log should use raw or formatted text.</param>
        public FileLog(string folderName, string logFileName, bool useFormattedText)
        {
            CurrentFileLogName = logFileName;
            CurrentFileLogFolderName = folderName;
            CurrentFileLogName = logFileName;
            UseFormattedText = useFormattedText;
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName));
            StreamWriter =
                new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName,
                    $"{logFileName}-{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.txt"))
                {AutoFlush = true};
            Log.AddReader(this);
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The Instance of <see cref="FileLog" />.
        ///     <remarks>Must be manually created in this case.</remarks>
        /// </summary>
        public static FileLog Instance { get; set; } = new FileLog("Logs", "Log", true);

        /// <summary>
        ///     The streamwrite to use to write to the file log.
        /// </summary>
        private StreamWriter StreamWriter { get; }

        /// <summary>
        ///     The folder name where the logs are stored.
        /// </summary>
        public string CurrentFileLogFolderName { get; private set; }

        /// <summary>
        ///     The name of the log text file.
        /// </summary>
        public string CurrentFileLogName { get; private set; }

        /// <summary>
        ///     If the log should use fully formatted text or the direct message given.
        /// </summary>
        public bool UseFormattedText { get; set; }
        #endregion

        #region  Interface members
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            StreamWriter.Flush();
            StreamWriter.Close();
            StreamWriter.Dispose();
        }

        public void LogNormal(string message)
        {
            StreamWriter.WriteLine(UseFormattedText ? FormatText("LogNormal", message) : message);
        }

        public void LogError(string message)
        {
            StreamWriter.WriteLine(UseFormattedText ? FormatText("LogError", message) : message);
        }

        public void LogInfo(string message)
        {
            StreamWriter.WriteLine(UseFormattedText ? FormatText("Information", message) : message);
        }

        public void LogWarning(string message)
        {
            StreamWriter.WriteLine(UseFormattedText ? FormatText("LogWarning", message) : message);
        }
        #endregion

        #region Methods
        private static string FormatText(string type, string text)
        {
            return $"{"["}{type}{"]"}{"["}{DateTime.Now.ToString("HH:mm:ss")}{"]: "}{text}";
        }
        #endregion
    }
}