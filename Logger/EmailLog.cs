using System;
using Binarysharp.MemoryManagement.Helpers;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;
using Binarysharp.MemoryManagement.Tools;

namespace Binarysharp.MemoryManagement.Logger
{
    /// <summary>
    ///     A class for sending log results to an email.
    /// </summary>
    public class EmailLog : ILog
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="EmailLog" /> class.
        /// </summary>
        /// <param name="emailer">The <see cref="Tools.Emailer" /> to use.</param>
        /// <param name="useFormattedText">State if the log should use raw or formatted text.</param>
        public EmailLog(Emailer emailer, bool useFormattedText)
        {
            UseFormattedText = useFormattedText;
            Emailer = emailer;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     If the log should use fully formatted text or the direct message given.
        /// </summary>
        public bool UseFormattedText { get; set; }

        /// <summary>
        ///     The emailer to use.
        /// </summary>
        public Emailer Emailer { get; }
        #endregion

        #region  Interface members
        public void LogNormal(string message)
        {
            Emailer.SendMe(ApplicationFinder.Name + " " + ApplicationFinder.ApplicationVersion + "LogNormal Log",
                UseFormattedText ? FormatText("LogNormal", message) : message);
        }

        public void LogError(string message)
        {
            Emailer.SendMe(ApplicationFinder.Name + " " + ApplicationFinder.ApplicationVersion + "LogError Log",
                UseFormattedText ? FormatText("LogError", message) : message);
        }

        public void LogInfo(string message)
        {
            Emailer.SendMe(ApplicationFinder.Name + " " + ApplicationFinder.ApplicationVersion + "LogInfo Log",
                UseFormattedText ? FormatText("LogNormal", message) : message);
        }

        public void LogWarning(string message)
        {
            Emailer.SendMe(ApplicationFinder.Name + " " + ApplicationFinder.ApplicationVersion + "LogWarning Log",
                UseFormattedText ? FormatText("LogNormal", message) : message);
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