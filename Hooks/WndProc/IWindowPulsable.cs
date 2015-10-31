namespace Binarysharp.MemoryManagement.Hooks.WndProc
{
    /// <summary>
    ///     Defines a method to be called each time the <see cref="WindowEngine.StartUp()" /> method is called.
    /// </summary>
    public interface IWindowPulsable
    {
        #region Methods
        /// <summary>
        ///     The method to execute.
        /// </summary>
        void Pulse();
        #endregion
    }
}