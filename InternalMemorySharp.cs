using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MemorySharp.Disassembly;
using MemorySharp.Internals;
using MemorySharp.Internals.Marshaling;
using MemorySharp.Memory;

namespace MemorySharp
{
    /// <summary>
    ///     A class providing support for in-process memory operations.
    ///     <remarks>
    ///         A lot of credits to https://github.com/aevitas/bluerain and of course the MemorySharp lib Author ZenLulz
    ///         http://binarysharp.com/
    ///     </remarks>
    /// </summary>
    public class InternalMemorySharp : MemoryBase
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="InternalMemorySharp" /> class.
        ///     <remarks>This class inherits from the <see cref="MemoryBase" /> class.</remarks>
        /// </summary>
        public InternalMemorySharp(Process process) : base(process)
        {
            Disassembler = new Disassembler();
            Detours = new DetourManager(this);
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The <see cref="Disassembly.Disassembler" /> Instance.
        /// </summary>
        public Disassembler Disassembler { get; }

        /// <summary>
        ///     A manager for detours. See <see cref="Detours" />
        /// </summary>
        public DetourManager Detours { get; }
        #endregion

        #region Methods
        /// <summary>
        ///     Reads the specified amount of bytes from the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public override unsafe byte[] ReadBytes(IntPtr address, int count, bool isRelative = false)
        {
            if (isRelative)
                address = ToAbsolute(address);

            var buffer = new byte[count];
            var ptr = (byte*) address;

            for (var i = 0; i < count; i++)
            {
                buffer[i] = ptr[i];
            }

            return buffer;
        }

        /// <summary>
        ///     Writes the specified bytes at the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        public override unsafe int WriteBytes(IntPtr address, byte[] bytes, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            var pointer = (byte*) address;
            for (var i = 0; i < bytes.Length; i++)
            {
                pointer[i] = bytes[i];
            }
            return bytes.Length;
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
            var size = MarshalType<T>.Size;
            for (var i = 0; i < value.Length; i++)
            {
                var val = value[i];
                Write(address + (i*size), val);
            }
        }

        /// <summary>
        ///     Reads the specified amount of values of the specified type at the specified address.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public override T[] ReadArray<T>(IntPtr address, int count, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }

            var ret = new T[count];
            for (var i = 0; i < count; i++)
            {
                ret[i] = Read<T>(address + MarshalType<T>.Size*i);
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
        public override unsafe void Write<T>(IntPtr address, T value, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }

            var pointer = address.ToPointer();

            object dataToWriteObj = value;

            // ReSharper disable once SwitchStatementMissingSomeCases
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
        ///     Reads the value of a specified type in the process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A value.</returns>
        public override unsafe T Read<T>(IntPtr address, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
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
        ///     Registers a function into a delegate. Note: The delegate must provide a proper function signature!
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A delegate.</returns>
        public T GetDelegate<T>(IntPtr address, bool isRelative = false) where T : class
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            if (typeof (T).GetCustomAttributes(typeof (UnmanagedFunctionPointerAttribute), true).Length == 0)
            {
                throw new InvalidOperationException(
                    "This operation can only convert to delegates adorned with the UnmanagedFunctionPointerAttribute");
            }
            return Marshal.GetDelegateForFunctionPointer(address, typeof (T)) as T;
        }

        /// <summary>
        ///     Gets a virtual function delegate. Note: The delegate must provide a proper function signature!
        /// </summary>
        /// <typeparam name="T">Type param.</typeparam>
        /// <param name="address">The address the class is located in memory.</param>
        /// <param name="functionIndex">The index the virtual method is located at.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns><see cref="IntPtr" /> to address to the function.</returns>
        public T GetVirtualFunction<T>(IntPtr address, int functionIndex, bool isRelative = false) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer<T>(GetVTablePointer(address, functionIndex));
        }

        /// <summary>
        ///     Gets a function pointer from an object's virtual method table at the supplied index
        /// </summary>
        /// <param name="address">Object base address</param>
        /// <param name="index">Virtual method index</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns><see cref="IntPtr" /> to address to the function.</returns>
        public unsafe IntPtr GetVTablePointer(IntPtr address, int index, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            var pAddr = (void***) address.ToPointer();
            return new IntPtr((*pAddr)[index]);
        }

        /// <summary>
        ///     Creates an new <see cref="RemoteVirtualClass" /> Instance.
        /// </summary>
        /// <param name="address">The Address the class is located in memory.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A <see cref="RemoteVirtualClass" /> Instance.</returns>
        public RemoteVirtualClass CreateVirtualClass(IntPtr address, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            return new RemoteVirtualClass(address);
        }
        #endregion
    }
}