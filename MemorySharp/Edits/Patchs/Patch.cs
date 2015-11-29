using System;
using System.Linq;
using Binarysharp.MemoryManagement.Internals;

namespace Binarysharp.MemoryManagement.Edits.Patchs
{
    /// <summary>
    ///     Class to maange memory patches.
    /// </summary>
    public class Patch : INamedElement, IFactory
    {
        #region Fields, Private Properties
        private MemoryPlus MemoryPlus { get; }
        private MemorySharp MemorySharp { get; }
        private bool IsInternalMemory { get; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="Patch" /> class.
        /// </summary>
        /// <param name="address">The address where the patch is located in memorySharp.</param>
        /// <param name="patchWith">The bytes to be written.</param>
        /// <param name="name">The name that represents the current instance.</param>
        /// <param name="memory">The <see cref="MemoryManagement.MemorySharp" /> reference.</param>
        public Patch(IntPtr address, byte[] patchWith, string name, MemorySharp memory)
        {
            Name = name;
            MemorySharp = memory;
            Address = address;
            PatchBytes = patchWith;
            OriginalBytes = MemorySharp.ReadBytes(address, patchWith.Length);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Patch" /> class.
        /// </summary>
        /// <param name="address">The address where the patch is located in Memory.</param>
        /// <param name="patchWith">The bytes to be written.</param>
        /// <param name="name">The name that represents the current instance.</param>
        /// <param name="memoryPlus">The <see cref="MemoryManagement.MemoryPlus" /> reference.</param>
        public Patch(IntPtr address, byte[] patchWith, string name, MemoryPlus memoryPlus)
        {
            Name = name;
            MemoryPlus = memoryPlus;
            Address = address;
            PatchBytes = patchWith;
            OriginalBytes = MemorySharp.ReadBytes(address, patchWith.Length);
            IsInternalMemory = true;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets a value indicating whether this memorySharp patch is applied.
        /// </summary>
        /// <value><c>true</c> if this memorySharp patch is applied; otherwise, <c>false</c>.</value>
        public bool IsApplied { get; private set; }


        /// <summary>
        ///     Gets the address.
        /// </summary>
        /// <value>The address.</value>
        public IntPtr Address { get; }

        /// <summary>
        ///     Gets the original bytes.
        /// </summary>
        /// <value>The original bytes.</value>
        public byte[] OriginalBytes { get; }

        /// <summary>
        ///     Gets the patch bytes.
        /// </summary>
        /// <value>The patch bytes.</value>
        public byte[] PatchBytes { get; }

        /// <summary>
        ///     The name of the memorySharp patch.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }


        /// <summary>
        ///     Gets a value indicating whether the element is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; set; } = true;

        /// <summary>
        ///     Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return CheckIfEnabled(); }
            set { IsApplied = value; }
        }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Disables the memorySharp patch.
        /// </summary>
        public void Disable()
        {
            try
            {
                MemorySharp.WriteBytes(Address, OriginalBytes);
                IsApplied = false;
            }
            catch
            {
                IsApplied = false;
                // Ignored.
            }
        }

        /// <summary>
        ///     Enables the memorySharp patch.
        /// </summary>
        public void Enable()
        {
            try
            {
                MemorySharp.WriteBytes(Address, PatchBytes);
                IsApplied = true;
            }
            catch
            {
                IsApplied = false;
                // Ignored
            }
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (!IsApplied || !MustBeDisposed) return;
            Disable();
            IsDisposed = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     States if the Patch is enabled.
        /// </summary>
        /// <returns>
        ///     <c>true</c>
        ///     if this instance is enabled; otherwise, <c>false</c>
        ///     .
        /// </returns>
        public bool CheckIfEnabled()
        {
            return IsInternalMemory
                       ? MemoryPlus.ReadBytes(Address, PatchBytes.Length).SequenceEqual(PatchBytes)
                       : MemorySharp.ReadBytes(Address, PatchBytes.Length).SequenceEqual(PatchBytes);
        }
        #endregion
    }
}