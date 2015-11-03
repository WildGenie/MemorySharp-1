using System;
using System.Text;
using Binarysharp.MemoryManagement.Extensions;

namespace Binarysharp.MemoryManagement.Memory.Local
{
    /// <summary>
    ///     A native object in the game, one whose resources we do not directly own, but do manipulate.
    /// </summary>
    public class ProxyPointer : IEquatable<ProxyPointer>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProxyPointer" /> class.
        /// </summary>
        /// <param name="address">The base address.</param>
        /// <param name="memoryPlus">The MemoryPlus Instance.</param>
        public ProxyPointer(MemoryPlus memoryPlus, IntPtr address)
        {
            MemoryPlus = memoryPlus;
            BaseAddress = address;
        }

        #endregion

        #region  Interface members

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ProxyPointer other)
        {
            return BaseAddress == other.BaseAddress;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     The reference of the <see cref="ProcessMemory" /> object.
        /// </summary>
        protected MemoryPlus MemoryPlus { get; }

        /// <summary>
        ///     Gets the base address of this object in the remote process.
        /// </summary>
        public IntPtr BaseAddress { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => BaseAddress != IntPtr.Zero;

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return BaseAddress.GetHashCode();
        }

        /// <summary>
        ///     Reads a member of the specified type at the specified offset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public T Read<T>(int offset)
        {
            // We don't check for IsValid here because reading even on an invalid object would
            // not directly be an invalid operation - the read operation will throw an exception later down the line.

            return MemoryPlus.Read<T>(BaseAddress + offset);
        }

        /// <summary>
        ///     Reads a member of the specified type at the specified offset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public T Read<T>(Enum offset)
        {
            return Read<T>(offset.ToInt());
        }

        /// <summary>
        ///     Reads a member of the specified type at the specified offset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public T Read<T>(uint offset)
        {
            return Read<T>((int) offset);
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A value.</returns>
        public T Read<T>()
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
        public T[] ReadArray<T>(int offset, int count)
        {
            return MemoryPlus.ReadArray<T>(BaseAddress + offset, count);
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is read from the pointer.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <returns>An array.</returns>
        public T[] ReadArray<T>(Enum offset, int count)
        {
            return MemoryPlus.ReadArray<T>(BaseAddress + offset.ToInt(), count);
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
            return MemoryPlus.ReadString(BaseAddress + offset, encoding, maxLength);
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
            return ReadString(offset.ToInt(), encoding, maxLength);
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is written from the pointer.</param>
        /// <param name="value">The value to write.</param>
        public void Write<T>(int offset, T value)
        {
            MemoryPlus.Write(BaseAddress + offset, value);
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is written from the pointer.</param>
        /// <param name="value">The value to write.</param>
        public void Write<T>(Enum offset, T value)
        {
            Write(offset.ToInt(), value);
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
        public void WriteArray<T>(int offset, T[] array)
        {
            MemoryPlus.Write(BaseAddress + offset, array);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is written from the pointer.</param>
        /// <param name="array">The array to write.</param>
        public void WriteArray<T>(Enum offset, T[] array)
        {
            Write(offset.ToInt(), array);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="array">The array to write.</param>
        public void WriteArray<T>(T[] array)
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
            MemoryPlus.WriteString(BaseAddress + offset, text, encoding);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is written from the pointer.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        public void WriteString(Enum offset, string text, Encoding encoding)
        {
            WriteString(offset.ToInt(), text, encoding);
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
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ProxyPointer) obj);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(ProxyPointer left, ProxyPointer right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(ProxyPointer left, ProxyPointer right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"BaseAddress = 0x{BaseAddress.ToInt64():X}";
        }

        #endregion
    }
}