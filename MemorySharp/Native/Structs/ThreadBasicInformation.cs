using System;
using System.Runtime.InteropServices;

namespace Binarysharp.MemoryManagement.Native.Structs
{
    /// <summary>
    ///     Structure containing basic information about a thread.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ThreadBasicInformation
    {
        /// <summary>
        ///     the exit status.
        /// </summary>
        public uint ExitStatus;

        /// <summary>
        ///     The base address of Thread Environment Block.
        /// </summary>
        public IntPtr TebBaseAdress;

        /// <summary>
        ///     The process id which owns the thread.
        /// </summary>
        public int ProcessId;

        /// <summary>
        ///     The thread id.
        /// </summary>
        public int ThreadId;

        /// <summary>
        ///     The affinity mask.
        /// </summary>
        public uint AffinityMask;

        /// <summary>
        ///     The priority.
        /// </summary>
        public uint Priority;

        /// <summary>
        ///     The base priority.
        /// </summary>
        public uint BasePriority;
    }
}