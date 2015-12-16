using System.Collections.Generic;

namespace Binarysharp.MemoryManagement.Common
{
    /// <summary>
    ///     Class to support a generic object dictionary.
    /// </summary>
    public class GenericDictionary
    {
        #region Fields, Private Properties
        /// <summary>
        ///     Gets the internal dictionary.
        /// </summary>
        /// <value>
        ///     The internal dictionary.
        /// </value>
        private Dictionary<int, object> InternalDictionary { get; } = new Dictionary<int, object>();
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     The dictionary's items as a <see cref="IReadOnlyDictionary{TKey,TValue}" />.
        /// </summary>
        public IReadOnlyDictionary<int, object> ItemsAsDictionary => InternalDictionary;
        #endregion

        #region Public Methods
        /// <summary>
        ///     Adds the specified key.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add<T>(int key, T value) where T : class
        {
            InternalDictionary.Add(key, value);
        }

        /// <summary>
        ///     Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(int key)
        {
            InternalDictionary.Remove(key);
        }

        /// <summary>
        ///     Gets the matching value from the specified key.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="ItemsAsDictionary" /> value matching the key given.</returns>
        public T GetValue<T>(int key) where T : class
        {
            return InternalDictionary[key] as T;
        }
        #endregion
    }
}