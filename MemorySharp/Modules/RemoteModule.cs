/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Patterns;

namespace Binarysharp.MemoryManagement.Modules
{
    /// <summary>
    ///     Class repesenting a module in the remote process.
    /// </summary>
    public class RemoteModule : RemoteRegion
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The dictionary containing all cached functions of the remote module.
        /// </summary>
        internal static readonly IDictionary<Tuple<string, SafeMemoryHandle>, RemoteFunction> CachedFunctions =
            new Dictionary<Tuple<string, SafeMemoryHandle>, RemoteFunction>();

        private Lazy<byte[]> LazyData { get; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="RemoteModule" /> class.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
        /// <param name="module">The native <see cref="ProcessModule" /> object corresponding to this module.</param>
        internal RemoteModule(MemoryBase memorySharp, ProcessModule module) : base(memorySharp, module.BaseAddress)
        {
            // Save the parameter
            Native = module;
            LazyData =
                new Lazy<byte[]>(
                    () => MemoryCore.ReadBytes(memorySharp.Handle, module.BaseAddress, module.ModuleMemorySize));
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     State if this is the main module of the remote process.
        /// </summary>
        public bool IsMainModule => MemorySharp.Process.MainModule.BaseAddress == BaseAddress;

        /// <summary>
        ///     Gets if the <see cref="RemoteModule" /> is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return base.IsValid &&
                    MemorySharp.Process.Modules.Cast<ProcessModule>()
                        .Any(m => m.BaseAddress == BaseAddress && m.ModuleName == Name);
            }
        }

        /// <summary>
        ///     Gets the modules data as an array of bytes.
        /// </summary>
        /// <value>
        ///     The modules data as a <see cref="byte" /> array.
        /// </value>
        public byte[] Data => LazyData.Value;

        /// <summary>
        ///     The name of the module.
        /// </summary>
        public string Name => Native.ModuleName;

        /// <summary>
        ///     The native <see cref="ProcessModule" /> object corresponding to this module.
        /// </summary>
        public ProcessModule Native { get; }

        /// <summary>
        ///     The full path of the module.
        /// </summary>
        public string Path => Native.FileName;

        /// <summary>
        ///     The size of the module in the memory of the remote process.
        /// </summary>
        public int Size => Native.ModuleMemorySize;

        /// <summary>
        ///     Gets the specified function in the remote module.
        /// </summary>
        /// <param name="functionName">The name of the function.</param>
        /// <returns>A new instance of a <see cref="RemoteFunction" /> class.</returns>
        public RemoteFunction this[string functionName] => FindFunction(functionName);
        #endregion

        #region Public Methods
        /// <summary>
        ///     Performs a pattern scan from a <see cref="SerializablePattern" /> struct.
        /// </summary>
        /// <param name="pattern">The <see cref="SerializablePattern" /> instance to use.</param>
        /// <returns>A new <see cref="PatternScanResult" /> instance.</returns>
        public PatternScanResult FindPattern(SerializablePattern pattern)
        {
            return FindPattern(pattern.TextPattern, pattern.OffsetToAdd, pattern.RebaseResult);
        }

        /// <summary>
        ///     Performs a pattern scan from a <see cref="Pattern" /> struct.
        /// </summary>
        /// m>
        /// <param name="pattern">The <see cref="Pattern" /> instance to use.</param>
        /// <returns>A new <see cref="PatternScanResult" /> instance.</returns>
        public PatternScanResult FindPattern(Pattern pattern)
        {
            return FindPattern(pattern.TextPattern, pattern.OffsetToAdd, pattern.RebaseResult);
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
        /// <returns>A new <see cref="PatternScanResult" /> instance.</returns>
        public PatternScanResult FindPattern(string patternText, int offsetToAdd, bool reBase)
        {
            var bytes = PatternCore.GetBytesFromDwordPattern(patternText);
            return FindPattern(bytes, offsetToAdd, reBase);
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
        /// <returns>A new <see cref="PatternScanResult" /> instance.</returns>
        public PatternScanResult FindPattern(byte[] pattern, int offsetToAdd, bool rebaseResult)
        {
            var mask = PatternCore.MaskFromPattern(pattern);
            return FindPattern(pattern, mask, offsetToAdd, rebaseResult);
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
        /// <returns>A new <see cref="PatternScanResult" /> instance.</returns>
        public PatternScanResult FindPattern(byte[] pattern, string mask, int offsetToAdd, bool rebaseResult)
        {
            for (var offset = 0; offset < Data.Length; offset++)
            {
                if (mask.Where((m, b) => m == 'x' && pattern[b] != Data[b + offset]).Any())
                {
                    continue;
                }

                var found = MemorySharp.Read<IntPtr>(BaseAddress + offset + offsetToAdd);

                var result = new PatternScanResult
                {
                    OriginalAddress = found,
                    Address = rebaseResult ? found : IntPtr.Subtract(found, (int) BaseAddress),
                    Offset = (IntPtr) offset
                };

                return result;
            }
            throw new Exception("Could not find the pattern.");
        }

        /// <summary>
        ///     Finds the specified function in the remote module.
        /// </summary>
        /// <param name="functionName">The name of the function (case sensitive).</param>
        /// <returns>A new instance of a <see cref="RemoteFunction" /> class.</returns>
        /// <remarks>
        ///     Interesting article on how DLL loading works: http://msdn.microsoft.com/en-us/magazine/bb985014.aspx
        /// </remarks>
        public RemoteFunction FindFunction(string functionName)
        {
            // Create the tuple
            var tuple = Tuple.Create(functionName, MemorySharp.Handle);

            // Check if the function is already cached
            if (CachedFunctions.ContainsKey(tuple))
                return CachedFunctions[tuple];

            // If the function is not cached
            // Check if the local process has this module loaded
            var localModule =
                Process.GetCurrentProcess()
                    .Modules.Cast<ProcessModule>()
                    .FirstOrDefault(m => string.Equals(m.FileName, Path, StringComparison.CurrentCultureIgnoreCase));
            var isManuallyLoaded = false;

            try
            {
                // If this is not the case, load the module inside the local process
                if (localModule == null)
                {
                    isManuallyLoaded = true;
                    localModule = ModuleCore.LoadLibrary(Native.FileName);
                }

                // Get the offset of the function
                var offset = ModuleCore.GetProcAddress(localModule, functionName).ToInt64() -
                    localModule.BaseAddress.ToInt64();

                // Rebase the function with the remote module
                var function = new RemoteFunction(MemorySharp, new IntPtr(Native.BaseAddress.ToInt64() + offset),
                    functionName);

                // Store the function in the cache
                CachedFunctions.Add(tuple, function);

                // Return the function rebased with the remote module
                return function;
            }
            finally
            {
                // Free the module if it was manually loaded
                if (isManuallyLoaded)
                    ModuleCore.FreeLibrary(localModule);
            }
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"BaseAddress = 0x{BaseAddress.ToInt64():X} Name = {Name}";
        }
        #endregion

        /// <summary>
        ///     Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
        /// <param name="module">The module to eject.</param>
        internal static void InternalEject(MemoryBase memorySharp, RemoteModule module)
        {
            // Call FreeLibrary remotely
            memorySharp.Threads.CreateAndJoin(memorySharp["kernel32"]["FreeLibrary"].BaseAddress, module.BaseAddress);
        }
    }
}