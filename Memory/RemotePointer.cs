/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.Text;
using MemorySharp.Native;

namespace MemorySharp.Memory
{
    /// <summary>
    ///     Class representing a pointer in the memory of the remote process.
    /// </summary>
    public class RemotePointer : IEquatable<RemotePointer>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RemotePointer" /> class.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemoryBase" /> object.</param>
        /// <param name="address">The location where the pointer points in the remote process.</param>
        public RemotePointer(MemoryBase memorySharp, IntPtr address)
        {
            // Save the parameters
            MemorySharp = memorySharp;
            BaseAddress = address;
            BaseAddressField = address;
        }

        public IntPtr BaseAddressField { get; set; }

        /// <summary>
        ///     The address of the pointer in the remote process.
        /// </summary>
        public IntPtr BaseAddress { get; }

        /// <summary>
        ///     Gets if the <see cref="RemotePointer" /> is valid.
        /// </summary>
        public virtual bool IsValid => MemorySharp.IsRunning && BaseAddress != IntPtr.Zero;

        /// <summary>
        ///     The reference of the <see cref="MemoryBase" /> object.
        /// </summary>
        public MemoryBase MemorySharp { get; }

        /// <summary>
        ///     Changes the protection of the n next bytes in remote process.
        /// </summary>
        /// <param name="size">The size of the memory to change.</param>
        /// <param name="protection">The new protection to apply.</param>
        /// <param name="mustBeDisposed">The resource will be automatically disposed when the finalizer collects the object.</param>
        /// <returns>A new instance of the <see cref="MemoryProtection" /> class.</returns>
        public MemoryProtection ChangeProtection(int size,
            MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite, bool mustBeDisposed = true)
        {
            return new MemoryProtection(MemorySharp, BaseAddress, size, protection, mustBeDisposed);
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RemotePointer) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            return BaseAddress.GetHashCode() ^ MemorySharp.GetHashCode();
        }

        public static bool operator ==(RemotePointer left, RemotePointer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RemotePointer left, RemotePointer right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is read from the pointer.</param>
        /// <returns>A value.</returns>
        public T Read<T>(int offset) where T : struct
        {
            return MemorySharp.Read<T>(BaseAddress + offset);
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is read from the pointer.</param>
        /// <returns>A value.</returns>
        public T Read<T>(Enum offset) where T : struct
        {
            return Read<T>(Convert.ToInt32(offset));
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A value.</returns>
        public T Read<T>() where T : struct
        {
            return Read<T>(0);
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is read from the pointer.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <returns>An array.</returns>
        public T[] Read<T>(int offset, int count) where T : struct
        {
            return MemorySharp.ReadArray<T>(BaseAddress + offset, count);
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is read from the pointer.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <returns>An array.</returns>
        public T[] Read<T>(Enum offset, int count) where T : struct
        {
            return Read<T>(Convert.ToInt32(offset), count);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is read from the pointer.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(int offset, Encoding encoding, int maxLength = 512)
        {
            return MemorySharp.ReadString(BaseAddress + offset, encoding, maxLength);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is read from the pointer.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(Enum offset, Encoding encoding, int maxLength = 512)
        {
            return ReadString(Convert.ToInt32(offset), encoding, maxLength);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(Encoding encoding, int maxLength = 512)
        {
            return ReadString(0, encoding, maxLength);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"BaseAddress = 0x{BaseAddress.ToInt64():X}";
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is written from the pointer.</param>
        /// <param name="value">The value to write.</param>
        public void Write<T>(int offset, T value)
        {
            MemorySharp.Write(BaseAddress + offset, value, false);
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is written from the pointer.</param>
        /// <param name="value">The value to write.</param>
        public void Write<T>(Enum offset, T value)
        {
            Write(Convert.ToInt32(offset), value);
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to write.</param>
        public void Write<T>(T value)
        {
            Write(0, value);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is written from the pointer.</param>
        /// <param name="array">The array to write.</param>
        public void Write<T>(int offset, T[] array)
        {
            MemorySharp.Write(BaseAddress + offset, array, false);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is written from the pointer.</param>
        /// <param name="array">The array to write.</param>
        public void Write<T>(Enum offset, T[] array)
        {
            Write(Convert.ToInt32(offset), array);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="array">The array to write.</param>
        public void Write<T>(T[] array)
        {
            Write(0, array);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is written from the pointer.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        public void WriteString(int offset, string text, Encoding encoding)
        {
            MemorySharp.WriteString(BaseAddress + offset, text, encoding, false);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is written from the pointer.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        public void WriteString(Enum offset, string text, Encoding encoding)
        {
            WriteString(Convert.ToInt32(offset), text, encoding);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        public void WriteString(string text, Encoding encoding)
        {
            WriteString(0, text, encoding);
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(RemotePointer other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                   (BaseAddress.Equals(other.BaseAddress) && MemorySharp.Equals(other.MemorySharp));
        }
    }
}