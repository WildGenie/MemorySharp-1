using System;
using System.Threading;

namespace Binarysharp.MemoryManagement.Tools
{
    public class Updater
    {
        #region  Fields

        private int _interval;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="Updater" /> instance.
        ///     <remarks>Credits for the class: Zat@ www.unknowncheats.com</remarks>
        /// </summary>
        /// <param name="tickRate">
        ///     The rate at which to fire the tick event. The value is 1000 divided by the tickrate supplied. 60
        ///     is a normal number supplied here.
        /// </param>
        public Updater(int tickRate)
        {
            Interval = tickRate;
            TickCount = 0;
        }

        #endregion

        #region Events

        /// <summary>
        ///     This event is fired when the <see cref="Start()" /> function is called at every interval.
        /// </summary>
        public event EventHandler<DeltaEventArgs> TickEvent;

        #endregion

        #region Nested

        /// <summary>
        ///     Event args for the <see cref="Updater" /> class events.
        /// </summary>
        public class DeltaEventArgs : EventArgs
        {
            #region Constructors

            /// <summary>
            ///     Initializes a new <see cref="DeltaEventArgs" /> instance.
            /// </summary>
            /// <param name="secondsElapsed">Seconds elapsed.</param>
            public DeltaEventArgs(double secondsElapsed)
            {
                SecondsElapsed = secondsElapsed;
            }

            #endregion

            #region  Properties

            /// <summary>
            ///     Seconds elapsed.
            /// </summary>
            public double SecondsElapsed { get; private set; }

            #endregion
        }

        #endregion

        #region  Properties

        private Thread Thread { get; set; }
        private bool Work { get; set; }
        private long LastTick { get; set; }
        private long FpsTick { get; set; }
        private long Begin { get; set; }

        /// <summary>
        ///     The interval between tick events. The value is set by dividing 1000 by the value given.
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set { _interval = 1000/value; }
        }

        /// <summary>
        ///     The total tick events fired.
        /// </summary>
        public int TickCount { get; private set; }

        /// <summary>
        ///     Current frames per second rate.
        /// </summary>
        public int FrameRate { get; private set; }

        /// <summary>
        ///     The last recorded frame rate.
        /// </summary>
        public int LastFrameRate { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Invokes a single action.
        /// </summary>
        /// <param name="e">The <see cref="DeltaEventArgs" />.</param>
        public virtual void OnTickEvent(DeltaEventArgs e)
        {
            TickEvent?.Invoke(this, e);
        }

        /// <summary>
        ///     Starts the updater.
        /// </summary>
        public void Start()
        {
            if (Thread != null)
                Stop();
            Work = true;
            Begin = DateTime.Now.Ticks;
            Thread = new Thread(Loop)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            Thread.Start();
        }

        /// <summary>
        ///     Stops the updater.
        /// </summary>
        public void Stop()
        {
            Work = false;
            if (Thread == null)
                return;
            if (Thread.ThreadState == ThreadState.Running)
                Thread.Abort();
            Thread = null;
        }

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
        ///     The total runtime of the updater.
        /// </summary>
        /// <returns>A <see cref="TimeSpan" /> representing the run time</returns>
        public TimeSpan GetRuntime()
        {
            return new TimeSpan(DateTime.Now.Ticks - Begin);
        }

        /// <summary>
        ///     Calculates the average frames per second of the updater instance.
        /// </summary>
        /// <returns>A <see cref="int" /> value representing the average frames per second.</returns>
        public int GetAverageFps()
        {
            return (int) (TickCount/GetRuntime().TotalSeconds);
        }

        private void Loop()
        {
            LastTick = DateTime.Now.Ticks;
            while (Work)
            {
                CalculateFps();
                var elapsedSeconds = new TimeSpan(DateTime.Now.Ticks - LastTick).TotalSeconds;
                OnTickEvent(new DeltaEventArgs(elapsedSeconds));
                TickCount++;
                LastTick = DateTime.Now.Ticks;
                Thread.Sleep(Interval);
            }
        }

        #endregion
    }
}