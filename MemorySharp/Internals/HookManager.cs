using System;
using System.Linq;
using ToolsSharp.Hooks.Input;
using ToolsSharp.Hooks.WndProc;
using ToolsSharp.Managment;
using ToolsSharp.Managment.Interfaces;
using DebugLog = Binarysharp.MemoryManagement.Logging.Default.DebugLog;

namespace Binarysharp.MemoryManagement.Internals
{
    /// <summary>
    ///     Class to manage hooks that implement the <see cref="INamedElement" /> Interface.
    /// </summary>
    public class HookManager : SafeManager<INamedElement>, IDisposable
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="HookManager" /> class.
        /// </summary>
        /// <param name="processMemory">The process memory.</param>
        public HookManager(MemoryPlus processMemory) : base(new DebugLog())
        {
            ProcessMemory = processMemory;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     The reference of the <see cref="ProcessMemory" /> object.
        /// </summary>
        protected MemoryPlus ProcessMemory { get; }

        /// <summary>
        ///     Gets a hook instance from the given name from the managers dictonary of current hooks.
        /// </summary>
        /// <param name="name">The name of the hook.</param>
        public INamedElement this[string name] => InternalItems[name];
        #endregion

        #region Interface Implementations
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

        /// <summary>
        ///     Creates an instance of a low level mouse hook. This instance is also added to the manager and returned as the
        ///     result.
        /// </summary>
        /// <param name="name">The name of the low level mouse hook.</param>
        /// <param name="mustBeDisposed">
        ///     if set to <c>true</c> the hook must be disposed], else it will not be disposed on garbage
        ///     collection.
        /// </param>
        /// <returns>MouseHook.</returns>
        public MouseHook CreateKMouseHook(string name, bool mustBeDisposed = true)
        {
            InternalItems[name] = new MouseHook(name, mustBeDisposed);
            return (MouseHook) InternalItems[name];
        }

        /// <summary>
        ///     Creates an instance of a low level keyboard hook. This instance is also added to the manager and returned as the
        ///     result.
        /// </summary>
        /// <param name="name">The name of the low level keyboard hook.</param>
        /// <param name="mustBeDisposed">
        ///     if set to <c>true</c> the hook must be disposed], else it will not be disposed on garbage
        ///     collection.
        /// </param>
        /// <returns>MouseHook.</returns>
        public KeyboardHook CreateKeyboardHook(string name, bool mustBeDisposed = true)
        {
            InternalItems[name] = new KeyboardHook(name, mustBeDisposed);
            return (KeyboardHook) InternalItems[name];
        }
    }
}