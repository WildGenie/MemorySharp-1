using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.Internals
{
    public static class InternalMarshals
    {
        #region Methods
        /// <summary>
        ///     Converts the given array of bytes to the specified type.
        ///     Uses either marshalling or unsafe code, depending on UseUnsafeReadWrite
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="data">Array of bytes</param>
        /// <param name="defVal">The default value of this operation (which is returned in case the Read-operation fails)</param>
        /// <returns></returns>
        public static unsafe T BytesToT<T>(byte[] data, T defVal = default(T))
        {
            var structure = defVal;

            //if (UseUnsafeReadWrite)
            {
                fixed (byte* b = data)
                    structure = (T) Marshal.PtrToStructure((IntPtr) b, typeof (T));
            }
            //else
            //{
            //    GCHandle gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            //    structure = (T)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(T));
            //    gcHandle.Free();
            //}
            return structure;
        }

        /// <summary>
        ///     Converts the given array of bytes to the specified type.
        ///     Uses either marshalling or unsafe code, depending on UseUnsafeReadWrite
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="data">Array of bytes</param>
        /// <param name="index">Index of the data to convert</param>
        /// <param name="defVal">The default value of this operation (which is returned in case the Read-operation fails)</param>
        /// <returns></returns>
        public static T BytesToT<T>(byte[] data, int index, T defVal = default(T)) where T : struct
        {
            var size = Marshal.SizeOf(typeof (T));
            var tmp = new byte[size];
            Array.Copy(data, index, tmp, 0, size);
            return BytesToT(tmp, defVal);
        }

        /// <summary>
        ///     Converts the given struct to a byte-array
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <param name="value">Value to conver to bytes</param>
        /// <returns></returns>
        public static unsafe byte[] TToBytes<T>(T value) where T : struct
        {
            var size = Marshal.SizeOf(typeof (T));
            var data = new byte[size];

            //if (UseUnsafeReadWrite)
            {
                fixed (byte* b = data)
                    Marshal.StructureToPtr(value, (IntPtr) b, true);
            }
            //else
            //{
            //    IntPtr ptr = Marshal.AllocHGlobal(size);
            //    Marshal.StructureToPtr(value, ptr, true);
            //    Marshal.Copy(ptr, data, 0, size);
            //    Marshal.FreeHGlobal(ptr);
            //}

            return data;
        }

        public static unsafe T PtrToStructure<T>(IntPtr address)
        {
            try
            {
                if (address == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Cannot retrieve a value at address 0");
                }

                object ptrToStructure;
                switch (MarshalCache<T>.TypeCode)
                {
                    case TypeCode.Object:

                        if (MarshalCache<T>.IsIntPtr)
                        {
                            return (T) (object) *(IntPtr*) address;
                        }

                        // If the type doesn't require an explicit Marshal call, then ignore it and memcpy the fuckin thing.
                        if (!MarshalCache<T>.TypeRequiresMarshal)
                        {
                            var o = default(T);
                            var ptr = MarshalCache<T>.GetUnsafePtr(ref o);

                            NativeMethods.MoveMemory(ptr, (void*) address, MarshalCache<T>.Size);

                            return o;
                        }

                        // All System.Object's require marshaling!
                        ptrToStructure = Marshal.PtrToStructure(address, typeof (T));
                        break;
                    case TypeCode.Boolean:
                        ptrToStructure = *(byte*) address != 0;
                        break;
                    case TypeCode.Char:
                        ptrToStructure = *(char*) address;
                        break;
                    case TypeCode.SByte:
                        ptrToStructure = *(sbyte*) address;
                        break;
                    case TypeCode.Byte:
                        ptrToStructure = *(byte*) address;
                        break;
                    case TypeCode.Int16:
                        ptrToStructure = *(short*) address;
                        break;
                    case TypeCode.UInt16:
                        ptrToStructure = *(ushort*) address;
                        break;
                    case TypeCode.Int32:
                        ptrToStructure = *(int*) address;
                        break;
                    case TypeCode.UInt32:
                        ptrToStructure = *(uint*) address;
                        break;
                    case TypeCode.Int64:
                        ptrToStructure = *(long*) address;
                        break;
                    case TypeCode.UInt64:
                        ptrToStructure = *(ulong*) address;
                        break;
                    case TypeCode.Single:
                        ptrToStructure = *(float*) address;
                        break;
                    case TypeCode.Double:
                        ptrToStructure = *(double*) address;
                        break;
                    case TypeCode.Decimal:
                        ptrToStructure = *(decimal*) address;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return (T) ptrToStructure;
            }
            catch (AccessViolationException)
            {
                Trace.WriteLine("Access Violation on " + address + " with type " + typeof (T).Name);
                return default(T);
            }
        }

        public static unsafe void StructureToPointerWrite<T>(IntPtr address, T value)
        {
            var pointer = address.ToPointer();

            object structureToPtr = value;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (MarshalType<T>.TypeCode)
            {
                case TypeCode.Boolean:
                    *(bool*) pointer = (bool) structureToPtr;
                    break;

                case TypeCode.Byte:
                    *(byte*) pointer = (byte) structureToPtr;
                    break;

                case TypeCode.SByte:
                    *(sbyte*) pointer = (sbyte) structureToPtr;
                    break;

                case TypeCode.Char:
                    *(char*) pointer = (char) structureToPtr;
                    break;

                case TypeCode.Int16:
                    *(short*) pointer = (short) structureToPtr;
                    break;

                case TypeCode.UInt16:
                    *(ushort*) pointer = (ushort) structureToPtr;
                    break;

                case TypeCode.Int32:
                    *(int*) pointer = (int) structureToPtr;
                    break;

                case TypeCode.UInt32:
                    *(uint*) pointer = (uint) structureToPtr;
                    break;

                case TypeCode.Int64:
                    *(long*) pointer = (long) structureToPtr;
                    break;

                case TypeCode.UInt64:
                    *(ulong*) pointer = (ulong) structureToPtr;
                    break;

                case TypeCode.Single:
                    *(float*) pointer = (float) structureToPtr;
                    break;

                case TypeCode.Double:
                    *(double*) pointer = (double) structureToPtr;
                    break;

                default:
                    // Assume the pointer is a custom structure.
                    // https://msdn.microsoft.com/en-us/library/vstudio/4ca6d5z7(v=vs.100).aspx for more dtails on PtrToStructure.
                    Marshal.StructureToPtr(structureToPtr, address, true);
                    break;
            }
        }
        #endregion
    }
}