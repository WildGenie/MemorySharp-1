using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.Modules;
using Binarysharp.MemoryManagement.Patterns.Structs;

namespace Binarysharp.MemoryManagement.Patterns
{
    /// <summary>
    ///     A class for extracting data via pattern scanning process modules for patterns.
    /// </summary>
    public class InternalPatternFactory : IFactory
    {
        #region Fields, Private Properties
        private byte[] InternalModuleData { get; set; }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets or sets the instance reference for the <see cref="MemoryManagement.MemoryPlus" /> class.
        /// </summary>
        /// <value>
        ///     The instance reference for the <see cref="MemoryManagement.MemoryPlus" /> class.
        /// </value>
        public MemoryPlus MemoryPlus { get; set; }

        /// <summary>
        ///     Gets or sets the instance reference for the <see cref="System.Diagnostics.ProcessModule" /> that contains the data
        ///     to scan for pattens in.
        /// </summary>
        /// <value>
        ///     The <see cref="System.Diagnostics.ProcessModule"></see> instance that contains the data to scan for pattens in.
        /// </value>
        public ProcessModule ProcessModule { get; set; }

        /// <summary>
        ///     A dump of the <see cref="System.Diagnostics.ProcessModule" /> read data as a <see cref="byte" /> array.
        /// </summary>
        public byte[] ModuleData => InternalModuleData ??
                                    (InternalModuleData =
                                     MemoryPlus.ReadBytes(ProcessModule.BaseAddress, ProcessModule.ModuleMemorySize));
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Nothing.
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Performs a pattern scan from a <see cref="SerializablePattern" /> struct.
        /// </summary>
        /// <param name="pattern">The <see cref="SerializablePattern" /> instance to use.</param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public ScanResult Find(SerializablePattern pattern)
        {
            return Find(pattern.TextPattern, pattern.OffsetToAdd, pattern.RebaseResult);
        }

        /// <summary>
        ///     Performs a pattern scan from a <see cref="Pattern" /> struct.
        /// </summary>
        /// m>
        /// <param name="pattern">The <see cref="Pattern" /> instance to use.</param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public ScanResult Find(Pattern pattern)
        {
            return Find(pattern.TextPattern, pattern.OffsetToAdd, pattern.RebaseResult);
            ;
        }

        /// <summary>
        ///     Preform a pattern scan from a dword string based pattern.
        /// </summary>
        /// m>
        /// <param name="patternText">
        ///     The dword string based pattern text containing the pattern to try and find matches against.
        ///     <example>
        ///         <code>
        /// var bytes = new byte[]{55,45,00,00,55} ;
        /// var mask = "xx??x";
        /// </code>
        ///     </example>
        /// </param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the pattern.</param>
        /// <param name="reBase">If the address should be rebased to this <see cref="RemoteModule" /> Instance's base address.</param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public ScanResult Find(string patternText, int offsetToAdd, bool reBase)
        {
            var bytes = PatternCore.GetBytesFromDwordPattern(patternText);
            var mask = PatternCore.GetMaskFromDwordPattern(patternText);
            return Find(bytes, mask, offsetToAdd, reBase);
        }

        /// <summary>
        ///     Preform a pattern scan from a byte[] array pattern.
        /// </summary>
        /// <param name="pattern">The byte array that contains the pattern of bytes we're looking for.</param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the pattern.</param>
        /// <param name="rebaseResult">
        ///     If the final address result should be rebased to the base address of the
        ///     <see cref="ProcessModule" /> the pattern data resides in.
        /// </param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public ScanResult Find(byte[] pattern, int offsetToAdd, bool rebaseResult)
        {
            var mask = PatternCore.MaskFromPattern(pattern);
            return Find(pattern, mask, offsetToAdd, rebaseResult);
        }

        /// <summary>
        ///     Performs a pattern scan.
        /// </summary>
        /// <param name="pattern">The byte array that contains the pattern of bytes we're looking for.</param>
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
        /// <param name="rebaseResult">
        ///     If the final address result should be rebased to the base address of the
        ///     <see cref="ProcessModule" /> the pattern data resides in.
        /// </param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public ScanResult Find(byte[] pattern, string mask, int offsetToAdd, bool rebaseResult)
        {
            var scanResult = PatternCore.Find(MemoryPlus.Native, ProcessModule, ModuleData, pattern, mask, offsetToAdd,
                                              rebaseResult);
            return scanResult;
        }

        /// <summary>
        ///     Adds all pointers found from scanning an array of <see cref="SerializablePattern" /> instances to the given
        ///     <see cref="Dictionary{TKey,TValue}" /> instance using the <see cref="IDictionary{TKey,TValue}" /> interface.
        /// </summary>
        /// <param name="patterns">The array of <see cref="SerializablePattern" /> instances to scan.</param>
        /// <param name="resultDictionary">
        ///     The <see cref="Dictionary{TKey,TValue}" /> to add the results to. The key of each
        ///     result is the <see cref="SerializablePattern" /> instances <see cref="SerializablePattern.Description" /> and the
        ///     value is the <see cref="ScanResult.Address" /> found.
        /// </param>
        public void CollectScanResults(SerializablePattern[] patterns, IDictionary<string, IntPtr> resultDictionary)
        {
            foreach (var pattern in patterns)
            {
                resultDictionary.Add(pattern.Description, Find(pattern).Address);
            }
        }

        /// <summary>
        ///     Adds all pointers found from scanning a xml file to a given dictonary using the <code>IDictonary</code> interface.
        /// </summary>
        /// <param name="xmlFileNameOrPath">The name or path of the xml pattern file to use.</param>
        /// <param name="resultDictionary">The <see cref="Dictionary{TKey,TValue}" /> instance to add the results too.</param>
        public void CollectXmlScanResults(string xmlFileNameOrPath, IDictionary<string, IntPtr> resultDictionary)
        {
            var patterns = PatternCore.LoadXmlPatternFile(xmlFileNameOrPath);
            CollectScanResults(patterns, resultDictionary);
        }

        /// <summary>
        ///     Adds all pointers found from scanning a json file to a given dictonary using the <code>IDictonary</code> interface.
        /// </summary>
        /// <param name="jsonFileNameOrPath">The name or path of the json pattern file to use.</param>
        /// <param name="resultDictionary">The <see cref="Dictionary{TKey,TValue}" /> instance to add the results too.</param>
        public void CollectJsonScanResults(string jsonFileNameOrPath, IDictionary<string, IntPtr> resultDictionary)
        {
            var patterns = PatternCore.LoadJsonPatternFile(jsonFileNameOrPath);
            CollectScanResults(patterns, resultDictionary);
        }
        #endregion
    }
}