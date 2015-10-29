using System;
using System.Runtime.InteropServices;

namespace MemorySharp.Helpers.Extensions
{
    /// <summary>
    ///     A class providing extension methods for <see cref="Delegate" /> Instance's.
    ///     <remarks>
    ///         eturns>Unfinshed documentation. Most credits go to: "Jeffora"'s extememory project.
    ///         https://github.com/jeffora/extemory
    ///     </remarks>
    /// </summary>
    public static class DelegateExtensions
    {
        #region Methods
        public static IntPtr ToFunctionPointer(this Delegate del)
        {
            return Marshal.GetFunctionPointerForDelegate(del);
        }
        #endregion
    }
}