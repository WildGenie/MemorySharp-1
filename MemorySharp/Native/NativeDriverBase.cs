using System;
using Binarysharp.MemoryManagement.RemoteProcess.Memory;

namespace Binarysharp.MemoryManagement.Native
{
    /// <summary>
    ///     Defines where the operating system information are collected.
    /// </summary>
    public abstract class NativeDriverBase
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The default native driver.
        /// </summary>
        private static readonly Lazy<NativeDriverBase> DefaultDriver = new Lazy<NativeDriverBase>(GetDefaultNativeDriver);
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets the default native driver.
        /// </summary>
        /// <remarks>The type of this driver depends on the architecture of the application that uses MemorySharp.</remarks>
        public static NativeDriverBase Default => DefaultDriver.Value;

        /// <summary>
        ///     The memory interaction.
        /// </summary>
        public abstract IMemoryCore MemoryCore { get; protected set; }
        #endregion

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