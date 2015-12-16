using System.Diagnostics;

namespace Binarysharp.MemoryManagement.Patterns
{
    /// <summary>
    ///     A class that represents basic pattern scanning properties for patterns that can Serializabled froma json or xml
    ///     file.
    /// </summary>
    public struct SerializablePattern
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Creates a new instance of <see cref="SerializablePattern" />.
        /// </summary>
        /// <param name="description">A description of the pattern being created.</param>
        /// <param name="dwordTextPattern">The dword pattern text containing the pattern to be scanned.</param>
        /// <example>
        ///     The example below will show you how a dword pattern would look for a given byte array.
        ///     <code>
        /// var bytes exampleBytes = new bytes[]{55,0x8B,0xEC,51,0xFF,05, 00, 00, 00, 00, 0xA2};
        /// var patternText = "55 8b ec 51 FF 05 ?? ?? ?? ?? A1";
        /// </code>
        /// </example>
        /// <param name="offseToAdd">The offset to add to the result found before returning the value.</param>
        /// >
        /// <param name="rebaseResult">
        ///     If the address should be rebased to the base address of the <see cref="ProcessModule" /> the
        ///     pattern data resides in.
        /// </param>
        /// <param name="comment">Any comments about the pattern you would like to have in the json or xml file you store it in.</param>
        public SerializablePattern(string description, string dwordTextPattern, int offseToAdd, bool rebaseResult,
            string comment = "None")
        {
            Description = description;
            TextPattern = dwordTextPattern;
            OffsetToAdd = offseToAdd;
            RebaseResult = rebaseResult;
            Comments = comment;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     A description of the pattern being scanned.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The Dword format text of the pattern.
        ///     <example>
        ///         The example below will show you how a dword pattern would look for a given byte array.
        ///         <code>
        /// var bytes exampleBytes = new bytes[]{55,0x8B,0xEC,51,0xFF,05, 00, 00, 00, 00, 0xA2};
        /// var patternText = "55 8b ec 51 FF 05 ?? ?? ?? ?? A1";
        /// </code>
        ///     </example>
        /// </summary>
        public string TextPattern { get; set; }

        /// <summary>
        ///     The value to add to the offset result when the pattern is found. This is used for when the pattern being found is
        ///     is x amount of bytes from the desired data.
        /// </summary>
        public int OffsetToAdd { get; set; }

        /// <summary>
        ///     If the address should be rebased to the base address of the <see cref="ProcessModule" /> the pattern data resides
        ///     in.
        /// </summary>
        public bool RebaseResult { get; set; }

        /// <summary>
        ///     Comments about this <see cref="SerializablePattern" /> instance.
        /// </summary>
        public string Comments { get; set; }
        #endregion
    }
}