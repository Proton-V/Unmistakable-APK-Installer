using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Configuration;
using UnmistakableAPKInstaller.Helpers;

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
                var logFileName = ConfigurationManager.AppSettings["LogFileName"];
                CustomLogger.Init(logFileName);
                desktop.MainWindow = new MainWindow(true);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
