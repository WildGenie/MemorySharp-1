/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Binarysharp.MemoryManagement.Marshaling;
using Binarysharp.MemoryManagement.Memory;

namespace Binarysharp.MemoryManagement
{
    /// <summary>
    ///     Class for memory editing a remote process.
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
    public class MemorySharp : MemoryBase
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemorySharp" /> class.
        /// </summary>
        /// <param name="proc">The process to open all rights to.</param>
        public MemorySharp(Process proc) : base(proc)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Created 2012-02-15</remarks>
        public override void Dispose()
        {
            Factories.ForEach(factory => factory.Dispose());
            base.Dispose();
        }

        /// <summary>
        ///     Reads a specific number of bytes from memory.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public override byte[] ReadBytes(IntPtr address, int count, bool isRelative = false)
        {
            return MemoryCore.ReadBytes(Handle, isRelative ? GetAbsolute(address) : address, count);
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
            // Change the protection of the memory to allow writable
            using (
                new MemoryProtection(this, isRelative ? GetAbsolute(address) : address,
                    MarshalType<byte>.Size*bytes.Length))
            {
                // Write the byte array
                MemoryCore.WriteBytes(Handle, isRelative ? GetAbsolute(address) : address, bytes);
            }
            return bytes.Length;
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A value.</returns>
        public override T Read<T>(IntPtr address, bool isRelative = true)
        {
            return MarshalType<T>.ByteArrayToObject(ReadBytes(address, MarshalType<T>.Size, isRelative));
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public override void Write<T>(IntPtr address, T value, bool isRelative = false)
        {
            WriteBytes(address, MarshalType<T>.ObjectToByteArray(value), isRelative);
        }
        #endregion
    }
}