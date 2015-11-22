/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2013 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.ComponentModel;
using System.Linq;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Native.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MemorySharpTests.Memory
{
    [TestClass]
    public class MemoryCore32Tests
    {
        /// <summary>
        /// Use the 32-bit memory driver.
        /// </summary>
        protected IMemoryCore MemoryCore = new MemoryCore32();
        /// <summary>
        /// The identifier of the process test.
        /// </summary>
        protected int TestProcessId = Resources.TestProcess32Bit.Id;

        /// <summary>
        /// Open and close a process.
        /// </summary>
        [TestMethod]
        public void OpenCloseProcess()
        {
            // Arrange
            var processId = TestProcessId;
            SafeMemoryHandle handle = null;

            // Act
            try
            {
                // Open the process and store the handle within a safe handle container
                handle = MemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, processId);
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The process couldn't be opened correctly: " + ex.Message);
            }

            try
            {
                // Close the handle
                MemoryCore.CloseHandle(handle.DangerousGetHandle());
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The handle couldn't be closed correctly: " + ex.Message);
            }

            // Remove the finalizer execution from the safe handle
            GC.SuppressFinalize(handle);
        }

        /// <summary>
        /// Allocates and free a memory page.
        /// </summary>
        [TestMethod]
        public void AllocateFree()
        {
            // Arrange
            var handle = MemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, TestProcessId);
            var address = IntPtr.Zero;

            // Act
            try
            {
                // Allocate a chunk of memory
                address = MemoryCore.AllocateMemory(handle, 1);
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The memory couldn't be allocated: " + ex.Message);
            }

            try
            {
                // Release the memory
                MemoryCore.FreeMemory(handle, address);
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The memory couldn't be freed: " + ex.Message);
            }
        }

        /// <summary>
        /// Change the protection, writes and reads the first bytes of the memory.
        /// </summary>
        [TestMethod]
        public void ChangeProtectionWriteReadBytes()
        {
            // Arrange
            var handle = MemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, TestProcessId);
            var expected = new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 };
            var memory = new IntPtr(0x00400000);

            // Act
            try
            {
                // Change the protection of the memory
                MemoryCore.ChangeMemoryProtection(handle, memory, 5, MemoryProtectionFlags.ExecuteReadWrite);
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The protection couldn't be changed: " + ex.Message);
            }

            try
            {
                // Write the collection of bytes
                MemoryCore.WriteBytes(handle, memory, expected);
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The collection bytes couldn't be written within the process: " + ex.Message);
            }

            try
            {
                // Read a collection of bytes
                var actual = MemoryCore.ReadBytes(handle, memory, 5);
                // Check the collection is equal to injected one
                CollectionAssert.AreEqual(expected, actual, "The collections are not equal.");
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The collection of bytes couldn't be read from the process: " + ex.Message);
            }
        }

        /// <summary>
        /// List all memory regions.
        /// </summary>
        [TestMethod]
        public void QueryInformationMemory_ListAllMemoryPages()
        {
            // Arrange
            var handle = MemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, TestProcessId);
            var starting = new IntPtr(0);
            var ending = new IntPtr(0x07fffffff);

            // Act
            try
            {
                // Query the memory
                var regions = MemoryCore.QueryInformationMemory(handle, starting, ending);
                // Assert
                Assert.AreNotEqual(0, regions.Count(), "Memory pages cannot be gathered.");
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The memory couldn't be queried: " + ex.Message);
            }
        }

        /// <summary>
        /// Checks the memory protection of the main module.
        /// </summary>
        [TestMethod]
        public void QueryInformationMemory_CheckProtection()
        {
            // Arrange
            var handle = MemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, TestProcessId);
            var address = new IntPtr(0x400000);

            // Act
            try
            {
                // query the memory
                var information = MemoryCore.QueryInformationMemory(handle, address);
                // Assert
                Assert.AreEqual(MemoryProtectionFlags.ReadOnly, information.Protect, "The protection of the main module is not set in Read-Only.");
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The memory couldn't be queried: " + ex.Message);
            }
        }

        /// <summary>
        /// Checks the process id using QueryInformationProcess.
        /// </summary>
        [TestMethod]
        public void QueryInformationProcess_CheckProcessId()
        {
            // Arrange
            var handle = MemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, TestProcessId);

            // Act
            try
            {
                // Query the process
                var information = MemoryCore.QueryInformationProcess(handle);
                // Assert
                Assert.AreEqual(TestProcessId, information.ProcessId, "The process identifier is not correct.");
            }
            catch (Win32Exception ex)
            {
                // Assert
                Assert.Fail("The process couldn't be queried: " + ex.Message);
            }
        }
    }
}
