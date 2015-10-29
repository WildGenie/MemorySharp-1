using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MemorySharp.Hooks
{
    public static class MouseHook
    {
        #region Events
        public static event EventHandler MouseAction = delegate { };
        #endregion

        #region  Fields
        private const int WhMouseLl = 14;
        private static readonly LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookId = IntPtr.Zero;
        #endregion

        #region Methods
        public static void Start()
        {
            _hookId = SetHook(_proc);
        }

        public static void Stop()
        {
            UnhookWindowsHookEx(_hookId);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WhMouseLl, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0 || MouseMessages.WmLbuttondown != (MouseMessages) wParam)
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            var hookStruct = (Msllhookstruct) Marshal.PtrToStructure(lParam, typeof (Msllhookstruct));
            MouseAction(null, new EventArgs());
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public static Point GetCursorPosition()
        {
            Point lpPoint;
            GetCursorPos(out lpPoint);
            //bool success = User32.GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);
        #endregion

        #region Nested
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private enum MouseMessages
        {
            WmLbuttondown = 0x0201,
            WmLbuttonup = 0x0202,
            WmMousemove = 0x0200,
            WmMousewheel = 0x020A,
            WmRbuttondown = 0x0204,
            WmRbuttonup = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Msllhookstruct
        {
            private readonly Point pt;
            private readonly uint mouseData;
            private readonly uint flags;
            private readonly uint time;
            private readonly IntPtr dwExtraInfo;
        }
        #endregion
    }
}