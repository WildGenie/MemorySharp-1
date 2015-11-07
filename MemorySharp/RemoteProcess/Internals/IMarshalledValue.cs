using System;
using Binarysharp.MemoryManagement.RemoteProcess.Memory;

namespace Binarysharp.MemoryManagement.RemoteProcess.Internals
{
    /// <summary>
    ///     Interface representing a value within the memory of a remote process.
    /// </summary>
    public interface IMarshalledValue : IDisposable
    {
        #region Public Properties, Indexers
        /// <summary>
        ///     The memory allocated where the value is fully written if needed. It can be unused.
        /// </summary>
        RemoteAllocation Allocated { get; }

        /// <summary>
        ///     The reference of the value. It can be directly the value or a pointer.
        /// </summary>
        IntPtr Reference { get; }
        #endregion
    }
}