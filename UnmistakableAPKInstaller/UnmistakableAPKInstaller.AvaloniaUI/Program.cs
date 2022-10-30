using Avalonia;
using Serilog;
using System;
using System.Configuration;
using System.IO;
using UnmistakableAPKInstaller.Helpers;

namespace UnmistakableAPKInstaller.AvaloniaUI
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            var logFilePath = Path.Combine(DiskCache.AppDataDirectory,
                ConfigurationManager.AppSettings["LogFileName"]);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(logFilePath, 
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10)
                .CreateLogger();

            var diskCacheFilePath = Path.Combine(DiskCache.AppDataDirectory,
                ConfigurationManager.AppSettings["DiskCacheFileName"]);
            DiskCache.Init(diskCacheFilePath);

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
        }
    }
}
