using System;

namespace Binarysharp.MemoryManagement.Patterns
{
    /// <summary>
    ///     Contains data regarding a pattern scan result.
    /// </summary>
    public class ScanResult
    {
        #region Properties
        /// <summary>
        ///     The address found.
        /// </summary>
        public IntPtr Address { get; set; }

        /// <summary>
        ///     The offset found.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        ///     The original address found.
        /// </summary>
        public IntPtr OriginalAddress { get; set; }
        #endregion
    }
}