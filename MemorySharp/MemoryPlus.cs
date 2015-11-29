using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Binarysharp.MemoryManagement.Edits.Detours;
using Binarysharp.MemoryManagement.Edits.Patchs;
using Binarysharp.MemoryManagement.Hooks;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Native.Enums;
using Binarysharp.MemoryManagement.Patterns;

namespace Binarysharp.MemoryManagement
{
    /// <summary>
    ///     Class for memory operations in a local process.
    /// </summary>
    public class MemoryPlus : IDisposable, IEquatable<MemoryPlus>
    {
        #region Fields, Private Properties
        private List<IFactory> Factories { get; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryPlus" /> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public MemoryPlus(Process process)
        {
            Native = process;
            MainModule = process.MainModule;
            ImageBase = process.MainModule.BaseAddress;
            Handle = new SafeMemoryHandle(ExternalMemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, process.Id));
            Factories = new List<IFactory>();
            Factories.AddRange(
                               new IFactory[]
                               {
                                   Patterns =
                                   new InternalPatternFactory {MemoryPlus = this, ProcessModule = process.MainModule},
                                   Hooks = new HookFactory {MemoryPlus = this},
                                   Patches = new InternalPatchFactory {MemoryPlus = this},
                                   Detours = new DetourFactory {MemoryPlus = this}
                               });
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets the pattern factory.
        /// </summary>
        /// <value>
        ///     The pattern factory.
        /// </value>
        public InternalPatternFactory Patterns { get; }

        /// <summary>
        ///     Gets the base address of the main <see cref="ProcessModule" />.
        /// </summary>
        /// <value>
        ///     The base address of the main <see cref="ProcessModule" />.
        /// </value>
        public IntPtr ImageBase { get; }

        /// <summary>
        ///     The remote process handle opened with all rights.
        /// </summary>
        public SafeMemoryHandle Handle { get; }

        /// <summary>
        ///     Gets the main module as a <see cref="ProcessModule" /> instance.
        /// </summary>
        public ProcessModule MainModule { get; }

        /// <summary>
        ///     Provide access to the opened process.
        /// </summary>
        public Process Native { get; }

        /// <summary>
        ///     A factory for hooks.
        /// </summary>
        public HookFactory Hooks { get; }

        /// <summary>
        ///     A factory for memory detours.
        /// </summary>
        public DetourFactory Detours { get; }

        /// <summary>
        ///     A factory for memory patches.
        /// </summary>
        public InternalPatchFactory Patches { get; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Factories.ForEach(factory => factory.Dispose());
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(MemoryPlus other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return ReferenceEquals(this, other) || Handle.Equals(other.Handle);
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A value.</returns>
        public T Read<T>(IntPtr address, bool isRelative = false)
        {
            if (isRelative)
            {
                address = MakeAbsolute(address);
            }
            return InternalMemoryCore.Read<T>(address);
        }

        /// <summary>
        ///     Reads the specified amount of bytes from the specified address.
        /// </summary>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns>An array of bytes.</returns>
        public byte[] ReadBytes(IntPtr address, int count, bool isRelative = false)
        {
            return InternalMemoryCore.ReadBytes(address, count, isRelative);
        }


        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A value.</returns>
        public T Read<T>(Enum address, bool isRelative = false)
        {
            return Read<T>(new IntPtr(Convert.ToInt64(address)), isRelative);
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is read.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>An array.</returns>
        public T[] Read<T>(IntPtr address, int count, bool isRelative = false)
        {
            // Allocate an array to store the results
            var array = new T[count];
            // Read the array in the remote process
            for (var i = 0; i < count; i++)
            {
                array[i] = Read<T>(address + MarshalType<T>.Size*i, isRelative);
            }
            return array;
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is read.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>An array.</returns>
        public T[] Read<T>(Enum address, int count, bool isRelative = false)
        {
            return Read<T>(new IntPtr(Convert.ToInt64(address)), count, isRelative);
        }


        /// <summary>
        ///     Makes an absolute address from a relative one based on the main module.
        /// </summary>
        /// <param name="address">The relative address.</param>
        /// <returns>The absolute address.</returns>
        public IntPtr MakeAbsolute(IntPtr address)
        {
            // Check if the relative address is not greater than the main module size
            if (address.ToInt64() > MainModule.ModuleMemorySize)
            {
                throw new ArgumentOutOfRangeException(nameof(address),
                                                      "The relative address cannot be greater than the main module size.");
            }
            // Compute the absolute address
            return new IntPtr(ImageBase.ToInt64() + address.ToInt64());
        }

        /// <summary>
        ///     Makes a relative address from an absolute one based on the main module.
        /// </summary>
        /// <param name="address">The absolute address.</param>
        /// <returns>The relative address.</returns>
        public IntPtr MakeRelative(IntPtr address)
        {
            // Check if the absolute address is smaller than the main module base address
            if (address.ToInt64() < ImageBase.ToInt64())
            {
                throw new ArgumentOutOfRangeException(nameof(address),
                                                      "The absolute address cannot be smaller than the main module base address.");
            }
            // Compute the relative address
            return new IntPtr(address.ToInt64() - ImageBase.ToInt64());
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == GetType() && Equals((MemoryPlus) obj);
        }

        /// <summary>
        ///     Write an array of bytes in the remote process.
        /// </summary>
        /// <param name="address">The address where the array is written.</param>
        /// <param name="byteArray">The array of bytes to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteBytes(IntPtr address, byte[] byteArray, bool isRelative = false)
        {
            // Change the protection of the memory to allow writable
            InternalMemoryCore.WriteBytes(address, byteArray, isRelative);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is read.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(IntPtr address, Encoding encoding, bool isRelative = false, int maxLength = 512)
        {
            // Read the string
            var data = encoding.GetString(ReadBytes(address, maxLength, isRelative));
            // Search the end of the string
            var end = data.IndexOf('\0');
            // Crop the string with this end
            return data.Substring(0, end);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is read.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(Enum address, Encoding encoding, bool isRelative = false, int maxLength = 512)
        {
            return ReadString(new IntPtr(Convert.ToInt64(address)), encoding, isRelative, maxLength);
        }

        /// <summary>
        ///     Reads a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(IntPtr address, bool isRelative = false, int maxLength = 512)
        {
            return ReadString(address, Encoding.UTF8, isRelative, maxLength);
        }

        /// <summary>
        ///     Reads a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(Enum address, bool isRelative = false, int maxLength = 512)
        {
            return ReadString(new IntPtr(Convert.ToInt64(address)), isRelative, maxLength);
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
            WriteBytes(address, MarshalType<T>.ObjectToByteArray(value), isRelative);
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void Write<T>(Enum address, T value, bool isRelative = false)
        {
            Write(new IntPtr(Convert.ToInt64(address)), value, isRelative);
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
            // Write the array in the remote process
            for (var i = 0; i < array.Length; i++)
            {
                Write(address + MarshalType<T>.Size*i, array[i], isRelative);
            }
        }


        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is written.</param>
        /// <param name="array">The array to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void Write<T>(Enum address, T[] array, bool isRelative = false)
        {
            Write(new IntPtr(Convert.ToInt64(address)), array, isRelative);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteString(IntPtr address, string text, Encoding encoding, bool isRelative = false)
        {
            // Write the text
            WriteBytes(address, encoding.GetBytes(text + '\0'), isRelative);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteString(Enum address, string text, Encoding encoding, bool isRelative = false)
        {
            WriteString(new IntPtr(Convert.ToInt64(address)), text, encoding, isRelative);
        }

        /// <summary>
        ///     Writes a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteString(IntPtr address, string text, bool isRelative = false)
        {
            WriteString(address, text, Encoding.UTF8, isRelative);
        }

        /// <summary>
        ///     Writes a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteString(Enum address, string text, bool isRelative = false)
        {
            WriteString(new IntPtr(Convert.ToInt64(address)), text, isRelative);
        }


        /// <summary>
        ///     Creates a function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <param name="isRelative">if set to <c>true</c> [address is relative].</param>
        /// <returns></returns>
        /// <remarks>Created 2012-01-16 20:40 by Nesox.</remarks>
        public T RegisterDelegate<T>(IntPtr address, bool isRelative = false) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer(isRelative ? MakeAbsolute(address) : address, typeof (T)) as T;
        }

        /// <summary>
        ///     Gets the funtion pointer from a delegate.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        /// <remarks>Created 2012-01-16 20:40 by Nesox.</remarks>
        public IntPtr GetFunctionPointer(Delegate d)
        {
            return Marshal.GetFunctionPointerForDelegate(d);
        }

        /// <summary>
        ///     Gets the VF table entry.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <remarks>Created 2012-01-16 20:40 by Nesox.</remarks>
        public IntPtr GetVfTableEntry(IntPtr address, uint index)
        {
            var vftable = Read<IntPtr>(address);
            return Read<IntPtr>(vftable + (int) (index*4));
        }

        /// <summary>
        ///     Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MemoryPlus left, MemoryPlus right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MemoryPlus left, MemoryPlus right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}