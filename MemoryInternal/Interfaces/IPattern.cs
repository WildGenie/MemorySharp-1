using Binarysharp.MemoryManagement.MemoryInternal.Patterns;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.MemoryInternal.Interfaces
{
    /// <summary>
    ///     A class that defines pattern scanning operations.
    /// </summary>
    public interface IPattern
    {
        #region  Properties
        /// <summary>
        ///     The <see cref="SafeHandle" /> Instance.
        /// </summary>
        SafeMemoryHandle SafeHandle { get; set; }
        #endregion

        #region Methods
        /// <summary>
        ///     Performs a pattern scan.
        /// </summary>
        /// <param name="myPattern">The patterns bytes.</param>
        /// <param name="mask">The mask of the pattern. ? Is for wild card, x otherwise.</param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the pattern.</param>
        /// <param name="isOffsetMode">If the address is found from the base address + offset or not.</param>
        /// <param name="reBase">If the address should be rebased to the proccess's main modules base address.</param>
        /// <returns>A new <see cref="ScanResult" /></returns>
        ScanResult Find(byte[] myPattern, string mask, int offsetToAdd, bool isOffsetMode, bool reBase);

        /// <summary>
        ///     Performs a pattern scan.
        /// </summary>
        /// <param name="patternText">
        ///     The dword formatted text of the pattern.
        ///     <example>A2 5B ?? ?? ?? A2</example>
        /// </param>
        /// <param name="offsetToAdd">The offset to add to the offset result found from the pattern.</param>
        /// <param name="isOffsetMode">If the address is found from the base address + offset or not.</param>
        /// <param name="reBase">If the address should be rebased to the proccess's main modules base address.</param>
        /// <returns>A new <see cref="ScanResult" /></returns>
        ScanResult Find(string patternText, int offsetToAdd, bool isOffsetMode, bool reBase);

        /// <summary>
        ///     Performs a pattern scan.
        /// </summary>
        /// <param name="pattern">The <see cref="Pattern" /> Instance containing the data to use.</param>
        /// <returns>A new <see cref="ScanResult" /></returns>
        ScanResult Find(Pattern pattern);
        #endregion
    }
}