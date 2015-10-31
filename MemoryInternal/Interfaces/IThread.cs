using System;
using System.Diagnostics;
using Binarysharp.MemoryManagement.MemoryExternal.Threading;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.MemoryInternal.Interfaces
{
    public interface IThread
    {
        #region  Properties
        /// <summary>
        ///     The remote thread handle opened with all rights.
        /// </summary>
        SafeMemoryHandle Handle { get; }

        /// <summary>
        ///     Gets or sets the full context of the thread.
        ///     If the thread is not already suspended, performs a <see cref="Suspend" /> and <see cref="Resume" /> call on the
        ///     thread.
        /// </summary>
        ThreadContext Context { get; }

        /// <summary>
        ///     Gets the unique identifier of the thread.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///     Gets if the thread is suspended.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///     Gets if the thread is terminated.
        /// </summary>
        bool IsTerminated { get; }

        /// <summary>
        ///     The native <see cref="ProcessThread" /> object corresponding to this thread.
        /// </summary>
        ProcessThread Native { get; }
        #endregion

        #region Methods
        /// <summary>
        ///     Gets if the thread is the main one in the remote process.
        /// </summary>
        bool IsMainThread();

        /// <summary>
        ///     Blocks the calling thread until a thread terminates or the specified time elapses.
        /// </summary>
        /// <param name="time">The timeout.</param>
        /// <returns>The return value is a flag that indicates if the thread terminated or if the time elapsed.</returns>
        WaitValues Join(TimeSpan time);

        /// <summary>
        ///     Resumes a thread that has been suspended.
        /// </summary>
        void Resume();

        /// <summary>
        ///     Terminates the thread.
        /// </summary>
        /// <param name="exitCode">The exit code of the thread to close.</param>
        void Terminate(int exitCode);

        /// <summary>
        ///     Either suspends the thread, or if the thread is already suspended, has no effect.
        /// </summary>
        void Suspend();

        /// <summary>
        ///     Discards any information about this thread that has been cached inside the process component.
        /// </summary>
        void Refresh();

        /// <summary>
        ///     Gets the linear address of a specified segment.
        /// </summary>
        /// <param name="segment">The segment to get.</param>
        /// <returns>A <see cref="IntPtr" /> pointer corresponding to the linear address of the segment.</returns>
        IntPtr GetRealSegmentAddress(SegmentRegisters segment);
        #endregion
    }
}