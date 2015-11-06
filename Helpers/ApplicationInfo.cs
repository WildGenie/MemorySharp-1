using System;
using System.IO;

namespace Binarysharp.MemoryManagement.Helpers
{
    public static class ApplicationInfo
    {
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
    }
}