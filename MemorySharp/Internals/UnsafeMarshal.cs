using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Binarysharp.MemoryManagement.Internals
{
    /// <summary>
    ///     Static class providing tools for extracting information related to types in an unsafe manner.
    /// </summary>
    /// <typeparam name="T">Type to analyse.</typeparam>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public static class UnsafeMarshal<T>
    {
        #region Public Methods
        /// <summary>
        ///     Converts a <see cref="IntPtr" /> to a structure.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>T.</returns>
        /// <exception cref="InvalidOperationException">Cannot retrieve a value at address 0</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public static unsafe T Read(IntPtr address)
        {
            var pAddr = address.ToPointer();

            object objectToReturn;

            if (MarshalType<T>.IsIntPtr)
            {
                objectToReturn = new IntPtr(*(void**) pAddr);
                return (T) objectToReturn;
            }

            switch (Type.GetTypeCode(typeof (T)))
            {
                case TypeCode.Boolean:
                    objectToReturn = *(bool*) pAddr;
                    break;
                case TypeCode.Byte:
                    objectToReturn = *(byte*) pAddr;
                    break;
                case TypeCode.SByte:
                    objectToReturn = *(sbyte*) pAddr;
                    break;
                case TypeCode.Char:
                    objectToReturn = *(char*) pAddr;
                    break;
                case TypeCode.Int16:
                    objectToReturn = *(short*) pAddr;
                    break;
                case TypeCode.UInt16:
                    objectToReturn = *(ushort*) pAddr;
                    break;
                case TypeCode.Int32:
                    objectToReturn = *(int*) pAddr;
                    break;
                case TypeCode.UInt32:
                    objectToReturn = *(uint*) pAddr;
                    break;
                case TypeCode.Int64:
                    objectToReturn = *(long*) pAddr;
                    break;
                case TypeCode.UInt64:
                    objectToReturn = *(ulong*) pAddr;
                    break;
                case TypeCode.Single:
                    objectToReturn = *(float*) pAddr;
                    break;
                case TypeCode.Double:
                    objectToReturn = *(double*) pAddr;
                    break;
                default:
                    // assume a custom struct, lets try
                    objectToReturn = Marshal.PtrToStructure(address, MarshalType<T>.RealType);
                    break;
            }

            return (T) objectToReturn;
        }

        /// <summary>
        ///     Structures to pointer.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="defaultValue">The value.</param>
        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public static unsafe void Write(IntPtr address, T defaultValue = default(T))
        {
            var pAddr = address.ToPointer();

            object oData = defaultValue;

            if (MarshalType<T>.IsIntPtr)
            {
                *(void**) pAddr = ((IntPtr) oData).ToPointer();
            }

            switch (Type.GetTypeCode(typeof (T)))
            {
                case TypeCode.Boolean:
                    *(bool*) pAddr = (bool) oData;
                    break;
                case TypeCode.Byte:
                    *(byte*) pAddr = (byte) oData;
                    break;
                case TypeCode.SByte:
                    *(sbyte*) pAddr = (sbyte) oData;
                    break;
                case TypeCode.Char:
                    *(char*) pAddr = (char) oData;
                    break;
                case TypeCode.Int16:
                    *(short*) pAddr = (short) oData;
                    break;
                case TypeCode.UInt16:
                    *(ushort*) pAddr = (ushort) oData;
                    break;
                case TypeCode.Int32:
                    *(int*) pAddr = (int) oData;
                    break;
                case TypeCode.UInt32:
                    *(uint*) pAddr = (uint) oData;
                    break;
                case TypeCode.Int64:
                    *(long*) pAddr = (long) oData;
                    break;
                case TypeCode.UInt64:
                    *(ulong*) pAddr = (ulong) oData;
                    break;
                case TypeCode.Single:
                    *(float*) pAddr = (float) oData;
                    break;
                case TypeCode.Double:
                    *(double*) pAddr = (double) oData;
                    break;
                default:
                    // assume a custom struct, lets try
                    Marshal.StructureToPtr(oData, address, true);
                    break;
            }
        }

        /// <summary>
        ///     Converts the given array of bytes to the specified type.
        ///     Uses either marshalling or unsafe code, depending on UseUnsafeReadWrite
        /// </summary>
        /// <param name="data">Array of bytes</param>
        /// <param name="defVal">The default value of this operation (which is returned in case the Read-operation fails)</param>
        /// <returns></returns>
        public static unsafe T ByteArrayToObject(byte[] data, T defVal = default(T))
        {
            T structure;
            fixed (byte* b = data)
                structure = (T) Marshal.PtrToStructure((IntPtr) b, typeof (T));
            return structure;
        }

        /// <summary>
        ///     Converts the given array of bytes to the specified type.
        ///     Uses either marshalling or unsafe code, depending on UseUnsafeReadWrite
        /// </summary>
        /// <param name="data">Array of bytes</param>
        /// <param name="index">Index of the data to convert</param>
        /// <param name="defVal">The default value of this operation (which is returned in case the Read-operation fails)</param>
        /// <returns></returns>
        public static T ByteArrayToObject(byte[] data, int index, T defVal = default(T))
        {
            var size = Marshal.SizeOf(typeof (T));
            var tmp = new byte[size];
            Array.Copy(data, index, tmp, 0, size);
            return ByteArrayToObject(tmp, defVal);
        }

        /// <summary>
        ///     Converts the given struct to a byte-array
        /// </summary>
        /// <param name="value">Value to conver to bytes</param>
        /// <returns></returns>
        public static unsafe byte[] ObjectToByteArray(T value)
        {
            var size = Marshal.SizeOf(typeof (T));
            var data = new byte[size];
            fixed (byte* b = data)
                Marshal.StructureToPtr(value, (IntPtr) b, true);
            return data;
        }
        #endregion
    }
}