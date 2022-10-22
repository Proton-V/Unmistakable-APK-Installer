using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Configuration;
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
                var logFileName = ConfigurationManager.AppSettings["LogFileName"];
                CustomLogger.Init(logFileName);
                desktop.MainWindow = new MainWindow(true);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void App_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            // Stop all adb processes
            CmdHelper.StopAllProcessesByName(AndroidPlatformTools.ADB_PROCESS_NAME);
        }
    }
}
