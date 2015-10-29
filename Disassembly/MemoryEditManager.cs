using System;
using System.Collections.Generic;
using System.Diagnostics;
using MemorySharp.Internals.Interfaces;

namespace MemorySharp.Disassembly
{
    public class MemoryEditManager : IDisposable
    {
        #region  Fields
        private readonly Dictionary<IntPtr, IApplicableElement> _applicationEdits;

        private static readonly Dictionary<int, MemoryEditManager> _processManagers =
            new Dictionary<int, MemoryEditManager>();
        #endregion

        #region Constructors
        private MemoryEditManager()
        {
            _applicationEdits = new Dictionary<IntPtr, IApplicableElement>();
        }
        #endregion

        #region  Properties
        public IApplicableElement this[IntPtr addr]
        {
            get
            {
                IApplicableElement edit;
                return _applicationEdits.TryGetValue(addr, out edit) ? edit : null;
            }
            internal set
            {
                if (_applicationEdits.ContainsKey(addr))
                {
                    _applicationEdits[addr].Dispose();
                }
                _applicationEdits[addr] = value;
            }
        }

        public IEnumerable<IApplicableElement> Edits => _applicationEdits.Values;
        #endregion

        #region  Interface members
        public void Dispose()
        {
            foreach (var edit in Edits)
            {
                edit.Dispose();
            }
            _applicationEdits.Clear();
        }
        #endregion

        #region Methods
        public static MemoryEditManager ForProcess(Process process)
        {
            process.Exited += ProcessExited;
            if (!_processManagers.ContainsKey(process.Id))
            {
                _processManagers[process.Id] = new MemoryEditManager();
            }
            return _processManagers[process.Id];
        }

        private static void ProcessExited(object sender, EventArgs e)
        {
            var procSender = (Process) sender;
            procSender.Exited -= ProcessExited;
            var manager = ForProcess(procSender);
            manager.Dispose();
            _processManagers.Remove(procSender.Id);
        }
        #endregion

        #region Misc
        ~MemoryEditManager()
        {
            Dispose();
        }
        #endregion
    }
}