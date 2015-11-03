using System;
using System.Linq;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.Memory;

namespace Binarysharp.MemoryManagement.Managment
{
    /// <summary>
    ///     A manager class to handle memory patches.
    ///     <remarks>All credits to Apoc.</remarks>
    /// </summary>
    public class PatchManager : Manager<Patch>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PatchManager" /> class.
        /// </summary>
        /// <param name="processMemory">The process memory.</param>
        public PatchManager(ProcessMemory processMemory)
        {
            ProcessMemory = processMemory;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PatchManager" /> class.
        /// </summary>
        /// <param name="processMemory">The <see cref="MemorySharp" /> Instance.</param>
        public PatchManager(MemorySharp processMemory)
        {
            MemorySharp = processMemory;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     The reference of the <see cref="ProcessMemory" /> object.
        ///     <remarks>This value is invalid if the manager was created for the <see cref="MemorySharp" /> class.</remarks>
        /// </summary>
        private ProcessMemory ProcessMemory { get; }

        /// <summary>
        ///     The reference to the <see cref="MemorySharp" /> class.
        ///     <remarks>This value is invalid if the manager was created for the <see cref="ProcessMemory" /> class.</remarks>
        /// </summary>
        private MemorySharp MemorySharp { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Applies all enabled patches in this manager via their Apply() method.
        /// </summary>
        public new void EnableAll()
        {
            foreach (var patch in InternalItems.Values.Where(patch => patch.IsEnabled && !patch.IsApplied))
            {
                patch.Enable();
            }
        }

        /// <summary>
        ///     Removes all the IMemoryOperations contained in this manager via their Remove() method.
        /// </summary>
        public new void RemoveAll()
        {
            foreach (var patch in InternalItems.Values.Where(patch => patch.IsApplied))
            {
                patch.Disable();
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Patch" /> at the specified address.
        /// </summary>
        /// <param name="address">The address to begin the patch.</param>
        /// <param name="patchWith">The bytes to be written as the patch.</param>
        /// <param name="name">The name of the patch.</param>
        /// <returns>A patch object that exposes the required methods to apply and remove the patch.</returns>
        public Patch Create(IntPtr address, byte[] patchWith, string name)
        {
            if (InternalItems.ContainsKey(name)) return InternalItems[name];
            var p = new Patch(address, patchWith, name, ProcessMemory);
            InternalItems.Add(name, p);
            return p;
        }

        /// <summary>
        ///     Creates a new <see cref="Patch" /> at the specified address, and applies it.
        /// </summary>
        /// <param name="address">The address to begin the patch.</param>
        /// <param name="patchWith">The bytes to be written as the patch.</param>
        /// <param name="name">The name of the patch.</param>
        /// <returns>A patch object that exposes the required methods to apply and remove the patch.</returns>
        public Patch CreateAndApply(IntPtr address, byte[] patchWith, string name)
        {
            var p = Create(address, patchWith, name);
            p?.Enable();
            return p;
        }

        #endregion
    }
}