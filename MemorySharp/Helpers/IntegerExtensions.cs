using System;

namespace Binarysharp.MemoryManagement.Helpers
{
    /// <summary>
    ///     Class containing extension methods for <see cref="int" /> types.
    /// </summary>
    public static class IntegerExtensions
    {
        #region Public Methods
        /// <summary>
        ///     Adds the specified integer to the module address supplied.
        /// </summary>
        /// <param name="addressToRebase">The address to rebase to the module address.</param>
        /// <param name="moduleAddress">The address of the module being rebased to.</param>
        /// <returns></returns>
        public static IntPtr Add(this int addressToRebase, IntPtr moduleAddress) => moduleAddress.Add(addressToRebase);
        #endregion
    }
}