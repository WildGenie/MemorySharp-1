using System;
using MemorySharp.Internals.Exceptions;

namespace MemorySharp.Tools.Scripts
{
    public class ScriptThread
    {
        #region Constructors
        public ScriptThread(Action action)
        {
            IsAlive = true;
            Action = action;
        }
        #endregion

        #region  Properties
        public bool IsAlive { get; private set; }
        public string ExitReason { get; private set; }
        private Action Action { get; }
        private DateTime SleepTime { get; set; }
        #endregion

        #region Methods
        internal void Tick()
        {
            if (SleepTime >= DateTime.Now)
                return;

            if (!IsAlive) return;
            try
            {
                Action();
            }
            catch (SleepException ex)
            {
                SleepTime = DateTime.Now + ex.Time;
            }
            catch (TerminateException ex)
            {
                IsAlive = false;
                ExitReason = ex.Reason;
            }
        }
        #endregion
    }
}