using System;
using MemorySharp.Helpers;
using MemorySharp.Internals.Exceptions;
using MemorySharp.Memory;

namespace MemorySharp.Internals
{
    /// <summary>
    ///     A manager class to handle function detours, and hooks.
    /// </summary>
    public class DetourManager : Manager<MemoryDetour>
    {
        #region Constructors
        internal DetourManager(InternalMemorySharp memory)
        {
            Memory = memory;
        }
        #endregion

        #region  Properties
        private InternalMemorySharp Memory { get; }
        #endregion

        #region Methods
        /// <summary>
        ///     Creates a new MemoryDetour.
        /// </summary>
        /// <param name="target">
        ///     The original function to detour. (This delegate should already be registered via
        ///     Magic.RegisterDelegate)
        /// </param>
        /// <param name="newTarget">The new function to be called. (This delegate should NOT be registered!)</param>
        /// <param name="name">The name of the detour.</param>
        /// <returns>
        ///     A <see cref="MemoryDetour" /> object containing the required methods to apply, remove, and call the original
        ///     function.
        /// </returns>
        public MemoryDetour Create(Delegate target, Delegate newTarget, string name)
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
            if (!AttributeHelper.HasUFPAttribute(target))
            {
                throw new MissingAttributeException(
                    "The target delegate does not have the proper UnmanagedFunctionPointer attribute!");
            }
            if (!AttributeHelper.HasUFPAttribute(newTarget))
            {
                throw new MissingAttributeException(
                    "The new target delegate does not have the proper UnmanagedFunctionPointer attribute!");
            }

            if (InternalItems.ContainsKey(name))
            {
                throw new ArgumentException($"The {name} detour already exists!", nameof(name));
            }

            var d = new MemoryDetour(target, newTarget, name, Memory);
            InternalItems.Add(name, d);
            return d;
        }

        /// <summary>
        ///     Creates and applies new MemoryDetour.
        /// </summary>
        /// <param name="target">
        ///     The original function to detour. (This delegate should already be registered via
        ///     Magic.RegisterDelegate)
        /// </param>
        /// <param name="newTarget">The new function to be called. (This delegate should NOT be registered!)</param>
        /// <param name="name">The name of the detour.</param>
        /// <returns>
        ///     A <see cref="MemoryDetour" /> object containing the required methods to apply, remove, and call the original
        ///     function.
        /// </returns>
        public MemoryDetour CreateAndApply(Delegate target, Delegate newTarget, string name)
        {
            var ret = Create(target, newTarget, name);
            ret?.Enable();
            return ret;
        }
        #endregion
    }
}