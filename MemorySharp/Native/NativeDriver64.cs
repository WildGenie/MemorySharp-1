using Binarysharp.MemoryManagement.RemoteProcess.Memory;

namespace Binarysharp.MemoryManagement.Native
{
    /// <summary>
    ///     Defines how the operating system information are collected for 64-bit architecture.
    /// </summary>
    public sealed class NativeDriver64 : NativeDriverBase
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the class <see cref="NativeDriver64" />.
        /// </summary>
        public NativeDriver64()
        {
            // Create the core components
            MemoryCore = new MemoryCore64();
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Memory interaction for 64-bit architecture.
        /// </summary>
        public override IMemoryCore MemoryCore { get; protected set; }
        #endregion
    }
}