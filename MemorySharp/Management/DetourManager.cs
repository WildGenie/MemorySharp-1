using System;
using Binarysharp.MemoryManagement.Common.Builders;
using Binarysharp.MemoryManagement.Common.Helpers;
using Binarysharp.MemoryManagement.Edits;

namespace Binarysharp.MemoryManagement.Management
{
    /// <summary>
    ///     A manager class to handle function detours, and hooks.
    ///     <remarks>All credits to Apoc.</remarks>
    /// </summary>
    public class DetourManager : Manager<Detour>, IFactory
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="DetourManager" /> class.
        /// </summary>
        public DetourManager(MemoryPlus memoryPlus)
        {
            MemoryPlus = memoryPlus;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets or sets the instance reference for the <see cref="MemoryManagement.MemoryPlus" /> class.
        /// </summary>
        /// <value>
        ///     The instance reference for the <see cref="MemoryManagement.MemoryPlus" /> class.
        /// </value>
        public MemoryPlus MemoryPlus { get; set; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Releases all resources used by the <see cref="DetourManager" /> object.
        /// </summary>
        public void Dispose()
        {
            RemoveAll();
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Creates a new Detour.
        /// </summary>
        /// <param name="target">
        ///     The original function to detour. (This delegate should already be registered via
        ///     Magic.RegisterDelegate)
        /// </param>
        /// <param name="newTarget">The new function to be called. (This delegate should NOT be registered!)</param>
        /// <param name="name">The name of the detour.</param>
        /// <returns>
        ///     A <see cref="Detour" /> object containing the required methods to apply, remove, and call the original
        ///     function.
        /// </returns>
        public Detour Create(Delegate target, Delegate newTarget, string name)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (newTarget == null)
            {
                throw new ArgumentNullException(nameof(newTarget));
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (!AttributesHelper.HasUfpAttribute(target))
            {
                throw new Exception(
                    "The target delegate does not have the proper UnmanagedFunctionPointer attribute!");
            }
            if (!AttributesHelper.HasUfpAttribute(newTarget))
            {
                throw new Exception(
                    "The new target delegate does not have the proper UnmanagedFunctionPointer attribute!");
            }

            if (InternalItems.ContainsKey(name))
            {
                throw new ArgumentException($"The {name} detour already exists!", nameof(name));
            }
            InternalItems[name] = new Detour(target, newTarget, name, MemoryPlus);
            return InternalItems[name];
        }

        /// <summary>
        ///     Creates and applies new Detour.
        /// </summary>
        /// <param name="target">
        ///     The original function to detour. (This delegate should already be registered via
        ///     Magic.RegisterDelegate)
        /// </param>
        /// <param name="newTarget">The new function to be called. (This delegate should NOT be registered!)</param>
        /// <param name="name">The name of the detour.</param>
        /// <returns>
        ///     A <see cref="Detour" /> object containing the required methods to apply, remove, and call the original
        ///     function.
        /// </returns>
        public Detour CreateAndApply(Delegate target, Delegate newTarget, string name)
        {
            Create(target, newTarget, name);
            InternalItems[name].Enable();
            return InternalItems[name];
        }
        #endregion
    }
}