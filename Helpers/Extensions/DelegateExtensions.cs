using System;
using System.Runtime.InteropServices;

namespace MemorySharp.Helpers.Extensions
{
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