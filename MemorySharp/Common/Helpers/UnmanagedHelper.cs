using System;
using System.Runtime.InteropServices;

namespace Binarysharp.MemoryManagement.Common.Helpers
{
    /// <summary>
    ///     Static class to help with unmanged code operations.
    /// </summary>
    public static class UnmanagedHelper
    {
        #region Public Methods
        /// <summary>
        ///     Creates a function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <param name="isRelative">if set to <c>true</c> [address is relative].</param>
        /// <returns></returns>
        /// <remarks>Created 2012-01-16 20:40 by Nesox.</remarks>
        public static T RegisterDelegate<T>(IntPtr address) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer(address, typeof (T)) as T;
        }

        /// <summary>
        ///     Gets the funtion pointer from a delegate.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        /// <remarks>Created 2012-01-16 20:40 by Nesox.</remarks>
        public static IntPtr GetDelegatePointer(Delegate d)
        {
            return Marshal.GetFunctionPointerForDelegate(d);
        }
        #endregion
    }
}