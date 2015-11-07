using System;
using System.Collections.Generic;
using System.Linq;
using Binarysharp.MemoryManagement.Managment.Builders;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.RemoteProcess.Memory
{
    /// <summary>
    ///     Class providing tools for manipulating memory space.
    /// </summary>
    public class MemoryFactory : IFactory
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The list containing all allocated memory.
        /// </summary>
        protected readonly List<RemoteAllocation> InternalRemoteAllocations;

        /// <summary>
        ///     The reference of the <see cref="MemorySharp" /> object.
        /// </summary>
        protected readonly MemorySharp MemorySharp;
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryFactory" /> class.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
        internal MemoryFactory(MemorySharp memorySharp)
        {
            // Save the parameter
            MemorySharp = memorySharp;
            // Create a list containing all allocated memory
            InternalRemoteAllocations = new List<RemoteAllocation>();
        }

        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~MemoryFactory()
        {
            Dispose();
        }
        #endregion

        #region Public Properties, Indexers
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
#if x64
                var adresseTo = new IntPtr(0x7fffffffffffffff);
#else
                var adresseTo = new IntPtr(0x7fffffff);
#endif
                return
                    MemorySharp.NativeDriver.MemoryCore.QueryInformationMemory(MemorySharp.SafeHandle, IntPtr.Zero,
                        adresseTo).Select(page => new RemoteRegion(MemorySharp, page.BaseAddress));
            }
        }
        #endregion

        #region Interface Implementations
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

        /// <summary>
        ///     Allocates a region of memory within the virtual address space of the remote process.
        /// </summary>
        /// <param name="size">The size of the memory to allocate.</param>
        /// <param name="protection">The protection of the memory to allocate.</param>
        /// <param name="mustBeDisposed">The allocated memory will be released when the finalizer collects the object.</param>
        /// <returns>A new instance of the <see cref="RemoteAllocation" /> class.</returns>
        public RemoteAllocation Allocate(int size,
                                         MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite,
                                         bool mustBeDisposed = true)
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
            // Remove the element from the allocated memory list
            if (InternalRemoteAllocations.Contains(allocation))
                InternalRemoteAllocations.Remove(allocation);
        }
    }
}