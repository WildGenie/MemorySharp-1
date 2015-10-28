/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using MemorySharp.Internals;
using MemorySharp.Native;

namespace MemorySharp.Memory
{
    /// <summary>
    ///     Class representing an allocated memory in a remote process.
    /// </summary>
    public class RemoteAllocation : RemoteRegion, IDisposableState
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RemoteAllocation" /> class.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
        /// <param name="size">The size of the allocated memory.</param>
        /// <param name="protection">The protection of the allocated memory.</param>
        /// <param name="mustBeDisposed">The allocated memory will be released when the finalizer collects the object.</param>
        internal RemoteAllocation(MemoryBase memorySharp, int size,
            MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite,
            bool mustBeDisposed = true)
            : base(memorySharp, memorySharp.AllocateMemory(memorySharp.Handle, size, protection)
                )
        {
            // Set local vars
            MustBeDisposed = mustBeDisposed;
            IsDisposed = false;
        }

        /// <summary>
        ///     Gets a value indicating whether the element is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; set; }

        /// <summary>
        ///     Releases all resources used by the <see cref="RemoteAllocation" /> object.
        /// </summary>
        /// <remarks>Don't use the IDisposable pattern because the class is sealed.</remarks>
        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                // Set the flag to true
                IsDisposed = true;
                // Release the allocated memory
                Release();
                // Remove this object from the collection of allocated memory
                MemorySharp.Memory.Deallocate(this);
                // Remove the pointer
                BaseAddressField = IntPtr.Zero;
                // Avoid the finalizer 
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~RemoteAllocation()
        {
            if (MustBeDisposed)
                Dispose();
        }
    }
}