/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Binarysharp.MemoryManagement.Assembly;
using Binarysharp.MemoryManagement.Assembly.CallingConvention;
using Binarysharp.MemoryManagement.Extensions;
using Binarysharp.MemoryManagement.Helpers;
using Binarysharp.MemoryManagement.Internals;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Modules;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Patterns;
using Binarysharp.MemoryManagement.Threading;
using Binarysharp.MemoryManagement.Windows;

namespace Binarysharp.MemoryManagement
{
    /// <summary>
    ///     Class for memory editing a remote process.
    /// </summary>
    public class MemorySharp : IDisposable, IEquatable<MemorySharp>
    {
        #region Fields
        /// <summary>
        ///     The factories embedded inside the library.
        /// </summary>
        protected List<IFactory> Factories;

        /// <summary>
        ///     The Process Environment Block of the process.
        /// </summary>
        /// <remarks>The operation is deferred because it can be potentially slow.</remarks>
        protected Lazy<ManagedPeb> InternalPeb;

        /// <summary>
        ///     Raises when the <see cref="MemorySharp" /> object is disposed.
        /// </summary>
        public event EventHandler OnDispose;
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the architecture of the process.
        /// </summary>
        public ProcessArchitectures Architecture => ArchitectureHelper.GetArchitectureByProcess(Native);

        /// <summary>
        ///     Factory for generating assembly code.
        ///     <remarks>Currently not a valid property for <see cref="MemorySharpType.Internal" /></remarks>
        /// </summary>
        public AssemblyFactory Assembly { get; }

        /// <summary>
        ///     Gets whether the process is 32-bit.
        /// </summary>
        public bool Is32BitProcess => Architecture == ProcessArchitectures.Ia32;

        /// <summary>
        ///     Gets whether the process is 64-bit.
        /// </summary>
        public bool Is64BitProcess => Architecture == ProcessArchitectures.Amd64;

        /// <summary>
        ///     Gets whether the process is being debugged.
        /// </summary>
        public bool IsDebugged
        {
            get { return Peb.BeingDebugged; }
            set { Peb.BeingDebugged = value; }
        }

        /// <summary>
        ///     State if the process is running.
        /// </summary>
        public bool IsRunning => !Handle.IsInvalid && !Handle.IsClosed && !Native.HasExited;

        /// <summary>
        ///     The remote process handle opened with all rights.
        /// </summary>
        public SafeMemoryHandle Handle { get; }

        /// <summary>
        ///     Factory for manipulating memory space.
        /// </summary>
        public MemoryFactory Memory { get; }

        /// <summary>
        ///     Factory for manipulating modules and libraries.
        /// </summary>
        public ModuleFactory Modules { get; }

        /// <summary>
        ///     Provide access to the opened process.
        /// </summary>
        public Process Native { get; }

        /// <summary>
        ///     Gets the native driver to interact with the API system/architecture dependant.
        /// </summary>
        public NativeDriverBase NativeDriver { get; protected set; }

        /// <summary>
        ///     The Process Environment Block of the process.
        ///     ///
        ///     <remarks>Currently not tested for x64 or <see cref="MemorySharpType.Internal" /></remarks>
        /// </summary>
        public ManagedPeb Peb => InternalPeb.Value;

        /// <summary>
        ///     Gets the unique identifier for the remote process.
        /// </summary>
        public int Pid => Native.Id;

        /// <summary>
        ///     The address of the processes main module. See the <see cref="ProcessModule" /> class.
        /// </summary>
        public IntPtr ImageBase { get; }

        /// <summary>
        ///     The size of the main process module.
        /// </summary>
        public int ImageSize { get; }

        /// <summary>
        ///     Gets the specified module in the remote process.
        /// </summary>
        /// <param name="moduleName">The name of module (not case sensitive).</param>
        /// <returns>A new instance of a <see cref="RemoteModule" /> class.</returns>
        public RemoteModule this[string moduleName] => Modules[moduleName];

        /// <summary>
        ///     Gets a pointer to the specified address in the remote process.
        ///     <remarks>
        ///         Currently this is not a fully functional property for <see cref="MemorySharpType.Internal" />, due to the
        ///         Execute/etc methods in the <see cref="RemotePointer" /> class not being needed while internal.
        ///     </remarks>
        /// </summary>
        /// <param name="address">The address pointed.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A new instance of a <see cref="RemotePointer" /> class.</returns>
        public RemotePointer this[IntPtr address, bool isRelative = false]
            => new RemotePointer(this, isRelative ? MakeAbsolute(address) : address);

        /// <summary>
        ///     Factory for manipulating threads.
        /// </summary>
        public ThreadFactory Threads { get; }

