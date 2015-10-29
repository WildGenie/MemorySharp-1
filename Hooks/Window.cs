using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using MemorySharp.Internals.Interfaces;
using MemorySharp.Native;
using MemorySharp.Tools.Logger;

namespace MemorySharp.Hooks
{
    internal enum UserMessage
    {
        StartUp,
        ShutDown
    }

    /// <summary>
    ///     A simple hooking class using basic window functions to accomplish the task. All code and ideas related to this
    ///     class credits are to Jadd @ http://www.ownedcore.com/ and a detailed explanation can be found here at his blog
    ///     http://blog.ntoskr.nl/hooking-threads-without-detours-or-patches/.
    /// </summary>
    public class Window
    {
        #region Events
        public static event EventHandler OnFirstFrame;
        public static event EventHandler OnLastFrame = (sender, e) => _frameQueueFinalized.Set();
        #endregion

        #region  Fields
        private readonly IntPtr _handle;
        private WindowProc _newCallback;
        private IntPtr _oldCallback;
        private const int GwlWndproc = -4;
        // WM_USER can be defined as anything between 0x0400 and 0x7FFF.
        private const int WmUser = 0x0400;
        private static ManualResetEventSlim _frameQueueFinalized;
        private static readonly LinkedList<IPulsable> _pulsables = new LinkedList<IPulsable>();
        #endregion

        #region Constructors
        /// <summary>
        ///     Initiates a new Instance of the <see cref="Window" /> class.
        ///     <param name="handle">The <see cref="IntPtr" /> address of the processes main window handle.</param>
        /// </summary>
        public Window(IntPtr handle)
        {
            _handle = handle;
            _frameQueueFinalized = new ManualResetEventSlim(false);
        }
        #endregion

        #region  Properties
        public static int FrameCount { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        ///     Applys the new call back using <see cref="NativeMethods.SetWindowLong" />.
        /// </summary>
        public void Attach()
        {
            _newCallback = WndProc; // Pins WndProc - will not be garbage collected.
            // This helper method will work with x32 or x64.
            _oldCallback = NativeMethods.SetWindowLongPtr(_handle, GwlWndproc,
                Marshal.GetFunctionPointerForDelegate(_newCallback));
            // Just to be sure...
            if (_oldCallback == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        ///     Deattachs from the window by removing the new call back.
        /// </summary>
        public void Detach()
        {
            if (_newCallback == null)
                return;
            // This helper method will work with x32 or x64.
            NativeMethods.SetWindowLongPtr(_handle, GwlWndproc, _oldCallback);
            _newCallback = null;
        }

        private int WndProc(IntPtr hWnd, int msg, int wParam, int lParam)
        {
            if (msg == WmUser && HandleUserMessage((UserMessage) wParam))
                return 0; // Already handled, woohoo!

            // Forward the message to the original WndProc function.
            return NativeMethods.CallWindowProc(_oldCallback, hWnd, msg, wParam, lParam);
        }

        /// <summary>
        ///     Used to invoke <see cref="UserMessage" />'s to the hooked window.
        /// </summary>
        /// <param name="message">The message to invoke.</param>
        internal void Invoke(UserMessage message)
        {
            NativeMethods.SendMessage(_handle, WmUser, (int) message, 0);
        }

        private bool HandleUserMessage(UserMessage message)
        {
            switch (message)
            {
                case UserMessage.StartUp:
                    try
                    {
                        if (FrameCount == -1)
                        {
                            Log.Normal("[D] OnLastFrame");
                            OnLastFrame?.Invoke(null, new EventArgs());
                        }
                        else
                        {
                            if (FrameCount == 0)
                            {
                                OnFirstFrame?.Invoke(null, new EventArgs());
                            }


                            foreach (var pulsable in _pulsables)
                                pulsable.Pulse();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                    }

                    if (FrameCount != -1)
                        FrameCount += 1;

                    return true;

                case UserMessage.ShutDown:
                    _pulsables.Clear();
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(message), message, null);
            }
        }

        public static void RegisterCallback(IPulsable pulsable)
        {
            _pulsables.AddLast(pulsable);
        }

        public static void RegisterCallbacks(params IPulsable[] pulsables)
        {
            foreach (var pulsable in pulsables)
                RegisterCallback(pulsable);
        }

        public static void RemoveCallback(IPulsable pulsable)
        {
            if (_pulsables.Contains(pulsable))
                _pulsables.Remove(pulsable);
        }
        #endregion

        #region Nested
        private delegate int WindowProc(IntPtr hWnd, int msg, int wParam, int lParam);
        #endregion
    }
}