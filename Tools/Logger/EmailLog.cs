using System;
using MemorySharp.Tools.Emails;
using MemorySharp.Tools.Helpers;

namespace MemorySharp.Tools.Logger
{
    /// <summary>
    ///     A class for sending log results to an email
    /// </summary>
    public class EmailLog : ILog
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="EmailLog" /> class.
        /// </summary>
        /// <param name="emailer">The <see cref="Emails.Emailer" /> to use.</param>
        public EmailLog(Emailer emailer)
        {
            Emailer = emailer;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     If the log should use fully formatted text or the direct message given.
        /// </summary>
        public bool UseFormattedText { get; set; } = true;

        /// <summary>
        ///     The emailer to use.
        /// </summary>
        public Emailer Emailer { get; }
        #endregion

        #region  Interface members
        public void Normal(string message)
        {
            Emailer.SendMe(ApplicationFinder.Name + " " + ApplicationFinder.ApplicationVersion + "Normal Log",
                UseFormattedText ? FormatText("Normal", message) : message);
        }

        public void Error(string message)
        {
            Emailer.SendMe(ApplicationFinder.Name + " " + ApplicationFinder.ApplicationVersion + "Error Log",
                UseFormattedText ? FormatText("Error", message) : message);
        }

        public void Info(string message)
        {
            Emailer.SendMe(ApplicationFinder.Name + " " + ApplicationFinder.ApplicationVersion + "Info Log",
                UseFormattedText ? FormatText("Normal", message) : message);
        }

        public void Warning(string message)
        {
            Emailer.SendMe(ApplicationFinder.Name + " " + ApplicationFinder.ApplicationVersion + "Warning Log",
                UseFormattedText ? FormatText("Normal", message) : message);
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