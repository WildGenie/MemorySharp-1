using System;

namespace Binarysharp.MemoryManagement.MemoryInternal.Interfaces
{
    /// <summary>
    ///     Defines a function inside of a <see cref="Process" />.
    /// </summary>
    public interface IFunction
    {
        #region  Properties
        /// <summary>
        ///     The pointer to the function in memory.
        /// </summary>
        IntPtr Pointer { get; }

        /// <summary>
        ///     The name representing the function.
        /// </summary>
        string Name { get; }
        #endregion

        #region Methods
        /// <summary>
        ///     Get the functions delegate.
        /// </summary>
        /// <typeparam name="T">The delegate.</typeparam>
        /// <returns>A type.</returns>
        T GetDelegate<T>();
        #endregion
    }
}