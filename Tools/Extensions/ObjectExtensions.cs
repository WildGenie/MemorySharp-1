/**************************************************************************************
 *	File:		 ObjectExtensions.cs
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
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using MemorySharp.Tools.Logger;

namespace MemorySharp.Tools.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="System.Object" /> data type.
    /// </summary>
    public static class ObjectExtensions
    {
        #region Methods
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static bool Between<T>(this T self, T lower, T upper) where T : IComparable<T>
        {
            return self.CompareTo(lower) >= 0 && self.CompareTo(upper) <= 0;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T>(this T value, params T[] list)
        {
            return list.Contains(value);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T value, params T[] list)
        {
            return !value.In(list);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefault<T>(this T value) where T : struct
        {
            return value.Equals(default(T));
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNull(this object value)
        {
            return value == null;
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object value)
        {
            return !value.IsNull();
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static Tuple<bool, DateTime> TryParseToDateTime(this object date)
        {
            return date.IsNull()
                ? new Tuple<bool, DateTime>(false, SettingsExtensions.DefaultDateTime)
                : date.ToString().TryParseToDateTime();
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object date)
        {
            return date.IsNull() ? SettingsExtensions.DefaultDateTime : ToDateTime(date.ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsBoolean(this object value)
        {
            return value.IsNull() ? false : IsBoolean(value.ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this object value)
        {
            return value.IsNull() ? false : ToBoolean(value.ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object value)
        {
            return value.IsNull() ? 0 : ToDecimal(value.ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInteger(this object value)
        {
            return value.IsNull() ? 0 : ToInteger(value.ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToLong(this object value)
        {
            return value.IsNull() ? 0 : ToLong(value.ToString());
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T To<T>(this IConvertible value)
        {
            try
            {
                return (T) Convert.ChangeType(value, typeof (T));
            }
            catch
            {
                return default(T);
            }
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type == null || !type.IsValueType || type == typeof (void) || type.ContainsGenericParameters)
            {
                return null;
            }

            if (!type.IsPrimitive && type.IsNotPublic) return null;
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                var ex =
                    new ArgumentException(
                        "{" + MethodBase.GetCurrentMethod() +
                        "} Error:\n\nThe Activator.CreateInstance method could not "
                        + "create a default instance of the supplied value type <" + type +
                        "> (Inner Exception message: \"" + e.Message
                        + "\")",
                        e);

                Log.Error(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertiesToSkip"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string DynamicToString<T>(this T obj, params string[] propertiesToSkip) where T : class
        {
            var properties =
                obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0}: {{ ", obj.GetType().Name);
            var propertyInfoArray = properties;
            foreach (var propertyInfo in propertyInfoArray)
            {
                if (propertiesToSkip.Contains(propertyInfo.Name))
                {
                    continue;
                }

                var value = propertyInfo.GetValue(obj, null);
                var defaultValueAttr = propertyInfo.GetCustomAttribute<DefaultValueAttribute>(true);
                var defaultValue = defaultValueAttr != null
                    ? defaultValueAttr.Value
                    : propertyInfo.PropertyType.GetDefaultValue();

                // Skip if we have the default value
                if (Equals(value, defaultValue))
                {
                    continue;
                }

                if (typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) &&
                    propertyInfo.PropertyType != typeof (string))
                {
                    var enumerableValue = value as IEnumerable;
                    if (enumerableValue == null)
                    {
                        continue;
                    }

                    stringBuilder.AppendFormat(
                        "{0}: [{1}], ",
                        propertyInfo.Name,
                        string.Join(", ",
                            enumerableValue.Cast<object>().Select(v => v == null ? "null" : string.Concat(v))));
                }
                else
                {
                    // Print 'null' if it is null
                    if (value == null)
                    {
                        value = "null";
                    }

                    stringBuilder.AppendFormat("{0}: {1}, ", propertyInfo.Name, value);
                }
            }

            stringBuilder.Replace(" ", string.Empty, stringBuilder.Length - 1, 1);
            stringBuilder.Replace(",", string.Empty, stringBuilder.Length - 1, 1);

            stringBuilder.Append(" }");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToDefault<T>(this T value)
        {
            return default(T);
        }
        #endregion
    }
}