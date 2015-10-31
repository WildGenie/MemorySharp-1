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
    public class WindowHook
    {
        #region  Fields
        private const int GwlWndproc = -4;
        // WM_USER can be defined as anything between 0x0400 and 0x7FFF.
        private const int WmUser = 0x0400;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initiates a new Instance of the <see cref="WindowHook" /> class used to apply a hook using <code>WndProc</code>.
        /// </summary>
        /// <param name="handle">The handle to the window to hook.</param>
        /// <param name="engine"></param>
        public WindowHook(IntPtr handle, IWindowEngine engine)
        {
            Handle = handle;
            Engine = engine;
        }
        #endregion

        #region  Properties
        private IntPtr Handle { get; }
        private WindowProc NewCallback { get; set; }
        private IntPtr OldCallback { get; set; }

        /// <summary>
        ///     The <see cref="IWindowEngine" /> member to use.
        ///     <remarks>
        ///         The <code>StartUp()/ShutDown()</code> are called when the window hook is attached and the proper
        ///         <see cref="UserMessage" /> is invoked.
        ///     </remarks>
        /// </summary>
        public IWindowEngine Engine { get; }
        #endregion

        #region Methods
        /// <summary>
        ///     Applys the new call back using
        ///     <see>
        ///         <cref>NativeMethods.SetWindowLongPtr</cref>
        ///     </see>
        ///     <remarks>This value is valid for x64 and x32.</remarks>
        /// </summary>
        public void Attach()
        {
            NewCallback = WndProc; // Pins WndProc - will not be garbage collected.
            // This helper method will work with x32 or x64.
            OldCallback = NativeMethods.SetWindowLongPtr(Handle, GwlWndproc,
                Marshal.GetFunctionPointerForDelegate(NewCallback));
            // Just to be sure...
            if (OldCallback == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        ///     Deattachs from the window by removing the new call back.
        /// </summary>
        public void Detach()
        {
            if (NewCallback == null)
                return;
            // This helper method will work with x32 or x64.
            NativeMethods.SetWindowLongPtr(Handle, GwlWndproc, OldCallback);
            NewCallback = null;
        }

        /// <summary>
        ///     Used to invoke <see cref="UserMessage" />'s to the hooked window.
        /// </summary>
        /// <param name="message">The message to invoke.</param>
        public void InvokeUserMessage(UserMessage message)
        {
            NativeMethods.SendMessage(Handle, WmUser, (int) message, 0);
        }

        // Core methods below.
        private int WndProc(IntPtr hWnd, int msg, int wParam, int lParam)
        {
            if (msg == WmUser && HandleUserMessage((UserMessage) wParam))
                return 0; // Already handled, woohoo!

            // Forward the message to the original WndProc function.
            return NativeMethods.CallWindowProc(OldCallback, hWnd, msg, wParam, lParam);
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

        #region Nested
        private delegate int WindowProc(IntPtr hWnd, int msg, int wParam, int lParam);
        #endregion
    }
}