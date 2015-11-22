using System;
using Binarysharp.MemoryManagement.Helpers;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Native.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MemorySharpTests.Helpers
{
    [TestClass]
    public class ArchitectureHelperTests
    {
        /// <summary>
        /// Checks if the 32-bit program is correctly recognized as IA32 architecture.
        /// </summary>
        [TestMethod]
        public void GetProcessArchitecture_On32BitProcess()
        {
            // Arrange
            var process = Resources.TestProcess32Bit;

            // Act
            var architecture = ArchitectureHelper.GetArchitectureByProcess(process);

            // Assert
            Assert.AreEqual(ProcessArchitectures.Ia32, architecture, "The architecture must be IA32 (32-bit).");
        }

        /// <summary>
        /// Checks if the 64-bit program is correctly recognized as AMD64 architecture.
        /// </summary>
        [TestMethod]
        public void GetProcessArchitecture_On64BitProcess()
        {
            // Arrange
            Resources.InterruptWhenNot64BitOs();
            var process = Resources.TestProcess64Bit;

            // Act
            var architecture = ArchitectureHelper.GetArchitectureByProcess(process);

            // Assert
            Assert.AreEqual(ProcessArchitectures.Amd64, architecture, "The architecture must be AMD64 (64-bit).");
        }

        /// <summary>
        /// Checks the architecture of the running process.
        /// </summary>
        [TestMethod]
        public void Is32BitProcess()
        {
            // Act
            var value = ArchitectureHelper.Is32BitProcess;
            var check1 = !Environment.Is64BitProcess;
            var check2 = IntPtr.Size == 4;

            // Assert
            Assert.AreEqual(check1, value, "The architecture of the current process couldn't be correctly determined.");
            Assert.AreEqual(check2, value, "The architecture of the current process couldn't be correctly determined.");
        }

        /// <summary>
        /// Checks the architecture of the running process.
        /// </summary>
        [TestMethod]
        public void Is64BitProcess()
        {
            // Arrange
            Resources.InterruptWhenNot64BitOs();

            // Act
            var value = ArchitectureHelper.Is64BitProcess;
            var check1 = Environment.Is64BitProcess;
            var check2 = IntPtr.Size == 8;

            // Assert
            Assert.AreEqual(check1, value, "The architecture of the current process couldn't be correctly determined.");
            Assert.AreEqual(check2, value, "The architecture of the current process couldn't be correctly determined.");
        }

        /// <summary>
        /// Checks if the the x86 emulator is correctly detected.
        /// Only works on 64-bit operating system.
        /// </summary>
        [TestMethod]
        public void IsWoW64Process()
        {
            // Arrange
            Resources.InterruptWhenNot64BitOs();

            var process32Bit = Resources.TestProcess32Bit;
            var process64Bit = Resources.TestProcess64Bit;

            // Act
            var mustBeActivated = ArchitectureHelper.IsWoW64Process(process32Bit);
            var mustBeDisabled = ArchitectureHelper.IsWoW64Process(process64Bit);

            // Assert
            Assert.IsTrue(mustBeActivated, "The 32-bit process doesn't run under WOW64.");
            Assert.IsFalse(mustBeDisabled, "The 64-bit process runs under WOW64.");
        }
    }
}
