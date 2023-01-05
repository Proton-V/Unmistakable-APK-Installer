using Avalonia.Controls;
using System.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using UnmistakableAPKInstaller.Core.Managers;

namespace UnmistakableAPKInstaller.AvaloniaUI
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public SettingsWindow(bool initInternalComponents) : this()
        {
            if (initInternalComponents)
            {
                Init();
                InitHandlers();
            }
        }

        /// <summary>
        /// Initialize handlers for UI elements
        /// </summary>
        public void InitHandlers()
        {
            ButtonLogFolderSelect.Click += ButtonLogFolderSelect_Click;
            ButtonSave.Click += ButtonSave_Click;
        }

        /// <summary>
        /// Initialize UI elements && set actual values
        /// </summary>
        private void Init()
        {
            CheckBoxAutoDelPrevApp.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoDelPrevApp"]);

            CheckBoxSetBufferSizeOnInstallAPK.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["DeviceLogEnabled"]);
            TextBoxBuffSize.Text = ConfigurationManager.AppSettings["DeviceLogBufferSize"];

            var deviceLogFolder = ConfigurationManager.AppSettings["DeviceLogFolderPath"];
            if (deviceLogFolder == string.Empty)
            {
                deviceLogFolder = Path.Combine(AppManager.AppDirectory,
                    ConfigurationManager.AppSettings["DeviceLogDefaultFolderName"]);
            }
            this.InputDeviceLogFolderPath.Text = deviceLogFolder;

            this.InputGDapiKey.Text = ConfigurationManager.AppSettings["GoogleDriveApiKey"];
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["AutoDelPrevApp"].Value = CheckBoxAutoDelPrevApp.IsChecked.ToString();
            config.AppSettings.Settings["DeviceLogEnabled"].Value = CheckBoxSetBufferSizeOnInstallAPK.IsChecked.ToString();
            config.AppSettings.Settings["DeviceLogBufferSize"].Value = TextBoxBuffSize.Text;
            config.AppSettings.Settings["DeviceLogFolderPath"].Value = InputDeviceLogFolderPath.Text;
            config.AppSettings.Settings["GoogleDriveApiKey"].Value = InputGDapiKey.Text;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            if (Owner is MainWindow mainWindow)
            {
                mainWindow.ForceUpdate();
            }

            Close();
        }

        private void ButtonLogFolderSelect_Click(object sender, EventArgs e)
        {
            OpenDeviceLogPathExplorer();
        }


        /// <summary>
        /// Open Folder Dialog
        /// </summary>
        private async Task OpenDeviceLogPathExplorer()
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.InitialDirectory = AppManager.AppDirectory;

            var res = await openFolderDialog.ShowAsync(this);
            if (res != null)
            {
                this.InputDeviceLogFolderPath.Text = res;
            }
        }
    }
}
