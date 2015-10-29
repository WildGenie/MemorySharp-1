using System.Collections.Generic;

namespace MemorySharp.Logger
{
    /// <summary>
    ///     A log class. TODO: Make this much more generic/flexible.
    /// </summary>
    public static class Log
    {
        #region  Fields
        private static readonly LinkedList<ILog> _logReaders = new LinkedList<ILog>();
        #endregion

        #region Constructors
        static Log()
        {
        }
        #endregion

        #region Methods
        public static void WriteInfo(string text)
        {
            foreach (var logReader in _logReaders)
            {
                logReader.WriteInfo(text);
            }
        }

        public static void WriteWarning(string text)
        {
            foreach (var logReader in _logReaders)
            {
                logReader.WriteWarning(text);
            }
        }

        public static void WriteError(string text)
        {
            ;
            foreach (var logReader in _logReaders)
            {
                logReader.WriteError(text);
            }
        }

        public static void Write(string text)
        {
            foreach (var logReader in _logReaders)
            {
                logReader.WriteNormal(text);
            }
        }

        public static void AddReader(ILog logReader)
        {
            _logReaders.AddLast(logReader);
        }

        public static void RemoveReader(ILog logReader)
        {
            _logReaders.Remove(logReader);
        }
        #endregion
    }
}