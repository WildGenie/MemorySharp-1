namespace Binarysharp.MemoryManagement.Tools.Updaters
{
    /// <summary>
    ///     Internal class to help provide thread saftey for the <see cref="ThreadedUpdater" /> class.
    /// </summary>
    internal class UpdateThrottler
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The threaded updater lock object to help provide thread saftey for the <see cref="ThreadedUpdater" /> class.
        /// </summary>
        internal static readonly object ThreadedUpdaterLockObject = new object();

        /// <summary>
        ///     The is busy flag states if the <see cref="ThreadedUpdater" /> event is currently busy or not.
        /// </summary>
        internal static volatile bool IsBusyFlag;
        #endregion

        /// <summary>
        ///     Determines whether this instance can acquire the update.
        /// </summary>
        /// <returns>if <code>true</code>, then the <see cref="IsBusyFlag" /> is set to <code>false</code>.</returns>
        internal static bool CanAcquire()
        {
            if (IsBusyFlag) return false;
            lock (ThreadedUpdaterLockObject)
                //could have changed by the time we acquired lock
                if (!IsBusyFlag)
                    return (IsBusyFlag = true);
            return false;
        }

        /// <summary>
        ///     Releases this instance by setting the <see cref="IsBusyFlag" /> to <code>false</code>.
        /// </summary>
        internal static void Release()
        {
            lock (ThreadedUpdaterLockObject)
                IsBusyFlag = false;
        }
    }
}