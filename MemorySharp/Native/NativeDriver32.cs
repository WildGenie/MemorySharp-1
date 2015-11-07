using Binarysharp.MemoryManagement.RemoteProcess.Memory;

namespace Binarysharp.MemoryManagement.Native
{
    /// <summary>
    ///     Defines how the operating system information are collected for 32-bit architecture.
    /// </summary>
    public sealed class NativeDriver32 : NativeDriverBase
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the class <see cref="NativeDriver32" />.
        /// </summary>
        public NativeDriver32()
        {
            // Create the core components
            MemoryCore = new MemoryCore32();
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Memory interaction for 32-bit architecture.
        /// </summary>
        public override IMemoryCore MemoryCore { get; protected set; }
        #endregion
    }
}