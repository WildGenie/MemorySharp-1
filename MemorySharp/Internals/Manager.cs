/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.Collections.Generic;
using System.Linq;
using Binarysharp.MemoryManagement.Logging.Interfaces;

namespace Binarysharp.MemoryManagement.Internals
{
    /// <summary>
    ///     Class managing objects implementing <see cref="INamedElement" /> interface.
    /// </summary>
    public abstract class Manager<T> where T : INamedElement
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The collection of the elements (writable).
        /// </summary>
        protected Dictionary<string, T> InternalItems = new Dictionary<string, T>();
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="Manager{T}" /> class.
        /// </summary>
        /// <param name="logger">The <see cref="IManagedLog" /> instance to use..</param>
        protected Manager(IManagedLog logger)
        {
            Logger = logger;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        /// </summary>
        public IManagedLog Logger { get; set; }

        /// <summary>
        ///     The collection of the elements.
        /// </summary>
        public IReadOnlyDictionary<string, T> ItemsAsDictionary => InternalItems;

        /// <summary>
        ///     Gets the collection of <see cref="INamedElement" /> instances as a <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <value>
        ///     The collection of <see cref="INamedElement" /> instances as a <see cref="IEnumerable{T}" />.
        /// </value>
        public IEnumerable<T> Items => InternalItems.Values;
        #endregion

        #region Public Methods
        /// <summary>
        ///     Disables all items in the manager.
        /// </summary>
        public void DisableAll()
        {
            InternalItems.Values.ToList().ForEach(i =>
                                                  {
                                                      i.Disable();
                                                      Logger.LogInfo(i.Name + " was disabled.");
                                                  });
        }

        /// <summary>
        ///     Enables all items in the manager.
        /// </summary>
        public void EnableAll()
        {
            InternalItems.Values.ToList().ForEach(i =>
                                                  {
                                                      i.Enable();
                                                      Logger.LogInfo(i.Name + " was Enabled.");
                                                  });
        }

        /// <summary>
        ///     Removes an element by its name in the manager.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            // Check if the element exists in the dictionary
            if (!InternalItems.ContainsKey(name)) return;
            try
            {
                // Dispose the element
                InternalItems[name].Dispose();
            }
            finally
            {
                // Remove the element from the dictionary
                InternalItems.Remove(name);
            }
        }

        /// <summary>
        ///     Remove a given element.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        public void Remove(T item)
        {
            Remove(item.Name);
        }

        /// <summary>
        ///     Removes all the elements in the manager.
        /// </summary>
        public void RemoveAll()
        {
            InternalItems.Values.ToList().ForEach(i => i.Dispose());
            InternalItems.Clear();
        }
        #endregion
    }
}