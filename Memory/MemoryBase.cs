using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MemorySharp.Extensions;
using MemorySharp.Helpers;
using MemorySharp.Internals.Exceptions;
using MemorySharp.Internals.Interfaces;
using MemorySharp.Internals.Marshaling;
using MemorySharp.Modules;
using MemorySharp.Native;
using MemorySharp.Patterns;
using MemorySharp.Threads;
using MemorySharp.Windows;

namespace MemorySharp.Memory
{
    public abstract class MemoryBase : IDisposable
    {
        #region  Fields
        /// <summary>
        ///     The factories embedded inside the library.
        /// </summary>
        protected List<IFactory> Factories;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryBase" /> class.
        /// </summary>
        /// <param name="process">The process.</param>
        protected MemoryBase(Process process)
        {
            // Requires.NotNull(process, nameof(process));
            Process = process;
            Handle = OpenProcess(ProcessAccessFlags.AllAccess, process.Id);
            BaseAddress = Process.MainModule.BaseAddress;
            Process.EnableRaisingEvents = true;
            ImageSize = Process.MainModule.ModuleMemorySize;
            Factories = new List<IFactory>();
            Factories.AddRange(
                new IFactory[]
                {
                    Memory = new MemoryFactory(this),
                    Modules = new ModuleFactory(this),
                    Threads = new ThreadFactory(this),
                    Windows = new WindowFactory(this),
                    Patterns = new PatternFactory(this)
                });
            MainModule = new RemoteModule(this, Process.MainModule);
        }
        #endregion

        #region  Properties
        /// <summary>
        ///     Factory for finding patterns in byte data.
        /// </summary>
        public PatternFactory Patterns { get; }

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
            => new RemotePointer(this, isRelative ? ToAbsolute(address) : address);

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
        ///     Factory for manipulating threads.
        /// </summary>
        public ThreadFactory Threads { get; }

        /// <summary>
        ///     Factory for manipulating windows.
        /// </summary>
        public WindowFactory Windows { get; }

        /// <summary>
        ///     The remote module Instance of the main process module.
        /// </summary>
        public RemoteModule MainModule { get; }

        /// <summary>
        ///     The size of the main process module.
        /// </summary>
        public int ImageSize { get; }

        /// <summary>
        ///     State if the process is running.
        /// </summary>
        public bool IsRunning => !Handle.IsInvalid && !Handle.IsClosed && !Process.HasExited;

        /// <summary>
        ///     Gets or sets the base address of the wrapped process' main module.
        /// </summary>
        public IntPtr BaseAddress { get; protected set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets or sets the process this NativeMemory instance is wrapped around.
        /// </summary>
        public Process Process { [Pure] get; protected set; }
        #endregion

        #region  Interface members
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
                return;
            // Pretty much all we "have" to clean up.
            Process.LeaveDebugMode();

            IsDisposed = true;
        }
        #endregion

        #region Methods
        public byte[] ReadProcessMemory(IntPtr address, int size, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            // Check if the handles are valid
            HandleManipulator.ValidateAsArgument(Handle, "processHandle");
            HandleManipulator.ValidateAsArgument(address, "address");

            // Allocate the buffer
            var buffer = new byte[size];
            IntPtr nbBytesRead;

            // ReadArray the data from the target process
            if (NativeMethods.ReadProcessMemory(Handle, address, buffer, size, out nbBytesRead) &&
                size == nbBytesRead.ToInt64())
                return buffer;

            // Else the data couldn't be read, throws an exception
            throw new Win32Exception($"Couldn't read {size} byte(s) from 0x{address.ToString("X")}.");
        }

        public int WriteProcessMemory(IntPtr address, byte[] byteArray, bool isRelative = false)
        {
            if (isRelative)
            {
                address = ToAbsolute(address);
            }
            // Check if the handles are valid
            HandleManipulator.ValidateAsArgument(Handle, "processHandle");
            HandleManipulator.ValidateAsArgument(address, "address");

            // Create the variable storing the number of bytes written
            IntPtr nbBytesWritten;

            // Write the data to the target process
            if (NativeMethods.WriteProcessMemory(Handle, address, byteArray, byteArray.Length, out nbBytesWritten))
            {
                // Check whether the length of the data written is equal to the inital array
                if (nbBytesWritten.ToInt64() == byteArray.Length)
                    return byteArray.Length;
            }

            // Else the data couldn't be written, throws an exception
            throw new Win32Exception($"Couldn't write {byteArray.Length} bytes to 0x{address.ToString("X")}");
        }

