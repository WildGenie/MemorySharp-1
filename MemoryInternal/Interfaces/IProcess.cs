using System;
using System.Collections.Generic;
using System.Diagnostics;
using Binarysharp.MemoryManagement.MemoryInternal.Processes;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.MemoryInternal.Interfaces
{
    /// <summary>
    ///     Defines a set of operations/values for a <see cref="Process" /> Instance.
    /// </summary>
    public interface IProcess : IDisposable
    {
        #region  Properties
        /// <summary>
        ///     The native process.
        /// </summary>
        Process Native { get; }

        /// <summary>
        ///     The <see cref="SafeMemoryHandle" /> for the process.
        /// </summary>
        SafeMemoryHandle SafeHandle { get; }

        /// <summary>
        ///     The address of the local processes main module.
        /// </summary>
        IntPtr ImageBase { get; }

        /// <summary>
        ///     A dictonary collection of <see cref="ProcessFunction" /> Instances.
        /// </summary>
        Dictionary<string, ProcessFunction> Functions { get; }

        /// <summary>
        ///     The handle to the process.
        /// </summary>
        IntPtr Handle { get; }

        /// <summary>
        ///     The main module of the processes.
        /// </summary>
        ProcessModule MainModule { get; }
        #endregion
    }
}