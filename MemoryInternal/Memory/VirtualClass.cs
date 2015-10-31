using System;
using System.Runtime.InteropServices;

namespace Binarysharp.MemoryManagement.MemoryInternal.Memory
{
    /// <summary>
    ///     A virtual class object with various methods/values related to virtual classes.
    /// </summary>
    public class VirtualClass
    {
        #region Constructors

        //private IMemory mem;
        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualClass" /> class.
        /// </summary>
        /// <param name="address">The address the class is located at in memory.</param>
        public VirtualClass(IntPtr address)
        {
            ClassAddress = address;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The address the class is located at in memory.
        /// </summary>
        public IntPtr ClassAddress { get; }
        #endregion

        #region Methods
        /// <summary>
        ///     Gets a virtual function delegate at the given index.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="functionIndex">The index the function is located at in memory from the base address.</param>
        /// <returns>A delegate.</returns>
        public T GetVirtualFunction<T>(int functionIndex) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer<T>(GetObjectVtableFunctionPointer(ClassAddress, functionIndex));
        }

        /// <summary>
        ///     Gets a virtual function pointer.
        /// </summary>
        /// <param name="objectBase">The base address.</param>
        /// <param name="functionIndex">The index the function is located at in memory from the base address.</param>
        /// <returns>A <code>IntPtr</code>.</returns>
        public IntPtr GetObjectVtableFunctionPointer(IntPtr objectBase, int functionIndex)
        {
            return objectBase.GetVTableEntry(functionIndex);
        }
        #endregion
    }

    /// <summary>
    ///     A small helper class for vtable related operations and values.
    /// </summary>
    public static class VTableHelper
    {
        #region Methods
        /// <summary>
        ///     Gets a virtual function pointer.
        /// </summary>
        /// <param name="addr">The base address.</param>
        /// <param name="index">The index the function is located at in memory from the base address.</param>
        /// <returns>A <code>IntPtr</code>.</returns>
        public static unsafe IntPtr GetVTableEntry(this IntPtr addr, int index)
        {
            var pAddr = (void***) addr.ToPointer();
            return new IntPtr((*pAddr)[index]);
        }
        #endregion
    }
}