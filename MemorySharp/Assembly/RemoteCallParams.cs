using System;
using Binarysharp.MemoryManagement.Assembly.CallingConvention;

namespace Binarysharp.MemoryManagement.Assembly
{
    /// <summary>
    ///     The paramters used for the <see cref="MemorySharp" /> classes
    ///     <code>Call{T}(RemoteCallParams remoteCallParams).(...)</code> methods.
    /// </summary>
    public struct RemoteCallParams
    {
        /// <summary>
        ///     Gets or sets the address where the remote function being called is located in memory.
        /// </summary>
        /// <value>
        ///     The address where the function being called is located in memory.
        /// </value>
        public IntPtr Address { get; set; }

        /// <summary>
        ///     Gets or sets the calling convention type of the function being called.
        /// </summary>
        /// <value>
        ///     The calling convention type of the function being called.
        /// </value>
        public CallingConventions CallingConvention { get; set; }
    }
}