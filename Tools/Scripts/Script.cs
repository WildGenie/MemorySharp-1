using System;
using System.Collections.Generic;
using System.Linq;
using MemorySharp.Tools.Logger;

namespace MemorySharp.Tools.Scripts
{
    public abstract class Script
    {
        #region Events
        public event EventHandler OnStartedEvent;
        public event EventHandler OnStoppedEvent;
        #endregion

        #region Constructors
        public Script(string name, string category)
        {
            Name = name;
            Category = category;
            IsRunning = false;
            ThreadPool = new List<ScriptThread>();
            MainThread = null;
        }
        #endregion

        #region  Properties
        public string Name { get; }
        public string Category { get; }
        public bool IsRunning { get; private set; }
        public List<ScriptThread> ThreadPool { get; }
        private ScriptThread MainThread { get; set; }
        #endregion

        #region Methods
        protected ScriptThread StartThread(Action action)
        {
            var thread = new ScriptThread(action);
            ThreadPool.Add(thread);
            return thread;
        }

        protected void StopThread(string reason = null)
        {
            throw new TerminateException(reason);
        }

        public void Start()
        {
            IsRunning = true;
        }

        private void StartInternal()
        {
            Print("Script starting");
            OnStart();
            MainThread = new ScriptThread(OnTick);
            OnStarted();
        }

        public void Stop()
        {
            IsRunning = false;
        }

        private void TerminateInternal(string message = "Script terminated")
        {
            OnTerminate();
            IsRunning = false;
            MainThread = null;
            ThreadPool.Clear();
            Print(message);
            OnTerminated();
        }

        internal void Tick()
        {
            try
            {
                if (IsRunning && MainThread == null)
                    StartInternal();

                if (IsRunning)
                {
                    MainThread.Tick();

                    foreach (var thread in ThreadPool)
                        thread.Tick();

                    ThreadPool.Where(t => !t.IsAlive).ToList().ForEach(t => ThreadPool.Remove(t));
                }

                if (!IsRunning && MainThread != null)
                    TerminateInternal();

                if (MainThread != null && !MainThread.IsAlive)
                    TerminateInternal(MainThread.ExitReason ?? "Script finished");
            }
            catch (Exception ex)
            {
                TerminateInternal("Error: " + ex);
            }
        }

        public void Print(string message, params object[] args)
        {
            Log.Info(Name + ": " + string.Format(message, args));
        }

        protected void Sleep(int ms)
        {
            throw new Exception("Failed to sleep for: " + ms + " (milaseconds)");
        }

        public virtual void RegisterEvents()
        {
        }

        public virtual void DeregisterEvents()
        {
        }

        public virtual void OnStart()
        {
        }

        //public virtual void OnTick() { Stop(); }
        public virtual void OnTick()
        {
        }

        public virtual void OnTerminate()
        {
        }

        private void OnStarted()
        {
            if (OnStartedEvent != null)
                OnStartedEvent(this, new EventArgs());
        }

        private void OnTerminated()
        {
            if (OnStoppedEvent != null)
                OnStoppedEvent(this, new EventArgs());
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Category, Name);
        }
        #endregion
    }
}