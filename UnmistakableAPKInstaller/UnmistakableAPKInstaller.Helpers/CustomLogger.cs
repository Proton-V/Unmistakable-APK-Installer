using System.Diagnostics;

namespace UnmistakableAPKInstaller.Helpers
{
    /// <summary>
    /// Simple logger
    /// </summary>
    public class CustomLogger
    {
        static string logFolder = Environment.CurrentDirectory;
        static string logFileName = "Temp.log";

        static string LogPath => $"{logFolder}/{logFileName}";

        static readonly object logLock = new object();

        public static void Init(string logFileName)
        {
            CustomLogger.logFileName = logFileName;
        }

        public static void WriteToLog(string formatStr, params string[] args)
        {
            var str = string.Format(formatStr, args);
            WriteToLog(str);
        }

        public static void WriteToLog(string msg)
        {
            lock (logLock)
            {
                using (var sw = File.AppendText(LogPath))
                {
                    sw.WriteLine(msg);
                }
            }
        }
    }
}
