using System;
using System.Collections.Generic;

namespace MemorySharp.Tools
{
    /// <summary>
    ///     A generic object cache class.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    public class ObjectCache<T>
    {
        #region  Fields
        private readonly Lazy<Dictionary<string, T>> _cache = new Lazy<Dictionary<string, T>>(
            () => new Dictionary<string, T>(),
            true);

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<ObjectCache<T>> _instance = new Lazy<ObjectCache<T>>(() => new ObjectCache<T>(),
            true);
        #endregion

        #region  Properties
        /// <summary>
        ///     Get or set a value in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks>The key will be set with the type default if trying to get the value before it was set</remarks>
        public T this[string key]
        {
            get { return _cache.Value.ContainsKey(key) ? _cache.Value[key] : default(T); }
            set
            {
                if (!_cache.Value.ContainsKey(key))
                {
                    _cache.Value.Add(key, value);
                    return;
                }

                _cache.Value[key] = value;
            }
        }

        /// <summary>
        ///     Gets the singleton instance of the <see cref="ObjectCache{T}" /> class
        /// </summary>
        public static ObjectCache<T> Instance => _instance.Value;
        #endregion

        #region Methods
        /// <summary>
        ///     Clears all values from the cache
        /// </summary>
        public void Clear()
        {
            _cache.Value.Clear();
        }
        #endregion
    }
}