using System;
using System.IO;
using System.Reflection;

namespace Binarysharp.MemoryManagement.Common.Helpers
{
    /// <summary>
    /// Static class providing tools for extracting information from an application.
    /// </summary>
    public static class ApplicationInfo
    {
        /// <summary>
        ///     Gets the application path.
        ///     <value>The application path.</value>
        /// </summary>
        public static string ApplicationPath
            => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     Gets the application version.
        /// </summary>
        public static Version ApplicationVersion => Assembly.GetExecutingAssembly().
            GetName().Version;
    }
}