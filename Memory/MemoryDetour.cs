using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using MemorySharp.Internals;

namespace MemorySharp.Memory
{
    /// <summary>
    ///     A  class to handle function detours.
    /// </summary>
    public class MemoryDetour : INamedElement
    {
        #region  Fields
        [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")] private readonly IntPtr _hook;

        /// <summary>
        ///     This var is not used within the detour itself. It is only here
        ///     to keep a reference, to avoid the GC from collecting the delegate instance!
        /// </summary>
        [SuppressMessage("ReSharper", "NotAccessedField.Local")] private readonly Delegate _hookDelegate;
        #endregion

        #region Constructors
        internal MemoryDetour(Delegate target, Delegate hook, string name, MemoryBase memory)
        {
            Memory = memory;
            Name = name;
            TargetDelegate = target;
            Target = Marshal.GetFunctionPointerForDelegate(target);
            _hookDelegate = hook;
            _hook = Marshal.GetFunctionPointerForDelegate(hook);

            // Store the orginal bytes
            Orginal = new List<byte>();
            Orginal.AddRange(memory.ReadBytes(Target, 6));

            // Setup the detour bytes
            New = new List<byte> {0x68};
            var bytes = BitConverter.GetBytes(_hook.ToInt32());
            New.AddRange(bytes);
            New.Add(0xC3);
            IsDisposed = false;
            MustBeDisposed = true;
        }
        #endregion

        #region  Properties
        private MemoryBase Memory { get; }
        private List<byte> New { get; }
        private List<byte> Orginal { get; }
        private IntPtr Target { get; }
        private Delegate TargetDelegate { get; }

        /// <summary>
        ///     Gets a value indicating whether the element is disposed.
        /// </summary>
        public bool IsDisposed { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; }

        /// <summary>
        ///     States if the element is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     The name of the element.
        /// </summary>
        public string Name { get; }
        #endregion

        #region  Interface members
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (IsEnabled && MustBeDisposed)
            {
                Disable();
                IsDisposed = true;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Removes this MemoryDetour from memory. (Reverts the bytes back to their originals.)
        /// </summary>
        /// <returns></returns>
        public void Disable()
        {
            if (Memory.WriteBytes(Target, Orginal.ToArray()) == Orginal.Count)
            {
                IsEnabled = false;
            }
        }

        /// <summary>
        ///     Applies this MemoryDetour to memory. (Writes new bytes to memory)
        /// </summary>
        /// <returns></returns>
        public void Enable()
        {
            if (Memory.WriteBytes(Target, New.ToArray()) == New.Count)
            {
                IsEnabled = true;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Calls the original function, and returns a return value.
        /// </summary>
        /// <param name="args">
        ///     The arguments to pass. If it is a 'void' argument list,
        ///     you MUST pass 'null'.
        /// </param>
        /// <returns>An object containing the original functions return value.</returns>
        public object CallOriginal(params object[] args)
        {
            Disable();
            var ret = TargetDelegate.DynamicInvoke(args);
            Enable();
            return ret;
        }
        #endregion

        #region Misc
        /// <summary>
        ///     Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage
        ///     collection.
        /// </summary>
        ~MemoryDetour()
        {
            Dispose();
        }
        #endregion
    }
}