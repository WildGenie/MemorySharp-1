using System.Globalization;
using System.Linq;
using MemorySharp.Helpers;

namespace MemorySharp.Patterns
{
    /// <summary>
    ///     Static class providing tools for pattern scanning.
    /// </summary>
    public static class PatternCore
    {
        #region Methods
        /// <summary>
        ///     Gets the mask from a string based byte pattern to scan for.
        /// </summary>
        /// <param name="pattern">The string pattern to search for. ?? is mask and space between each byte and mask.</param>
        /// <returns>The mask from the pattern.</returns>
        public static string GetMaskFromDwordPattern(string pattern)
        {
            var mask = pattern
                .Split(' ')
                .Select(s => s.Contains('?') ? "?" : "x");

            return string.Concat(mask);
        }

        /// <summary>
        ///     Gets the byte[] pattern from string format patterns.
        /// </summary>
        /// <param name="pattern">The string pattern to search for. ?? is mask and space between each byte and mask.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] GetBytesFromDwordPattern(string pattern)
        {
            return pattern
                .Split(' ')
                .Select(s => s.Contains('?') ? (byte) 0 : byte.Parse(s, NumberStyles.HexNumber))
                .ToArray();
        }

        /// <summary>
        ///     Gets the mask from a string based byte pattern to scan for.
        /// </summary>
        /// <param name="pattern">The string pattern to search for. ?? is mask and space between each byte and mask.</param>
        /// <returns>The mask from the pattern.</returns>
        public static string GetPatternMask(this Pattern pattern)
        {
            return GetMaskFromDwordPattern(pattern.TextPattern);
        }

        /// <summary>
        ///     Gets the byte[] pattern from string format patterns.
        /// </summary>
        /// <param name="pattern">The string pattern to search for. ?? is mask and space between each byte and mask.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] GetPatternBytes(this Pattern pattern)
        {
            return GetBytesFromDwordPattern(pattern.TextPattern);
        }

        /// <summary>
        ///     Gets an array of patterns from a xml file.
        /// </summary>
        /// <param name="nameOrPath">Name or path to the file.</param>
        /// <returns>A <see cref="Pattern" /> array.</returns>
        public static Pattern[] LoadXmlPatternFile(string nameOrPath)
        {
            return SerializationHelper.ImportFromXmlFile<Pattern[]>(nameOrPath);
        }
        #endregion
    }
}