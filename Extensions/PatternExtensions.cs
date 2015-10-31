using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Binarysharp.MemoryManagement.Helpers;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;
using Binarysharp.MemoryManagement.MemoryInternal.Patterns;

namespace Binarysharp.MemoryManagement.Extensions
{
    /// <summary>
    ///     Static class providing tools for pattern scanning.
    /// </summary>
    public static class PatternExtensions
    {
        #region Methods
        /// <summary>
        ///     Gets the mask from a string based byte pattern to scan for.
        /// </summary>
        /// <param name="pattern">The string pattern to search for. ?? is mask and space between each byte and mask.</param>
        /// <returns>The mask from the pattern.</returns>
        public static string GetMaskFromDwordPattern(this string pattern)
        {
            var mask = pattern
                .Split(' ')
                .Select(s => s.Contains('?') ? "?" : "x");

            return string.Concat(mask);
        }

        /// <summary>
        ///     Adds all pointers found from scanning a xml file to a given dictonary using the IDictonary interface.
        /// </summary>
        /// <param name="patternInstance">The <see cref="IPattern" /> Instance.</param>
        /// <param name="xmlFileNameOrPath">The name or path to the xml pattern file to use.</param>
        /// <param name="thePointerDictionary">The dictonary to fill.</param>
        public static void AddPatternScannedPointersTo(this IPattern patternInstance, string xmlFileNameOrPath,
            IDictionary<string, IntPtr> thePointerDictionary)
        {
            var patterns = xmlFileNameOrPath.LoadXmlPatternFile();
            foreach (var pattern in patterns)
            {
                thePointerDictionary.Add(pattern.Description, patternInstance.Find(pattern).Address);
            }
        }

        /// <summary>
        ///     Gets the byte[] pattern from string format patterns.
        /// </summary>
        /// <param name="pattern">The string pattern to search for. ?? is mask and space between each byte and mask.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] GetBytesFromDwordPattern(this string pattern)
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
        public static Pattern[] LoadXmlPatternFile(this string nameOrPath)
        {
            return SerializationHelper.ImportFromXmlFile<Pattern[]>(nameOrPath);
        }
        #endregion
    }
}