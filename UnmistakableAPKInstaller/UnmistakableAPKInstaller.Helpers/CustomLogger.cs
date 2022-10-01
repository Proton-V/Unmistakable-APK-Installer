namespace UnmistakableAPKInstaller.Helpers
{
    // TODO: add log type

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
            Clear();
        }

        public static void Clear()
        {
            File.WriteAllText(LogPath, string.Empty);
        }

        public static void Log(string formatStr, params string[] formatArgs)
        {
            var str = string.Format(formatStr, formatArgs);
            Log(str);
        }

        public static void Log(string msg)
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
