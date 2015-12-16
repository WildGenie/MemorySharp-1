using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Binarysharp.MemoryManagement.Assembly;
using Binarysharp.MemoryManagement.Common.Builders;
using Binarysharp.MemoryManagement.Edits;
using Binarysharp.MemoryManagement.Management;
using Binarysharp.MemoryManagement.Marshaling;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Modules;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Native.Enums;
using Binarysharp.MemoryManagement.Threading;
using Binarysharp.MemoryManagement.Windows;

namespace Binarysharp.MemoryManagement
{
    /// <summary>
    ///     The base class for MemorySharp and MemoryPlus.
    /// </summary>
    public abstract class MemoryBase : IDisposable
    {
        #region Public Delegates/Events
        /// <summary> Event queue for all listeners interested in ProcessExited events. </summary>
        public event EventHandler ProcessExited;
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryBase" /> class.
        /// </summary>
        /// <param name="proc">The process.</param>
        /// <remarks>Created 2012-02-15</remarks>
        protected MemoryBase(Process proc)
        {
            if (proc.HasExited)
            {
                throw new AccessViolationException("Process: " + proc.Id + " has already exited. Can not attach to it.");
            }
            //   Process.EnterDebugMode();
            // Good to set this too if ure using events.
            proc.EnableRaisingEvents = true;

            // Since people tend to not realize it exists, we make sure to handle it.
            proc.Exited += (s, e) =>
            {
                ProcessExited?.Invoke(s, e);
                HandleProcessExiting();
            };
            Process = proc;
            proc.ErrorDataReceived += OutputDataReceived;
            proc.OutputDataReceived += OutputDataReceived;
            Handle = NativeMethods.OpenProcess(ProcessAccessFlags.AllAccess, false, proc.Id);
            ImageBase = Process.MainModule.BaseAddress;
            // Create instances of the factories
            Factories = new List<IFactory>();
            Factories.AddRange(
                new IFactory[]
                {
                    Assembly = new AssemblyFactory(this),
                    Memory = new MemoryFactory(this),
                    Modules = new ModuleFactory(this),
                    Threads = new ThreadFactory(this),
                    Windows = new WindowFactory(this),
                    Patches = new PatchManager(this)
                });
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets the specified module in the remote process.
        /// </summary>
        /// <param name="moduleName">The name of module (not case sensitive).</param>
        /// <returns>A new instance of a <see cref="RemoteModule" /> class.</returns>
        public RemoteModule this[string moduleName] => Modules[moduleName];

        /// <summary>
        ///     Gets a pointer to the specified address in the remote process.
        /// </summary>
        /// <param name="address">The address pointed.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A new instance of a <see cref="RemotePointer" /> class.</returns>
        public RemotePointer this[IntPtr address, bool isRelative = true]
            => new RemotePointer(this, isRelative ? GetAbsolute(address) : address);

        /// <summary>
        ///     Factory for manipulating modules and libraries.
        /// </summary>
        public ModuleFactory Modules { get; protected set; }

        /// <summary>
        ///     Factory for manipulating threads.
        /// </summary>
        public ThreadFactory Threads { get; protected set; }

        /// <summary>
        ///     Factory for generating assembly code.
        /// </summary>
        public AssemblyFactory Assembly { get; protected set; }

        /// <summary>
        ///     Factory for manipulating windows.
        /// </summary>
        public WindowFactory Windows { get; protected set; }

        /// <summary>
        ///     Factory for manipulating memory space.
        /// </summary>
        public MemoryFactory Memory { get; protected set; }

        /// <summary>
        ///     Manager for <see cref="Patch" /> objects.
        /// </summary>
        public PatchManager Patches { get; }

        /// <summary>
        ///     The factories embedded inside the library.
        /// </summary>
        protected List<IFactory> Factories { get; set; }

        /// <summary>
        ///     The Process Environment Block of the process.
        /// </summary>
        public ManagedPeb Peb { get; }

        /// <summary>Gets the image base.</summary>
        public IntPtr ImageBase { get; set; }

        /// <summary>
        ///     Gets or sets the process handle.
        /// </summary>
        /// <value>
        ///     The process handle.
        /// </value>
        /// <remarks>Created 2012-02-15</remarks>
        public SafeMemoryHandle Handle { get; set; }


        /// <summary>
        ///     Gets the process.
        /// </summary>
        /// <remarks>Created 2012-02-15</remarks>
        public Process Process { get; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Created 2012-02-15</remarks>
        public virtual void Dispose()
        {
            try
            {
                Process.LeaveDebugMode();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Reads a specific number of bytes from memory.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public abstract byte[] ReadBytes(IntPtr address, int count, bool isRelative = false);

        /// <summary>
        ///     Writes a set of bytes to memory.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns>
        ///     Number of bytes written.
        /// </returns>
        public abstract int WriteBytes(IntPtr address, byte[] bytes, bool isRelative = false);

        /// <summary>
        ///     Reads the struct array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <param name="length">The elements.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public virtual T[] Read<T>(IntPtr address, int length, bool isRelative = false)
        {
            if (isRelative)
            {
                address = GetAbsolute(address);
            }

            var ret = new T[length];

            for (var i = 0; i < length; i++)
            {
                ret[i] = Read<T>(address + i*MarshalCache<T>.Size);
            }

            return ret;
        }


        /// <summary> Reads a value from the specified address in memory. </summary>
        /// <remarks> Created 3/24/2012. </remarks>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="address"> The address. </param>
        /// <param name="isRelative"> (optional) the relative. </param>
        /// <returns> . </returns>
        public abstract T Read<T>(IntPtr address, bool isRelative = false);

        /// <summary> Writes a value specified to the address in memory. </summary>
        /// <remarks> Created 3/24/2012. </remarks>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="address"> The address. </param>
        /// <param name="value"> The value. </param>
        /// <param name="isRelative"> (optional) the relative. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public abstract void Write<T>(IntPtr address, T value, bool isRelative = false);


        /// <summary> Writes an array of values to the address in memory. </summary>
        /// <remarks> Created 3/24/2012. </remarks>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="address"> The address. </param>
        /// <param name="values"> The value. </param>
        /// <param name="isRelative"> (optional) the relative. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public void Write<T>(IntPtr address, T[] values, bool isRelative = false)
        {
            for (var i = 0; i < values.Length; i++)
            {
                Write(address + MarshalType<T>.Size*i, values[i], isRelative);
            }
        }

        /// <summary> Reads a value from the specified address in memory. This method is used for multi-pointer dereferencing.</summary>
        /// <remarks> Created 3/24/2012. </remarks>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="isRelative"> (optional) the relative. </param>
        /// <param name="addresses"> A variable-length parameters list containing addresses. </param>
        /// <returns> . </returns>
        public T Read<T>(bool isRelative = false, params IntPtr[] addresses)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (addresses.Length == 0)
            {
                throw new InvalidOperationException("Cannot read a value from unspecified addresses.");
            }

            if (addresses.Length == 1)
            {
                return Read<T>(addresses[0], isRelative);
            }

            var temp = Read<IntPtr>(addresses[0], isRelative);

            for (var i = 1; i < addresses.Length - 1; i++)
            {
                temp = Read<IntPtr>(temp + (int) addresses[i]);
            }
            return Read<T>(temp + (int) addresses[addresses.Length - 1]);
        }

        /// <summary> Writes a value specified to the address in memory. This method is used for multi-pointer dereferencing.</summary>
        /// <remarks> Created 3/24/2012. </remarks>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="isRelative"> (optional) the relative. </param>
        /// <param name="value"> The value. </param>
        /// <param name="addresses"> A variable-length parameters list containing addresses. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public void Write<T>(bool isRelative = false, T value = default(T), params IntPtr[] addresses)
        {
            if (addresses.Length == 0)
            {
                throw new InvalidOperationException("Cannot write a value to unspecified addresses.");
            }
            if (addresses.Length == 1)
            {
                Write(addresses[0], value, isRelative);
            }

            var temp = Read<IntPtr>(addresses[0], isRelative);
            for (var i = 1; i < addresses.Length - 1; i++)
            {
                temp = Read<IntPtr>(temp + (int) addresses[i]);
            }
            Write(temp + (int) addresses[addresses.Length - 1], value);
        }

        /// <summary> Reads a string. </summary>
        /// <remarks> Created 3/27/2012. </remarks>
        /// <param name="address"> The address. </param>
        /// <param name="encoding"> The encoding. </param>
        /// <param name="maxLength"> (optional) length of the maximum. </param>
        /// <param name="relative"> (optional) the relative. </param>
        /// <returns> The string. </returns>
        public virtual string ReadString(IntPtr address, Encoding encoding, int maxLength = 512, bool relative = false)
        {
            var buffer = ReadBytes(address, maxLength, relative);
            var ret = encoding.GetString(buffer);
            if (ret.IndexOf('\0') != -1)
            {
                ret = ret.Remove(ret.IndexOf('\0'));
            }
            return ret;
        }

        /// <summary> Writes a string. </summary>
        /// <remarks> Created 3/27/2012. </remarks>
        /// <param name="address"> The address. </param>
        /// <param name="value"> The value. </param>
        /// <param name="encoding"> The encoding. </param>
        /// <param name="relative"> (optional) the relative. </param>
        public virtual bool WriteString(IntPtr address, string value, Encoding encoding, bool relative = false)
        {
            if (value[value.Length - 1] != '\0')
            {
                value += '\0';
            }

            var b = encoding.GetBytes(value);
            var written = WriteBytes(address, b, relative);
            return written == b.Length;
        }

        /// <summary>
        ///     Gets the absolute.
        /// </summary>
        /// <param name="relative">The relative.</param>
        /// <returns></returns>
        /// <remarks>Created 2012-01-16 19:41</remarks>
        public IntPtr GetAbsolute(IntPtr relative)
        {
            return ImageBase + (int) relative;
        }

        /// <summary>
        ///     Gets the relative.
        /// </summary>
        /// <param name="absolute">The absolute.</param>
        /// <returns></returns>
        /// <remarks>Created 2012-01-16 19:41</remarks>
        public IntPtr GetRelative(IntPtr absolute)
        {
            return ImageBase - (int) absolute;
        }
        #endregion

        #region Private Methods
        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.Write(e.Data);
        }
        #endregion

        /// <summary>
        ///     Handles the process exiting.
        /// </summary>
        /// <remarks>Created 2012-02-15</remarks>
        protected virtual void HandleProcessExiting()
        {
        }
    }
}