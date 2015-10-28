using System;

namespace MemorySharp.Extensions
{
    /// <summary>
    ///     A static class providing extension methods for <see cref="IntPtr" />'s.
    /// </summary>
    public static class IntPtrExtensions
    {
        /// <summary>
        ///     Adds an offset to a pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="offset">The offset to add.</param>
        /// <returns>The new <see cref="IntPtr" /></returns>
        public static IntPtr Add(this IntPtr pointer, int offset)
        {
            return IntPtr.Add(pointer, offset);
        }

        /// <summary>
        ///     Adds an offset to a pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="offset">The offset to add.</param>
        /// <returns>The new <see cref="IntPtr" /></returns>
        public static IntPtr Add(this IntPtr pointer, uint offset)
        {
            return IntPtr.Add(pointer, (int) offset);
        }

        /// <summary>
        ///     Adds an offset to a pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="pointer2">The offset to add.</param>
        /// <returns>The new <see cref="IntPtr" /></returns>
        public static IntPtr Add(this IntPtr pointer, IntPtr pointer2)
        {
            return IntPtr.Add(pointer, pointer2.ToInt32());
        }

        /// <summary>
        ///     Subtracts an offset from a pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="offset">The offset value to subtract.</param>
        /// <returns>The new <see cref="IntPtr" /> value.</returns>
        public static IntPtr Subtract(this IntPtr pointer, int offset)
        {
            return IntPtr.Subtract(pointer, offset);
        }

        /// <summary>
        ///     Subtracts an offset from a pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="pointer2">The offset value to subtract.</param>
        /// <returns>The new <see cref="IntPtr" /> value.</returns>
        public static IntPtr Subtract(this IntPtr pointer, IntPtr pointer2)
        {
            return IntPtr.Subtract(pointer, pointer2.ToInt32());
        }
    }
}