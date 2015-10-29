using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MemorySharp.Internals.Interfaces;

namespace MemorySharp.Hooks
{
    public class ActionQueue : IPulsable
    {
        #region  Fields
        private readonly Queue<Action> _executionQueue;
        #endregion

        #region Constructors
        public ActionQueue()
        {
            _executionQueue = new Queue<Action>();
        }
        #endregion

        #region  Interface members
        public void Pulse()
        {
            if (_executionQueue == null)
                return;

            if (_executionQueue.Count == 0)
                return;

            var action = _executionQueue.Dequeue();
            action.Invoke();
        }
        #endregion

        #region Methods
        public void InvokeAction(Action action)
        {
            action.Invoke();
        }

        public void AddExececution(Action action, bool postpone = false)
        {
            // If we're already in the main thread we're also in the EndScene hook which means we can run the command without any problems
            // sometimes this is however not desired (maybe this needs to happen the next frame) so we check if we want it postponed
            if (!postpone && Thread.CurrentThread.ManagedThreadId == Process.GetCurrentProcess().Threads[0].Id)
                action.Invoke();
            else
                _executionQueue.Enqueue(action);
        }
        #endregion
    }
}