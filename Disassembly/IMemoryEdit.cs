using System;

namespace MemorySharp.Disassembly
{
    public interface IMemoryEdit : IDisposable
    {
        #region  Properties
        bool IsApplied { get; }
        #endregion

        #region Methods
        void Apply();
        void Remove();
        #endregion
    }
}