/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.Native.Enums;

namespace Binarysharp.MemoryManagement.Native.Structs
{
    /// <summary>
    ///     Contains the flash status for a window and the number of times the system should flash the window.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FlashInfo
    {
        public int Size;
        public IntPtr Hwnd;
        public FlashWindowFlags Flags;
        public uint Count;
        public int Timeout;
    }
}