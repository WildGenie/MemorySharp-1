using System;
using System.Runtime.InteropServices;

namespace MemorySharp.Extensions
{
    public static class DelegateExtensions
    {
        public static IntPtr ToFunctionPointer(this Delegate del)
        {
            return Marshal.GetFunctionPointerForDelegate(del);
        }
    }
}