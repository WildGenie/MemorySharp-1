using System;
using Binarysharp.MemoryManagement.MemoryExternal.Memory;
using Binarysharp.MemoryManagement.Native;
using RemotePointer = Binarysharp.MemoryManagement.MemoryInternal.Memory.RemotePointer;

// ReSharper disable once CheckNamespace

namespace Binarysharp.MemoryManagement.Temp
{
    /// <summary>
    ///     Represents a contiguous block of memory in the remote process.
    /// </summary>
    public class RemoteRegion : RemotePointer, IEquatable<RemoteRegion>
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryExternal.Memory.RemoteRegion" /> class.
        /// </summary>
        /// <param name="memoryPlus">The reference of the <see cref="MemorySharp" /> object.</param>
        /// <param name="baseAddress">The base address of the memory region.</param>
        internal RemoteRegion(MemoryPlus memoryPlus, IntPtr baseAddress) : base(memoryPlus, baseAddress)
        {
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     Contains information about the memory.
        /// </summary>
        public MemoryBasicInformation Information => MemoryCore.Query(MemoryPlus.Process.SafeHandle, BaseAddress);

        /// <summary>
        ///     Gets if the <see cref="MemoryExternal.Memory.RemoteRegion" /> is valid.
        /// </summary>
        public override bool IsValid => base.IsValid && Information.State != MemoryStateFlags.Free;
        #endregion

        #region  Interface members
        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(RemoteRegion other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                   (BaseAddress.Equals(other.BaseAddress) && MemoryPlus.Equals(other.MemoryPlus) &&
                    Information.RegionSize.Equals(other.Information.RegionSize));
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MemoryExternal.Memory.RemoteRegion) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            return BaseAddress.GetHashCode() ^ MemoryPlus.GetHashCode() ^ Information.RegionSize.GetHashCode();
        }

        public static bool operator ==(RemoteRegion left, RemoteRegion right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RemoteRegion left, RemoteRegion right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Releases the memory used by the region.
        /// </summary>
        public void Release()
        {
            // Release the memory
            MemoryCore.Free(MemoryPlus.Process.SafeHandle, BaseAddress);
            // Remove the pointer
            BaseAddress = IntPtr.Zero;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return
                $"BaseAddress = 0x{BaseAddress.ToInt64():X} Size = 0x{Information.RegionSize:X} Protection = {Information.Protect}";
        }
        #endregion
    }
}