using System;
using System.Collections.Generic;

namespace Binarysharp.MemoryManagement.MemoryExternal.Threading
{
    public interface IThreadFactory
    {
        #region  Properties
        RemoteThread this[int threadId] { get; }
        RemoteThread MainThread { get; }
        IEnumerable<RemoteThread> RemoteThreads { get; }
        #endregion

        #region Methods
        RemoteThread Create(IntPtr address, bool isStarted = true);
        RemoteThread Create(IntPtr address, dynamic parameter, bool isStarted = true);
        RemoteThread CreateAndJoin(IntPtr address);
        RemoteThread CreateAndJoin(IntPtr address, dynamic parameter);
        void Dispose();
        RemoteThread GetThreadById(int id);
        void ResumeAll();
        void SuspendAll();
        #endregion
    }
}