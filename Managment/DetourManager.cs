using System;
using System.Diagnostics.CodeAnalysis;
using Binarysharp.MemoryManagement.Helpers;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Memory.Local;

namespace Binarysharp.MemoryManagement.Managment
{
    /// <summary>
    ///     A manager class to handle function detours, and hooks.
    ///     <remarks>All credits to Apoc.</remarks>
    /// </summary>
    [SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
    public class DetourManager : Manager<Detour>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DetourManager" /> class.
        /// </summary>
        /// <param name="memory">The <see cref="ProcessMemory" /> Instance.</param>
        public DetourManager(ProcessMemory memory)
        {
            ProcessMemory = memory;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     The reference of the <see cref="ProcessMemory" /> object.
        /// </summary>
        public ProcessMemory ProcessMemory { get; }

        #endregion

        #region Methods

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

            var d = new Detour(target, newTarget, name, ProcessMemory);
            InternalItems.Add(name, d);
            return d;
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
            var ret = Create(target, newTarget, name);
            ret?.Enable();
            return ret;
        }

        #endregion
    }
}