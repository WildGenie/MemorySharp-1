/**************************************************************************************
 *	File:		 TypeExtensions.cs
 *	Description: Extension methods for the System.Object data type.
 *
 *
 *	Author:		 infloper@gmail.com
 *	Created:	 6/14/2014 5:52:30 PM
 *	CLR ver:	 4.0.30319.18444
 *
 **************************************************************************************
 * Changes history.
 **************************************************************************************
 * Date:		Author:				  Description:
 * --------		--------------------
 **************************************************************************************/

using System;
using System.Linq;

namespace Binarysharp.MemoryManagement.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="System.Object" /> data type.
    /// </summary>
    public static class TypeExtensions
    {
        #region Methods

        public static bool Between<T>(this T self, T lower, T upper) where T : IComparable<T>
            => self.CompareTo(lower) >= 0 && self.CompareTo(upper) <= 0;

        public static bool In<T>(this T value, params T[] list) => list.Contains(value);
        public static bool NotIn<T>(this T value, params T[] list) => !value.In(list);
        public static bool IsDefault<T>(this T value) where T : struct => value.Equals(default(T));
        public static bool IsNull(this object value) => value == null;
        public static bool IsNotNull(this object value) => !value.IsNull();
        public static bool IsBoolean(this object value) => !value.IsNull() && IsBoolean(value.ToString());
        public static bool ToBoolean(this object value) => !value.IsNull() && ToBoolean(value.ToString());
        public static T ToDefault<T>(this T value) => default(T);

        #endregion
    }
}