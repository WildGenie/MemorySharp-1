using System.Diagnostics;

namespace MemorySharp.Tools.Extensions
{
    public static class ProcessExtensions
    {
        #region Methods
        public static string GetVersionInfo(this Process process)
        {
            return
                $"{process.MainModule.FileVersionInfo.FileDescription} {process.MainModule.FileVersionInfo.FileMajorPart}.{process.MainModule.FileVersionInfo.FileMinorPart}.{process.MainModule.FileVersionInfo.FileBuildPart} {process.MainModule.FileVersionInfo.FilePrivatePart}";
        }
        #endregion
    }
}