        /// <summary>
        ///     Reserves a region of memory within the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">The handle to a process.</param>
        /// <param name="size">The size of the region of memory to allocate, in bytes.</param>
        /// <param name="protectionFlags">The memory protection for the region of pages to be allocated.</param>
        /// <param name="allocationFlags">The type of memory allocation.</param>
        /// <returns>The base address of the allocated region.</returns>
        public IntPtr AllocateMemory(SafeMemoryHandle processHandle, int size,
            MemoryProtectionFlags protectionFlags = MemoryProtectionFlags.ExecuteReadWrite,
            MemoryAllocationFlags allocationFlags = MemoryAllocationFlags.Commit)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(processHandle, "processHandle");

            // Allocate a memory page
            var ret = NativeMethods.VirtualAllocEx(processHandle, IntPtr.Zero, size, allocationFlags, protectionFlags);

            // Check whether the memory page is valid
            if (ret != IntPtr.Zero)
                return ret;

            // If the pointer isn't valid, throws an exception
            throw new Win32Exception($"Couldn't allocate memory of {size} byte(s).");
        }

        /// <summary>
        ///     Converts the specified absolute address to a relative address.
        /// </summary>
        /// <param name="absoluteAddress">The absolute address.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">absoluteAddress may not be IntPtr.Zero.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr ToRelative(IntPtr absoluteAddress)
        {
            Requires.NotEqual(absoluteAddress, IntPtr.Zero, nameof(absoluteAddress));

            return BaseAddress.Subtract(absoluteAddress);
        }

        /// <summary>
        ///     Converts the specified relative address to an absolute address.
        /// </summary>
        /// <param name="relativeAddress">The relative address.</param>
        /// <returns></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr ToAbsolute(IntPtr relativeAddress)
        {
            // In this case, we allow IntPtr zero - relative + base = base, so no harm done.
            return BaseAddress.Add(relativeAddress);
        }

        /// <summary>
        ///     Closes an open object handle.
        /// </summary>
        /// <param name="handle">A valid handle to an open object.</param>
        public void CloseHandle(IntPtr handle)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(handle, "handle");

