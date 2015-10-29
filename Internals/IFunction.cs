using System;
using System.Runtime.InteropServices;

namespace MemorySharp.Internals
{
    /// <summary>
    ///     A class for managing process functions.
    /// </summary>
    public class Function : IProcessFunction
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="Function" /> class.
        /// </summary>
        /// >
        /// <param name="address">The address the function is located at in memory.</param>
        /// <param name="name">The name representing the function.</param>
        public Function(IntPtr address, string name)
        {
            Pointer = address;
            Name = name;
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