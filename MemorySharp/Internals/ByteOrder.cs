namespace Binarysharp.MemoryManagement.Internals
{
    /// <summary>
    ///     Byte order types.
    /// </summary>
    public enum ByteOrder
    {
        /// <summary>
        ///     Determine byte order from input
        /// </summary>
        Determine = 0,

        /// <summary>
        ///     Byte order is little endian
        /// </summary>
        LittleEndian,

        /// <summary>
        ///     Byte order is big endian
        /// </summary>
        BigEndian
    }
}