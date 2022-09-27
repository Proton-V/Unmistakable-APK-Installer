using System.Configuration;

namespace UnmistakableAPKInstaller
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.CheckBoxSetBufferSizeOnInstallAPK.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["DeviceLogEnabled"]);
            this.TextBoxBuffSize.Text = ConfigurationManager.AppSettings["DeviceLogBufferSize"];

            var deviceLogFolder = ConfigurationManager.AppSettings["DeviceLogFolderPath"];
            if (deviceLogFolder == string.Empty)
            {
                deviceLogFolder = Path.Combine(Environment.CurrentDirectory,
                    ConfigurationManager.AppSettings["DeviceLogDefaultFolderName"]);
            }
            this.InputDeviceLogFolderPath.Text = deviceLogFolder;

            this.InputGDapiKey.Text = ConfigurationManager.AppSettings["GoogleDriveApiKey"];
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["DeviceLogEnabled"].Value = CheckBoxSetBufferSizeOnInstallAPK.Checked.ToString();
            config.AppSettings.Settings["DeviceLogBufferSize"].Value = TextBoxBuffSize.Text;
            config.AppSettings.Settings["DeviceLogFolderPath"].Value = InputDeviceLogFolderPath.Text;
            config.AppSettings.Settings["GoogleDriveApiKey"].Value = InputGDapiKey.Text;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            Close();
        }

        private void ButtonLogFolderSelect_Click(object sender, EventArgs e)
        {
            OpenDeviceLogPathExplorer();
        }

        private void OpenDeviceLogPathExplorer()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.InitialDirectory = Environment.CurrentDirectory;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    this.InputDeviceLogFolderPath.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }
    }
}
