using System;
using System.Net;
using System.Net.Mail;
using MemorySharp.Tools.Extensions;

namespace MemorySharp.Tools.Emails
{
    /// <summary>
    ///     Contains basic information about SMTP client.
    /// </summary>
    public class SmtpSettings
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref=" SmtpSettings" /> class.
        /// </summary>
        /// <param name="userName">The user name associated with the credentials.</param>
        /// <param name="userPasword"> The password for the user name associated with the credentials.</param>
        /// <param name="host">The name or IP a ddress of the host used for SMTP transactions.</param>
        /// <param name="port">The port used for SMTP transactions.</param>
        /// <param name="enableSsl">Specify whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection.</param>
        public SmtpSettings(string userName, string userPasword, string host, int port, bool enableSsl)
        {
            Host = host;
            EnableSsl = enableSsl;
            Port = port;
            if (!userName.IsValidEmail())
            {
                throw new Exception("Invalid email username. Use the full username. Example: JohnDoe@outlook.com");
            }
            UserName = userName;
            Password = userPasword;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     Gets or sets the name or IP address of the host used for SMTP transactions.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Gets or sets the port used for SMTP transactions.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     Specify whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection.
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        ///     Gets or sets the user name associated with the credentials.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the password for the user name associated with the credentials.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Get SMTP client based on properties:
        ///     <see cref="SmtpSettings.Host" />,
        ///     <see cref="SmtpSettings.Port" />,
        ///     <see cref="SmtpSettings.UserName" />,
        ///     <see cref="SmtpSettings.EnableSsl" />,
        ///     <see cref="SmtpSettings.Password" />.
        /// </summary>
        public SmtpClient Client => new SmtpClient
        {
            EnableSsl = EnableSsl,
            Host = Host,
            Port = Port,
            Credentials = new NetworkCredential(UserName, Password),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
        #endregion
    }
}