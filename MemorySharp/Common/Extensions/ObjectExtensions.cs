using System;
using System.Linq;

namespace Binarysharp.MemoryManagement.Common.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="System.Object" /> data type.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Betweens the specified self.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="lower">The lower.</param>
        /// <param name="upper">The upper.</param>
        /// <returns>System.Boolean.</returns>
        public static bool Between<T>(this T self, T lower, T upper) where T : IComparable<T>
            => self.CompareTo(lower) >= 0 && self.CompareTo(upper) <= 0;

        /// <summary>
        ///     Ins the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="list">The list.</param>
        /// <returns>System.Boolean.</returns>
        public static bool In<T>(this T value, params T[] list) => list.Contains(value);

        /// <summary>
        ///     Nots the in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="list">The list.</param>
        /// <returns>System.Boolean.</returns>
        public static bool NotIn<T>(this T value, params T[] list) => !value.In(list);

        /// <summary>
        ///     Determines whether the specified value is default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.Boolean.</returns>
        public static bool IsDefault<T>(this T value) where T : struct => value.Equals(default(T));

        /// <summary>
        ///     Determines whether the specified value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Boolean.</returns>
        public static bool IsNull(this object value) => value == null;

        /// <summary>
        ///     Determines whether [is not null] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Boolean.</returns>
        public static bool IsNotNull(this object value) => !value.IsNull();

        /// <summary>
        ///     Determines whether the specified value is boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Boolean.</returns>
        public static bool IsBoolean(this object value) => !value.IsNull() && IsBoolean(value.ToString());

        /// <summary>
        ///     To the boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Boolean.</returns>
        public static bool ToBoolean(this object value) => !value.IsNull() && ToBoolean(value.ToString());

        /// <summary>
        ///     To the default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T ToDefault<T>(this T value) => default(T);
    }
}