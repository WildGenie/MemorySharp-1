using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MemorySharp.Internals.Interfaces;
using MemorySharp.Tools.Extensions;

namespace MemorySharp.Tools.Scripts
{
    public class ScriptManager : IPulsable
    {
        #region Events
        public event EventHandler CompilerStarted;
        public event EventHandler CompilerFinished;
        public event EventHandler ScriptRegistered;
        #endregion

        #region Constructors
        public ScriptManager()
        {
            Scripts = new List<Script>();

            CompileInternal();
        }
        #endregion

        #region  Properties
        public List<Script> Scripts { get; }
        private Script CurrentScript { get; set; }
        #endregion

        #region  Interface members
        public void Pulse()
        {
            foreach (var script in Scripts)
            {
                CurrentScript = script;
                script.Tick();
            }

            CurrentScript = null;
        }
        #endregion

        #region Methods
        private void CompileInternal()
        {
            OnCompilerStarted();

            Scripts.AddRange(
                Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.IsSubclassOf(typeof (Script)))
                    .Select(Register)
                    .Where(s => s != null)
                );

            OnCompilerFinished();
        }

        private Script Register(Type type)
        {
            try
            {
                var ctor = type.GetConstructor(new Type[] {});
                if (ctor == null)
                    throw new Exception("Constructor not found");

                var script = (Script) ctor.Invoke(new object[] {});
                if (script == null)
                    throw new Exception("Unable to instantiate script");

                OnScriptRegistered(script);
                return script;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.FullInfo());
            }
            return null;
        }

        private void OnCompilerStarted()
        {
            CompilerStarted?.Invoke(null, new EventArgs());
        }

        private void OnCompilerFinished()
        {
            CompilerFinished?.Invoke(null, new EventArgs());
        }

        private void OnScriptRegistered(Script script)
        {
            ScriptRegistered?.Invoke(script, new EventArgs());
        }
        #endregion
    }
}