using System.Net.Mail;

namespace MemorySharp.Tools.Emails
{
    /// <summary>
    ///     A class providing helper methods related to the<see cref="Emailer" /> class.
    /// </summary>
    public static class EmailHelper
    {
        #region Methods
        /// <summary>
        ///     Gets an Instance of <see cref="Emailer" /> for U.S's outlook client.
        /// </summary>
        /// <param name="userName">Your full email.</param>
        /// <param name="userPassword">Password.</param>
        /// <returns>Settings for outlook.</returns>
        public static Emailer GetOutlookClient(string userName, string userPassword)
        {
            return new Emailer(CreateSmtpSettings(userName, userPassword, "smtp-mail.outlook.com", 25, true));
        }

        /// <summary>
        ///     Creates a <see cref="SmtpSettings" /> Instance.
        /// </summary>
        /// <param name="userName">Full username.</param>
        /// <param name="userPasword">Password.</param>
        /// <param name="host">Host or I.P</param>
        /// <param name="port">The port.</param>
        /// <param name="enableSsl">If SSL should be enabled.</param>
        /// <returns>A new <see cref="SmtpSettings" /> Instance.</returns>
        public static SmtpSettings CreateSmtpSettings(string userName, string userPasword, string host, int port,
            bool enableSsl)
        {
            return new SmtpSettings(userName, userPasword, host, port, enableSsl);
        }

        /// <summary>
        ///     Creates a <see cref="SmtpClient" /> Instance.
        /// </summary>
        /// <param name="userName">Full username.</param>
        /// <param name="userPasword">Password.</param>
        /// <param name="host">Host or I.P</param>
        /// <param name="port">The port.</param>
        /// <param name="enableSsl">If SSL should be enabled.</param>
        /// <returns>A new <see cref="SmtpClient" /> Instance.</returns>
        public static SmtpClient CreateSmtpClient(string userName, string userPasword, string host, int port,
            bool enableSsl)
        {
            return new SmtpSettings(userName, userPasword, host, port, enableSsl).Client;
        }
        #endregion
    }
}