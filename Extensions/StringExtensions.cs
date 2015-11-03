using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Binarysharp.MemoryManagement.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="System.String" /> data type.
    /// </summary>
    public static class StringExtensions
    {
        #region Methods

        public static string SurroundWithDoubleQuotes(this string text) => SurroundWith(text, "\"");
        public static string Quoted(this string str) => "\"" + str + "\"";
        public static string SurroundWith(this string text, string ends) => ends + text + ends;

        /// <summary>
        ///     Determines whether the specified string can convert to <see cref="System.Decimal" />.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns>Returns <c>true</c> if the conversion of input string was successfull. Otherwise, it returns <c>false</c>.</returns>
        public static bool IsDecimal(this string value)
        {
            decimal result;
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
            return decimal.TryParse(value.Replace(".", ","), NumberStyles.Currency, CultureInfo.CurrentCulture,
                out result)
                ? result
                : default(decimal);
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
        public static string ToSha256Hash(this string value, Encoding encoding)
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
        public static bool IsValidNip(this string value)
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
        public static bool IsValidRegon(this string value)
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
        public static bool IsValidPesel(this string value)
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

        #endregion
    }
}