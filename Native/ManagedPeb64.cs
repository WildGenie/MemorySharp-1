using Binarysharp.MemoryManagement.Memory.Remote;

namespace Binarysharp.MemoryManagement.Native
{
    /// <summary>
    ///     Class representing the Process Environment Block of a remote process.
    /// </summary>
    public class ManagedPeb64 : RemotePointer
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManagedPeb32" /> class.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
        public ManagedPeb64(MemorySharp memorySharp)
            : base(
                memorySharp,
                memorySharp.NativeDriver.MemoryCore.QueryInformationProcess(memorySharp.SafeHandle).PebBaseAddress)
        {
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Returns the read <see cref="Peb64" /> structure.
        ///     <remarks>
        ///         Currently, this is not a fully managed peb like the x32 bit version. However, for now until updated it
        ///         still provides a lot of information that is easily accessible about the peb for 64 bit and releated values.
        ///     </remarks>
        /// </summary>
        public Peb64 NativePeb => Read<Peb64>();

        /// <summary>
        ///     Returns the read <see cref="PebLdrData64" /> structure.
        ///     <remarks>
        ///         Currently, this is not a fully managed peb like the x32 bit version. However, for now until updated it
        ///         still provides a lot of information that is easily accessible about the peb for 64 bit and releated values.
        ///     </remarks>
        /// </summary>
        public PebLdrData64 NativePebLdrData => MemorySharp.Read<PebLdrData64>(NativePeb.pLdr);

        #endregion
    }
}