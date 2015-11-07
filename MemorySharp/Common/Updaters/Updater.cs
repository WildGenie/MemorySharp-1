using System;
using System.Threading;
using Binarysharp.MemoryManagement.Common.Logging.Core;
using Binarysharp.MemoryManagement.Managment.Builders;

namespace Binarysharp.MemoryManagement.Common.Updaters
{
    /// <summary>
    ///     Class to provide an effective way to use task to run events and methods at a specified interval.
    /// </summary>
    public class Updater : INamedElement
    {
        #region Public Delegates/Events
        /// <summary>
        ///     An event that Occurs repeatedly at the Interval for this Instance.
        /// </summary>
        public event EventHandler<DeltaEventArgs> OnUpdate;
        #endregion

        #region Fields, Private Properties
        private long BeginTime { get; set; }
        private long FpsTick { get; set; }
        private long LastTick { get; set; }
        private Thread WorkThread { get; set; }
        #endregion

        #region Constructors, Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="Updater" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="updateRateMs">The rate the the <code>OnUpdate</code> event is fired in milaseconds.</param>
        public Updater(string name, int updateRateMs)
        {
            Name = name;
            Interval = updateRateMs;
            IsDisposed = false;
            MustBeDisposed = false;
        }
        #endregion

        #region Public Properties, Indexers
        /// <summary>
        ///     Gets or sets the log. The default is the debug log.
        /// </summary>
        /// <value>The log.</value>
        public ILog Log { get; set; } = new DebugLog();

        /// <summary>
        ///     Gets or sets the updates for this updater in milaseconds.
        /// </summary>
        /// <value>The updates per second.</value>
        public int Interval { get; set; }

        /// <summary>
        ///     Gets the frame rate as frames per second.
        /// </summary>
        /// <value>The frame rate.</value>
        public int FrameRate { get; private set; }

        /// <summary>
        ///     Gets the last frames per second rate.
        /// </summary>
        /// <value>The last frame per second rate.</value>
        public int LastFrameRate { get; private set; }

        /// <summary>
        ///     Gets the total tick count since the updater was enabled.
        /// </summary>
        /// <value>The tick count.</value>
        public int TickCount { get; private set; }

        /// <summary>
        ///     States if the updater is enabled.
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        ///     The name of the element.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        ///     Gets a value indicating whether the element is disposed.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; protected set; }
        #endregion

        #region Interface Implementations
        /// <summary>
        ///     Enables the updater.
        /// </summary>
        public void Enable()
        {
            if (WorkThread != null)
                Disable();
            IsEnabled = true;
            BeginTime = DateTime.Now.Ticks;
            WorkThread = new Thread(Loop)
                         {
                             IsBackground = true,
                             Priority = ThreadPriority.Highest
                         };
            WorkThread.Start();
            Log.LogInfo(Name + " enabled.");
        }

        /// <summary>
        ///     Disables the updater.
        /// </summary>
        public void Disable()
        {
            IsEnabled = false;
            if (WorkThread == null)
                return;
            if (WorkThread.ThreadState == ThreadState.Running)
                WorkThread.Abort();
            WorkThread = null;
            Log.LogInfo(Name + " disabled.");
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            // Nothing for now.
        }
        #endregion

        /// <summary>
        ///     Calculates the average frames per second of the updater instance.
        /// </summary>
        /// <returns>The average frames per second.</returns>
        public int GetAverageFps()
        {
            return (int) (TickCount/GetRuntime().TotalSeconds);
        }

        /// <summary>
        ///     Gets the total run time since the updater was started.
        /// </summary>
        /// <returns>TimeSpan representing the total run time since the updater was started.</returns>
        public TimeSpan GetRuntime()
        {
            return new TimeSpan(DateTime.Now.Ticks - BeginTime);
        }

        /// <summary>
        ///     Handles the <see cref="E:OnUpdate" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DeltaEventArgs" /> instance containing the event data.</param>
        public virtual void OnUpdateEvent(DeltaEventArgs e)
        {
            OnUpdate?.Invoke(this, e);
        }

        /// <summary>
        ///     Calculates the current frames per second.
        /// </summary>
        private void CalculateFps()
        {
            if (DateTime.Now.Ticks - FpsTick >= TimeSpan.TicksPerSecond)
            {
                LastFrameRate = FrameRate;
                FrameRate = 0;
                FpsTick = DateTime.Now.Ticks;
            }
            FrameRate++;
        }

        /// <summary>
        ///     The loop thread for the updater.
        /// </summary>
        private void Loop()
        {
            LastTick = DateTime.Now.Ticks;
            while (IsEnabled)
            {
                CalculateFps();
                var elapsedSeconds = new TimeSpan(DateTime.Now.Ticks - LastTick).TotalSeconds;
                OnUpdateEvent(new DeltaEventArgs(elapsedSeconds));
                TickCount++;
                LastTick = DateTime.Now.Ticks;
                Thread.Sleep(Interval);
            }
        }

        /// <summary>
        ///     Event args for the <see cref="Updater" /> class events.
        /// </summary>
        public class DeltaEventArgs : EventArgs
        {
            #region Constructors, Destructors
            /// <summary>
            ///     Initializes a new <see cref="DeltaEventArgs" /> instance.
            /// </summary>
            /// <param name="secondsElapsed">Seconds elapsed.</param>
            public DeltaEventArgs(double secondsElapsed)
            {
                SecondsElapsed = secondsElapsed;
            }
            #endregion

            #region Public Properties, Indexers
            /// <summary>
            ///     Seconds elapsed.
            /// </summary>
            public double SecondsElapsed { get; private set; }
            #endregion
        }
    }
}