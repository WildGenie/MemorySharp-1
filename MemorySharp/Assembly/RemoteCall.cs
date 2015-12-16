using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Binarysharp.MemoryManagement.Assembly.CallingConvention;

namespace Binarysharp.MemoryManagement.Assembly
{
    /// <summary>
    ///     A class representing a remote function which you wish to call with assembly injection. This class will the calling
    ///     convention of the function and provide accsess quick methods to call and execute it.
    /// </summary>
    public class RemoteCall : IEquatable<RemoteCall>
    {
        #region Constructors, Destructors
        /// <summary>
        ///     RemoteCall constructor.
        /// </summary>
        /// <param name="sharp">The <see cref="MemoryManagement.MemoryBase" /> Instance to use.</param>
        /// <param name="address">The address where the RemoteCall function is stored in memory.</param>
        /// <param name="callingConvention">The <see cref="CallingConventions" /> the RemoteCall uses.</param>
        public RemoteCall(MemoryBase sharp, IntPtr address, CallingConventions callingConvention)
        {
            Memory = sharp;
            BaseAddress = address;
            CallingConvention = callingConvention;
        }

        /// <summary>
        ///     Do not create this object with the no-param struct unless you know what you're doing!
        /// </summary>
        public RemoteCall()
        {
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     The address where the RemoteCall function is stored in memory.
        /// </summary>
        public IntPtr BaseAddress { get; set; }

        /// <summary>
        ///     Instance of <see cref="MemoryManagement.MemorySharp" /> to use.
        /// </summary>
        internal MemoryBase Memory { get; set; }

        /// <summary>
        ///     The <see cref="CallingConventions" /> used for this call.
        /// </summary>
        internal CallingConventions CallingConvention { get; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(RemoteCall other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                (BaseAddress.Equals(other.BaseAddress) && Memory.Equals(other.Memory));
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Executes the assembly code in the remote process.
        /// </summary>
        /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T Execute<T>(params dynamic[] parameters)
        {
            return Memory.Assembly.Execute<T>(BaseAddress, CallingConvention, parameters);
        }


        /// <summary>
        ///     Executes the assembly code in the remote process.
        /// </summary>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public IntPtr Execute(params dynamic[] parameters)
        {
            return Execute<IntPtr>(BaseAddress, CallingConvention, parameters);
        }

        /// <summary>
        ///     Executes the assembly code in the remote process.
        /// </summary>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public IntPtr Execute()
        {
            return Execute<IntPtr>(BaseAddress, CallingConvention);
        }


        /// <summary>
        ///     Executes the assembly code in the remote process.
        /// </summary>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T Execute<T>()
        {
            return Execute<T>(BaseAddress, CallingConvention);
        }

        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<T> ExecuteAsycn<T>(params dynamic[] parameters)
        {
            return Memory.Assembly.ExecuteAsync<T>(BaseAddress, CallingConvention, parameters);
        }


        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task ExecuteAsycn(params dynamic[] parameters)
        {
            return ExecuteAsycn<IntPtr>(BaseAddress, CallingConvention, parameters);
        }

        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task ExecuteAsycn()
        {
            return ExecuteAsycn<IntPtr>(BaseAddress, CallingConvention);
        }

        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task ExecuteAsycn<T>()
        {
            return ExecuteAsycn<T>(BaseAddress, CallingConvention);
        }


        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RemoteCall) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return BaseAddress.GetHashCode() ^ Memory.GetHashCode();
        }

        /// <summary>
        ///     Operator overide.
        /// </summary>
        public static bool operator ==(RemoteCall left, RemoteCall right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Operator overide.
        /// </summary>
        public static bool operator !=(RemoteCall left, RemoteCall right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}