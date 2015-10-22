using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Binarysharp.MemoryManagement.Internals;

namespace Binarysharp.MemoryManagement.Memory
{
    /// <summary>
    ///     Class for memory editing a <see cref="Process" /> that your application is injected to.
    /// </summary>
    public class InternalReadWrite : IReadWriteMemory
    {
        #region Properties
        /// <summary>
        ///     The protected <see cref="MemorySharp" /> Instance.
        /// </summary>
        public MemorySharp MemorySharp { get; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExternalReadWrite" /> class.
        /// </summary>
        /// <param name="sharp">The MemorySharp Instance.</param>
        public InternalReadWrite(MemorySharp sharp)
        {
            MemorySharp = sharp;
        }
        #endregion

        #region IReadWriteMemory Members
        /// <summary>
        ///     Reads the specified amount of bytes from the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns>An array of bytes.</returns>
        public unsafe byte[] ReadBytes(IntPtr address, int count, bool isRelative = false)
        {
            if (isRelative)
            {
                address = MemorySharp.MakeAbsolute(address);
            }

            var buffer = new byte[count];
            var ptr = (byte*) address;

            for (var i = 0; i < count; i++)
                buffer[i] = ptr[i];

            return buffer;
        }

        /// <summary>
        ///     Writes the specified bytes at the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        public unsafe void WriteBytes(IntPtr address, byte[] bytes, bool isRelative = false)
        {
            if (isRelative)
            {
                MemorySharp.MakeAbsolute(address);
            }
            var pointer = (byte*) address;
            {
                for (var i = 0; i < bytes.Length; i++)
                    pointer[i] = bytes[i];
            }
        }

        /// <summary>
        ///     Reads the value of a specified type in the process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A value.</returns>
        public unsafe T Read<T>(IntPtr address, bool isRelative = false)
        {
            if (isRelative)
            {
                address = MemorySharp.MakeAbsolute(address);
            }
            var pointer = address.ToPointer();

            object ptrObject;
            // Trys to use pointer dereference to get the value a pointer holds.
            switch (MarshalType<T>.TypeCode)
            {
                case TypeCode.Boolean:
                    ptrObject = *(bool*) pointer;
                    break;

                case TypeCode.Byte:
                    ptrObject = *(byte*) pointer;
                    break;

                case TypeCode.SByte:
                    ptrObject = *(sbyte*) pointer;
                    break;

                case TypeCode.Char:
                    ptrObject = *(char*) pointer;
                    break;

                case TypeCode.Int16:
                    ptrObject = *(short*) pointer;
                    break;

                case TypeCode.UInt16:
                    ptrObject = *(ushort*) pointer;
                    break;

                case TypeCode.Int32:
                    ptrObject = *(int*) pointer;
                    break;

                case TypeCode.UInt32:
                    ptrObject = *(uint*) pointer;
                    break;

                case TypeCode.Int64:
                    ptrObject = *(long*) pointer;
                    break;

                case TypeCode.UInt64:
                    ptrObject = *(ulong*) pointer;
                    break;

                case TypeCode.Single:
                    ptrObject = *(float*) pointer;
                    break;

                case TypeCode.Double:
                    ptrObject = *(double*) pointer;
                    break;

                default:
                    // Assume the pointer is a custom structure.
                    // https://msdn.microsoft.com/en-us/library/vstudio/4ca6d5z7(v=vs.100).aspx for more dtails on PtrToStructure.
                    ptrObject = Marshal.PtrToStructure(address, MarshalType<T>.RealType);
                    break;
            }

            return (T) ptrObject;
        }

        /// <summary>
        ///     Reads the value of a specified type in the process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="offsets">The offsets to apply in order to the given address.</param>
        /// <returns>A value.</returns>
        public T ReadMultilevelPointer<T>(bool isRelative, IntPtr address, params int[] offsets)
        {
            address = isRelative ? MemorySharp.MakeRelative(address) : address;
            for (var i = 0; i < offsets.Length - 1; i++)
            {
                address = Read<IntPtr>(address + offsets[i]);
            }
            return Read<T>(address + offsets[offsets.Length - 1]);
        }

        /// <summary>
        ///     Read an array of integral types (int, float, byte, etc) from unmanaged memory.
        /// </summary>
        /// <typeparam name="T">
        ///     Integral type to read. Must be struct, but not all structs are supported (only those supported by
        ///     Marshal.Copy
        /// </typeparam>
        /// <param name="address">Address to read array from</param>
        /// <param name="count">Size of the array to read (number of elements)</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public T[] ReadArray<T>(IntPtr address, int count, bool isRelative = false)
        {
            if (isRelative)
            {
                address = MemorySharp.MakeAbsolute(address);
            }

            switch (MarshalType<T>.TypeCode)
            {
                case TypeCode.Byte:
                    var bytes = new byte[count];
                    Marshal.Copy(address, bytes, 0, count);
                    return bytes.Cast<T>().ToArray();

                case TypeCode.Char:
                    var chars = new char[count];
                    Marshal.Copy(address, chars, 0, count);
                    return chars.Cast<T>().ToArray();

                case TypeCode.Int16:
                    var shorts = new short[count];
                    Marshal.Copy(address, shorts, 0, count);
                    return shorts.Cast<T>().ToArray();

                case TypeCode.Int32:
                    var ints = new int[count];
                    Marshal.Copy(address, ints, 0, count);
                    return ints.Cast<T>().ToArray();

                case TypeCode.Int64:
                    var longs = new long[count];
                    Marshal.Copy(address, longs, 0, count);
                    return longs.Cast<T>().ToArray();

                case TypeCode.Single:
                    var floats = new float[count];
                    Marshal.Copy(address, floats, 0, count);
                    return floats.Cast<T>().ToArray();

                case TypeCode.Double:
                    var doubles = new double[count];
                    Marshal.Copy(address, doubles, 0, count);
                    return doubles.Cast<T>().ToArray();

                default:
                    throw new ArgumentException($"Unsupported type argument supplied: {MarshalType<T>.RealType.Name}");
            }
        }

        /// <summary>
        ///     Read a string of the supplied encoding from an unmanaged pointer
        /// </summary>
        /// <param name="address">Pointer address to read from</param>
        /// <param name="encoding">Encoding to read.</param>
        /// <param name="maxLength">Max bytes possible the string contains.</param>
        /// <returns>A string.</returns>
        public string ReadString(IntPtr address, Encoding encoding, int maxLength = 256, bool isRelative = false)
        {
            if (
                !(encoding.Equals(Encoding.UTF8) || encoding.Equals(Encoding.Unicode) || encoding.Equals(Encoding.ASCII)))
            {
                throw new ArgumentException($"Encoding type {encoding.EncodingName} is not supported", nameof(encoding));
            }
            var bytes = ReadArray<byte>(address, maxLength);
            var terminalCharacterByte = encoding.GetBytes(new[] {'\0'});
            var buffer = new List<byte>();
            for (var i = 0; i < bytes.Length;)
            {
                var match = true;
                var shortBuffer = new List<byte>();
                for (var j = 0; j < terminalCharacterByte.Length; j++)
                {
                    shortBuffer.Add(bytes[i + j]);
                    if (bytes[i + j] == terminalCharacterByte[j])
                        continue;
                    match = false;
                    break;
                }
                if (match)
                {
                    break;
                }
                buffer.AddRange(shortBuffer);
                i += shortBuffer.Count;
            }
            var result = encoding.GetString(buffer.ToArray());
            return result;
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="appendNullCharacter">Ignore this for the external class.</param>
        public unsafe void WriteString(IntPtr address, string text, Encoding encoding, bool isRelative = false,
            bool appendNullCharacter = true)
        {
            var bytes = encoding.GetBytes(text);
            if (appendNullCharacter)
            {
                bytes = bytes.Concat(encoding.GetBytes(new[] {'\0'})).ToArray();
            }

            var pointer = (byte*) address.ToPointer();
            for (var i = 0; i < bytes.Length; i++)
            {
                pointer[i] = bytes[i];
            }
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public unsafe void Write<T>(IntPtr address, T value, bool isRelative = false)
        {
            if (isRelative)
            {
                address = MemorySharp.MakeAbsolute(address);
            }

            var pointer = address.ToPointer();

            object dataToWriteObj = value;

            switch (MarshalType<T>.TypeCode)
            {
                case TypeCode.Boolean:
                    *(bool*) pointer = (bool) dataToWriteObj;
                    break;

                case TypeCode.Byte:
                    *(byte*) pointer = (byte) dataToWriteObj;
                    break;

                case TypeCode.SByte:
                    *(sbyte*) pointer = (sbyte) dataToWriteObj;
                    break;

                case TypeCode.Char:
                    *(char*) pointer = (char) dataToWriteObj;
                    break;

                case TypeCode.Int16:
                    *(short*) pointer = (short) dataToWriteObj;
                    break;

                case TypeCode.UInt16:
                    *(ushort*) pointer = (ushort) dataToWriteObj;
                    break;

                case TypeCode.Int32:
                    *(int*) pointer = (int) dataToWriteObj;
                    break;

                case TypeCode.UInt32:
                    *(uint*) pointer = (uint) dataToWriteObj;
                    break;

                case TypeCode.Int64:
                    *(long*) pointer = (long) dataToWriteObj;
                    break;

                case TypeCode.UInt64:
                    *(ulong*) pointer = (ulong) dataToWriteObj;
                    break;

                case TypeCode.Single:
                    *(float*) pointer = (float) dataToWriteObj;
                    break;

                case TypeCode.Double:
                    *(double*) pointer = (double) dataToWriteObj;
                    break;

                default:
                    // Assume the pointer is a custom structure.
                    // https://msdn.microsoft.com/en-us/library/vstudio/4ca6d5z7(v=vs.100).aspx for more dtails on PtrToStructure.
                    Marshal.StructureToPtr(dataToWriteObj, address, true);
                    break;
            }
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The array of values to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteArray<T>(IntPtr address, T[] value, bool isRelative = false)
        {
            var size = MarshalType<T>.Size;
            for (var i = 0; i < value.Length; i++)
            {
                var val = value[i];
                Write(address + (i*size), val);
            }
        }
        #endregion
    }
}