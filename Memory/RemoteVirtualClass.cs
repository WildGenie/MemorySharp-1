using System;
using System.Runtime.InteropServices;

namespace MemorySharp.Memory
{
    /// <summary>
    ///     A virtual class helper. Thanks to 'aganonki' @ unknowncheats.me and github.
    /// </summary>
    public class RemoteVirtualClass
    {
        #region Constructors
        //private IMemory mem;
        /// <summary>
        ///     Initializes a new instance of the <see cref="RemoteVirtualClass" /> class.
        /// </summary>
        /// <param name="address"></param>
        public RemoteVirtualClass(IntPtr address)
        {
            ClassAddress = address;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The address where the class is located in memory.
        /// </summary>
        public IntPtr ClassAddress { get; }
        #endregion

        #region Methods
        /// <summary>
        ///     Gets a virtual function from the class.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="functionIndex">The index the function is located at.</param>
        /// <returns>A type.</returns>
        public T GetVirtualFunction<T>(int functionIndex) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer<T>(GetObjectVtableFunction(ClassAddress, functionIndex));
        }

        /// <summary>
        ///     Gets a virtual function pointer from the given address/index.
        /// </summary>
        /// <param name="address">The address the class is located in memory.</param>
        /// <param name="functionIndex">The index the function is located at.</param>
        /// <returns>A <see cref="IntPtr" />.</returns>
        public IntPtr GetObjectVtableFunction(IntPtr address, int functionIndex)
        {
            return address.VTable(functionIndex);
        }
        #endregion
    }

    public static class VTableHelper
    {
        #region Methods
        public static unsafe IntPtr VTable(this IntPtr addr, int index)
        {
            var pAddr = (void***) addr.ToPointer();
            return new IntPtr((*pAddr)[index]);
        }
        #endregion
    }
}