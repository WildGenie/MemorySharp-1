using System;

namespace MemorySharp.Exceptions
{
    /// <summary>
    ///     Base exception type thrown by BlueRain.
    /// </summary>
    public class MemorySharpException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemorySharpException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MemorySharpException(string message)
            : base(message)
        {
        }
    }
}