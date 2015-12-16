using System;
using Binarysharp.MemoryManagement.Common.Builders;
using Binarysharp.MemoryManagement.Hooks;

namespace Binarysharp.MemoryManagement.Management
{
    /// <summary>
    ///     Class [sealed, singleton] to manage <see cref="IHook" /> members.
    /// </summary>
    public sealed class HookManager : Manager<IHook>, IFactory
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The lazyily installed <see cref="HookManager" /> instance.
        /// </summary>
        private static readonly Lazy<HookManager> LazyInstance = new Lazy<HookManager>(() => new HookManager());
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     The [lazy installed] <see cref="HookManager" /> instance.
        /// </summary>
        public static HookManager Instance => LazyInstance.Value;


        /// <summary>
        ///     Gets the <see cref="IHook" /> with the specified key.
        /// </summary>
        /// <value>
        ///     The <see cref="IHook" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>A <see cref="IHook" /> instance.</returns>
        public IHook this[string key] => (WindowProc) InternalItems[key];
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            RemoveAll();
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Adds the specified <see cref="IHook" /> member to the manager.
        /// </summary>
        /// <typeparam name="T">The type/</typeparam>
        /// <param name="iHook">The <see cref="IHook" /> member.</param>
        /// <returns>A <see cref="IHook" /> member casted to <code>(T)</code>.</returns>
        public T Add<T>(IHook iHook) where T : IHook
        {
            InternalItems[iHook.Name] = iHook;
            return (T) InternalItems[iHook.Name];
        }


        /// <summary>
        ///     Gets <see cref="IHook" /> instance with the the specified key.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>A <see cref="IHook" /> member casted to <code>(T)</code>.</returns>
        public T Get<T>(string key)
        {
            return (T) InternalItems[key];
        }
        #endregion
    }
}