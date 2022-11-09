using System.Diagnostics;

namespace UnmistakableAPKInstaller.Core.Managers
{
    /// <summary>
    /// Default manager class for App
    /// </summary>
    public class AppManager
    {
        /// <summary>
        /// Get AppDirectory from current process.
        /// Cached from first call
        /// </summary>
        public static string AppDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_appDirectory))
                {
                    _appDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                }
                return _appDirectory;
            }
        }
        private static string _appDirectory;
    }
}
