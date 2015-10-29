using System;
using MemorySharp.Tools.Logger;

namespace MemorySharp.Internals.Interfaces
{
    public interface IModule<T> : IDisposable
    {
        #region Events
        /// <summary>
        ///     On update event
        /// </summary>
        event EventHandler<T> OnUpdate;
        #endregion

        #region  Properties
        string Name { get; }
        ILog Logger { get; set; }
        bool IsRunning { get; }

        /// <summary>
        ///     Ticks module thread will repeat per second
        /// </summary>
        int Ticks { get; set; }
        #endregion

        #region Methods
        /// <summary>
        ///     Configurations
        /// </summary>
        /// <summary>
        ///     Start
        /// </summary>
        void Start();

        /// <summary>
        ///     Stop module
        /// </summary>
        void Stop();
        #endregion
    }
}