using System.Diagnostics;

namespace UnmistakableAPKInstaller.Core.Managers
{
    public class AppManager
    {
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
