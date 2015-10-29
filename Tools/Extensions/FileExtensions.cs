using System.Web.Hosting;

namespace MemorySharp.Tools.Extensions
{
    public static class FileExtensions
    {
        #region Methods
        /// <summary>
        ///     Maps a virtual path to a physical path on the server.
        /// </summary>
        /// <param name="virtualpath">The virtual path (absolute or relative).</param>
        /// <returns>The physical path on the server specified by virtualPath.</returns>
        public static string MapPath(this string virtualpath)
        {
            return !string.IsNullOrWhiteSpace(virtualpath) ? HostingEnvironment.MapPath(virtualpath) : string.Empty;
        }

        /// <summary>
        ///     Determines whether the specified file exists.
        /// </summary>
        /// <param name="virtualpath">The virtual path (absolute or relative).</param>
        /// <returns>True if the path contains the name of an existing file; otherwise, false.</returns>
        public static bool FileExists(this string virtualpath)
        {
            return !string.IsNullOrWhiteSpace(virtualpath) &&
                   HostingEnvironment.VirtualPathProvider.FileExists(virtualpath);
        }

        /// <summary>
        ///     Determines whether the specified folder exists.
        /// </summary>
        /// <param name="virtualpath">The virtual path (absolute or relative).</param>
        /// <returns>True if the path contains the name of an existing folder; otherwise, false.</returns>
        public static bool DirectoryExists(this string virtualpath)
        {
            return !string.IsNullOrWhiteSpace(virtualpath) &&
                   HostingEnvironment.VirtualPathProvider.DirectoryExists(virtualpath);
        }
        #endregion
    }
}