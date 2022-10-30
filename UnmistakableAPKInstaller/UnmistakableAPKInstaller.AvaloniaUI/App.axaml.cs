using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Serilog;
using System;
using System.Configuration;
using UnmistakableAPKInstaller.Core.Controllers.UI;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools.Android;

namespace UnmistakableAPKInstaller.AvaloniaUI
{
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
                desktop.Exit += App_Exit;
                InitDiskCacheAndLogger();
                desktop.MainWindow = new MainWindow(true);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void InitDiskCacheAndLogger()
        {
            var logFilePath = System.IO.Path.Combine(DiskCache.AppDataDirectory,
                ConfigurationManager.AppSettings["LogFileName"]);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10)
                .CreateLogger();

            var diskCacheFilePath = System.IO.Path.Combine(DiskCache.AppDataDirectory,
                ConfigurationManager.AppSettings["DiskCacheFileName"]);
            DiskCache.Init(diskCacheFilePath);
        }

        private void App_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            // Stop all adb processes
            CmdHelper.StopAllProcessesByName(AndroidPlatformTools.ADB_PROCESS_NAME);
            // Save cache to disk
            DiskCache.SaveToDisk();
        }
    }
}
