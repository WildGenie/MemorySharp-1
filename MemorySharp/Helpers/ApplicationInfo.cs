using System;
using System.IO;

namespace Binarysharp.MemoryManagement.Helpers
{
    /// <summary>
    ///     Static class providing tools for extracting information from an application.
    /// </summary>
    public static class ApplicationInfo
    {
        #region Public Properties, Indexers
        /// <summary>
        ///     Gets the application path.
        ///     <value>The application path.</value>
        /// </summary>
        public static string ApplicationPath
            => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     Gets the application version.
        /// </summary>
        public static Version ApplicationVersion => System.Reflection.Assembly.GetExecutingAssembly().
                                                           GetName().Version;
        #endregion
    }
}