        /// <summary>
        ///     Factory for manipulating windows.
        /// </summary>
        public WindowFactory Windows { get; }

        /// <summary>
        ///     Factory for finding patterns in byte data.
        /// </summary>
        public PatternFactory Patterns { get; }

        /// <summary>
        ///     The <see cref="IReadWriteMemory" /> interface.
        /// </summary>
        public IReadWriteMemory Core { get; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemorySharp" /> class.
        /// </summary>
        /// <param name="process">Process to open.</param>
        /// <param name="memorySharpType">Internal or external memory support.</param>
        public MemorySharp(Process process, MemorySharpType memorySharpType)
        {
            // Save the reference of the process
            Native = process;
            // Use the correct API depending on the architecture of the opened process
            switch (Architecture)
            {
                case ProcessArchitectures.Amd64:
                    NativeDriver = new NativeDriver64();
                    // TODO: PEB 64 ?
                    break;
                default:
                    NativeDriver = new NativeDriver32();
                    // Initialize the PEB
                    InternalPeb = new Lazy<ManagedPeb>(() => new ManagedPeb(this));
                    break;
            }
            // Open the process with all rights
            Handle = NativeDriver.MemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, process.Id);
            ImageBase = Native.MainModule.BaseAddress;
            ImageSize = Native.MainModule.ModuleMemorySize;
            Core = memorySharpType == MemorySharpType.External
                ? (IReadWriteMemory) new ExternalReadWrite(this)
                : new InternalReadWrite(this);
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
                    Patterns = new PatternFactory(this)
                });
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemorySharp" /> class.
        /// </summary>
        /// <param name="processId">Process id of the process to open.</param>
        /// <param name="memorySharpType">Internal or external memory support.</param>
        public MemorySharp(int processId, MemorySharpType memorySharpType)
            : this(ApplicationFinder.FromProcessId(processId), memorySharpType)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemorySharp" /> class.
        /// </summary>
        /// <param name="processName">The name of the process to open.</param>
        /// <param name="memorySharpType">Internal or external memory support.</param>
        public MemorySharp(string processName, MemorySharpType memorySharpType)
            : this(ApplicationFinder.FromProcessName(processName).First(), memorySharpType)
        {
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        ///     Releases all resources used by the <see cref="MemorySharp" /> object.
        /// </summary>
        public virtual void Dispose()
        {
            // Raise the event OnDispose
            OnDispose?.Invoke(this, new EventArgs());

            // Dispose all factories
            Factories.ForEach(factory => factory.Dispose());

            // Close the process handle
            Handle.Close();

            // Avoid the finalizer
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IEquatable<MemorySharp> Members
        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(MemorySharp other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Handle.Equals(other.Handle);
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
            return obj.GetType() == GetType() && Equals((MemorySharp) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        /// <summary>
        ///     Makes an absolute address from a relative one based on the main module.
        /// </summary>
        /// <param name="address">The relative address.</param>
        /// <returns>The absolute address.</returns>
        public IntPtr MakeAbsolute(IntPtr address)
        {
            return ImageBase.Add(address);
        }

        /// <summary>
        ///     Makes a relative address from an absolute one based on the main module.
        /// </summary>
        /// <param name="address">The absolute address.</param>
        /// <returns>The relative address.</returns>
        public IntPtr MakeRelative(IntPtr address)
        {
            return ImageBase.Subtract(address);
        }

        /// <summary>
        ///     Operator.
        /// </summary>
        public static bool operator ==(MemorySharp left, MemorySharp right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Operator.
        /// </summary>
        public static bool operator !=(MemorySharp left, MemorySharp right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Reads the specified amount of bytes from the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns>An array of bytes.</returns>
        public byte[] ReadBytes(IntPtr address, int count, bool isRelative = false)
        {
            return Core.ReadBytes(address, count, isRelative);
        }

        /// <summary>
        ///     Writes the specified bytes at the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        public void WriteBytes(IntPtr address, byte[] bytes, bool isRelative = false)
        {
            Core.WriteBytes(address, bytes, isRelative);
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A value.</returns>
        public T Read<T>(IntPtr address, bool isRelative = false)
        {
            return Core.Read<T>(address, isRelative);
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A value.</returns>
        public T Read<T>(Enum address, bool isRelative = false)
        {
            return Read<T>(new IntPtr(Convert.ToInt64(address)), isRelative);
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is read.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>An array.</returns>
        public T[] ReadArray<T>(IntPtr address, int count, bool isRelative = false)
        {
            return Core.ReadArray<T>(address, count, isRelative);
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is read.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>An array.</returns>
        public T[] ReadArray<T>(Enum address, int count, bool isRelative = false)
        {
            return ReadArray<T>(new IntPtr(Convert.ToInt64(address)), count, isRelative);
        }

        /// <summary>
        ///     Reads an array of bytes in the remote process.
        /// </summary>
        /// <param name="address">The address where the array is read.</param>
        /// <param name="count">The number of cells.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>The array of bytes.</returns>
        public byte[] ReadProcessMemory(IntPtr address, int count, bool isRelative = false)
        {
            return NativeDriver.MemoryCore.ReadBytes(Handle, isRelative ? MakeAbsolute(address) : address, count);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is read.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(IntPtr address, Encoding encoding, int maxLength = 224, bool isRelative = false)
        {
            return Core.ReadString(address, encoding, maxLength, isRelative);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is read.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(Enum address, Encoding encoding, bool isRelative = false, int maxLength = 512)
        {
            return ReadString(new IntPtr(Convert.ToInt64(address)), encoding, maxLength, isRelative);
        }

        /// <summary>
        ///     Reads a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(IntPtr address, bool isRelative = false, int maxLength = 512)
        {
            return ReadString(address, Encoding.UTF8, maxLength, isRelative);
        }

        /// <summary>
        ///     Reads a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is read.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string ReadString(Enum address, bool isRelative = false, int maxLength = 512)
        {
            return ReadString(new IntPtr(Convert.ToInt64(address)), isRelative, maxLength);
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void Write<T>(IntPtr address, T value, bool isRelative = false)
        {
            Core.Write(address, value, isRelative);
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void Write<T>(Enum address, T value, bool isRelative = false)
        {
            Write(new IntPtr(Convert.ToInt64(address)), value, isRelative);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is written.</param>
        /// <param name="array">The array to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteArray<T>(IntPtr address, T[] array, bool isRelative = false)
        {
            // Write the array in the remote process
            Core.WriteArray(address, array, isRelative);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="address">The address where the values is written.</param>
        /// <param name="array">The array to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void Write<T>(Enum address, T[] array, bool isRelative = false)
        {
            Write(new IntPtr(Convert.ToInt64(address)), array, isRelative);
        }

        /// <summary>
        ///     Write an array of bytes in the remote process.
        /// </summary>
        /// <param name="address">The address where the array is written.</param>
        /// <param name="byteArray">The array of bytes to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteProcessMemory(IntPtr address, byte[] byteArray, bool isRelative = false)
        {
            // Change the protection of the memory to allow writable
            using (
                new MemoryProtection(this, isRelative ? MakeAbsolute(address) : address,
                    MarshalType<byte>.Size*byteArray.Length))
            {
                // Write the byte array
                NativeDriver.MemoryCore.WriteBytes(Handle, isRelative ? MakeAbsolute(address) : address, byteArray);
            }
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteString(IntPtr address, string text, Encoding encoding, bool isRelative = false)
        {
            Core.WriteString(address, text, encoding, isRelative);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteString(Enum address, string text, Encoding encoding, bool isRelative = false)
        {
            WriteString(new IntPtr(Convert.ToInt64(address)), text, encoding, isRelative);
        }

        /// <summary>
        ///     Writes a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteString(IntPtr address, string text, bool isRelative = false)
        {
            WriteString(address, text, Encoding.UTF8, isRelative);
        }

        /// <summary>
        ///     Writes a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public void WriteString(Enum address, string text, bool isRelative = false)
        {
            WriteString(new IntPtr(Convert.ToInt64(address)), text, isRelative);
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
        /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T Call<T>(IntPtr address, CallingConventions callingConvention, params dynamic[] parameters)
        {
            return Assembly.Execute<T>(address, callingConvention, parameters);
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T Call<T>(IntPtr address, CallingConventions callingConvention)
        {
            return Assembly.Execute<T>(address, callingConvention);
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T Call<T>(IntPtr address)
        {
            return Assembly.Execute<T>(address);
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="call">The <see cref="RemoteCall" /> structure to use.</param>
        /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T Call<T>(RemoteCall call, params dynamic[] parameters)
        {
            return Assembly.Execute<T>(call.Address, call.CallingConvention, parameters);
        }

        /// <summary>
        ///     Registers a function into a delegate. Note: The delegate must provide a proper function signature!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        /// <returns>A delegate.</returns>
        public T RegisterDelegate<T>(IntPtr address, bool isRelative = false) where T : class
        {
            if (isRelative)
            {
                address = MakeAbsolute(address);
            }
            return Marshal.GetDelegateForFunctionPointer(address, typeof (T)) as T;
        }
        #endregion

        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~MemorySharp()
        {
            Dispose();
        }
    }
}