namespace MemorySharp.Helpers.Logger
{
    /// <summary>
    ///     An interface for creating very basic logger classes.
    /// </summary>
    public interface ILog
    {
        #region Methods
        /// <summary>
        ///     Write a warning log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void WriteWarning(string message);

        /// <summary>
        ///     Write a normal log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void WriteNormal(string message);

        /// <summary>
        ///     Write an error log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void WriteError(string message);

        /// <summary>
        ///     Write an information log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void WriteInfo(string message);
        #endregion
    }
}