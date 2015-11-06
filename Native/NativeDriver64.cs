/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using Binarysharp.MemoryManagement.RemoteProcess.Memory;

namespace Binarysharp.MemoryManagement.Native
{
    /// <summary>
    ///     Defines how the operating system information are collected for 64-bit architecture.
    /// </summary>
    public sealed class NativeDriver64 : NativeDriverBase
    {
        /// <summary>
        ///     Initializes a new instance of the class <see cref="NativeDriver64" />.
        /// </summary>
        public NativeDriver64()
        {
            // Create the core components
            MemoryCore = new MemoryCore64();
        }

        /// <summary>
        ///     Memory interaction for 64-bit architecture.
        /// </summary>
        public override IMemoryCore MemoryCore { get; protected set; }
    }
}