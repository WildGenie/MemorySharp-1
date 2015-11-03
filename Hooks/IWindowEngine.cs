namespace Binarysharp.MemoryManagement.Hooks
{
    /// <summary>
    ///     Defines the <code>StartUp();</code> and <code>ShutDown()</code> methods for the <see cref="WindowHook" /> class to
    ///     use.
    /// </summary>
    public interface IWindowEngine
    {
        #region Methods

        /// <summary>
        ///     Starts the engine.
        /// </summary>
        void StartUp();

        /// <summary>
        ///     Shuts the engine down.
        /// </summary>
        void ShutDown();

        #endregion
    }
}