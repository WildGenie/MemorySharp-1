using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ToolsSharp.Extensions;
using ToolsSharp.Helpers;
using ToolsSharp.Marshaling;
using ToolsSharp.Memory;
using ToolsSharp.Patterns.Objects;

namespace ToolsSharp.Patterns
{
    public static class PatternCore
    {
        /// <summary>
        ///     Performs a ProcessModulePattern scan.
        /// </summary>
        /// <param name="end">The length of the data.</param>
        /// <param name="myPattern">The processModulePatterns bytes.</param>
        /// <param name="mask">The mask of the ProcessModulePattern. ? Is for wild card, x otherwise.</param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the ProcessModulePattern.</param>
        /// <param name="isOffsetMode">If the address is found from the base address + offset or not.</param>
        /// <param name="reBase">If the address should be rebased to process modules base address.</param>
        /// <param name="process">The process the process module is contained in.</param>
        /// <param name="module">The process module the pattern data is contained in.</param>
        /// <param name="start">The starting address of the pattern data.</param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public static ScanResult Find(Process process, ProcessModule module, IntPtr start, int end, byte[] myPattern,
                                      string mask, int offsetToAdd, bool isOffsetMode,
                                      bool reBase)
        {
            var patternData = ExternalMemoryCore.ReadProcessMemory(process.Handle, start, end);
            var patternBytes = myPattern;
            var patternMask = mask;
            var result = new ScanResult();
            for (var offset = 0; offset < patternData.Length; offset++)
            {
                if (patternMask.Where((m, b) => m == 'x' && patternBytes[b] != patternData[b + offset]).Any()) continue;
                // If this area is reached, the ProcessModulePattern has been found.
                result.OriginalAddress = SafeMarshal<IntPtr>.PtrToObject(process.Handle,
                    module.BaseAddress + offset + offsetToAdd);
                result.Address = result.OriginalAddress.Subtract(module.BaseAddress);
                result.Offset = offset;
                if (!isOffsetMode)
                {
                    result.Address = reBase ? module.BaseAddress.Add(result.Address) : result.Address;
                    return result;
                }
                result.Offset = reBase ? (int) module.BaseAddress.Add(offset) : result.Offset;
                result.Address = (IntPtr) result.Offset;
                return result;
            }
            // If this is reached, the ProcessModulePattern was not found.
            throw new Exception("The ProcessModulePattern " + "[" + myPattern + "]" + " was not found.");
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
    }
}