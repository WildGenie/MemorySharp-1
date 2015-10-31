using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.MemoryExternal.Modules;
using Binarysharp.MemoryManagement.MemoryInternal.Memory;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.MemoryInternal.Modules
{
    /// <summary>
    ///     Class providing tools for manipulating modules and libraries.
    /// </summary>
    public class ModuleFactory : IFactory
    {
        #region  Fields
        /// <summary>
        ///     The list containing all injected modules (writable).
        /// </summary>
        protected readonly Dictionary<string, InjectedModule> InternalInjectedModules;

        /// <summary>
        ///     The reference of the <see cref="MemorySharp" /> object.
        /// </summary>
        protected readonly MemoryPlus MemoryPlus;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="ModuleFactory" /> class.
        /// </summary>
        /// <param name="memoryPlus">The reference of the <see cref="MemoryPlus" /> object.</param>
        internal ModuleFactory(MemoryPlus memoryPlus)
        {
            // Save the parameter
            MemoryPlus = memoryPlus;
            // Create a list containing all injected modules
            InternalInjectedModules = new Dictionary<string, InjectedModule>();
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     A collection containing all injected modules.
        /// </summary>
        public IReadOnlyDictionary<string, InjectedModule> InjectedModules => InternalInjectedModules;

        /// <summary>
        ///     Gets the main module for the remote process.
        /// </summary>
        public RemoteModule MainModule => FetchModule(MemoryPlus.Process.Native.MainModule);

        /// <summary>
        ///     Gets the modules that have been loaded in the remote process.
        /// </summary>
        public IEnumerable<RemoteModule> RemoteModules => NativeModules.Select(FetchModule);

        /// <summary>
        ///     Gets the native modules that have been loaded in the remote process.
        /// </summary>
        public IEnumerable<ProcessModule> NativeModules => MemoryPlus.Process.Native.Modules.Cast<ProcessModule>();

        /// <summary>
        ///     Gets a pointer from the remote process.
        /// </summary>
        /// <param name="address">The address of the pointer.</param>
        /// <returns>A new instance of a <see cref="Memory.RemotePointer" /> class.</returns>
        public RemotePointer this[IntPtr address] => new RemotePointer(MemoryPlus, address);

        /// <summary>
        ///     Gets the specified module in the remote process.
        /// </summary>
        /// <param name="moduleName">The name of module (not case sensitive).</param>
        /// <returns>A new instance of a <see cref="RemoteModule" /> class.</returns>
        public RemoteModule this[string moduleName] => FetchModule(moduleName);
        #endregion

        #region  Interface members
        /// <summary>
        ///     Releases all resources used by the <see cref="ModuleFactory" /> object.
        /// </summary>
        public virtual void Dispose()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Injects a Dll into the process.
        /// </summary>
        /// <param name="modulePath">The full path to the module.</param>
        /// <param name="moduleName">The name of the module.</param>
        /// <returns>A new Instance of the <see cref="InjectedModule" /> class.</returns>
        public InjectedModule InjectLibrary(string modulePath, string moduleName = null)
        {
            if (!File.Exists(modulePath))
            {
                throw new FileNotFoundException("Unable to find the specified module", modulePath);
            }
            modulePath = Path.GetFullPath(modulePath);
            if (string.IsNullOrEmpty(moduleName))
            {
                moduleName = Path.GetFileName(modulePath) ?? modulePath;
            }


            if (MemoryPlus.Modules.InjectedModules.ContainsKey(moduleName))
                throw new ArgumentException("Module with this name has already been injected", nameof(moduleName));

            // unmanaged resources that need to be freed
            var pLibRemote = IntPtr.Zero;
            var hThread = IntPtr.Zero;
            var pLibFullPath = Marshal.StringToHGlobalUni(modulePath);

            try
            {
                var sizeUni = Encoding.Unicode.GetByteCount(modulePath);


                var hLoadLib = ModuleCore.GetProcAddress("kernel32", "LoadLibraryW");
                if (hLoadLib == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // allocate memory to the local process for libFullPath
                pLibRemote = NativeMethods.VirtualAllocEx(MemoryPlus.Process.SafeHandle, IntPtr.Zero, sizeUni,
                    MemoryAllocationFlags.Commit,
                    MemoryProtectionFlags.ReadWrite);
                if (pLibRemote == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                int bytesWritten;
                if (
                    !NativeMethods.WriteProcessMemory(MemoryPlus.Process.Native.Handle, pLibRemote, pLibFullPath,
                        (uint) sizeUni, out bytesWritten) || bytesWritten != sizeUni)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // load dll via call to LoadLibrary using CreateRemoteThread
                hThread = NativeMethods.CreateRemoteThread(MemoryPlus.Process.Native.Handle, IntPtr.Zero, 0, hLoadLib,
                    pLibRemote, 0,
                    IntPtr.Zero);
                if (hThread == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                if (NativeMethods.WaitForSingleObject(hThread, (uint) ThreadWaitValue.Infinite) !=
                    (uint) ThreadWaitValue.Object0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // get address of loaded module - this doesn't work in x64, so just iterate module list to find injected module
                // TODO: fix for x64
                IntPtr hLibModule;
                if (!NativeMethods.GetExitCodeThread(hThread, out hLibModule))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                if (hLibModule == IntPtr.Zero)
                    throw new Exception("Code executed properly but unable to get get module base address");

                var module = new InjectedModule(MemoryPlus, hLibModule, moduleName);
                InternalInjectedModules[moduleName] = module;
                return new InjectedModule(MemoryPlus, hLibModule, moduleName);
            }
            finally
            {
                Marshal.FreeHGlobal(pLibFullPath);
                if (hThread != IntPtr.Zero)
                    NativeMethods.CloseHandle(hThread);
                if (pLibRemote != IntPtr.Zero)
                    NativeMethods.VirtualFreeEx(MemoryPlus.Process.SafeHandle, pLibRemote, 0,
                        MemoryReleaseFlags.Release);
            }
        }

        /// <summary>
        ///     Fetches a module from the remote process.
        /// </summary>
        /// <param name="moduleName">
        ///     A module name (not case sensitive). If the file name extension is omitted, the default library
        ///     extension .dll is appended.
        /// </param>
        /// <returns>A new instance of a <see cref="RemoteModule" /> class.</returns>
        protected RemoteModule FetchModule(string moduleName)
        {
            // Convert module name with lower chars
            moduleName = moduleName.ToLower();
            // Check if the module name has an extension
            if (!Path.HasExtension(moduleName))
                moduleName += ".dll";

            // Fetch and return the module
            return new RemoteModule(MemoryPlus, NativeModules.First(m => m.ModuleName.ToLower() == moduleName));
        }

        /// <summary>
        ///     Fetches a module from the remote process.
        /// </summary>
        /// <param name="module">A module in the remote process.</param>
        /// <returns>A new instance of a <see cref="RemoteModule" /> class.</returns>
        private RemoteModule FetchModule(ProcessModule module)
        {
            return FetchModule(module.ModuleName);
        }
        #endregion

        #region Misc
        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~ModuleFactory()
        {
            Dispose();
        }
        #endregion
    }
}