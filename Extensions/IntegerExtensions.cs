using System;
using System.Text;
using MemorySharp.Disassembly;

namespace MemorySharp.Helpers.Extensions
{
    /// <summary>
    ///     A class providing extension methods for <see cref="int" /> Instance's.
    ///     <remarks>
    ///         eturns>Unfinshed documentation. Most credits go to: "Jeffora"'s extememory project.
    ///         https://github.com/jeffora/extemory
    ///     </remarks>
    /// </summary>
    public static class IntegerExtensions
    {
        #region Methods
        /// <summary>
        ///     Read a struct type from an unmanaged pointer.
        /// </summary>
        /// <typeparam name="T">Struct type to read</typeparam>
        /// <param name="addr">Pointer address to read from</param>
        /// <returns></returns>
        public static T Read<T>(this int addr, bool isRelative = false) where T : struct
        {
            return new IntPtr(addr).Read<T>(isRelative);
        }

        /// <summary>
        ///     Write a structure to unmanaged memory
        /// </summary>
        /// <typeparam name="T">Struct type to write</typeparam>
        /// <param name="addr">Address to write to</param>
        /// <param name="data">Struct data to write</param>
        public static void Write<T>(this int addr, T data, bool isRelative = false) where T : struct
        {
            new IntPtr(addr).Write(data, isRelative);
        }

        /// <summary>
        ///     Read an array of integral types (int, float, byte, etc) from unmanaged memory.
        /// </summary>
        /// <typeparam name="T">
        ///     Integral type to read. Must be struct, but not all structs are supported (only those supported by
        ///     Marshal.Copy
        /// </typeparam>
        /// <param name="addr">Address to read array from</param>
        /// <param name="size">Size of the array to read (number of elements)</param>
        /// <returns></returns>
        public static T[] ReadArray<T>(this int addr, int size, bool isRelative = false) where T : struct
        {
            return new IntPtr(addr).ReadArray<T>(size, isRelative);
        }

        /// <summary>
        ///     Write an array of integral types (int, float, byte, etc) to unmanaged memory.
        /// </summary>
        /// <typeparam name="T">
        ///     Integral type to write. Must be struct, but not all structs are supported (only those supported by
        ///     Marshal.Copy
        /// </typeparam>
        /// <param name="addr">Address to write array to</param>
        /// <param name="data">Array data to write</param>
        public static void WriteArray<T>(this int addr, T[] data, bool isRelative = false) where T : struct
        {
            new IntPtr(addr).WriteArray(data, isRelative);
        }

        public static string ReadString(this int addr, Encoding encoding, bool isRelative = false)
        {
            return new IntPtr(addr).ReadString(encoding, isRelative);
        }

        public static void WriteString(this int addr, string value, Encoding encoding, bool isRelative = false)
        {
            new IntPtr(addr).WriteString(value, encoding, true, isRelative);
        }

        public static IntPtr VTable(this int addr, int index, bool isRelative = false)
        {
            return new IntPtr(addr).VTable(index);
        }

        public static T ToDelegate<T>(this int addr) where T : class
        {
            return new IntPtr(addr).ToDelegate<T>();
        }

        public static BrokenDetours<T> DetourWith<T>(this int addr, T del) where T : class
        {
            return new IntPtr(addr).DetourWith(del);
        }

        public static T Read<T>(this uint addr, bool isRelative = false) where T : struct
        {
            return new IntPtr(addr).Read<T>(isRelative);
        }

        public static void Write<T>(this uint addr, T data, bool isRelative = false) where T : struct
        {
            new IntPtr(addr).Write(data, isRelative);
        }

        public static T[] ReadArray<T>(this uint addr, int size, bool isRelative = false) where T : struct
        {
            return new IntPtr(addr).ReadArray<T>(size, isRelative);
        }

        public static void WriteArray<T>(this uint addr, T[] data, bool isRelative = false) where T : struct
        {
            new IntPtr(addr).WriteArray(data, isRelative);
        }

        public static string ReadString(this uint addr, Encoding encoding, bool isRelative = false)
        {
            return new IntPtr(addr).ReadString(encoding, isRelative);
        }

        public static void WriteString(this uint addr, string value, Encoding encoding, bool isRelative = false)
        {
            new IntPtr(addr).WriteString(value, encoding, true, isRelative);
        }

        public static IntPtr VTable(this uint addr, int index, bool isRelative = false)
        {
            return new IntPtr(addr).VTable(index);
        }

        public static T ToDelegate<T>(this uint addr) where T : class
        {
            return new IntPtr(addr).ToDelegate<T>();
        }

        public static BrokenDetours<T> DetourWith<T>(this uint addr, T del) where T : class
        {
            return new IntPtr(addr).DetourWith(del);
        }

        public static T Read<T>(this long addr, bool isRelative = false) where T : struct
        {
            return new IntPtr(addr).Read<T>(isRelative);
        }

        public static void Write<T>(this long addr, T data, bool isRelative = false) where T : struct
        {
            new IntPtr(addr).Write(data, isRelative);
        }

        public static T[] ReadArray<T>(this long addr, int size, bool isRelative = false) where T : struct
        {
            return new IntPtr(addr).ReadArray<T>(size, isRelative);
        }

        public static void WriteArray<T>(this long addr, T[] data, bool isRelative = false) where T : struct
        {
            new IntPtr(addr).WriteArray(data, isRelative);
        }

        public static string ReadString(this long addr, Encoding encoding, bool isRelative = false)
        {
            return new IntPtr(addr).ReadString(encoding, isRelative);
        }

        public static void WriteString(this long addr, string value, Encoding encoding, bool isRelative = false)
        {
            new IntPtr(addr).WriteString(value, encoding, true, false);
        }

        public static IntPtr VTable(this long addr, int index)
        {
            return new IntPtr(addr).VTable(index);
        }

        public static T ToDelegate<T>(this long addr) where T : class
        {
            return new IntPtr(addr).ToDelegate<T>();
        }
        #endregion
    }
}