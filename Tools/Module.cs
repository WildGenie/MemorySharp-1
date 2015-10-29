using System;
using System.Threading;
using MemorySharp.Internals.Interfaces;
using MemorySharp.Tools.Logger;

namespace MemorySharp.Tools
{
    /// <summary>
    ///     IModule partial implementation
    /// </summary>
    /// <typeparam name="T">Update Object type</typeparam>
    public abstract class Module<T> : IModule<T>
    {
        #region Events
        public event EventHandler<T> OnUpdate;
        #endregion

        #region  Fields
        private int _interval;
        #endregion

        #region Constructors
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="updateData">Object that will be used in Process method</param>
        /// <param name="ticks">Ticks per second</param>
        protected Module(T updateData, int ticks)
        {
            State = updateData;
            IsRunning = false;
            Callback += Process;
            Timer = new Timer(Callback, State, Timeout.Infinite, Timeout.Infinite);
            if (Logger == null)
                Logger = new FileLog("Logs", "Log");
            Ticks = ticks;
        }

        /// <summary>
        ///     Constructor for module
        /// </summary>
        /// <param name="updateData">Object that will be used in Process method</param>
        /// <param name="ticks">Ticks per second</param>
        /// <param name="logger">Logger instance</param>
        protected Module(T updateData, ILog logger, int ticks = 60) : this(updateData, ticks)
        {
            Logger = logger;
        }
        #endregion

        #region  Properties
        private T State { get; }
        private Timer Timer { get; }
        private TimerCallback Callback { get; }

        //public Hashtable Configuration { get; set; }
        public int Ticks
        {
            get { return 1000/_interval; }
            set
            {
                _interval = 1000/value;
                if (IsRunning)
                    Timer.Change(0, _interval);
            }
        }

        public ILog Logger { get; set; }
        public bool IsRunning { get; private set; }
        public virtual string Name => GetType().Name;
        #endregion

        #region  Interface members
        public virtual void Dispose()
        {
            Timer.Change(int.MaxValue, _interval);
            Timer.Dispose();
        }

        /// <summary>
        ///     Start module
        /// </summary>
        public virtual void Start()
        {
            Timer.Change(_interval, _interval);
            IsRunning = true;
            //Logger.Info("Started: " + Name);
        }

        /// <summary>
        ///     Stop module
        /// </summary>
        public virtual void Stop()
        {
            Timer.Change(int.MaxValue, _interval);
            IsRunning = false;
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Handle module logic
        /// </summary>
        /// <param name="state"></param>
        protected virtual void Process(object state)
        {
            //Do stuff here
            OnUpdate?.Invoke(this, (T) state);
        }
        #endregion

        #region Misc
        ~Module()
        {
            Dispose();
            // Construct.
        }
        #endregion
    }
}