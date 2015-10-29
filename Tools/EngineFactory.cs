using System;
using System.Threading;

namespace MemorySharp.Tools
{
    public class EngineFactory<T>
    {
        #region Events
        public virtual event EventHandler<T> OnUpdate;
        #endregion

        #region Constructors
        private EngineFactory(T state, int ticks)
        {
            State = state;
            Started = false;
            TimerCallback += Process;
            Timer = new Timer(TimerCallback, State, Timeout.Infinite, Timeout.Infinite);
            Ticks = ticks;
        }
        #endregion

        #region  Properties
        private T State { get; }
        private Timer Timer { get; }
        private TimerCallback TimerCallback { get; }
        private int Interval { get; set; }
        private bool Started { get; set; }

        public int Ticks
        {
            get { return 1000/Interval; }
            set
            {
                Interval = 1000/value;
                if (IsRunning)
                    Timer.Change(0, Interval);
            }
        }

        public virtual string Name => State.GetType().ToString();
        public virtual bool IsRunning => Started;
        #endregion

        #region Methods
        public static EngineFactory<T> CreateEngine(T state, int ticks)
        {
            return new EngineFactory<T>(state, ticks);
        }

        public virtual void Start()
        {
            Timer.Change(0, Interval);
            Started = true;
            Console.WriteLine(@"Started: " + Name);
        }

        public virtual void Stop()
        {
            Timer.Change(int.MaxValue, Interval);
            Started = false;
            Console.WriteLine(@"Stopped: " + Name);
        }

        public virtual void Dispose()
        {
            Timer.Change(int.MaxValue, Interval);
            Timer.Dispose();
        }

        public virtual void Process(object state)
        {
            OnUpdate?.Invoke(this, (T) state);
        }
        #endregion
    }
}