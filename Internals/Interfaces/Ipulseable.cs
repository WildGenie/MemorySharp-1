namespace MemorySharp.Internals.Interfaces
{
    /// <summary>
    ///     Defines an element which pulses a method or set of methods.
    /// </summary>
    public interface IPulseable
    {
        #region Methods
        /// <summary>
        ///     Defines the pulse execution.
        /// </summary>
        void Pulse();
        #endregion
    }
}