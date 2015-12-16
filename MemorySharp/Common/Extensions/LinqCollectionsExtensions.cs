using System;
using System.Collections.Generic;

namespace Binarysharp.MemoryManagement.Common.Extensions
{
    /// <summary>
    ///     Static class containing linq extension methods.
    /// </summary>
    public static class LinqCollectionsExtensions
    {
        #region Public Methods
        /// <summary>
        ///     Performs a <code>ForEach</code> style linq operation for collections types other than <see cref="List{T}" />.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}" />.</param>
        /// <param name="action">The action to perform for each member.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        ///     Performs a <code>ForEach</code> style linq operation for collections types other than <see cref="List{T}" />.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="action">The action to perform for each member.</param>
        public static void ForEach<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
            Action<TKey, TValue> action)
        {
            foreach (var pair in dictionary)
            {
                action(pair.Key, pair.Value);
            }
        }
        #endregion
    }
}