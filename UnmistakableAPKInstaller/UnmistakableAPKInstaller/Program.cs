using System.Configuration;
using System.Diagnostics;
using UnmistakableAPKInstaller.Helpers;

namespace UnmistakableAPKInstaller
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var logFileName = ConfigurationManager.AppSettings["LogFileName"];
            CustomLogger.Init(logFileName);

            ApplicationConfiguration.Initialize();
            
            var mainForm = new MainForm();
            Application.Run(mainForm);
        }
    }
}