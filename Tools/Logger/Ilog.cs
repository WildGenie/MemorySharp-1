namespace MemorySharp.Tools.Logger
{
    /// <summary>
    ///     An interface for creating very basic logger classes.
    /// </summary>
    public interface ILog
    {
        #region Methods
        /// <summary>
        ///     Normal a warning log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Warning(string message);

        /// <summary>
        ///     Normal a normal log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Normal(string message);

        /// <summary>
        ///     Normal an error log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Error(string message);

        /// <summary>
        ///     Normal an information log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Info(string message);
        #endregion
    }
}