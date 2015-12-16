using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.Native.Enums;

namespace Binarysharp.MemoryManagement.Native.Structs
{
    /// <summary>
    ///     Returned if <see cref="ThreadContextFlags.FloatingPoint" /> flag is set.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FloatingSaveArea
    {
        public uint ControlWord;
        public uint StatusWord;
        public uint TagWord;
        public uint ErrorOffset;
        public uint ErrorSelector;
        public uint DataOffset;
        public uint DataSelector;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)] public byte[] RegisterArea;
        public uint Cr0NpxState;
    }
}