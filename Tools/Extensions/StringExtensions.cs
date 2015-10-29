using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MemorySharp.Tools.Emails;

namespace MemorySharp.Tools.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="System.String" /> data type.
    /// </summary>
    public static class StringExtensions
    {
        #region Methods
        /// <summary>
        ///     Surrounds text with double quotes.
        /// </summary>
        /// <param name="text">The text to quote.</param>
        /// <returns>A string with double quotes.</returns>
        public static string SurroundWithDoubleQuotes(this string text)
        {
            return SurroundWith(text, "\"");
        }

        /// <summary>
        ///     Returns a string as a quote.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>A string.</returns>
        public static string Quoted(this string str)
        {
            return "\"" + str + "\"";
        }

        /// <summary>
        ///     Surrounds a string with a given string.
        /// </summary>
        /// <param name="text">The string to surround.</param>
        /// <param name="ends">Where th end the surround.</param>
        /// <returns>A string surrounded by new text.</returns>
        public static string SurroundWith(this string text, string ends)
        {
            return ends + text + ends;
        }

        /// <summary>
        ///     Determines whether the specified string can convert to <see cref="System.Boolean" />.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns>Returns <c>true</c> if the conversion of input string was successfull. Otherwise, it returns <c>false</c>.</returns>
        /// <remarks>
        ///     Based on <see cref="BooleanExtensions.BooleanMapping" />
        /// </remarks>
        public static bool IsBoolean(this string value)
        {
            var itemValue =
                BooleanExtensions.BooleanMapping.FirstOrDefault(v => v.Key == value.RemoveWhiteSpaces().ToUpperTrim());
            return !itemValue.IsDefault();
        }

        /// <summary>
        ///     Converts input string to <see cref="System.Boolean" />.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>Returns <c>true</c> if the conversion of input string was successfull. Otherwise, it returns <c>false</c>.</returns>
        /// <remarks>
        ///     Convertion is based on <see cref="BooleanExtensions.BooleanMapping" />.
        /// </remarks>
        public static bool ToBoolean(this string value)
        {
            var result = false;
            var itemValue =
                BooleanExtensions.BooleanMapping.FirstOrDefault(v => v.Key == value.RemoveWhiteSpaces().ToUpperTrim());
            if (!itemValue.IsDefault())
                result = itemValue.Value;

            return result;
        }

        /// <summary>
        ///     Determines whether the specified string can convert to <see cref="System.Decimal" />.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns>Returns <c>true</c> if the conversion of input string was successfull. Otherwise, it returns <c>false</c>.</returns>
        public static bool IsDecimal(this string value)
        {
            decimal result = 0;
            return decimal.TryParse(value.Replace(".", ","), NumberStyles.Currency, CultureInfo.CurrentCulture,
                out result);
        }

        /// <summary>
        ///     Converts input string to <see cref="System.Decimal" />.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>
        ///     If convertion succeed , returns parsed value. If failed, returns
        ///     <see cref="decimal">Inflop.Common.Settings.Default.Decimal</see>.
        /// </returns>
        public static decimal ToDecimal(this string value)
        {
            decimal result = 0;
            if (decimal.TryParse(value.Replace(".", ","), NumberStyles.Currency, CultureInfo.CurrentCulture,
                out result))
                return result;

            return default(decimal);
        }

        /// <summary>
        ///     Determines whether the specified string can convert to <see cref="System.Int32" />.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns>Returns <c>true</c> if the conversion of input string was successfull. Otherwise, it returns <c>false</c>.</returns>
        public static bool IsInteger(this string value)
        {
            int result;
            return int.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        ///     Converts input string to <see cref="System.Int32" />.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>
        ///     If convertion succeed , returns parsed value. If failed, returns
        ///     <see cref="Inflop.Common.Settings.Default.Integer">Inflop.Common.Settings.Default.Integer</see>.
        /// </returns>
        public static int ToInteger(this string value)
        {
            int result;
            return int.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result) ? result : 0;
        }

        /// <summary>
        ///     Determines whether the specified string can convert to <see cref="System.Int64" />.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns>Returns <c>true</c> if the conversion of input string was successfull. Otherwise, it returns <c>false</c>.</returns>
        public static bool IsLong(this string value)
        {
            long result;
            return long.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        ///     Converts input string to <see cref="System.Int64" />.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>
        ///     If convertion succeed , returns parsed value. If failed, returns
        ///     <see cref="Inflop.Common.Settings.Default.Long">Inflop.Common.Settings.Default.Long</see>.
        /// </returns>
        public static long ToLong(this string value)
        {
            long result;
            return long.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result) ? result : 0;
        }

        /// <summary>
        ///     Determines whether the specified string is null, empty or whitespace.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns></returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        ///     Determines whether the specified string is not null, empty or whitespace.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }

        /// <summary>
        ///     Returns a copy of this string converted to uppercase in which all leading and trailing occurrences
        ///     of a set of specified characters from the current String object are removed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUpperTrim(this string value)
        {
            return value.Trim().ToUpper();
        }

        /// <summary>
        ///     Returns a copy of this string converted to lowercase in which all leading and trailing occurrences
        ///     of a set of specified characters from the current String object are removed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLowerTrim(this string value)
        {
            return value.Trim().ToLower();
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveWhiteSpaces(this string value)
        {
            return Regex.Replace(value, @"\s|\t|\n|\r", string.Empty);
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string value)
        {
            var guid = Guid.Empty;
            Guid.TryParse(value, out guid);
            return guid;
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding">
        ///     The encoding to be used to convert.
        ///     If <c>null</c> then use <see cref="Encoding">Inflop.Common.Settings.Default.Encoding</see>.
        /// </param>
        /// <returns></returns>
        public static string ToSHA256Hash(this string value, Encoding encoding)
        {
            var e = encoding ?? Encoding.UTF8;
            byte[] hashValue = null;
            HashAlgorithm hash = SHA256.Create();
            var bytes = e.GetBytes(value);
            hashValue = hash.ComputeHash(bytes);

            return Convert.ToBase64String(hashValue);
        }

        /// <summary>
        ///     Converts the string to a byte-array using the supplied encoding
        /// </summary>
        /// <param name="value">
        ///     The input string.
        /// </param>
        /// <param name="encoding">
        ///     The encoding to be used to convert.
        ///     If <c>null</c> then use <see cref="Encoding">Inflop.Common.Settings.Default.Encoding</see>.
        /// </param>
        /// <returns>The created byte array</returns>
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            var e = encoding ?? Encoding.UTF8;
            return e.GetBytes(value);
        }

        /// <summary>
        ///     Checks the input string is a is valid email address.
        /// </summary>
        /// <param name="value">The input string to check.</param>
        /// <returns>Returns <see langword="true" /> if valid, otherwise returns <see langword="false" />.</returns>
        public static bool IsValidEmail(this string value)
        {
            var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(value);
        }

        /// <summary>
        ///     Checks the input string is a is valid NIP number.
        /// </summary>
        /// <param name="value">The input string to check.</param>
        /// <returns>Returns <see langword="true" /> if valid, otherwise returns <see langword="false" />.</returns>
        public static bool IsValidNIP(this string value)
        {
            int[] weights = {6, 5, 7, 2, 3, 4, 5, 6, 7};
            var result = false;
            if (value.Length == 10)
            {
                var controlSum = CalculateControlSum(value, weights);
                var controlNum = controlSum%11;
                if (controlNum == 10)
                {
                    controlNum = 0;
                }
                var lastDigit = int.Parse(value[value.Length - 1].ToString());
                result = controlNum == lastDigit;
            }
            return result;
        }

        /// <summary>
        ///     Checks the input string is a is valid REGON address.
        /// </summary>
        /// <param name="value">The input string to check.</param>
        /// <returns>Returns <see langword="true" /> if valid, otherwise returns <see langword="false" />.</returns>
        public static bool IsValidREGON(this string value)
        {
            int controlSum;
            if (value.Length == 7 || value.Length == 9)
            {
                int[] weights = {8, 9, 2, 3, 4, 5, 6, 7};
                var offset = 9 - value.Length;
                controlSum = CalculateControlSum(value, weights, offset);
            }
            else if (value.Length == 14)
            {
                int[] weights = {2, 4, 8, 5, 0, 9, 7, 3, 6, 1, 2, 4, 8};
                controlSum = CalculateControlSum(value, weights);
            }
            else
            {
                return false;
            }

            var controlNum = controlSum%11;
            if (controlNum == 10)
            {
                controlNum = 0;
            }
            var lastDigit = int.Parse(value[value.Length - 1].ToString());

            return controlNum == lastDigit;
        }

        /// <summary>
        ///     Checks the input string is a is valid PESEL address.
        /// </summary>
        /// <param name="value">The input string to check.</param>
        /// <returns>Returns <see langword="true" /> if valid, otherwise returns <see langword="false" />.</returns>
        public static bool IsValidPESEL(this string value)
        {
            int[] weights = {1, 3, 7, 9, 1, 3, 7, 9, 1, 3};
            var result = false;
            if (value.Length == 11)
            {
                var controlSum = CalculateControlSum(value, weights);
                var controlNum = controlSum%10;
                controlNum = 10 - controlNum;
                if (controlNum == 10)
                {
                    controlNum = 0;
                }
                var lastDigit = int.Parse(value[value.Length - 1].ToString());
                result = controlNum == lastDigit;
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weights"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static int CalculateControlSum(string input, int[] weights, int offset = 0)
        {
            var controlSum = 0;
            for (var i = 0; i < input.Length - 1; i++)
            {
                controlSum += weights[i + offset]*int.Parse(input[i].ToString());
            }
            return controlSum;
        }

        /// <summary>
        ///     Converts <see cref="string" /> into an <see cref="System.Enum" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="System.Enum" />.</typeparam>
        /// <param name="value">String value to parse</param>
        /// <param name="ignorecase">Ignore case or not.</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, bool ignorecase = true)
        {
            if (value.IsNull())
                return default(T);

            value = value.Trim();

            if (value.Length == 0)
                return default(T);

            var t = typeof (T);

            if (!t.IsEnum)
                return default(T);

            try
            {
                return (T) Enum.Parse(t, value, ignorecase);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        ///     Sends an email to address defined in the <see cref="Emailer.Settings" /> property,
        ///     using settings defined in <see cref="Emailer.Settings" /> class.
        /// </summary>
        /// <param name="emailer">The <see cref="Emailer" /> Instance.</param>
        /// <param name="body">The body of the message.</param>
        /// <param name="subject">The subject of the message.</param>
        public static void SendMe(this Emailer emailer, string body, string subject)
        {
            emailer.SendMe(subject, body);
        }
        #endregion
    }
}