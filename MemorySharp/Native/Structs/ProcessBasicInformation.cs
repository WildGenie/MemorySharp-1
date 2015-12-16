using System;
using Binarysharp.MemoryManagement.Marshaling;

namespace Binarysharp.MemoryManagement.Native.Structs
{
    /// <summary>
    ///     Structure containing basic information about a process.
    /// </summary>
    public struct ProcessBasicInformation
    {
        /// <summary>
        ///     The exit status.
        /// </summary>
        public uint ExitStatus;

        /// <summary>
        ///     The base address of Process Environment Block.
        /// </summary>
        public IntPtr PebBaseAddress;

        /// <summary>
        ///     The affinity mask.
        /// </summary>
        public uint AffinityMask;

        /// <summary>
        ///     The base priority.
        /// </summary>
        public uint BasePriority;

        /// <summary>
        ///     The process id.
        /// </summary>
        public int ProcessId;

        /// <summary>
        ///     The process id of the parent process.
        /// </summary>
        public int InheritedFromUniqueProcessId;

        /// <summary>
        ///     The size of this structure.
        /// </summary>
        public int Size => MarshalType<ProcessBasicInformation>.Size;
    }
}