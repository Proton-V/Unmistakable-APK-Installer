using System.Configuration;
using UnmistakableAPKInstaller.Helpers;

namespace UnmistakableAPKInstaller
{
    internal static class Program
    {
        private static MainForm mainForm;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var logFileName = ConfigurationManager.AppSettings["LogFileName"];
            CustomLogger.Init(logFileName);

            ApplicationConfiguration.Initialize();
            
            mainForm = new MainForm();
            Application.Run(mainForm);
        }

        public static void ForceUpdateMainForm()
        {
            mainForm.ForceUpdate();
        }
    }
}