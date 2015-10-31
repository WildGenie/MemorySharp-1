using System;
using Binarysharp.MemoryManagement.MemoryExternal.Assembly.CallingConvention;

namespace Binarysharp.MemoryManagement.MemoryExternal.Assembly
{
    /// <summary>
    ///     A class to store properties needed to execute code in another process.
    /// </summary>
    public class RemoteCall
    {
        #region Fields
        /// <summary>
        ///     The address where the function to be executed is located in memory.
        /// </summary>
        public IntPtr Address;

        /// <summary>
        ///     The functions <see cref="CallingConventions" />.
        /// </summary>
        public CallingConventions CallingConvention;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="RemoteCall" /> class.
        /// </summary>
        /// <param name="address">The address where the function is located.</param>
        /// <param name="callingConvention">The calling convention of the function.</param>
        public RemoteCall(IntPtr address, CallingConventions callingConvention)
        {
            Address = address;
            CallingConvention = callingConvention;
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Creates a new <see cref="RemoteCall" /> Instance.
        /// </summary>
        /// <param name="address">The address where the function is located.</param>
        /// <param name="callingConvention">The calling convention of the function.</param>
        /// <returns></returns>
        public static RemoteCall Create(IntPtr address, CallingConventions callingConvention)
        {
            return new RemoteCall(address, callingConvention);
        }
        #endregion
    }
}