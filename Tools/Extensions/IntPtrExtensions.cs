using System;

namespace MemorySharp.Tools.Extensions
{
    public static class IntPtrExtensions
    {
        #region Methods
        public static IntPtr Add(this IntPtr pointer, int offset)
        {
            return IntPtr.Add(pointer, offset);
        }

        public static IntPtr Add(this IntPtr pointer, uint offset)
        {
            return IntPtr.Add(pointer, (int) offset);
        }

        public static IntPtr Add(this IntPtr pointer, IntPtr pointer2)
        {
            return IntPtr.Add(pointer, pointer2.ToInt32());
        }

        public static IntPtr Subtract(this IntPtr pointer, int offset)
        {
            return IntPtr.Subtract(pointer, offset);
        }

        public static IntPtr Subtract(this IntPtr pointer, IntPtr pointer2)
        {
            return IntPtr.Subtract(pointer, pointer2.ToInt32());
        }
        #endregion
    }
}