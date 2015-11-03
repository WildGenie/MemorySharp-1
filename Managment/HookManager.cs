using System;
using System.Linq;
using Binarysharp.MemoryManagement.Hooks;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.Memory;

namespace Binarysharp.MemoryManagement.Managment
{
    /// <summary>
    ///     Class to manage hooks that implement the <see cref="IHook" /> Interface.
    /// </summary>
    public class HookManager : Manager<IHook>, IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HookManager" /> class.
        /// </summary>
        /// <param name="processMemory">The process memory.</param>
        public HookManager(ProcessMemory processMemory)
        {
            ProcessMemory = processMemory;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     The reference of the <see cref="ProcessMemory" /> object.
        /// </summary>
        private ProcessMemory ProcessMemory { get; }

        #endregion

        #region  Interface members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var hookValue in InternalItems.Values.Where(hook => hook.IsEnabled))
            {
                hookValue.Disable();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the WND proc hook.
        /// </summary>
        /// <param name="name">The name of the hook Instance.</param>
        /// <param name="windowHandle">The window handle.</param>
        /// <param name="windowEngine">The window engine.</param>
        /// <returns>WindowHook.</returns>
        public WindowHook CreateWndProcHook(string name, IntPtr windowHandle, IWindowEngine windowEngine)
        {
            var wndProcHook = new WindowHook(name, windowHandle, windowEngine);
            InternalItems.Add(name, wndProcHook);
            return wndProcHook;
        }

        #endregion
    }
}