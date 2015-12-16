namespace Binarysharp.MemoryManagement.Common.Builders
{
    /// <summary>
    ///     Defines an interface for managed loggers.
    /// </summary>
    public interface IManagedLog : ILog, INamedElement
    {
        // Nothing for now.
    }

    /// <summary>
    ///     Defines an interface for logging messages.
    /// </summary>
    public interface ILog
    {
        #region Public Methods
        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        /// <param name="message">The message being logged.</param>
        void Write(string message);
        #endregion
    }
}