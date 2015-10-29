using System;
using System.Diagnostics;
using MemorySharp.Helpers;
using MemorySharp.Internals.Marshaling;
using MemorySharp.Memory;

namespace MemorySharp
{
    /// <summary>
    ///     Provides functionality for reading from, writing to, and allocating memory in external processes.
    ///     This class relies on a handle to the remote process and to its main thread to perform memory reading and writing.
    ///     <remarks>
    ///         A lot of credits to https://github.com/aevitas/bluerain and of course the MemorySharp lib Author ZenLulz
    ///         http://binarysharp.com/
    ///     </remarks>
    /// </summary>
    public sealed class ExternalMemorySharp : MemoryBase
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExternalMemorySharp" /> class.
        ///     <remarks>This class inherits from the <see cref="MemoryBase" /> class.</remarks>
        /// </summary>
        public ExternalMemorySharp(Process process) : base(process)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Reads an array of bytes in the remote process.
        /// </summary>
        /// <param name="address">The address where the array is read.</param>
        /// <param name="size">The number of cells.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>The array of bytes.</returns>
        public override byte[] ReadBytes(IntPtr address, int size, bool isRelative = false)
        {
            return ReadProcessMemory(address, size, isRelative);
        }

        /// <summary>
        ///     Write an array of bytes in the remote process.
        /// </summary>
        /// <param name="address">The address where the array is written.</param>
        /// <param name="bytes">The array of bytes to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public override int WriteBytes(IntPtr address, byte[] bytes, bool isRelative = false)
        {
            return WriteProcessMemory(address, bytes, isRelative);
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A value.</returns>
        public override T Read<T>(IntPtr address, bool isRelative = false)
        {
            return
                MarshalType<T>.ByteArrayToObject(ReadBytes(address, MarshalType<T>.Size, isRelative));
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is read.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>An array.</returns>
        public override T[] ReadArray<T>(IntPtr address, int count, bool isRelative = false)
        {
            Requires.NotEqual(address, IntPtr.Zero, nameof(address));

            var size = MarshalType<T>.Size;

            var ret = new T[count];
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            // ReadArray = add + n * size
            for (var i = 0; i < count; i++)
            {
                ret[i] = Read<T>(address + (i*size));
            }
            return ret;
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public override void Write<T>(IntPtr address, T value, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            WriteProcessMemory(address, MarshalType<T>.ObjectToByteArray(value));
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The array of values to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public override void WriteArray<T>(IntPtr address, T[] value, bool isRelative = false)
        {
            // Write the array in the remote process
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            for (var i = 0; i < value.Length; i++)
            {
                Write(address + MarshalType<T>.Size*i, value[i]);
            }
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is written.</param>
        /// <param name="array">The array to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void Write<T>(IntPtr address, T[] array, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            // Write the array in the remote process
            for (var i = 0; i < array.Length; i++)
            {
                Write(address + MarshalType<T>.Size*i, array[i]);
            }
        }
        #endregion
    }
}