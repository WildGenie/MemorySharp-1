using System;

namespace Binarysharp.MemoryManagement.Common.Extensions
{
    public static class Memory<T>
    {
        #region Fields, Private Properties
        /// <summary>
        ///     Gets a
        /// </summary>
        public static readonly Func<IntPtr, T> Get = ptr => MemorySharp.Read<T>(ptr);
        #endregion

        #region Public Properties, Indexers
        public static MemorySharp MemorySharp { get; set; }
        #endregion
    }

    /// <summary>
    ///     A class containing extension methods releated to <see cref="IntPtr" /> values.
    ///     <remarks>
    ///         This classes Read/Write methods are intended to be used for local reading and writing memory, aka
    ///         "injected" or "internal".
    ///     </remarks>
    /// </summary>
    public static class IntPtrExtensions
    {
        #region Fields, Private Properties
        /// <summary>
        /// </summary>
        public static Func<MemorySharp, IntPtr, IntPtr> Get = (sharp, ptr) => sharp.Read<IntPtr>(ptr);
        #endregion

        #region Public Methods
        /// <summary>
        ///     Adds the specified pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>System.IntPtr.</returns>
        public static IntPtr Add(this IntPtr pointer, int offset) => IntPtr.Add(pointer, offset);

        /// <summary>
        ///     Adds the specified pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>System.IntPtr.</returns>
        public static IntPtr Add(this IntPtr pointer, uint offset) => IntPtr.Add(pointer, (int) offset);

        /// <summary>
        ///     Adds the specified pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="pointer2">The pointer2.</param>
        /// <returns>System.IntPtr.</returns>
        public static IntPtr Add(this IntPtr pointer, IntPtr pointer2) => IntPtr.Add(pointer, pointer2.ToInt32());

        /// <summary>
        ///     Subtracts the specified pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>System.IntPtr.</returns>
        public static IntPtr Subtract(this IntPtr pointer, int offset) => IntPtr.Subtract(pointer, offset);

        /// <summary>
        ///     Subtracts the specified pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="pointer2">The pointer2.</param>
        /// <returns>System.IntPtr.</returns>
        public static IntPtr Subtract(this IntPtr pointer, IntPtr pointer2)
            => IntPtr.Subtract(pointer, pointer2.ToInt32());
        #endregion
    }
}