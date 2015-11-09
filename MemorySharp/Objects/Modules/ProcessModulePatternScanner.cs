using System;
using System.Collections.Generic;
using System.Diagnostics;
using ToolsSharp.Memory;
using ToolsSharp.Native.Enums;
using ToolsSharp.Patterns;
using ToolsSharp.Patterns.Objects;

namespace Binarysharp.MemoryManagement.Objects.Modules
{
    /// <summary>
    ///     Class containing tools and values to perform various types of ProcessModulePattern scans on the given
    ///     <see cref="System.Diagnostics.ProcessModule" /> Instance.
    /// </summary>
    public class ProcessModulePatternScanner
    {
        #region Fields, Private Properties
        private Lazy<byte[]> LazyProcesProccessModuleData { get; }
        private Process Process { get; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessModulePatternScanner" /> class.
        /// </summary>
        /// <param name="processMemory">The process the process module is contained in.</param>
        /// <param name="processModule">The process module.</param>
        /// <param name="openHandle">if set to <c>true</c> [open handle].</param>
        public ProcessModulePatternScanner(Process processMemory, ProcessModule processModule, bool openHandle = true)
        {
            Process = processMemory;
            ProcessHandle = !openHandle
                ? processMemory.Handle
                : ExternalMemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, processMemory.Id);
            ProcessModule = processModule;
            ProcessModuleAddress = processModule.BaseAddress;
            ProcessModuleSize = processModule.ModuleMemorySize;
            LazyProcesProccessModuleData =
                new Lazy<byte[]>(
                    (() => ExternalMemoryCore.ReadProcessMemory(ProcessHandle, ProcessModuleAddress, ProcessModuleSize)));
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets the open handle to the process.
        /// </summary>
        /// <value>The process handle.</value>
        public IntPtr ProcessHandle { get; }

        /// <summary>
        ///     Gets the process module reference for this instance.
        /// </summary>
        /// <value>The process module.</value>
        public ProcessModule ProcessModule { get; }

        /// <summary>
        ///     Gets the process module address in memory.
        /// </summary>
        /// <value>The process module address.</value>
        public IntPtr ProcessModuleAddress { get; }

        /// <summary>
        ///     Gets the size of the process modules data.
        /// </summary>
        /// <value>The size of the process module's data.</value>
        public int ProcessModuleSize { get; }
        #endregion

        /// <summary>
        ///     Adds all pointers found from scanning a xml file to a given dictonary using the <code>IDictonary</code> interface.
        /// </summary>
        /// <param name="xmlFileNameOrPath">The name or path to the xml ProcessModulePattern file to use.</param>
        /// <param name="thePointerDictionary">The dictonary to fill.</param>
        public void CollectXmlScanResults(string xmlFileNameOrPath, IDictionary<string, IntPtr> thePointerDictionary)
        {
            var patterns = PatternCore.LoadXmlPatternFile(xmlFileNameOrPath);
            foreach (var pattern in patterns)
            {
                thePointerDictionary.Add(pattern.Description, Find(pattern).Address);
            }
        }


        /// <summary>
        ///     Adds all pointers found from scanning a json file to a given dictonary using the <code>IDictonary</code> interface.
        /// </summary>
        /// <param name="xmlFileNameOrPath">The name or path to the xml ProcessModulePattern file to use.</param>
        /// <param name="thePointerDictionary">The dictonary to fill.</param>
        public void CollectJsonScanResults(string xmlFileNameOrPath, IDictionary<string, IntPtr> thePointerDictionary)
        {
            var patterns = PatternCore.LoadJsonPatternFile(xmlFileNameOrPath);
            foreach (var pattern in patterns)
            {
                thePointerDictionary.Add(pattern.Description, Find(pattern).Address);
            }
        }

        /// <summary>
        ///     Adds all pointers found from scanning an array of <see cref="SerializablePattern" /> objects to a given dictonary
        ///     using the
        ///     IDictonary interface.
        /// </summary>
        /// <param name="processModulePatterns"></param>
        /// <param name="thePointerDictionary">The dictonary to fill.</param>
        public void CollectScanResults(SerializablePattern[] processModulePatterns,
                                       IDictionary<string, IntPtr> thePointerDictionary)
        {
            if (processModulePatterns == null) throw new ArgumentNullException(nameof(processModulePatterns));
            foreach (var pattern in processModulePatterns)
            {
                thePointerDictionary.Add(pattern.Description, Find(pattern).Address);
            }
        }

        /// <summary>
        ///     Performs a ProcessModulePattern scan.
        /// </summary>
        /// <param name="pattern">The <see cref="SerializablePattern" /> Instance containing the data to use.</param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public ScanResult Find(SerializablePattern pattern)
        {
            return Find(pattern.TextPattern, pattern.OffsetToAdd,
                pattern.IsOffsetMode, pattern.RebaseAddress);
        }

        /// <summary>
        ///     Performs a ProcessModulePattern scan.
        /// </summary>
        /// <param name="pattern">The <see cref="SerializablePattern" /> Instance containing the data to use.</param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public ScanResult Find(Pattern pattern)
        {
            return Find(pattern.TextPattern, pattern.OffsetToAdd,
                pattern.IsOffsetMode, pattern.RebaseAddress);
        }

        /// <summary>
        ///     Performs a ProcessModulePattern scan.
        /// </summary>
        /// <param name="patternText">
        ///     The dword formatted text of the ProcessModulePattern.
        ///     <example>A2 5B ?? ?? ?? A2</example>
        /// </param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the ProcessModulePattern.</param>
        /// <param name="isOffsetMode">If the address is found from the base address + offset or not.</param>
        /// <param name="reBase">If the address should be rebased to this <see cref="RemoteModule" /> Instance's base address.</param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public ScanResult Find(string patternText, int offsetToAdd, bool isOffsetMode, bool reBase)
        {
            var bytes = PatternCore.GetBytesFromDwordPattern(patternText);
            var mask = PatternCore.GetMaskFromDwordPattern(patternText);
            return Find(bytes, mask, offsetToAdd, isOffsetMode, reBase);
        }

        /// <summary>
        ///     Performs a ProcessModulePattern scan.
        /// </summary>
        /// <param name="myPattern">The processModulePatterns bytes.</param>
        /// <param name="mask">The mask of the ProcessModulePattern. ? Is for wild card, x otherwise.</param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the ProcessModulePattern.</param>
        /// <param name="isOffsetMode">If the address is found from the base address + offset or not.</param>
        /// <param name="reBase">If the address should be rebased to this <see cref="RemoteModule" /> Instance's base address.</param>
        /// <returns>A new <see cref="ScanResult" /> instance.</returns>
        public ScanResult Find(byte[] myPattern, string mask, int offsetToAdd, bool isOffsetMode,
                               bool reBase)
        {
            return PatternCore.Find(Process, ProcessModule, ProcessModule.BaseAddress, ProcessModuleSize, myPattern,
                mask, offsetToAdd, isOffsetMode, reBase);
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            ProcessModule.Dispose();
        }
    }
}