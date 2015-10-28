using System;

namespace MemorySharp.Exceptions
{
    /// <summary>
    ///     Exception thrown when a reading operation fails.
    /// </summary>
    public class MemorySharpReadException : MemorySharpException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemorySharpReadException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MemorySharpReadException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemorySharpReadException" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        public MemorySharpReadException(IntPtr address, int count)
            : this($"ReadProcessMemory failed! Could not read {count} bytes from {address.ToString("X")}!"
                )
        {
        }
    }
}