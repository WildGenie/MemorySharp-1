using System;
using System.Text;

namespace Binarysharp.MemoryManagement.MemoryInternal.Memory
{
    /// <summary>
    ///     Class representing a pointer in the memory of the remote process.
    /// </summary>
    public class RemotePointer : IEquatable<RemotePointer>
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="RemotePointer" /> class.
        /// </summary>
        /// <param name="memoryPlus">The reference of the <see cref="MemoryManagement.MemoryPlus" /> object.</param>
        /// <param name="address">The location where the pointer points in the remote process.</param>
        public RemotePointer(MemoryPlus memoryPlus, IntPtr address)
        {
            // Save the parameters
            MemoryPlus = memoryPlus;
            BaseAddress = address;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The address of the pointer in the remote process.
        /// </summary>
        public IntPtr BaseAddress { get; set; }

        /// <summary>
        ///     Gets if the <see cref="RemotePointer" /> is valid.
        /// </summary>
        public virtual bool IsValid => MemoryPlus.IsRunning && BaseAddress != IntPtr.Zero;

        /// <summary>
        ///     The reference of the <see cref="MemoryManagement.MemoryPlus" /> object.
        /// </summary>
        public MemoryPlus MemoryPlus { get; }
        #endregion

        #region  Interface members
        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(RemotePointer other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                   (BaseAddress.Equals(other.BaseAddress) && MemoryPlus.Equals(other.MemoryPlus));
        }
        #endregion

        #region Methods
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
            return BaseAddress.GetHashCode() ^ MemoryPlus.GetHashCode();
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
            return MemoryPlus.Read<T>(BaseAddress + offset);
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
            return MemoryPlus.ReadArray<T>(BaseAddress + offset, count);
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
            MemoryPlus.Write(BaseAddress + offset, array);
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
        #endregion
    }
}