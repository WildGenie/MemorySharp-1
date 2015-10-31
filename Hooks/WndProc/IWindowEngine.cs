namespace Binarysharp.MemoryManagement.Hooks.WndProc
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

        /// <summary>
        ///     Registers a call back for the <see cref="WindowHook" /> class to use.
        /// </summary>
        /// <param name="pulsable">The <see cref="WindowHook" /> Instace to register</param>
        void RegisterCallback(IWindowPulsable pulsable);

        /// <summary>
        ///     Registers multiple call backs.
        /// </summary>
        /// <param name="pulsables">The <see cref="IWindowPulse" /> Instaces to register</param>
        void RegisterCallbacks(params IWindowPulsable[] pulsables);

        /// <summary>
        ///     Removes a call back.
        /// </summary>
        /// <param name="pulsable">The <see cref="IWindowPulse" /> Instace to remove.r</param>
        void RemoveCallback(IWindowPulsable pulsable);
        #endregion
    }
}