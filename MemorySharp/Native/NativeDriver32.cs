/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using Binarysharp.MemoryManagement.Memory;

namespace Binarysharp.MemoryManagement.Native
{
    /// <summary>
    ///     Defines how the operating system information are collected for 32-bit architecture.
    /// </summary>
    public sealed class NativeDriver32 : NativeDriverBase
    {
        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the class <see cref="NativeDriver32" />.
        /// </summary>
        public NativeDriver32()
        {
            // Create the core components
            MemoryCore = new MemoryCore32();
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Memory interaction for 32-bit architecture.
        /// </summary>
        public override IMemoryCore MemoryCore { get; protected set; }
        #endregion
    }
}