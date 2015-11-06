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

namespace Binarysharp.MemoryManagement.Native
{
    /// <summary>
    ///     Defines where the operating system information are collected.
    /// </summary>
    public abstract class NativeDriverBase
    {
        /// <summary>
        ///     The default native driver.
        /// </summary>
        private static readonly Lazy<NativeDriverBase> DefaultDriver = new Lazy<NativeDriverBase>(GetDefaultNativeDriver);

        /// <summary>
        ///     Gets the default native driver.
        /// </summary>
        /// <remarks>The type of this driver depends on the architecture of the application that uses MemorySharp.</remarks>
        public static NativeDriverBase Default => DefaultDriver.Value;

        /// <summary>
        ///     The memory interaction.
        /// </summary>
        public abstract IMemoryCore MemoryCore { get; protected set; }

        /// <summary>
        ///     Determines which native driver must be used as default.
        /// </summary>
        /// <returns>The default native driver.</returns>
        private static NativeDriverBase GetDefaultNativeDriver()
        {
            // If the application is 64-bit architecture
            if (IntPtr.Size == 8)
            {
                // TODO: Add the return statement
            }

            // Else use a 32-bit driver
            return new NativeDriver32();
        }
    }
}