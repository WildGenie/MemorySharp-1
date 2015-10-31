using System;
using System.Runtime.InteropServices;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;

namespace Binarysharp.MemoryManagement.MemoryInternal.Processes
{
    /// <summary>
    ///     A class for managing process functions.
    /// </summary>
    public class ProcessFunction : IFunction
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessFunction" /> class.
        /// </summary>
        /// >
        /// <param name="address">The address the function is located at in memory.</param>
        /// <param name="name">The name representing the function.</param>
        public ProcessFunction(string name, IntPtr address)
        {
            Name = name;
            Pointer = address;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The pointer to the function in memory.
        /// </summary>
        public IntPtr Pointer { get; }

        /// <summary>
        ///     The name representing the function
        /// </summary>
        public string Name { get; }
        #endregion

        #region  Interface members
        /// <summary>
        ///     Registers a function into a delegate. Note: The delegate must provide a proper function signature!
        /// </summary>
        public T GetDelegate<T>()
        {
            return Marshal.GetDelegateForFunctionPointer<T>(Pointer);
        }
        #endregion
    }
}