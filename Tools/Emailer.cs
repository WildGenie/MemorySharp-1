using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using Binarysharp.MemoryManagement.Extensions;
using Binarysharp.MemoryManagement.Helpers;

namespace Binarysharp.MemoryManagement.Tools
{
    /// <summary>
    ///     Allows you to send e-mail messages based on the settings contained in the
    ///     <see cref="SmtpSettings" /> class.
    /// </summary>
    public class Emailer
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="Emailer" /> class.
        /// </summary>
        /// <param name="settings">The smpt settings.</param>
        public Emailer(SmtpSettings settings)
        {
            Settings = settings;
            Client = settings.Client;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The smpt settngs.
        /// </summary>
        public SmtpSettings Settings { get; }

        /// <summary>
        ///     The smtp client.
        /// </summary>
        public SmtpClient Client { get; }
        #endregion

        #region Methods
        /// <summary>
        ///     Allows asynchronous sending email message.
        /// </summary>
        /// <param name="subject">E-mail subject text</param>
        /// <param name="body">Message body.</param>
        /// <param name="to">E-mail adddres of the recipient.</param>
        /// <param name="from">E-mail adddres of the sender.</param>
        /// <param name="attachments">Collections of full paths to attachments.</param>
        public void Send(string subject, string body, string to, string from = "",
            IEnumerable<string> attachments = null)
        {
            if (from.IsEmpty())
                from = Settings.UserName;

            var mailMsg = new MailMessage(from, to, subject, body);
            Send(mailMsg);
        }

        /// <summary>
        ///     Allows to sending email message.
        /// </summary>
        /// <param name="subject">E-mail subject text</param>
        /// <param name="body">Message body.</param>
        /// <param name="attachments">Collections of full paths to attachments.</param>
        public void SendMe(string subject, string body, IEnumerable<string> attachments = null)
        {
            Send(subject, body, Settings.UserName);
        }

        /// <summary>
        ///     Allows to sending email message.
        /// </summary>
        /// <param name="mailMessage">E-mail message</param>
        public async void Send(MailMessage mailMessage)
        {
            if (mailMessage.IsNull())
                throw new ArgumentNullException(nameof(mailMessage), "Parameter cannot be null");

            if (Settings.Host.IsEmpty())
                throw new ArgumentException("Smtp host cannot be empty.");

            if (Settings.Port < 1)
                throw new ArgumentException("Smtp port must be specified.");

            if (Settings.UserName.IsEmpty())
                throw new ArgumentException("Smtp user name must be specified.");

            if (Settings.Password.IsEmpty())
                throw new ArgumentException("Smtp user password must be specified.");

            SmtpClient client = null;

            try
            {
                using (client = Client)
                {
                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException ex)
            {
                ex.Log();
                throw;
            }
            catch (Exception ex)
            {
                ex.Log();
                throw;
            }
            finally
            {
                if (client.IsNotNull())
                {
                    Debug.Assert(client != null, "client != null");
                    client.Dispose();
                }
            }
        }
        #endregion
    }
}