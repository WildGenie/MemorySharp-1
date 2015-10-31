using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;
using Binarysharp.MemoryManagement.MemoryInternal.Modules;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.MemoryInternal.Processes
{
    /// <summary>
    ///     <see cref="IProcess" /> default implementation.
    /// </summary>
    public class LocalProcess : IProcess
    {
        #region Constructors
        /// <summary>
        /// </summary>
        /// <param name="process"></param>
        public LocalProcess(Process process)
        {
            Native = process;
            SafeHandle = new SafeMemoryHandle(Native.Handle);
            ImageBase = Native.MainModule.BaseAddress;
            Handle = Native.Handle;
            MainModule = Native.MainModule;
            Functions = new Dictionary<string, ProcessFunction>();
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The native process.
        /// </summary>
        public Process Native { get; }

        /// <summary>
        ///     The <see cref="SafeMemoryHandle" /> for the process.
        /// </summary>
        public SafeMemoryHandle SafeHandle { get; }

        /// <summary>
        ///     The address of the local processes main module.
        /// </summary>
        public IntPtr ImageBase { get; }

        /// <summary>
        ///     A dictonary collection of <see cref="ProcessFunction" /> Instances.
        /// </summary>
        public Dictionary<string, ProcessFunction> Functions { get; }

        /// <summary>
        ///     The handle to the process.
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        ///     The <see cref="RemoteModule" /> Instance of the processes main module.
        /// </summary>
        public ProcessModule MainModule { get; }
        #endregion

        #region  Interface members
        /// <summary>
        /// </summary>
        public void Dispose()
        {
            Native.Dispose();
            SafeHandle.Dispose();
            // Avoid the finalizer
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Resumes the current proccesss
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        public void Resume()
        {
            var hThread = IntPtr.Zero;
            try
            {
                hThread = NativeMethods.OpenThread(ThreadAccessFlags.AllAccess, false,
                    Native.Threads[0].Id).DangerousGetHandle();
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
        #endregion
    }
}