using System;
using Binarysharp.MemoryManagement.MemoryInternal.Memory;

namespace Binarysharp.MemoryManagement.MemoryInternal.Modules
{
    /// <summary>
    ///     Class representing a function in the remote process.
    /// </summary>
    public class RemoteFunction : RemotePointer
    {
        #region Constructors
        /// <summary>
        /// </summary>
        /// <param name="memoryPlus"></param>
        /// <param name="address"></param>
        /// <param name="functionName"></param>
        public RemoteFunction(MemoryPlus memoryPlus, IntPtr address, string functionName) : base(memoryPlus, address)
        {
            // Save the parameter
            Name = functionName;
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     The name of the function.
        /// </summary>
        public string Name { get; }
        #endregion

        #region Methods
        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"BaseAddress = 0x{BaseAddress.ToInt64():X} Name = {Name}";
        }
        #endregion
    }
}