            // Close the handle
            if (!NativeMethods.CloseHandle(handle))
            {
                throw new Win32Exception($"Couldn't close he handle 0x{handle}.");
            }
        }

        /// <summary>
        ///     Releases a region of memory within the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">A handle to a process.</param>
        /// <param name="address">A pointer to the starting address of the region of memory to be freed.</param>
        public void FreeMemory(IntPtr address)
        {
            // Check if the handles are valid
            HandleManipulator.ValidateAsArgument(Handle, "processHandle");
            HandleManipulator.ValidateAsArgument(address, "address");

            // Free the memory
            if (!NativeMethods.VirtualFreeEx(Handle, address, 0, MemoryReleaseFlags.Release))
            {
                // If the memory wasn't correctly freed, throws an exception
                throw new Win32Exception($"The memory page 0x{address.ToString("X")} cannot be freed.");
            }
        }

        /// <summary>
        ///     Opens an existing local process object.
        /// </summary>
        /// <param name="accessFlags">The access level to the process object.</param>
        /// <param name="processId">The identifier of the local process to be opened.</param>
        /// <returns>An open handle to the specified process.</returns>
        public SafeMemoryHandle OpenProcess(ProcessAccessFlags accessFlags, int processId)
        {
            // Get an handle from the remote process
            var handle = NativeMethods.OpenProcess(accessFlags, false, processId);

            // Check whether the handle is valid
            if (!handle.IsInvalid && !handle.IsClosed)
                return handle;

            // Else the handle isn't valid, throws an exception
            throw new Win32Exception($"Couldn't open the process {processId}.");
        }

        /// <summary>
        ///     Changes the protection on a region of committed pages in the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process whose memory protection is to be changed.</param>
        /// <param name="address">
        ///     A pointer to the base address of the region of pages whose access protection attributes are to be
        ///     changed.
        /// </param>
        /// <param name="size">The size of the region whose access protection attributes are changed, in bytes.</param>
        /// <param name="protection">The memory protection option.</param>
        /// <returns>The old protection of the region in a <see cref="Native.MemoryBasicInformation" /> structure.</returns>
        public MemoryProtectionFlags ChangeMemoryProtection(IntPtr address, int size,
            MemoryProtectionFlags protection)
        {
            // Check if the handles are valid
            HandleManipulator.ValidateAsArgument(Handle, "processHandle");
            HandleManipulator.ValidateAsArgument(address, "address");

            // Create the variable storing the old protection of the memory page
            MemoryProtectionFlags oldProtection;

            // Change the protection in the target process
            if (NativeMethods.VirtualProtectEx(Handle, address, size, protection, out oldProtection))
            {
                // Return the old protection
                return oldProtection;
            }

            // Else the protection couldn't be changed, throws an exception
            throw new Win32Exception(
                $"Couldn't change the protection of the memory at 0x{address.ToString("X")} of {size} byte(s) to {protection}.");
        }

        /// <summary>
        ///     Retrieves information about a range of pages within the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process whose memory information is queried.</param>
        /// <param name="baseAddress">A pointer to the base address of the region of pages to be queried.</param>
        /// <returns>
        ///     A <see cref="Native.MemoryBasicInformation" /> structures in which information about the specified page range
        ///     is returned.
        /// </returns>
        public MemoryBasicInformation QueryInformationMemory(IntPtr baseAddress)
        {
            // Allocate the structure to store information of memory
            MemoryBasicInformation memoryInfo;

            // Query the memory region
            if (
                NativeMethods.VirtualQueryEx(Handle, baseAddress, out memoryInfo,
                    MarshalType<MemoryBasicInformation>.Size) != 0)
                return memoryInfo;

            // Else the information couldn't be got
            throw new Win32Exception($"Couldn't query information about the memory region 0x{baseAddress.ToString("X")}");
        }

        /// <summary>
        ///     Retrieves information about a range of pages within the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process whose memory information is queried.</param>
        /// <param name="addressFrom">A pointer to the starting address of the region of pages to be queried.</param>
        /// <param name="addressTo">A pointer to the ending address of the region of pages to be queried.</param>
        /// <returns>A collection of <see cref="Native.MemoryBasicInformation" /> structures.</returns>
        public IEnumerable<MemoryBasicInformation> QueryInformationMemory(
            IntPtr addressFrom, IntPtr addressTo)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(Handle, "processHandle");

            // Convert the addresses to Int64
            var numberFrom = addressFrom.ToInt64();
            var numberTo = addressTo.ToInt64();

            // The first address must be lower than the second
            if (numberFrom >= numberTo)
                throw new ArgumentException("The starting address must be lower than the ending address.",
                    nameof(addressFrom));

            // Create the variable storing the result of the call of VirtualQueryEx
            int ret;

            // Enumerate the memory pages
            do
            {
                // Allocate the structure to store information of memory
                MemoryBasicInformation memoryInfo;

                // Get the next memory page
                ret = NativeMethods.VirtualQueryEx(Handle, new IntPtr(numberFrom), out memoryInfo,
                    MarshalType<MemoryBasicInformation>.Size);

                // Increment the starting address with the size of the page
                numberFrom += memoryInfo.RegionSize;

                // Return the memory page
                if (memoryInfo.State != MemoryStateFlags.Free)
                    yield return memoryInfo;
            } while (numberFrom < numberTo && ret != 0);
        }

        /// <summary>
        ///     Retrieves information about the specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process to query.</param>
        /// <returns>A <see cref="ProcessBasicInformation" /> structure containg process information.</returns>
        public ProcessBasicInformation QueryInformationProcess()
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(Handle, "processHandle");

            // Create a structure to store process info
            var info = new ProcessBasicInformation();

            // Get the process info
            var ret = NativeMethods.NtQueryInformationProcess(Handle,
                ProcessInformationClass.ProcessBasicInformation, ref info, info.Size, IntPtr.Zero);

            // If the function succeeded
            if (ret == 0)
                return info;

            // Else, couldn't get the process info, throws an exception
            throw new ApplicationException(
                $"Couldn't get the information from the process, error code '{ret}'.");
        }

        // We can declare string reading and writing right here - they won't differ based on whether we're internal or external.
        // The actual memory read/write methods depend on whether or not we're injected - we'll have to resort to RPM/WPM for external,
        // while injected libraries can simply use Marshal.Copy back and forth.
        // Theoretically these methods could be marked as [Pure] as they do not modify the state of the object itself, but rather
        // of the process they are currently "manipulating".
        /// <summary>
        ///     Reads a string with the specified encoding at the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref>
        ///         <name>startIndex</name>
        ///     </paramref>
        ///     is less than zero.-or-
        ///     <paramref>
        ///         <name>startIndex</name>
        ///     </paramref>
        ///     specifies a position that is not within this string.
        /// </exception>
        /// <exception cref="ArgumentNullException">Encoding may not be null.</exception>
        /// <exception cref="ArgumentException">Address may not be IntPtr.Zero.</exception>
        /// <exception cref="DecoderFallbackException">
        ///     A fallback occurred (see Character Encoding in the .NET Framework for
        ///     complete explanation)-and-<see cref="P:System.Text.Encoding.DecoderFallback" /> is set to
        ///     <see cref="T:System.Text.DecoderExceptionFallback" />.
        /// </exception>
        public virtual string ReadString(IntPtr address, Encoding encoding, int maximumLength = 512,
            bool isRelative = false)
        {
            Requires.NotEqual(address, IntPtr.Zero, nameof(address));
            Requires.NotNull(encoding, nameof(encoding));

            var buffer = ReadBytes(address, maximumLength, isRelative);
            var ret = encoding.GetString(buffer);
            if (ret.IndexOf('\0') != -1)
            {
                ret = ret.Remove(ret.IndexOf('\0'));
            }
            return ret;
        }

        /// <summary>
        ///     Writes the specified string at the specified address using the specified encoding.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Address may not be IntPtr.Zero.</exception>
        /// <exception cref="ArgumentNullException">Encoding may not be null.</exception>
        /// <exception cref="IndexOutOfRangeException">
        ///     <paramref>
        ///         <name>index</name>
        ///     </paramref>
        ///     is greater than or equal to the length of this
        ///     object or less than zero.
        /// </exception>
        /// <exception cref="EncoderFallbackException">
        ///     A fallback occurred (see Character Encoding in the .NET Framework for
        ///     complete explanation)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to
        ///     <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        public virtual void WriteString(IntPtr address, string value, Encoding encoding, bool isRelative = false)
        {
            Requires.NotEqual(address, IntPtr.Zero, nameof(address));
            Requires.NotNull(encoding, nameof(encoding));

            if (value[value.Length - 1] != '\0')
            {
                value += '\0';
            }

            WriteBytes(address, encoding.GetBytes(value), isRelative);
        }

        /// <summary>
        ///     Reads a value of the specified type at the specified address. This method is used if multiple-pointer dereferences
        ///     are required.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <param name="addresses">The addresses.</param>
        /// <returns></returns>
        /// <exception cref="MemorySharpReadException">
        ///     Thrown if the ReadProcessMemory operation fails, or doesn't return the
        ///     specified amount of bytes.
        /// </exception>
        /// <exception cref="MissingMethodException">
        ///     The class specified by <paramref name="T" /> does not have an accessible
        ///     default constructor.
        /// </exception>
        /// <exception cref="ArgumentException">Address may not be zero, and count may not be zero.</exception>
        /// <exception cref="OverflowException">
        ///     On a 64-bit platform, the value of this instance is too large or too small to
        ///     represent as a 32-bit signed integer.
        /// </exception>
        public virtual T ReadMultiLevelPointer<T>(bool isRelative = false, params IntPtr[] addresses) where T : struct
        {
            Requires.Condition(() => addresses.Length > 0, nameof(addresses));
            Requires.NotEqual(addresses[0], IntPtr.Zero, nameof(addresses));
            if (isRelative)
            {
                addresses[0] = ToAbsolute(addresses[0]);
            }
            // We can just read right away if it's a single address - avoid the hassle.
            if (addresses.Length == 1)
            {
                return Read<T>(addresses[0]);
            }
            var tempPtr = Read<IntPtr>(addresses[0]);

            for (var i = 1; i < addresses.Length - 1; i++)
            {
                tempPtr = Read<IntPtr>(tempPtr + addresses[i].ToInt32());
            }
            return Read<T>(tempPtr, isRelative);
        }

        /// <summary>
        ///     Writes the specified value at the specified address. This method is used if multiple-pointer dereferences are
        ///     required.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <param name="value">The value.</param>
        /// <param name="addresses">The addresses.</param>
        /// <returns></returns>
        /// <exception cref="MissingMethodException">
        ///     The class specified by <paramref name="T" /> does not have an accessible
        ///     default constructor.
        /// </exception>
        /// <exception cref="MemorySharpReadException">
        ///     Thrown if the ReadProcessMemory operation fails, or doesn't return the
        ///     specified amount of bytes.
        /// </exception>
        /// <exception cref="MemorySharpWriteException">WriteProcessMemory failed.</exception>
        /// <exception cref="OverflowException">
        ///     The array is multidimensional and contains more than
        ///     <see cref="F:System.Int32.MaxValue" /> elements.
        /// </exception>
        public virtual void Write<T>(bool isRelative, T value = default(T), params IntPtr[] addresses) where T : struct
        {
            Requires.Condition(() => addresses.Length > 0, nameof(addresses));
            Requires.NotEqual(addresses[0], IntPtr.Zero, nameof(addresses));
            if (isRelative)
            {
                addresses[0] = ToAbsolute(addresses[0]);
            }
            // If a single addr is passed, just write it right away.
            if (addresses.Length == 1)
            {
                Write(addresses[0], value, isRelative);
                return;
            }

            // Same thing as sequential reads - we read until we find the last addr, then we write to it.
            var tempPtr = Read<IntPtr>(addresses[0]);

            for (var i = 1; i < addresses.Length - 1; i++)
                tempPtr = Read<IntPtr>(tempPtr + addresses[i].ToInt32());

            Write(tempPtr + addresses.Last().ToInt32(), value);
        }

        /// <summary>
        ///     Reads the specified amount of bytes from the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public abstract byte[] ReadBytes(IntPtr address, int count, bool isRelative = false);

        /// <summary>
        ///     Writes the specified bytes at the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public abstract int WriteBytes(IntPtr address, byte[] bytes, bool isRelative = false);

        /// <summary>
        ///     Reads a value of the specified type at the specified address.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public abstract T Read<T>(IntPtr address, bool isRelative = false) where T : struct;

        /// <summary>
        ///     Reads the specified amount of values of the specified type at the specified address.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public abstract T[] ReadArray<T>(IntPtr address, int count, bool isRelative = false) where T : struct;

        /// <summary>
        ///     Writes the specified value at the specfied address.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="isRelative">if set to <c>true</c> [is relative].</param>
        /// <returns></returns>
        public abstract void Write<T>(IntPtr address, T value, bool isRelative = false);

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The array of values to write.</param>
        /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
        public abstract void WriteArray<T>(IntPtr address, T[] value, bool isRelative = false);
#pragma warning disable CS1998
        /// Async method lacks 'await' operators and will run synchronously
        /// <summary>
        ///     Called when the process this Memory instance is attach to exits.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="eventArgs">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <returns>Asycnh task.</returns>
        protected virtual async Task OnExited(int exitCode, EventArgs eventArgs)
            // Async method lacks 'await' operators and will run synchronously
        {
        }
#pragma warning restore CS1998
        #endregion
    }
}