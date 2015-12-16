using System;
using System.Diagnostics;
using System.Globalization;
using Binarysharp.MemoryManagement.Common.Builders;

namespace Binarysharp.MemoryManagement.Common.Helpers
{
    /// <summary>
    ///     A class to perform simple benchmark operations on actions.
    /// </summary>
    public static class BenchMarkHelper
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
            log.Write("Total time to complete: " +
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
        public static void ProfileAction(string description, int iterations, Action func, IManagedLog log)
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
            log.Write("Total time to complete: " + watch.Elapsed.TotalMilliseconds);
        }
        #endregion
    }
}