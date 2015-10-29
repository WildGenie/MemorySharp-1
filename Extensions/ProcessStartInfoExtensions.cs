using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MemorySharp.Internals;
using MemorySharp.Native;

namespace MemorySharp.Extensions
{
    /// <summary>
    ///     A class providing extension methods for <see cref="Process" /> Instance's.
    ///     <remarks>
    ///         eturns>Unfinshed documentation. Most credits go to: "Jeffora"'s extememory project.
    ///         https://github.com/jeffora/extemory
    ///     </remarks>
    /// </summary>
    public static class ProcessStartInfoExtensions
    {
        #region Methods
        public static Process CreateProcessWithFlags(this ProcessStartInfo startInfo, ProcessCreationFlags creationFlags)
        {
            if (string.IsNullOrEmpty(startInfo.FileName))
                throw new ArgumentException("No FileName was specified in ProcessStartInfo", nameof(startInfo));
            if (!File.Exists(startInfo.FileName))
                throw new FileNotFoundException("Unable to find the specified the process file", startInfo.FileName);

            var startupInfo = new Startupinfo();
            startupInfo.cb = Marshal.SizeOf(startupInfo);

            var args = string.IsNullOrEmpty(startInfo.Arguments) ? null : new StringBuilder(startInfo.Arguments);
            var workingDirectory = string.IsNullOrEmpty(startInfo.WorkingDirectory) ? null : startInfo.WorkingDirectory;

            var procInfo = new ProcessInformation();

            if (
                !NativeMethods.CreateProcess(startInfo.FileName, args, IntPtr.Zero, IntPtr.Zero, false, creationFlags,
                    IntPtr.Zero, workingDirectory, ref startupInfo, out procInfo))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var ret = Process.GetProcessById(procInfo.dwProcessId);
            ProcessMemoryManager.ForProcess(ret).MainThreadId = procInfo.dwThreadId;
            return ret;
        }

        public static Process CreateProcessSuspended(this ProcessStartInfo startInfo)
        {
            return startInfo.CreateProcessWithFlags(ProcessCreationFlags.CreateSuspended);
        }
        #endregion
    }
}