using System;
using System.Collections.Generic;
using System.Linq;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.MemoryExternal.Modules;

namespace Binarysharp.MemoryManagement.MemoryExternal.Patterns
{
    /// <summary>
    /// </summary>
    public class PatternFactory : IFactory
    {
        #region  Fields
        private readonly MemorySharp _memory;
        private readonly RemoteModule _remoteModule;

        /// <summary>
        ///     The field for storing the modules data once dumped.
        /// </summary>
        private byte[] _moduleData;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="PatternFactory" /> class.
        /// </summary>
        /// <param name="memorySharp">The <see cref="MemorySharp" /> instance.</param>
        public PatternFactory(MemorySharp memorySharp, RemoteModule module)
        {
            _memory = memorySharp;
            _remoteModule = module;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     A dump of the modules data as a byte array.
        ///     <remarks>This value has a setter, should different module data than the processes main module data be desired.</remarks>
        /// </summary>
        public byte[] MainModuleData
        {
            get
            {
                return _moduleData ?? (_moduleData = _memory.ReadBytes(_remoteModule.BaseAddress, _remoteModule.Size));
            }
            set { _moduleData = value; }
        }
        #endregion

        #region  Interface members
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Nothing at the moment.
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Adds all pointers found from scanning a xml file to a given dictonary using the IDictonary interface.
        /// </summary>
        /// <param name="xmlFileNameOrPath">The name or path to the xml pattern file to use.</param>
        /// <param name="thePointerDictionary">The dictonary to fill.</param>
        public void AddPatternScannedPointersTo(string xmlFileNameOrPath,
            IDictionary<string, IntPtr> thePointerDictionary)
        {
            var patterns = PatternCore.LoadXmlPatternFile(xmlFileNameOrPath);
            foreach (var pattern in patterns)
            {
                thePointerDictionary.Add(pattern.Description, Find(pattern).Address);
            }
        }

        /// <summary>
        ///     Performs a pattern scan.
        /// </summary>
        /// <param name="pattern">The <see cref="Pattern" /> Instance containing the data to use.</param>
        /// <returns>A new <see cref="ScanResult" /></returns>
        public ScanResult Find(Pattern pattern)
        {
            return Find(pattern.TextPattern, pattern.OffsetToAdd, pattern.IsOffsetMode, pattern.RebaseAddress);
        }

        /// <summary>
        ///     Performs a pattern scan.
        /// </summary>
        /// <param name="patternText">
        ///     The dword formatted text of the pattern.
        ///     <example>A2 5B ?? ?? ?? A2</example>
        /// </param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the pattern.</param>
        /// <param name="isOffsetMode">If the address is found from the base address + offset or not.</param>
        /// <param name="reBase">If the address should be rebased to this <see cref="RemoteModule" /> Instance's base address.</param>
        /// <returns>A new <see cref="ScanResult" /></returns>
        public ScanResult Find(string patternText, int offsetToAdd, bool isOffsetMode, bool reBase)
        {
            var bytes = PatternCore.GetBytesFromDwordPattern(patternText);
            var mask = PatternCore.GetMaskFromDwordPattern(patternText);
            return Find(bytes, mask, offsetToAdd, isOffsetMode, reBase);
        }

        /// <summary>
        ///     Performs a pattern scan.
        /// </summary>
        /// <param name="myPattern">The patterns bytes.</param>
        /// <param name="mask">The mask of the pattern. ? Is for wild card, x otherwise.</param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the pattern.</param>
        /// <param name="isOffsetMode">If the address is found from the base address + offset or not.</param>
        /// <param name="reBase">If the address should be rebased to this <see cref="RemoteModule" /> Instance's base address.</param>
        /// <returns>A new <see cref="ScanResult" /></returns>
        public ScanResult Find(byte[] myPattern, string mask, int offsetToAdd, bool isOffsetMode,
            bool reBase)
        {
            var patternData = MainModuleData;
            var patternBytes = myPattern;
            var patternMask = mask;
            var result = new ScanResult();
            for (var offset = 0; offset < patternData.Length; offset++)
            {
                if (patternMask.Where((m, b) => m == 'x' && patternBytes[b] != patternData[b + offset]).Any()) continue;
                // If this area is reached, the pattern has been found.
                result.OriginalAddress = _memory.Read<IntPtr>(_remoteModule.BaseAddress + offset + offsetToAdd);
                result.Address = IntPtr.Subtract(result.OriginalAddress, (int) _remoteModule.BaseAddress);
                result.Offset = offset;
                if (!isOffsetMode)
                {
                    result.Address = reBase ? (_remoteModule.BaseAddress + (int) result.Address) : result.Address;
                    return result;
                }
                result.Offset = reBase ? (int) (_remoteModule.BaseAddress + offset) : result.Offset;
                result.Address = (IntPtr) result.Offset;
                return result;
            }
            // If this is reached, the pattern was not found.
            throw new Exception("The pattern " + "[" + myPattern + "]" + " was not found.");
        }
        #endregion
    }
}