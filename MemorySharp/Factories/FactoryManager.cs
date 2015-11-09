using System;
using System.Linq;
using ToolsSharp.Managment.BaseClasses;
using ToolsSharp.Managment.Interfaces;

namespace Binarysharp.MemoryManagement.Factories
{
    /// <summary>
    ///     Class for managing of <see cref="IFactory" /> instances.
    /// </summary>
    public class FactoryManager : BaseManager<IFactory>, IDisposable
    {
        #region Fields, Private Properties
        /// <summary>
        ///     Gets the memory sharp reference for this instance.
        /// </summary>
        /// <value>The memory sharp reference for this instance.</value>
        private MemorySharp MemorySharp { get; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="FactoryManager" /> class.
        /// </summary>
        /// <param name="memorySharp">The memory sharp instance to use for reference in the managers instance.</param>
        public FactoryManager(MemorySharp memorySharp)
        {
            MemorySharp = memorySharp;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets the instance of the <see cref="Factories.ThreadFactory" /> class.
        /// </summary>
        /// <value>The thread factory.</value>
        public ThreadFactory ThreadFactory { get; private set; }


        /// <summary>
        ///     Gets the instance of the <see cref="Factories.WindowFactory" /> class.
        /// </summary>
        /// <value>The window factory.</value>
        public WindowFactory WindowFactory { get; private set; }


        /// <summary>
        ///     Gets the instance of the <see cref="Factories.MemoryFactory" /> class.
        /// </summary>
        /// <value>The memory factory.</value>
        public MemoryFactory MemoryFactory { get; private set; }

        /// <summary>
        ///     Gets the instance of the <see cref="Factories.ModuleFactory" /> class.
        /// </summary>
        /// <value>The module factory.</value>
        public ModuleFactory ModuleFactory { get; private set; }

        /// <summary>
        ///     Gets the the instance of the <see cref="Factories.AssemblyFactory" /> class.
        /// </summary>
        /// <value>The assembly factory.</value>
        public AssemblyFactory AssemblyFactory { get; private set; }
        #endregion

        #region Interface Implementations
        public void Dispose()
        {
            RemoveAll();
        }
        #endregion

        /// <summary>
        ///     Disables all items in the manager by removing from the managers items and then disposing them.
        /// </summary>
        public override void DisableAll()
        {
            RemoveAll();
        }

        /// <summary>
        ///     Enables all factories in the manager by creating their object instaces.
        /// </summary>
        public override void EnableAll()
        {
            InternalItems.Add("ThreadFactory", ThreadFactory = new ThreadFactory(MemorySharp));
            InternalItems.Add("WindowFactory", WindowFactory = new WindowFactory(MemorySharp));
            InternalItems.Add("MemoryFactory", MemoryFactory = new MemoryFactory(MemorySharp));
            InternalItems.Add("ModuleFactory", ModuleFactory = new ModuleFactory(MemorySharp));
            InternalItems.Add("AssemblyFactory", AssemblyFactory = new AssemblyFactory(MemorySharp));
        }

        /// <summary>
        ///     Removes a factory by its name in the manager and then disposes it.
        /// </summary>
        /// <param name="name">The name of the factory key to remove.</param>
        public override void Remove(string name)
        {
            var factory = InternalItems[name];
            InternalItems.Remove(name);
            factory.Dispose();
        }

        /// <summary>
        ///     Removes a factory by its instance reference in the manager.
        /// </summary>
        /// <param name="iFactory">The <see cref="IFactory" /> instance to remove and dispose.</param>
        public override void Remove(IFactory iFactory)
        {
            var keyValuePair = InternalItems.First(factory => factory.Value.Equals(iFactory));
            InternalItems.Remove(keyValuePair.Key);
            keyValuePair.Value.Dispose();
        }

        /// <summary>
        ///     Removes all factories in the manager by removing from the managers items and then disposing them.
        /// </summary>
        public override void RemoveAll()
        {
            foreach (var value in InternalItems)
            {
                Remove(value.Key);
            }
        }
    }
}