using Binarysharp.MemoryManagement.Common.Logging.Core;
using Binarysharp.MemoryManagement.Managment;

namespace Binarysharp.MemoryManagement.Common.Updaters
{
    /// <summary>
    ///     Class to manage updaters.
    /// </summary>
    public sealed class UpdaterManager : SafeManager<Updater>
    {
        /// <summary>
        ///     The thread-safe singleton of the plugin manager. The object is created when any static members/functions of this
        ///     class is called.
        /// </summary>
        public static readonly UpdaterManager Instance = new UpdaterManager();

        /// <summary>
        ///     Prevents a default instance of the <see cref="UpdaterManager" /> class from being created.
        /// </summary>
        private UpdaterManager() : base(new DebugLog())
        {
        }

        /// <summary>
        ///     Gets the <see cref="Updater" /> with the specified updater name.
        /// </summary>
        /// <param name="updaterName">Name of the updater.</param>
        /// <returns>Updater.</returns>
        public Updater this[string updaterName] => InternalItems[updaterName];

        /// <summary>
        ///     Adds a new updater instance to the manager.
        /// </summary>
        /// <param name="name">The name that represents the updater.</param>
        /// <param name="ticksPerSecond">The ticks per second.</param>
        /// <returns>A new Updater instance.</returns>
        public Updater Add(string name, int ticksPerSecond)
        {
            InternalItems[name] = new Updater(name, ticksPerSecond);
            return InternalItems[name];
        }
    }
}