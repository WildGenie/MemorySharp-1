namespace Binarysharp.MemoryManagement.Managment.Builders
{
    /// <summary>
    ///     Defines an element able to be activated in the remote process.
    /// </summary>
    public interface IApplicableElement : IDisposableState
    {
        /// <summary>
        ///     States if the element is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        ///     Disables the element.
        /// </summary>
        void Disable();

        /// <summary>
        ///     Enables the element.
        /// </summary>
        void Enable();
    }
}