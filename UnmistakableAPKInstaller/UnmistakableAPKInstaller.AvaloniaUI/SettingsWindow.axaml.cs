using Avalonia.Controls;
using System.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using UnmistakableAPKInstaller.Core.Managers;
using UnmistakableAPKInstaller.AvaloniaUI.Utils;
using System.Numerics;

namespace UnmistakableAPKInstaller.AvaloniaUI
{
    public partial class SettingsWindow : Window
    {
        const float DEFAULT_SIZE_FACTOR = 1f / 2;
        Vector2 defaultWindowSize = new Vector2(300, 400);

        public SettingsWindow()
        {
            this.SetDefaultWindowSize(Screens.Primary,
                defaultWindowSize, DEFAULT_SIZE_FACTOR);
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

        public void InitHandlers()
        {
            ButtonLogFolderSelect.Click += ButtonLogFolderSelect_Click;
            ButtonSave.Click += ButtonSave_Click;
        }

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
