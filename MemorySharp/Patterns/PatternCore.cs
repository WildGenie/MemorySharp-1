using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Binarysharp.MemoryManagement.Common.Builders;
using Binarysharp.MemoryManagement.Common.Extensions;
using Binarysharp.MemoryManagement.Common.Helpers;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.Patterns
{
    /// <summary>
    ///     Static core class for performing pattern scans.
    /// </summary>
    public static class PatternCore
    {
        #region Public Methods
        /// <summary>
        ///     Performs a pattern scan.
        /// </summary>
        /// <param name="processHandle">The process the <see cref="ProcessModule" /> containing the data resides in.</param>
        /// <param name="processModule">The <see cref="ProcessModule" /> that contains the pattern data resides in.</param>
        /// <param name="data">The array of bytes containing the data to search for matches in.</param>
        /// <param name="mask">
        ///     The mask that defines the byte pattern we are searching for.
        ///     <example>
        ///         <code>
        /// var bytes = new byte[]{55,45,00,00,55} ;
        /// var mask = "xx??x";
        /// </code>
        ///     </example>
        /// </param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the pattern.</param>
        /// <param name="reBase">If the address should be rebased to this  Instance's base address.</param>
        /// <param name="wildCardChar">
        ///     [Optinal] The 'wild card' defines the <see cref="char" /> value that the mask uses to differentiate
        ///     between pattern data that is relevant, and pattern data that should be ignored. The default value is 'x'.
        /// </param>
        /// <param name="pattern">The byte array that contains the pattern of bytes we're looking for.</param>
        /// <returns>A new <see cref="PatternScanResult" /> instance.</returns>
        public static PatternScanResult Find(SafeMemoryHandle processHandle, ProcessModule processModule, byte[] data,
            byte[] pattern,
            string mask, int offsetToAdd, bool reBase, char wildCardChar = 'x')
        {
            for (var offset = 0; offset < data.Length; offset++)
            {
                if (mask.Where((m, b) => m == 'x' && pattern[b] != data[b + offset]).Any()) continue;
                var found = MemoryCore.Read<IntPtr>(processHandle,
                    processModule.BaseAddress + offset + offsetToAdd);
                var result = new PatternScanResult
                {
                    OriginalAddress = found,
                    Address = reBase ? found : found.Subtract(processModule.BaseAddress),
                    Offset = (IntPtr) offset
                };
                return result;
            }
            // If this is reached, the pattern was not found.
            throw new Exception("The pattern scan for the pattern mask: " + "[" + mask + "]" + " was not found.");
        }

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
        public static string GetPatternMask(this SerializablePattern pattern)
        {
            return GetMaskFromDwordPattern(pattern.TextPattern);
        }

        /// <summary>
        ///     Gets the byte[] pattern from string format patterns.
        /// </summary>
        /// <param name="pattern">The string pattern to search for. ?? is mask and space between each byte and mask.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] GetPatternBytes(this SerializablePattern pattern)
        {
            return GetBytesFromDwordPattern(pattern.TextPattern);
        }

        /// <summary>
        ///     Gets an array of patterns from a xml file.
        /// </summary>
        /// <param name="nameOrPath">Name or path to the file.</param>
        /// <returns>A <see cref="SerializablePattern" /> array.</returns>
        public static SerializablePattern[] LoadXmlPatternFile(string nameOrPath)
        {
            return XmlHelper.ImportFromXmlFile<SerializablePattern[]>(nameOrPath);
        }


        /// <summary>
        ///     Gets an array of patterns from a xml file.
        /// </summary>
        /// <param name="nameOrPath">Name or path to the file.</param>
        /// <returns>A <see cref="SerializablePattern" /> array.</returns>
        public static SerializablePattern[] LoadJsonPatternFile(string nameOrPath)
        {
            return XmlHelper.ImportFromXmlFile<SerializablePattern[]>(nameOrPath);
        }

        /// <summary>
        ///     Creates a mask from a given pattern, using the given chars.
        /// </summary>
        /// <param name="pattern">The pattern this functions designs a mask for.</param>
        /// <param name="wildcardByte">Byte that is interpreted as a wildcard.</param>
        /// <param name="wildcardChar">Char that is used as wildcard.</param>
        /// <param name="matchChar">Char that is no wildcard.</param>
        /// <returns>A <see cref="string" /> containing the mask to use for the given array of bytes that make up the pattern.</returns>
        public static string MaskFromPattern(byte[] pattern, byte wildcardByte = 0, char wildcardChar = '?',
            char matchChar = 'x')
        {
            var chr = new char[pattern.Length];
            for (var i = 0; i < chr.Length; i++)
            {
                chr[i] = pattern[i] == wildcardByte ? wildcardChar : matchChar;
            }
            return new string(chr);
        }

        /// <summary>
        ///     Logs the pattern scan result to a text file as a useable pattern format for C#.
        /// </summary>
        /// <param name="log">The <see cref="ILog" /> member to use to log the result.</param>
        /// <param name="name">Name that represents the address.ed.</param>
        /// <param name="address">The address found from the pattern scan.</param>
        public static void LogFoundAddressToFile(ILog log, string name, IntPtr address)
        {
            try
            {
                log.Write(FormatAddressForFileLog(name, address));
            }
            catch (Exception e)
            {
                log.Write(e.ToString());
            }
        }

        /// <summary>
        ///     Formats a name and address result from a pattern scan to a useable format for C#.
        /// </summary>
        /// <param name="patternName">The name that represents the pattern.</param>
        /// <param name="address">The address found from pattern scan to log.</param>
        /// <returns>A C# useable string from the address found from a pattern scan.</returns>
        public static string FormatAddressForFileLog(string patternName, IntPtr address)
        {
            return $"public IntPtr {patternName} {" {get;} = "} {"(IntPtr) 0x"}{address.ToString("X")}{";"}";
        }
        #endregion
    }
}