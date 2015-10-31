using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Binarysharp.MemoryManagement.Extensions
{
    public static class ProcessExtensionMethods
    {
        #region Methods
        public static IEnumerable<ProcessThread> GetProcessThreads(this Process process)
        {
            // Refresh the process info
            process.Refresh();
            // Enumerates all threads
            return process.Threads.Cast<ProcessThread>();
        }
        #endregion
    }
}