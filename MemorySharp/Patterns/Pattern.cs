using System.Diagnostics;
using Binarysharp.MemoryManagement.Modules;

namespace Binarysharp.MemoryManagement.Patterns
{
    /// <summary>
    ///     A class that represents basic pattrn scanning properties.
    /// </summary>
    public struct Pattern
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Creates a new instance of <see cref="Pattern" />.
        /// </summary>
        /// <param name="dwordTextPattern">The dword pattern text containing the pattern to be scanned.</param>
        /// <example>
        ///     The example below will show you how a dword pattern would look for a given byte array.
        ///     <code>
        /// var bytes exampleBytes = new bytes[]{55,0x8B,0xEC,51,0xFF,05, 00, 00, 00, 00, 0xA2};
        /// var patternText = "55 8b ec 51 FF 05 ?? ?? ?? ?? A1";
        /// </code>
        /// </example>
        /// <param name="offseToAdd">The offset to add to the result found before returning the value.</param>
        /// <param name="rebaseResult">
        ///     If the address should be rebased to the base address of the <see cref="ProcessModule" />
        ///     the pattern data resides in.
        /// </param>
        public Pattern(string dwordTextPattern, int offseToAdd, bool rebaseResult)
        {
            TextPattern = dwordTextPattern;
            OffsetToAdd = offseToAdd;
            RebaseResult = rebaseResult;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     The dword text-based pattern.
        ///     <example>
        ///         The example below will show you how a dword pattern would look for a given byte array.
        ///         <code>
        /// var bytes exampleBytes = new bytes[]{55,0x8B,0xEC,51,0xFF,05, 00, 00, 00, 00, 0xA2};
        /// var patternText = "55 8b ec 51 FF 05 ?? ?? ?? ?? A1";
        /// </code>
        ///     </example>
        /// </summary>
        public string TextPattern { get; set; }

        /// <summary>
        ///     The value to add to the offset result when the pattern is first found.
        /// </summary>
        public int OffsetToAdd { get; set; }

        /// <summary>
        ///     If the address should be rebased to the base address of the <see cref="ProcessModule" /> the pattern data resides
        ///     in.
        /// </summary>
        public bool RebaseResult { get; set; }

        /// <summary>
        ///     Finds the pattern scan result from this instances data.
        /// </summary>
        /// <param name="memorySharp">The <see cref="MemorySharp" /> reference.</param>
        /// <param name="remoteModule">The <see cref="RemoteModule" /> the pattern is found in.</param>
        /// <returns></returns>
        public PatternScanResult Find(MemorySharp memorySharp, RemoteModule remoteModule)
            => remoteModule.FindPattern(this);
        #endregion
    }
}