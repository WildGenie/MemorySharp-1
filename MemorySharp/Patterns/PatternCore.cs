using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Binarysharp.MemoryManagement.Helpers;
using Binarysharp.MemoryManagement.Memory;

namespace Binarysharp.MemoryManagement.Patterns
{
    /// <summary>
    ///     Static core class for performing pattern scans.
    /// </summary>
    public static class PatternCore
    {
        /// <summary>
        ///     Performs a ProcessModulePattern scan.
        /// </summary>
        /// <param name="pattern">The processModulePatterns bytes.</param>
        /// <param name="mask">The mask of the ProcessModulePattern. ? Is for wild card, x otherwise.</param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the ProcessModulePattern.</param>
        /// <param name="isOffsetMode">If the address is found from the base address + offset or not.</param>
        /// <param name="reBase">If the address should be rebased to process modules base address.</param>
        /// <param name="handle">The handle to the process module is contained in.</param>
        /// <param name="module">The process module the pattern data is contained in.</param>
        /// <param name="wildCardChar">
        ///     [Optinal] The 'wild card' defines the <see cref="char" /> value that the mask uses to differentiate
        ///     between pattern data that is relevant, and pattern data that should be ignored. The default value is 'x'.
        /// </param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public static ScanResult Find(IntPtr handle, ProcessModule module, byte[] pattern, string mask, int offsetToAdd,
                                      bool isOffsetMode, bool reBase, char wildCardChar = 'x')
        {
            var patternData = ExternalMemoryCore.ReadProcessMemory(handle, module.BaseAddress, module.ModuleMemorySize);
            for (var offset = 0; offset < patternData.Length; offset++)
            {
                if (mask.Where((c, b) => c == 'x' && pattern[b] != patternData[b + offset]).Any())
                    continue;
                // If this code is reached, then the pattern was located.
                IntPtr found;
                TryGetPatternAddress(out found, handle, module, offsetToAdd, offset);
                if (found != IntPtr.Zero)
                    return new ScanResult
                           {
                               OriginalAddress = found,
                               Address = !reBase ? found : found.Subtract(module.BaseAddress),
                               Offset = !reBase ? (IntPtr) offset : module.BaseAddress.Add(offset)
                           };
            }
            // If this code is reached, it is likely no pattern match was found.
            throw new Exception("The ProcessModulePattern " + "[" + pattern + "]" + " was not found.");
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
            return XmlHelper.ImportFromFile<SerializablePattern[]>(nameOrPath);
        }


        /// <summary>
        ///     Gets an array of patterns from a xml file.
        /// </summary>
        /// <param name="nameOrPath">Name or path to the file.</param>
        /// <returns>A <see cref="SerializablePattern" /> array.</returns>
        public static SerializablePattern[] LoadJsonPatternFile(string nameOrPath)
        {
            return JsonHelper.ImportFromFile<SerializablePattern[]>(nameOrPath);
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

        private static void TryGetPatternAddress(out IntPtr found, IntPtr handle, ProcessModule module, int offsetToAdd,
                                                 int offset)
        {
            found = ExternalMemoryCore.Read<IntPtr>(handle, module.BaseAddress + offset + offsetToAdd);
        }
    }
}