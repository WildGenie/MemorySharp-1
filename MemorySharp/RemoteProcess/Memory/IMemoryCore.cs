using System;
using System.Collections.Generic;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.MemoryManagement.RemoteProcess.Memory
{
    /// <summary>
    ///     Defines a system and architecture specific way to query the operating system about its memory.
    /// </summary>
    public interface IMemoryCore
    {
        /// <summary>
        ///     Reserves a region of memory within the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">The handle to a process.</param>
        /// <param name="size">The size of the region of memory to allocate, in bytes.</param>
        /// <param name="protectionFlags">The memory protection for the region of pages to be allocated.</param>
        /// <param name="allocationFlags">The type of memory allocation.</param>
        /// <returns>The base address of the allocated region.</returns>
        IntPtr AllocateMemory(SafeMemoryHandle processHandle, int size,
                              MemoryProtectionFlags protectionFlags = MemoryProtectionFlags.ExecuteReadWrite,
                              MemoryAllocationFlags allocationFlags = MemoryAllocationFlags.Commit);

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
        MemoryProtectionFlags ChangeMemoryProtection(SafeMemoryHandle processHandle, IntPtr address, int size,
                                                     MemoryProtectionFlags protection);

        /// <summary>
        ///     Closes an open object handle.
        /// </summary>
        /// <param name="handle">A valid handle to an open object.</param>
        void CloseHandle(IntPtr handle);

        /// <summary>
        ///     Releases a region of memory within the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">A handle to a process.</param>
        /// <param name="address">A pointer to the starting address of the region of memory to be freed.</param>
        void FreeMemory(SafeMemoryHandle processHandle, IntPtr address);

        /// <summary>
        ///     Retrieves information about the specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process to query.</param>
        /// <returns>A <see cref="ProcessBasicInformation" /> structure containg process information.</returns>
        ProcessBasicInformation QueryInformationProcess(SafeMemoryHandle processHandle);

        /// <summary>
        ///     Opens an existing local process object.
        /// </summary>
        /// <param name="accessFlags">The access level to the process object.</param>
        /// <param name="processId">The identifier of the local process to be opened.</param>
        /// <returns>An open handle to the specified process.</returns>
        SafeMemoryHandle OpenProcess(ProcessAccessFlags accessFlags, int processId);

        /// <summary>
        ///     Reads an array of bytes in the memory form the target process.
        /// </summary>
        /// <param name="processHandle">A handle to the process with memory that is being read.</param>
        /// <param name="address">A pointer to the base address in the specified process from which to read.</param>
        /// <param name="size">The number of bytes to be read from the specified process.</param>
        /// <returns>The collection of read bytes.</returns>
        byte[] ReadBytes(SafeMemoryHandle processHandle, IntPtr address, int size);

        /// <summary>
        ///     Retrieves information about a range of pages within the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process whose memory information is queried.</param>
        /// <param name="baseAddress">A pointer to the base address of the region of pages to be queried.</param>
        /// <returns>
        ///     A <see cref="Native.MemoryBasicInformation" /> structures in which information about the specified page range
        ///     is returned.
        /// </returns>
        MemoryBasicInformation QueryInformationMemory(SafeMemoryHandle processHandle, IntPtr baseAddress);

        /// <summary>
        ///     Retrieves information about a range of pages within the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process whose memory information is queried.</param>
        /// <param name="addressFrom">A pointer to the starting address of the region of pages to be queried.</param>
        /// <param name="addressTo">A pointer to the ending address of the region of pages to be queried.</param>
        /// <returns>A collection of <see cref="Native.MemoryBasicInformation" /> structures.</returns>
        IEnumerable<MemoryBasicInformation> QueryInformationMemory(SafeMemoryHandle processHandle, IntPtr addressFrom,
                                                                   IntPtr addressTo);

        /// <summary>
        ///     Writes data to an area of memory in a specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process memory to be modified.</param>
        /// <param name="address">A pointer to the base address in the specified process to which data is written.</param>
        /// <param name="byteArray">A buffer that contains data to be written in the address space of the specified process.</param>
        /// <returns>The number of bytes written.</returns>
        int WriteBytes(SafeMemoryHandle processHandle, IntPtr address, byte[] byteArray);
    }
}