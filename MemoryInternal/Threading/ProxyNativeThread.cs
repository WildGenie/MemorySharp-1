using System;
using System.Diagnostics;
using System.Linq;
using Binarysharp.MemoryManagement.MemoryExternal.Threading;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.MemoryInternal.Threading
{
    /// <summary>
    ///     Class repesenting a thread in the local process.
    /// </summary>
    public class ProxyNativeThread : IDisposable, IEquatable<ProxyNativeThread>
    {
        #region  Fields
        /// <summary>
        ///     The reference of the <see cref="MemoryManagement.MemoryPlus" /> object.
        /// </summary>
        protected readonly MemoryPlus MemoryPlus;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProxyNativeThread" /> class.
        /// </summary>
        /// <param name="thread">The native <see cref="ProcessThread" /> object.</param>
        /// <param name="memoryPlus">The <see cref="MemoryManagement.MemoryPlus" /> object.</param>
        public ProxyNativeThread(ProcessThread thread, MemoryPlus memoryPlus)
        {
            MemoryPlus = memoryPlus;
            Native = thread;
            Handle = ThreadCore.OpenThread(ThreadAccessFlags.AllAccess, Native.Id);
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The remote thread handle opened with all rights.
        /// </summary>
        public SafeMemoryHandle Handle { get; }

        /// <summary>
        ///     Gets or sets the full context of the thread.
        ///     If the thread is not already suspended, performs a <see cref="IThread.Suspend" /> and <see cref="IThread.Resume" />
        ///     call on the
        ///     thread.
        /// </summary>
        public ThreadContext Context
            => ThreadCore.GetThreadContext(Handle, ThreadContextFlags.All | ThreadContextFlags.FloatingPoint |
                                                   ThreadContextFlags.DebugRegisters |
                                                   ThreadContextFlags.ExtendedRegisters);

        /// <summary>
        ///     Gets the unique identifier of the thread.
        /// </summary>
        public int Id => Native.Id;

        /// <summary>
        ///     Gets if the thread is suspended.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                // Refresh the thread info
                Refresh();
                // Return if the thread is suspended
                return Native != null && Native.ThreadState == ThreadState.Wait &&
                       Native.WaitReason == ThreadWaitReason.Suspended;
            }
        }

        /// <summary>
        ///     Gets if the thread is alive.
        /// </summary>
        public bool IsAlive => !IsTerminated;

        /// <summary>
        ///     Gets if the thread is terminated.
        /// </summary>
        public bool IsTerminated
        {
            get
            {
                // Refresh the thread info
                Refresh();
                // Check if the thread is terminated
                return Native == null;
            }
        }

        /// <summary>
        ///     The native <see cref="ProcessThread" /> object corresponding to this thread.
        /// </summary>
        public ProcessThread Native { get; set; }

        /// <summary>
        ///     Gets if the thread is the main one in the remote process.
        /// </summary>
        public bool IsMainThread => Id == MemoryPlus.Threads.MainThread.Id;
        #endregion

        #region  Interface members
        /// <summary>
        ///     Releases all resources used by the <see cref="RemoteThread" /> object.
        /// </summary>
        public virtual void Dispose()
        {
            // Close the thread handle
            Handle.Close();
            // Avoid the finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(ProxyNativeThread other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || (Id == other.Id && MemoryPlus.Equals(other.MemoryPlus));
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Blocks the calling thread until a thread terminates or the specified time elapses.
        /// </summary>
        /// <param name="time">The timeout.</param>
        /// <returns>The return value is a flag that indicates if the thread terminated or if the time elapsed.</returns>
        public WaitValues Join(TimeSpan time)
        {
            return ThreadCore.WaitForSingleObject(Handle, time);
        }

        /// <summary>
        ///     Discards any information about this thread that has been cached inside the process component.
        /// </summary>
        public void Refresh()
        {
            if (Native == null)
                return;
            // Refresh the process info
            Process.GetCurrentProcess().Refresh();
            // Get new info about the thread
            Native =
                Process.GetCurrentProcess()
                    .Threads.Cast<ProcessThread>()
                    .First(thread => thread.Id == Id);
        }

        /// <summary>
        ///     Resumes a thread that has been suspended.
        /// </summary>
        public void Resume()
        {
            // Check if the thread is still alive
            if (!IsAlive) return;

            // Start the thread
            ThreadCore.ResumeThread(Handle);
        }

        /// <summary>
        ///     Either suspends the thread, or if the thread is already suspended, has no effect.
        /// </summary>
        public void Suspend()
        {
            if (IsAlive)
            {
                ThreadCore.SuspendThread(Handle);
            }
        }

        /// <summary>
        ///     Terminates the thread.
        /// </summary>
        /// <param name="exitCode">The exit code of the thread to close.</param>
        public void Terminate(int exitCode = 0)
        {
            if (IsAlive)
                ThreadCore.TerminateThread(Handle, exitCode);
        }

        /// <summary>
        ///     Gets the linear address of a specified segment.
        /// </summary>
        /// <param name="segment">The segment to get.</param>
        /// <returns>A <see cref="IntPtr" /> pointer corresponding to the linear address of the segment.</returns>
        public IntPtr GetRealSegmentAddress(SegmentRegisters segment)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(ProxyNativeThread left, ProxyNativeThread right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProxyNativeThread left, ProxyNativeThread right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ProxyNativeThread) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ MemoryPlus.GetHashCode();
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"Id = {Id} IsAlive = {IsAlive} IsMainThread = {IsMainThread}";
        }
        #endregion
    }
}