using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Serilog;
using System.Configuration;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools.Android;

namespace UnmistakableAPKInstaller.AvaloniaUI
{
    /// <summary>
    /// Default main App class
    /// </summary>
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                InitApp(desktop);
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// Init App method.
        /// Call after AvaloniaUI initialization
        /// </summary>
        /// <param name="desktop"></param>
        private void InitApp(IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += App_Exit;
            InitLogger();
            InitDiskCache();
            desktop.MainWindow = new MainWindow(true);
        }

        /// <summary>
        /// Initialize app logger
        /// </summary>
        private void InitLogger()
        {
            var logFilePath = System.IO.Path.Combine(DiskCache.AppDataDirectory,
                ConfigurationManager.AppSettings["LogFileName"]);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10)
                .CreateLogger();
        }

        /// <summary>
        /// Initialize disk cache
        /// </summary>
        private void InitDiskCache()
        {
            var diskCacheFilePath = System.IO.Path.Combine(DiskCache.AppDataDirectory,
                ConfigurationManager.AppSettings["DiskCacheFileName"]);
            DiskCache.Init(diskCacheFilePath);
        }

        /// <summary>
        /// On AppExit method.
        /// Save current cache to disk && stop unused processes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            // Stop all adb processes
            CmdHelper.StopAllProcessesByName(AndroidPlatformTools.ADB_PROCESS_NAME);
            // Save cache to disk
            DiskCache.SaveToDisk();
        }
    }
}
