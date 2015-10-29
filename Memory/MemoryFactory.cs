/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using MemorySharp.Internals.Interfaces;
using MemorySharp.Native;

namespace MemorySharp.Memory
{
    /// <summary>
    ///     Class providing tools for manipulating memory space.
    /// </summary>
    public class MemoryFactory : IFactory
    {
        #region  Fields
        /// <summary>
        ///     The list containing all allocated memory.
        /// </summary>
        protected readonly List<RemoteAllocation> InternalRemoteAllocations;

        /// <summary>
        ///     The reference of the <see cref="MemorySharp" /> object.
        /// </summary>
        protected readonly MemoryBase MemorySharp;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryFactory" /> class.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
        internal MemoryFactory(MemoryBase memorySharp)
        {
            // Save the parameter
            MemorySharp = memorySharp;
            // Create a list containing all allocated memory
            InternalRemoteAllocations = new List<RemoteAllocation>();
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     A collection containing all allocated memory in the remote process.
        /// </summary>
        public IEnumerable<RemoteAllocation> RemoteAllocations => InternalRemoteAllocations.AsReadOnly();

        /// <summary>
        ///     Gets all blocks of memory allocated in the remote process.
        /// </summary>
        public IEnumerable<RemoteRegion> Regions
        {
            // TODO: Old 64-bit check
            get
            {
                var address = IntPtr.Size == 8 ? new IntPtr(0x7fffffffffffffff) : new IntPtr(0x7fffffff);
                return
                    MemorySharp.QueryInformationMemory(IntPtr.Zero,
                        address).Select(page => new RemoteRegion(MemorySharp, page.BaseAddress));
            }
        }
        #endregion

        #region  Interface members
        /// <summary>
        ///     Releases all resources used by the <see cref="MemoryFactory" /> object.
        /// </summary>
        public virtual void Dispose()
        {
            // Release all allocated memories which must be disposed
            foreach (var allocatedMemory in InternalRemoteAllocations.Where(m => m.MustBeDisposed).ToArray())
            {
                allocatedMemory.Dispose();
            }
            // Avoid the finalizer
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Allocates a region of memory within the virtual address space of the remote process.
        /// </summary>
        /// <param name="size">The size of the memory to allocate.</param>
        /// <param name="protection">The protection of the memory to allocate.</param>
        /// <param name="mustBeDisposed">The allocated memory will be released when the finalizer collects the object.</param>
        /// <returns>A new instance of the <see cref="RemoteAllocation" /> class.</returns>
        public RemoteAllocation Allocate(int size,
            MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite, bool mustBeDisposed = true)
        {
            // Allocate a memory space
            var memory = new RemoteAllocation(MemorySharp, size, protection, mustBeDisposed);
            // Add the memory in the list
            InternalRemoteAllocations.Add(memory);
            return memory;
        }

        /// <summary>
        ///     Deallocates a region of memory previously allocated within the virtual address space of the remote process.
        /// </summary>
        /// <param name="allocation">The allocated memory to release.</param>
        public void Deallocate(RemoteAllocation allocation)
        {
            // Dispose the element
            if (!allocation.IsDisposed)
                allocation.Dispose();
            // Disable the element from the allocated memory list
            if (InternalRemoteAllocations.Contains(allocation))
                InternalRemoteAllocations.Remove(allocation);
        }
        #endregion

        #region Misc
        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~MemoryFactory()
        {
            Dispose();
        }
        #endregion
    }
}