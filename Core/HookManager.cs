using System;
using System.Linq;
using Binarysharp.MemoryManagement.Core.Shared;
using Binarysharp.MemoryManagement.LocalProcess.Hooks;
using Binarysharp.MemoryManagement.LocalProcess.Hooks.WndProc;
using Binarysharp.MemoryManagement.Managment;
using Binarysharp.MemoryManagement.Managment.Logging.Core;

namespace Binarysharp.MemoryManagement.Core
{
    /// <summary>
    ///     Class to manage hooks that implement the <see cref="IHook" /> Interface.
    /// </summary>
    public class HookManager : SafeManager<IHook>, IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HookManager" /> class.
        /// </summary>
        /// <param name="processMemory">The process memory.</param>
        public HookManager(ProcessMemory processMemory) : base(new DebugLog())
        {
            ProcessMemory = processMemory;
        }

        /// <summary>
        ///     The reference of the <see cref="ProcessMemory" /> object.
        /// </summary>
        protected ProcessMemory ProcessMemory { get; }

        #region IDisposable Members

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

        /// <summary>
        ///     Creates the WND proc hook.
        /// </summary>
        /// <param name="name">The name representingthe hook Instance.</param>
        /// <param name="windowHandle">The window handle.</param>
        /// <returns>WindowHook.</returns>
        public WindowHook CreateWndProcHook(IntPtr windowHandle, string name)
        {
            InternalItems[name] = new WindowHook(windowHandle, name);
            return (WindowHook) InternalItems[name];
        }
    }
}