using System;
using System.Text;
using Binarysharp.MemoryManagement.Memory;

namespace Binarysharp.MemoryManagement.Marshaling
{
    namespace GreyMagic
    {
    }

    /// <summary>
    ///     Interface representing a value within the memory of a remote process.
    /// </summary>
    public interface IMarshalledValue : IDisposable
    {
        #region Public Properties, Indexers
        /// <summary>
        ///     The memory allocated where the value is fully written if needed. It can be unused.
        /// </summary>
        RemoteAllocation Allocated { get; }

        /// <summary>
        ///     The reference of the value. It can be directly the value or a pointer.
        /// </summary>
        IntPtr Reference { get; }
        #endregion
    }

    /// <summary>
    ///     Class marshalling a value into the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value. It can be a primitive or reference data type.</typeparam>
    public class MarshalledValue<T> : IMarshalledValue
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The reference of the <see cref="MemorySharp" /> object.
        /// </summary>
        protected readonly MemoryBase MemorySharp;
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MarshalledValue{T}" /> class.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
        /// <param name="value">The value to marshal.</param>
        public MarshalledValue(MemoryBase memorySharp, T value)
        {
            // Save the parameters
            MemorySharp = memorySharp;
            Value = value;
            // Marshal the value
            Marshal();
        }

        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~MarshalledValue()
        {
            Dispose();
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     The memory allocated where the value is fully written if needed. It can be unused.
        /// </summary>
        public RemoteAllocation Allocated { get; private set; }

        /// <summary>
        ///     The reference of the value. It can be directly the value or a pointer.
        /// </summary>
        public IntPtr Reference { get; private set; }

        /// <summary>
        ///     The initial value.
        /// </summary>
        public T Value { get; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Releases all resources used by the <see cref="RemoteAllocation" /> object.
        /// </summary>
        public void Dispose()
        {
            // Free the allocated memory
            Allocated?.Dispose();
            // Set the pointer to zero
            Reference = IntPtr.Zero;
            // Avoid the finalizer
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///     Marshals the value into the remote process.
        /// </summary>
        private void Marshal()
        {
            // If the type is string, it's a special case
            if (typeof (T) == typeof (string))
            {
                var text = Value.ToString();
                // Allocate memory in the remote process (string + '\0')
                Allocated = MemorySharp.Memory.Allocate(text.Length + 1);
                // Write the value
                Allocated.WriteString(0, text, Encoding.UTF8);
                // Get the pointer
                Reference = Allocated.BaseAddress;
            }
            else
            {
                // For all other types
                // Convert the value into a byte array
                var byteArray = MarshalType<T>.ObjectToByteArray(Value);

                // If the value can be stored directly in registers
                if (MarshalType<T>.CanBeStoredInRegisters)
                {
                    // Convert the byte array into a pointer
                    Reference = MarshalType<IntPtr>.ByteArrayToObject(byteArray);
                }
                else
                {
                    // It's a bit more complicated, we must allocate some space into
                    // the remote process to store the value and get its pointer

                    // Allocate memory in the remote process
                    Allocated = MemorySharp.Memory.Allocate(MarshalType<T>.Size);
                    // Write the value
                    Allocated.Write(0, Value);
                    // Get the pointer
                    Reference = Allocated.BaseAddress;
                }
            }
        }
        #endregion
    }
}