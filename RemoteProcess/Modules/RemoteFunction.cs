/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using Binarysharp.MemoryManagement.RemoteProcess.Memory;

namespace Binarysharp.MemoryManagement.RemoteProcess.Modules
{
    /// <summary>
    ///     Class representing a function in the remote process.
    /// </summary>
    public class RemoteFunction : RemotePointer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RemoteFunction" /> class.
        /// </summary>
        /// <param name="memorySharp">The memory sharp.</param>
        /// <param name="address">The address.</param>
        /// <param name="functionName">Name of the function.</param>
        public RemoteFunction(MemorySharp memorySharp, IntPtr address, string functionName) : base(memorySharp, address)
        {
            // Save the parameter
            Name = functionName;
        }

        /// <summary>
        ///     The name of the function.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"BaseAddress = 0x{BaseAddress.ToInt64():X} Name = {Name}";
        }
    }
}