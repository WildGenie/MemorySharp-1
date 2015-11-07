using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.Hooks.WndProc
{
    /// <summary>
    ///     A simple hooking class using basic window functions to accomplish the task. All code and ideas related to this
    ///     class credits are to Jadd @ http://www.ownedcore.com/ and a detailed explanation can be found here at his blog
    ///     http://blog.ntoskr.nl/hooking-threads-without-detours-or-patches/.
    /// </summary>
    public class WindowHook : IHook
    {
        #region Fields, Private Properties

        // WM_USER can be defined as anything between 0x0400 and 0x7FFF.
        private int WmUser { get; } = 0x0400;
        private int GwlWndproc { get; } = -4;
        private IntPtr Handle { get; }
        private WindowProc NewCallback { get; set; }
        private IntPtr OldCallback { get; set; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initiates a new Instance of the <see cref="WindowHook" /> class used to apply a hook using <code>WndProc</code>.
        /// </summary>
        /// <param name="name">The name representing the Instance.</param>
        /// <param name="handle">The handle to the window to apply the WndProc hook to.</param>
        /// <param name="mustBeDispose">Whether the hook must be disposed when the Garbage Collector collects the object</param>
        public WindowHook(IntPtr handle, string name, bool mustBeDispose = true)
        {
            Handle = handle;
            Name = name;
            PulseEngine = new WindowHookEngine();
            MustBeDisposed = mustBeDispose;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets the pulse engine instance used to add pulses to be executed in the same thread that the hook is applied to.
        /// </summary>
        /// <value>The engine.</value>
        public WindowHookEngine PulseEngine { get; }

        /// <summary>
        ///     The name of the hook.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     States if the element is enabled.
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the hook is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the hook must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; set; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Disables the hook.
        /// </summary>
        public void Disable()
        {
            if (NewCallback == null)
            {
                IsEnabled = false;
                return;
            }
            // This helper method will work with x32 or x64.
            UnsafeNativeMethods.SetWindowLongPtr(Handle, GwlWndproc, OldCallback);
            NewCallback = null;
            IsEnabled = false;
        }

        /// <summary>
        ///     Enables the hook.
        /// </summary>
        public void Enable()
        {
            if (IsEnabled)
            {
                return;
            }
            // Pins WndProc - will not be garbage collected.
            NewCallback = WndProc;
            // This helper method will work with x32 or x64.
            OldCallback = UnsafeNativeMethods.SetWindowLongPtr(Handle, GwlWndproc,
                Marshal.GetFunctionPointerForDelegate(NewCallback));
            // Just to be sure..
            if (OldCallback == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            IsEnabled = true;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!IsEnabled || !MustBeDisposed) return;
            Disable();
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        ///     Used to invoke <see cref="UserMessage" />'s to the hooked window.
        /// </summary>
        /// <param name="message">The message to invoke.</param>
        public void InvokeUserMessage(UserMessage message)
        {
            UnsafeNativeMethods.SendMessage(Handle, WmUser, (int) message, 0);
        }

        /// <summary>
        ///     The call back for the hook.
        /// </summary>
        /// <param name="hWnd">The handle of the window.</param>
        /// <param name="msg">The message to send, which can be intercepted in the hook.</param>
        /// <param name="wParam">The w parame.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns>System.Int32.</returns>
        private int WndProc(IntPtr hWnd, int msg, int wParam, int lParam)
        {
            if (msg == WmUser && HandleUserMessage((UserMessage) wParam))
            {
                // Already handled, so no need to do anything.
                return 0;
            }

            // Forward the message to the original WndProc function.
            return UnsafeNativeMethods.CallWindowProc(OldCallback, hWnd, msg, wParam, lParam);
        }

        /// <summary>
        ///     Intercepts and handles the UserMessage.
        /// </summary>
        /// <param name="message">The user message.</param>
        /// <returns>
        ///     <code>true/false.</code>
        /// </returns>
        private bool HandleUserMessage(UserMessage message)
        {
            switch (message)
            {
                case UserMessage.StartUp:
                    PulseEngine.StartUp();
                    return true;
                case UserMessage.ShutDown:
                    PulseEngine.ShutDown();
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message), message, null);
            }
        }

        /// <summary>
        ///     Delegate WindowProc
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns>System.Int32.</returns>
        private delegate int WindowProc(IntPtr hWnd, int msg, int wParam, int lParam);
    }
}