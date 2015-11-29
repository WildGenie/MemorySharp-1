using System;
using System.Collections.Generic;
using System.Diagnostics;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.Modules;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Patterns.Structs;

namespace Binarysharp.MemoryManagement.Patterns
{
    /// <summary>
    ///     A class for extracting data via pattern scanning process modules for patterns.
    /// </summary>
    public class PatternFactory : IFactory
    {
        #region Fields, Private Properties
        private byte[] InternalModuleData { get; set; }
        private MemorySharp MemorySharp { get; }
        private ProcessModule ProcessModule { get; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="PatternFactory" /> class.
        /// </summary>
        /// <param name="memorySharp">The memory sharp instance.</param>
        /// <param name="processModule">The process module the pattern data is contained in,</param>
        public PatternFactory(MemorySharp memorySharp, ProcessModule processModule)
        {
            MemorySharp = memorySharp;
            ProcessModule = processModule;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     A dump of the modules data as a byte array.
        /// </summary>
        public byte[] ModuleData => InternalModuleData ??
                                    (InternalModuleData =
                                     MemorySharp.ReadBytes(ProcessModule.BaseAddress, ProcessModule.ModuleMemorySize))
            ;
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
            return Find(bytes, offsetToAdd, reBase);
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
            return PatternCore.Find(MemorySharp.Native, ProcessModule, ModuleData, pattern, mask, offsetToAdd,
                                    rebaseResult);
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

        /// <summary>
        ///     Logs pattern scan results from n xml file or json file containing an array of <see cref="SerializablePattern" />
        ///     objects to a text file. The log should give ready-to-go formatting results from the pattern scan. An example result
        ///     logged would be <code>public IntPtr MyPattern {get;} = (IntPtr)0x0000;</code>
        /// </summary>
        /// <param name="xmlFileNameOrPath">The XML file name or path.</param>
        /// <param name="patternFileType">Type of the pattern file.</param>
        public void LogScanResultsToFile(string xmlFileNameOrPath, PatternFileType patternFileType)
        {
            var results = new Dictionary<string, IntPtr>();
            switch (patternFileType)
            {
                case PatternFileType.Xml:
                    CollectXmlScanResults(xmlFileNameOrPath, results);
                    foreach (var result in results)
                    {
                        PatternCore.LogFoundAddressToFile(result.Key, result.Value);
                    }
                    return;
                case PatternFileType.Json:
                    CollectJsonScanResults(xmlFileNameOrPath, results);
                    foreach (var result in results)
                    {
                        PatternCore.LogFoundAddressToFile(result.Key, result.Value);
                    }
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(patternFileType), patternFileType, null);
            }
        }
        #endregion
    }
}