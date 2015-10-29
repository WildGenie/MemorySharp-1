using System;

namespace MemorySharp.Internals
{
    /// <summary>
    ///     Exception thrown when a writing operation fails.
    /// </summary>
    public class MemorySharpWriteException : MemorySharpException
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemorySharpWriteException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MemorySharpWriteException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemorySharpWriteException" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        public MemorySharpWriteException(IntPtr address, int count)
            : this($"WriteProcessMemory failed! Could not write {count} bytes at {address.ToString("X")}!")
        {
        }
        #endregion
    }
}