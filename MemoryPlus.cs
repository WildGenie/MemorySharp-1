using System;
using System.Linq;
using System.Text;
using Binarysharp.MemoryManagement.Extensions;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;
using Binarysharp.MemoryManagement.MemoryInternal.Memory;
using Binarysharp.MemoryManagement.MemoryInternal.Modules;
using Binarysharp.MemoryManagement.MemoryInternal.Threading;

namespace Binarysharp.MemoryManagement
{
    /// <summary>
    ///     Class for memory editing a process.
    /// </summary>
    public class MemoryPlus : IDisposable, IEquatable<MemoryPlus>
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryPlus" /> class.
        /// </summary>
        /// <param name="process">Process to open.</param>
        public MemoryPlus(IProcess process)
        {
            Process = process;
            Modules = new ModuleFactory(this);
            Threads = new ThreadFactory(this);
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The <see cref="IProcess" /> member.
        /// </summary>
        public IProcess Process { get; }

        /// <summary>
        ///     State if the process is running.
        /// </summary>
        public bool IsRunning
            => !Process.SafeHandle.IsInvalid && !Process.SafeHandle.IsClosed && !Process.Native.HasExited;

        /// <summary>
        ///     Factory for manipulating modules and libraries.
        /// </summary>
        public ModuleFactory Modules { get; }

        /// <summary>
        ///     Factory for manipulating threads.
        /// </summary>
        public ThreadFactory Threads { get; }
        #endregion

        #region  Interface members
        /// <summary>
        /// </summary>
        public void Dispose()
        {
            Process.Dispose();
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(MemoryPlus other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Process.SafeHandle.Equals(other.Process.SafeHandle);
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Reads the specified amount of bytes from the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns>An array of bytes.</returns>
        public unsafe byte[] ReadBytes(IntPtr address, int count, bool isRelative = false)
        {
            AdjustAddress(ref address, isRelative);
            var readBytes = new byte[count];
            var b = (byte*) address;
            for (var i = 0; i < count; i++)
            {
                readBytes[i] = b[i];
            }
            return readBytes;
        }

        /// <summary>
        ///     Reads the value of a specified type in the process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A value.</returns>
        public T Read<T>(IntPtr address, bool isRelative = false)
        {
            AdjustAddress(ref address, isRelative);
            return InternalMarshals.PtrToStructure<T>(address);
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
            AdjustAddress(ref address, isRelative);
            if (offsets.Length == 0)
            {
                throw new InvalidOperationException("Cannot read a value from unspecified addresses.");
            }

            var temp = Read<IntPtr>(address);

            for (var i = 0; i < offsets.Length - 1; i++)
            {
                temp = Read<IntPtr>(temp + offsets[i]);
            }
            return Read<T>(temp + offsets[offsets.Length - 1]);
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is read.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>An array.</returns>
        public T[] ReadArray<T>(IntPtr address, int count, bool isRelative = false)
        {
            AdjustAddress(ref address, isRelative);
            var size = MarshalCache<T>.Size;
            var ret = new T[count];
            for (var i = 0; i < count; i++)
            {
                ret[i] = Read<T>(address + (i*size));
            }
            return ret;
        }

        /// <summary>
        ///     Read a string of the supplied encoding from an unmanaged pointer
        /// </summary>
        /// <param name="address">Pointer address to read from</param>
        /// <param name="encoding">Encoding to read</param>
        /// <param name="maxLength"></param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A string.</returns>
        public string ReadString(IntPtr address, Encoding encoding, int maxLength = 256, bool isRelative = false)
        {
            var data = ReadBytes(address, maxLength);
            var text = new string(encoding.GetChars(data));
            if (text.Contains("\0"))
                text = text.Substring(0, text.IndexOf('\0'));
            return text;
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="appendNullCharacter"></param>
        public unsafe void WriteString(IntPtr address, string text, Encoding encoding, bool isRelative = false,
            bool appendNullCharacter = true)
        {
            AdjustAddress(ref address, isRelative);
            var bytes = encoding.GetBytes(text);
            if (appendNullCharacter)
            {
                bytes = bytes.Concat(encoding.GetBytes(new[] {'\0'})).ToArray();
            }

            var pDest = (byte*) address.ToPointer();
            for (var i = 0; i < bytes.Length; i++)
            {
                pDest[i] = bytes[i];
            }
        }

        /// <summary>
        ///     Writes the specified bytes at the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        public void WriteBytes(IntPtr address, byte[] bytes, bool isRelative = false)
        {
            AdjustAddress(ref address, isRelative);
            using (new MemoryProtectionOperation(Process.SafeHandle, address, bytes.Length, 0x40))
                unsafe
                {
                    var ptr = (byte*) address;
                    for (var i = 0; i < bytes.Length; i++)
                    {
                        ptr[i] = bytes[i];
                    }
                }
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void Write<T>(IntPtr address, T value, bool isRelative = false)
        {
            AdjustAddress(ref address, isRelative);
            InternalMarshals.StructureToPointerWrite(address, value);
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
            AdjustAddress(ref address, isRelative);
            var size = MarshalCache<T>.Size;
            for (var i = 0; i < value.Length; i++)
            {
                var val = value[i];
                Write(address + (i*size), val);
            }
        }

        /// <summary>
        ///     Helper method to rebase address if needed.
        /// </summary>
        /// <param name="address">The address to rebase by ref.</param>
        /// <param name="isRelative">State if the address is relative to the main module.</param>
        private void AdjustAddress(ref IntPtr address, bool isRelative)
        {
            if (isRelative)
            {
                address = Rebase(address);
            }
        }

        /// <summary>
        ///     Rebases the given address to the main module.
        /// </summary>
        /// <param name="address">The address to rebase.</param>
        /// <returns>The rebased address</returns>
        private IntPtr Rebase(IntPtr address)
        {
            return Process.ImageBase.Add(address);
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MemorySharp) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            return Process.SafeHandle.GetHashCode();
        }
        #endregion
    }
}