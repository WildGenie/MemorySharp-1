using System;
using System.Diagnostics;
using Binarysharp.MemoryManagement.MemoryInternal.Interfaces;

namespace Binarysharp.MemoryManagement.Helpers
{
    public enum QuickProfileWriteMode
    {
    }

    /// <summary>
    ///     A class to perform simple benchmark operations on actions.
    /// </summary>
    public static class QuickBenchMark
    {
        #region Methods
        /// <summary>
        ///     Performs a quick profile of an action.
        ///     <remarks>This should be used for simple actions which could possibly be resource intensive.</remarks>
        /// </summary>
        /// <param name="description">Description of the action being profiled.</param>
        /// <param name="iterations">Total iterations to perform.</param>
        /// <param name="func">The action to profile.</param>
        /// <param name="log">The logger to use.</param>
        public static void ProfileAction(string description, int iterations, Action func, ILog log)
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


            log.LogInfo(" Time Elapsed in ms: " + watch.Elapsed.TotalMilliseconds);
        }
        #endregion
    }
}