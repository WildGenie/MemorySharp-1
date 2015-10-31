using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.Logger;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.MemoryInternal.Threading
{
    /// <summary>
    ///     Class providing tools for manipulating threads.
    /// </summary>
    public class ThreadFactory
    {
        #region  Fields
        /// <summary>
        ///     The reference of the <see cref="MemoryPlus" /> object.
        /// </summary>
        protected readonly MemoryPlus MemoryPlus;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="ThreadFactory" /> class.
        /// </summary>
        /// <param name="memoryPlus">The reference of the <see cref="MemoryManagement.MemoryPlus" /> object.</param>
        public ThreadFactory(MemoryPlus memoryPlus)
        {
            MemoryPlus = memoryPlus;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     Gets the thread corresponding to an id.
        /// </summary>
        /// <param name="threadId">The unique identifier of the thread to get.</param>
        /// <returns>A new instance of a <see cref="ProxyNativeThread" /> class.</returns>
        public ProcessThread this[int threadId] => NativeThreads.FirstOrDefault(thread => thread.Id == threadId);

        /// <summary>
        ///     Gets the main thread of the remote process.
        /// </summary>
        public ProcessThread MainThread
        {
            get
            {
                return
                    NativeThreads
                        .Aggregate((current, next) => next.StartTime < current.StartTime ? next : current);
            }
        }

        /// <summary>
        /// </summary>
        public IReadOnlyList<ProxyNativeThread> ProxyThreads
        {
            get
            {
                List<ProxyNativeThread> nativeThreads = null;
                try
                {
                    nativeThreads =
                        NativeThreads.Select(processThread => new ProxyNativeThread(processThread, MemoryPlus)).ToList();
                    return nativeThreads;
                }
                catch (Exception exception)
                {
                    Log.Error(exception.ToString());
                    if (nativeThreads != null) return nativeThreads.AsReadOnly();
                    throw exception;
                }
            }
        }

        /// <summary>
        ///     Gets the native threads from the remote process.
        /// </summary>
        public IEnumerable<ProcessThread> NativeThreads
        {
            get
            {
                // Refresh the process info
                MemoryPlus.Process.Native.Refresh();
                // Enumerates all threads
                return MemoryPlus.Process.Native.Threads.Cast<ProcessThread>();
            }
        }
        #endregion

        #region Methods
        public void Test(IList<ProxyNativeThread> threadslList)
        {
            try
            {
                foreach (var variable in NativeThreads)
                {
                    threadslList.Add(new ProxyNativeThread(variable, MemoryPlus));
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        /// <summary>
        ///     Releases all resources used by the <see cref="ThreadFactory" /> object.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose... yet
        }

        /// <summary>
        ///     Gets the thread corresponding to an id.
        /// </summary>
        /// <param name="id">The unique identifier of the thread to get.</param>
        /// <returns>A new instance of a <see cref="ProxyNativeThread" /> class.</returns>
        public ProcessThread GetThreadById(int id)
        {
            return NativeThreads.FirstOrDefault(thread => thread.Id == id);
        }

        /// <summary>
        ///     Resumes the given thread.
        /// </summary>
        /// <param name="thread">The thread to resume.</param>
        /// <exception cref="Win32Exception"></exception>
        public void ResumeThread(ProcessThread thread)
        {
            var hThread = IntPtr.Zero;
            try
            {
                hThread = NativeMethods.OpenThread(ThreadAccessFlags.AllAccess, false,
                    thread.Id).DangerousGetHandle();
                if (hThread == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var hr = NativeMethods.ResumeThread(hThread);
                Marshal.ThrowExceptionForHR(hr);
            }
            finally
            {
                if (hThread != IntPtr.Zero)
                    NativeMethods.CloseHandle(hThread);
            }
        }

        /// <summary>
        ///     Resumes all threads.
        /// </summary>
        public void ResumeAll()
        {
            foreach (var processThread in NativeThreads)
            {
                ResumeThread(processThread);
            }
        }

        /// <summary>
        ///     Suspends all threads.
        /// </summary>
        public void SuspendAll()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}