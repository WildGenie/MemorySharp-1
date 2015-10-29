/**************************************************************************************
 *	File:		 ExceptionExtensions.cs
 *	Description: Extension methods for the System.Exception data type.
 *
 * 
 *	Author:		 infloper@gmail.com
 *	Created:	 6/14/2014 5:52:30 PM
 *	CLR ver:	 4.0.30319.18444
 * 
 **************************************************************************************
 * Changes history.
 **************************************************************************************
 * Date:		Author:				  Description:
 * --------		--------------------
 **************************************************************************************/

using System;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using MemorySharp.Tools.Emails;
using MemorySharp.Tools.Helpers;

namespace MemorySharp.Tools.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="System.Exception" /> data type.
    /// </summary>
    public static class ExceptionExtensions
    {
        #region  Fields
        /// <summary>
        /// </summary>
        private static int _exceptionLevel;
        #endregion

        #region Methods
        /// <summary>
        ///     Logs full information about the exception.
        /// </summary>
        /// <param name="ex">Thrown an exception to be logged.</param>
        public static void Log(this Exception ex)
        {
            Logger.Log.Error(ex.FullInfo());
        }

        /// <summary>
        ///     Returns full information about the exception
        /// </summary>
        /// <param name="ex">Thrown an exception to be reported.</param>
        /// <param name="htmlFormatted">Replace newline to HTML tag. Default <c>false</c>. </param>
        /// <returns></returns>
        public static string FullInfo(this Exception ex, bool htmlFormatted = false)
        {
            var sb = new StringBuilder();
            var boldFontTagOpen = htmlFormatted ? "<b>" : "";
            var boldFontTagClose = htmlFormatted ? "</b>" : "";

            _exceptionLevel++;
            var indent = new string('\t', _exceptionLevel - 1);
            sb.AppendFormat("{3}{0}*** Exception level {1} *************************************************{4}{2}",
                indent, _exceptionLevel, Environment.NewLine, boldFontTagOpen, boldFontTagClose);
            sb.AppendFormat("{3}{0}ExceptionType:{4} {1}{2}", indent, ex.GetType().Name, Environment.NewLine,
                boldFontTagOpen, boldFontTagClose);
            sb.AppendFormat("{3}{0}HelpLink:{4} {1}{2}", indent, ex.HelpLink, Environment.NewLine, boldFontTagOpen,
                boldFontTagClose);
            sb.AppendFormat("{3}{0}Message:{4} {1}{2}", indent, ex.Message, Environment.NewLine, boldFontTagOpen,
                boldFontTagClose);
            sb.AppendFormat("{3}{0}Source:{4} {1}{2}", indent, ex.Source, Environment.NewLine, boldFontTagOpen,
                boldFontTagClose);
            sb.AppendFormat("{3}{0}StackTrace:{4} {1}{2}", indent, ex.StackTrace, Environment.NewLine, boldFontTagOpen,
                boldFontTagClose);
            sb.AppendFormat("{3}{0}TargetSite:{4} {1}{2}", indent, ex.TargetSite, Environment.NewLine, boldFontTagOpen,
                boldFontTagClose);

            if (ex.Data.Count > 0)
            {
                sb.AppendFormat("{2}{0}Data:{3}{1}", indent, Environment.NewLine, boldFontTagOpen, boldFontTagClose);
                foreach (DictionaryEntry de in ex.Data)
                    sb.AppendFormat("{0}\t{1} : {2}", indent, de.Key, de.Value);
            }

            var innerException = ex.InnerException;

            while (innerException.IsNotNull())
            {
                sb.Append(innerException.FullInfo());
                innerException = _exceptionLevel > 1 ? innerException?.InnerException : null;
            }

            _exceptionLevel--;

            var result = htmlFormatted ? sb.ToString().Replace(Environment.NewLine, "<br />") : sb.ToString();

            return result;
        }

        /// <summary>
        ///     Sends an email with <see cref="ExceptionExtensions.FullInfo(Exception, bool)" /> to address defined in the
        /// </summary>
        /// <param name="ex">Thrown an exception to be reported.</param>
        /// <param name="emailer">The emailer Instance.</param>
        public static void SendToEmail(this Exception ex, Emailer emailer)
        {
            emailer.SendMe($"{ApplicationFinder.Name} - {ex.GetType()}{ex.FullInfo(true)}", "Exception log");
        }

        /// <summary>
        ///     Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void LogException(this Exception exception)
        {
            Logger.Log.Error(exception.ToString());
        }

        /// <summary>
        ///     Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void LogException(this SqlException exception)
        {
            Logger.Log.Error(exception.ToString());
        }

        /// <summary>
        ///     Gets the error message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The Exception Message text</returns>
        public static string GetErrorMessage(this Exception exception)
        {
            if (exception == null) return string.Empty;
            return exception.InnerException?.Message ?? exception.Message;
        }

        /// <summary>
        ///     Gets the error source.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>the exception source text</returns>
        public static string GetErrorSource(this Exception exception)
        {
            if (exception == null) return string.Empty;
            return exception.InnerException != null ? exception.InnerException.Source : exception.Source;
        }

        /// <summary>
        ///     Gets the name of the error type.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>the exception type name text</returns>
        public static string GetErrorTypeName(this Exception exception)
        {
            if (exception == null) return string.Empty;
            return exception.InnerException != null
                ? exception.InnerException.GetType().FullName
                : exception.GetType().FullName;
        }

        /// <summary>
        ///     Gets the error error details.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>the exception detail text</returns>
        public static string GetErrorErrorDetails(this Exception exception)
        {
            if (exception == null) return string.Empty;
            return exception.InnerException?.ToString() ?? exception.ToString();
        }
        #endregion
    }
}