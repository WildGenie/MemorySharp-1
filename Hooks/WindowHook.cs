using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.Hooks
{
    /// <summary>
    ///     A simple hooking class using basic window functions to accomplish the task. All code and ideas related to this
    ///     class credits are to Jadd @ http://www.ownedcore.com/ and a detailed explanation can be found here at his blog
    ///     http://blog.ntoskr.nl/hooking-threads-without-detours-or-patches/.
    /// </summary>
    public class WindowHook : IHook
    {
        #region Constructors

        /// <summary>
        ///     Initiates a new Instance of the <see cref="WindowHook" /> class used to apply a hook using <code>WndProc</code>.
        /// </summary>
        /// <param name="name">The name representing the Instance.</param>
        /// <param name="handle">The handle to the window to hook.</param>
        /// <param name="engine">
        ///     The <see cref="IWindowEngine" /> Instance to use for the methods called when the corresponding
        ///     <see cref="UserMessage" /> is invoked.
        /// </param>
        public WindowHook(string name, IntPtr handle, IWindowEngine engine)
        {
            Handle = handle;
            Engine = engine;
            Name = name;
        }

        #endregion

        #region Nested

        private delegate int WindowProc(IntPtr hWnd, int msg, int wParam, int lParam);

        #endregion

        #region  Fields

        private const int GwlWndproc = -4;
        // WM_USER can be defined as anything between 0x0400 and 0x7FFF.
        private const int WmUser = 0x0400;

        #endregion

        #region  Properties

        private IntPtr Handle { get; }
        private WindowProc NewCallback { get; set; }
        private IntPtr OldCallback { get; set; }
        public bool IsEnabled { get; private set; }
        public string Name { get; }

        /// <summary>
        ///     The <see cref="IWindowEngine" /> member to use.
        ///     <remarks>
        ///         The <code>StartUp()/ShutDown()</code> are called when the window hook is attached and the proper
        ///         <see cref="UserMessage" /> is invoked.
        ///     </remarks>
        /// </summary>
        private IWindowEngine Engine { get; }

        bool IApplicableElement.IsEnabled
        {
            get { return IsEnabled; }
            set { IsEnabled = value; }
        }

        /// <summary>
        ///     Gets a value indicating whether the element is disposed.
        /// </summary>
        public bool IsDisposed { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; set; } = true;

        #endregion

        #region  Interface members

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

        public void Enable()
        {
            if (IsEnabled)
            {
                return;
            }
            NewCallback = WndProc; // Pins WndProc - will not be garbage collected.
            // This helper method will work with x32 or x64.
            OldCallback = UnsafeNativeMethods.SetWindowLongPtr(Handle, GwlWndproc,
                Marshal.GetFunctionPointerForDelegate(NewCallback));
            // Just to be sure...
            if (OldCallback == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            IsEnabled = true;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (IsEnabled && MustBeDisposed)
            {
                Disable();
                IsDisposed = true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Used to invoke <see cref="UserMessage" />'s to the hooked window.
        /// </summary>
        /// <param name="message">The message to invoke.</param>
        public void InvokeUserMessage(UserMessage message)
        {
            UnsafeNativeMethods.SendMessage(Handle, WmUser, (int) message, 0);
        }

        // Core methods below.
        private int WndProc(IntPtr hWnd, int msg, int wParam, int lParam)
        {
            if (msg == WmUser && HandleUserMessage((UserMessage) wParam))
                return 0; // Already handled, woohoo!

            // Forward the message to the original WndProc function.
            return UnsafeNativeMethods.CallWindowProc(OldCallback, hWnd, msg, wParam, lParam);
        }

        /// <summary>
        ///     Intercepts and handles the user message.
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
                    Engine.StartUp();
                    return true;
                case UserMessage.ShutDown:
                    Engine.ShutDown();
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(message), message, null);
            }
        }

        #endregion
    }
}