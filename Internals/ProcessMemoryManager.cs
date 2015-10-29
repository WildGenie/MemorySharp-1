using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MemorySharp.Modules;
using MemorySharp.Native;

namespace MemorySharp.Internals
{
    public class ProcessMemoryManager : IDisposable
    {
        #region  Fields
        private readonly Process _process;
        private IntPtr _handle;

        private const ProcessAccessFlags DefaultAccessFlags =
            ProcessAccessFlags.QueryInformation | ProcessAccessFlags.CreateThread |
            ProcessAccessFlags.VmOperation | ProcessAccessFlags.VmWrite |
            ProcessAccessFlags.VmRead;

        private static readonly Dictionary<int, ProcessMemoryManager> _processManagers =
            new Dictionary<int, ProcessMemoryManager>();
        #endregion

        #region Constructors
        private ProcessMemoryManager(Process process)
        {
            _process = process;
        }
        #endregion

        #region  Properties
        public IntPtr Handle
        {
            get
            {
                if (_handle != IntPtr.Zero) return _handle;
                _handle = NativeMethods.OpenProcess(DefaultAccessFlags, false, _process.Id).DangerousGetHandle();
                if (_handle == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                return _handle;
            }
        }

        public int MainThreadId { get; set; }

        public Dictionary<string, AlternateInjectedModule> InjectedModules { get; } =
            new Dictionary<string, AlternateInjectedModule>();
        #endregion

        #region  Interface members
        public void Dispose()
        {
            CloseHandle();
        }
        #endregion

        #region Methods
        private void CloseHandle()
        {
            if (_handle == IntPtr.Zero) return;
            NativeMethods.CloseHandle(_handle);
            _handle = IntPtr.Zero;
        }

        internal static ProcessMemoryManager ForProcess(Process process)
        {
            if (!_processManagers.ContainsKey(process.Id))
            {
                _processManagers[process.Id] = new ProcessMemoryManager(process);
            }
            return _processManagers[process.Id];
        }
        #endregion

        #region Misc
        ~ProcessMemoryManager()
        {
            Dispose();
        }
        #endregion
    }
}