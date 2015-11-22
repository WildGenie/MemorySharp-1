namespace Binarysharp.MemoryManagement.Internals
{
    /// <summary>
    ///     Defines an element able to have a <code>void Pulse()</code> method to run and perform some logic when executed.
    /// </summary>
    public interface IPulsableElement
    {
        /// <summary>
        ///     Pulse one iteration of this instance's logic.
        /// </summary>
        void Pulse();
    }
}