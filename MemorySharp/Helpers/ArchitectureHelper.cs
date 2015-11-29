/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Binarysharp.MemoryManagement.Logging.Interfaces;
using Binarysharp.MemoryManagement.Native;
using Binarysharp.MemoryManagement.Native.Enums;

namespace Binarysharp.MemoryManagement.Helpers
{
    /// <summary>
    ///     A class to perform simple benchmark operations on actions.
    /// </summary>
    public static class QuickBenchMark
    {
        #region Public Methods
        /// <summary>
        ///     Performs a quick profile of an action.
        ///     <remarks>This should be used for simple actions which could possibly be resource intensive.</remarks>
        /// </summary>
        /// <param name="description">Description of the action being profiled.</param>
        /// <param name="iterations">Total iterations to perform.</param>
        /// <param name="func">The action to profile.</param>
        /// <param name="log">The <see cref="ILog" /> instance for this class to use.</param>
        public static void BenchmarkAction(string description, int iterations, Action func, ILog log)
        {
            // Warm up 
            func();

            var watch = new Stopwatch();

            // Clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            watch.Start();
            for (var i = 0; i < iterations; i++)
            {
                func();
            }
            watch.Stop();
            log.LogInfo("Total time to complete: " +
                        watch.Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Performs a quick profile of an action.
        ///     <remarks>This should be used for simple actions which could possibly be resource intensive.</remarks>
        /// </summary>
        /// <param name="description">Description of the action being profiled.</param>
        /// <param name="iterations">Total iterations to perform.</param>
        /// <param name="func">The action to profile.</param>
        /// <param name="log">The <see cref="ILog" /> instance for this class to use.</param>
        public static void BenchmarkAction(string description, int iterations, Action func, IManagedLog log)
        {
            // Warm up 
            func();

            var watch = new Stopwatch();

            // Clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            watch.Start();
            for (var i = 0; i < iterations; i++)
            {
                func();
            }
            watch.Stop();
            log.LogInfo("Total time to complete: " + watch.Elapsed.TotalMilliseconds);
        }
        #endregion
    }

    /// <summary>
    ///     Static helper class providing tools for detecting architecture.
    /// </summary>
    public static class ArchitectureHelper
    {
        #region Public Properties, Indexers
        /// <summary>
        ///     Determines whether the current process is a 32-bit process.
        /// </summary>
        public static bool Is32BitProcess => !Is64BitProcess;

        /// <summary>
        ///     Determines whether the current process is a 64-bit process.
        /// </summary>
        public static bool Is64BitProcess => Environment.Is64BitProcess;

        /// <summary>
        ///     Determines whether the current operating system is a 32-bit operating system.
        /// </summary>
        public static bool Is32BitOperatingSystem => !Is64BitOperatingSystem;

        /// <summary>
        ///     Determines whether the current operating system is a 64-bit operating system.
        /// </summary>
        public static bool Is64BitOperatingSystem => Environment.Is64BitOperatingSystem;
        #endregion

        #region Public Methods
        /// <summary>
        ///     Determines whether the specified process is running under WOW64.
        ///     WOW64 is the x86 emulator that allows 32-bit Windows-based applications to run seamlessly on 64-bit Windows.
        /// </summary>
        /// <param name="process">The process to analyse.</param>
        /// <returns>A value indicating whether the process is running under WOW64.</returns>
        public static bool IsWoW64Process(Process process)
        {
            // Check the library kernel32 contains the function
            if (!NativeMethods.DoesWin32MethodExist("kernel32.dll", "IsWow64Process"))
                return false;

            // Create a var to store the result
            bool x86Emulator;
            // Determine if the process is running under the x86 emulator
            if (!NativeMethods.IsWow64Process(process.Handle, out x86Emulator))
                throw new Win32Exception($"Couldn't determine if the process '{process.ProcessName}' is using WOW64.");

            // Return the result
            return x86Emulator;
        }

        /// <summary>
        ///     Gets the architecture of a specified process.
        /// </summary>
        /// <param name="process">The process to analyse.</param>
        /// <returns>The architecture of the process.</returns>
        public static ProcessArchitectures GetArchitectureByProcess(Process process)
        {
            // Set the 32-bit architecture as default value
            var architecture = ProcessArchitectures.Ia32;

            // Check if the operating system is 64-bit
            if (Is64BitOperatingSystem)
            {
                // Determine if the process is running under the x86 emulator
                if (!IsWoW64Process(process))
                    architecture = ProcessArchitectures.Amd64;
            }

            return architecture;
        }
        #endregion
    }
}