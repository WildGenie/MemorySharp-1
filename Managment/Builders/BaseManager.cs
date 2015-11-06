using System.Collections.Generic;

namespace Binarysharp.MemoryManagement.Managment.Builders
{
    /// <summary>
    ///     Class managing objects implementing <see cref="INamedElement" /> interface.
    /// </summary>
    public abstract class BaseManager<T>
    {
        /// <summary>
        ///     The collection of the elements.
        /// </summary>
        protected Dictionary<string, T> InternalItems { get; set; } = new Dictionary<string, T>();


        /// <summary>
        ///     The collection of the elements.
        /// </summary>
        public IEnumerable<T> Items => InternalItems.Values;

        /// <summary>
        ///     Disables all items in the manager.
        /// </summary>
        public abstract void DisableAll();

        /// <summary>
        ///     Enables all items in the manager.
        /// </summary>
        public abstract void EnableAll();

        /// <summary>
        ///     Removes an element by its name in the manager.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public abstract void Remove(string name);

        /// <summary>
        ///     Remove a given element.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        public abstract void Remove(T item);

        /// <summary>
        ///     Removes all the elements in the manager.
        /// </summary>
        public abstract void RemoveAll();
    }
}