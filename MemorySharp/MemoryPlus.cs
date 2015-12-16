using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.Common.Builders;
using Binarysharp.MemoryManagement.Edits;
using Binarysharp.MemoryManagement.Management;
using Binarysharp.MemoryManagement.Marshaling;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement
{
    /// <summary>
    ///     Class for editing memory of a process <see cref="MemoryPlus" /> is injected into.
    /// </summary>
    public class MemoryPlus : MemoryBase
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryPlus" /> class.
        /// </summary>
        /// <param name="proc">The process.</param>
        /// <remarks>
        ///     Created 2012-02-15
        /// </remarks>
        public MemoryPlus(Process proc) : base(proc)
        {
            Factories.Add(Detours = new DetourManager(this));
            Factories.Add(Hooks = new HookManager());
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets the manager for <see cref="Detour" /> objects.
        /// </summary>
        public DetourManager Detours { get; }

        /// <summary>
        ///     Gets the manager for <see cref="IHook" /> objects.
        /// </summary>
        public HookManager Hooks { get; }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Reads a specific number of bytes from memory.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns>An array of bytes.</returns>
        public override unsafe byte[] ReadBytes(IntPtr address, int count, bool isRelative = false)
        {
            if (isRelative)
            {
                address = GetAbsolute(address);
            }

            var ret = new byte[count];
            var ptr = (byte*) address;
            for (var i = 0; i < count; i++)
            {
                ret[i] = ptr[i];
            }
            return ret;
        }

        /// <summary>
        ///     Writes a set of bytes to memory.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns>
        ///     Number of bytes written.
        /// </returns>
        public override int WriteBytes(IntPtr address, byte[] bytes, bool isRelative = false)
        {
            using (
                new MemoryProtection(this, isRelative ? GetAbsolute(address) : address,
                    MarshalType<byte>.Size*bytes.Length))
                unsafe
                {
                    var ptr = (byte*) address;
                    for (var i = 0; i < bytes.Length; i++)
                    {
                        ptr[i] = bytes[i];
                    }
                }
            return bytes.Length;
        }

        /// <summary> Reads a value from the specified address in memory. </summary>
        /// <remarks> Created 3/24/2012. </remarks>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="address"> The address. </param>
        /// <param name="isRelative"> (optional) the relative. </param>
        /// <returns> . </returns>
        public override T Read<T>(IntPtr address, bool isRelative = false)
        {
            if (isRelative)
            {
                address = GetAbsolute(address);
            }
            return InternalRead<T>(address);
        }

        /// <summary> Writes a value specified to the address in memory. </summary>
        /// <remarks> Created 3/24/2012. </remarks>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="address"> The address. </param>
        /// <param name="value"> The value. </param>
        /// <param name="isRelative"> (optional) the relative. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public override void Write<T>(IntPtr address, T value, bool isRelative = false)
        {
            if (isRelative)
            {
                address = GetAbsolute(address);
            }

            Marshal.StructureToPtr(value, address, false);
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///     Static method to read addresses while <see cref="MemoryPlus" /> injected into this instances process.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Cannot retrieve a value at address 0</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [HandleProcessCorruptedStateExceptions]
        private static unsafe T InternalRead<T>(IntPtr address)
        {
            try
            {
                // TODO: Optimize this more. The boxing/unboxing required tends to slow this down.
                // It may be worth it to simply use memcpy to avoid it, but I doubt thats going to give any noticeable increase in speed.
                if (address == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Cannot retrieve a value at address 0");
                }

                object ret;
                switch (MarshalCache<T>.TypeCode)
                {
                    case TypeCode.Object:

                        if (MarshalCache<T>.RealType == typeof (IntPtr))
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
                        ret = Marshal.PtrToStructure(address, typeof (T));
                        break;
                    case TypeCode.Boolean:
                        ret = *(byte*) address != 0;
                        break;
                    case TypeCode.Char:
                        ret = *(char*) address;
                        break;
                    case TypeCode.SByte:
                        ret = *(sbyte*) address;
                        break;
                    case TypeCode.Byte:
                        ret = *(byte*) address;
                        break;
                    case TypeCode.Int16:
                        ret = *(short*) address;
                        break;
                    case TypeCode.UInt16:
                        ret = *(ushort*) address;
                        break;
                    case TypeCode.Int32:
                        ret = *(int*) address;
                        break;
                    case TypeCode.UInt32:
                        ret = *(uint*) address;
                        break;
                    case TypeCode.Int64:
                        ret = *(long*) address;
                        break;
                    case TypeCode.UInt64:
                        ret = *(ulong*) address;
                        break;
                    case TypeCode.Single:
                        ret = *(float*) address;
                        break;
                    case TypeCode.Double:
                        ret = *(double*) address;
                        break;
                    case TypeCode.Decimal:
                        // Probably safe to remove this. I'm unaware of anything that actually uses "decimal" that would require memory reading...
                        ret = *(decimal*) address;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return (T) ret;
            }
            catch (AccessViolationException ex)
            {
                Trace.WriteLine("Access Violation on " + address + " with type " + typeof (T).Name + Environment.NewLine +
                    ex);
                return default(T);
            }
        }
        #endregion
    }
}