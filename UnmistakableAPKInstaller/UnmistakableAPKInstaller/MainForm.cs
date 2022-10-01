using System.Configuration;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools;
using UnmistakableAPKInstaller.Tools.Android;

namespace UnmistakableAPKInstaller
{
    public partial class MainForm : Form
    {
        enum MainFormState
        {
            Idle,
            APKLoading,
            ToolsLoading,
        }

        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        bool AutoDelPrevApp => Convert.ToBoolean(ConfigurationManager.AppSettings["AutoDelPrevApp"]);
        bool DeviceLogEnabled => Convert.ToBoolean(ConfigurationManager.AppSettings["DeviceLogEnabled"]);
        int DeviceLogBufferSize => Convert.ToInt32(ConfigurationManager.AppSettings["DeviceLogBufferSize"]);

        string AppDirectory => Environment.CurrentDirectory;
        string DownloadAPKFolder => $"{AppDirectory}GoogleDrive";

        CmdToolsProvider cmdToolsProvider;
        GoogleDriveDownloadHelper gdDownloadHelper;

        string deviceLogFolderPath;

        public void ForceUpdate()
        {
            Init();
        }

        private void Init()
        {
            deviceLogFolderPath = ConfigurationManager.AppSettings["DeviceLogFolderPath"];
            if (deviceLogFolderPath == string.Empty)
            {
                deviceLogFolderPath = Path.Combine(Environment.CurrentDirectory,
                    ConfigurationManager.AppSettings["DeviceLogDefaultFolderName"]);
            }
            Directory.CreateDirectory(deviceLogFolderPath);

            Directory.CreateDirectory(DownloadAPKFolder);

            var platformToolsDownloadLink = ConfigurationManager.AppSettings["PlatformToolsDownloadLink"];
            var platformToolsFolderPath = ConfigurationManager.AppSettings["AndroidPlatformToolsFolderPath"];
            var platformTools = new AndroidPlatformTools(platformToolsDownloadLink, GetFullPath(platformToolsFolderPath));

            var aapt2DownloadLink = ConfigurationManager.AppSettings["Aapt2DownloadLink"];
            var aapt2FolderPath = ConfigurationManager.AppSettings["Aapt2FolderPath"];
            var aapt2Tool = new Aapt2Tool(aapt2DownloadLink, GetFullPath(aapt2FolderPath));

            var gdApiKey = ConfigurationManager.AppSettings["GoogleDriveApiKey"];
            gdDownloadHelper = new GoogleDriveDownloadHelper(gdApiKey, DownloadAPKFolder);

            cmdToolsProvider = new CmdToolsProvider()
                .AddTool(platformTools)
                .AddTool(aapt2Tool);

            if (!cmdToolsProvider.CheckExistsTools())
            {
                DownloadToolsAsync();
            }

            InitInternalComponents();
        }

        private void InitInternalComponents()
        {
            InitDevicesDropDownListAsync();
        }

        private string GetFullPath(string relativePath) => $"{AppDirectory}/{relativePath}";

        private async void DownloadToolsAsync()
        {
            ChangeFormState(MainFormState.ToolsLoading);

            var status = await cmdToolsProvider.TryDownloadToolsAsync(
                (str) => OutputDownload.Text = str,
                (progress) => ProgressBar.Value = progress);

            MessageBox.Show(status ? "Download is completed!" : "Fail with download...",
                "Download status", MessageBoxButtons.OK);

            OutputDownload.ResetText();
            ProgressBar.Value = 0;

            ChangeFormState(MainFormState.Idle);
        }

        private void ChangeFormState(MainFormState state)
        {
            switch (state)
            {
                case MainFormState.Idle:
                    ChangeVisibility(true);
                    break;
                case MainFormState.ToolsLoading:
                    ChangeVisibility(false);
                    OutputDownload.Visible = true;
                    ProgressBar.Visible = true;
                    break;
                case MainFormState.APKLoading:
                    ChangeVisibility(true);
                    ButtonDownload.Visible = false;
                    ButtonDownloadInstall.Visible = false;
                    ButtonInstall.Visible = false;
                    ButtonSettings.Visible = false;
                    ButtonSaveLogToFile.Visible = false;
                    break;
            }
        }

        private void ChangeVisibility(bool value)
        {
            ButtonSettings.Visible = value;
            ButtonSaveLogToFile.Visible = value;
            ButtonDownload.Visible = value;
            InputDownload.Visible = value;
            LabelDownload.Visible = value;
            OutputDownload.Visible = value;
            ProgressBar.Visible = value;
            LabelPath.Visible = value;
            InputPath.Visible = value;
            ButtonPath.Visible = value;
            LabelDevices.Visible = value;
            OutputDevices.Visible = value;
            ButtonInstall.Visible = value;
            ButtonDownloadInstall.Visible = value;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            ButtonDownload_ClickActionAsync();
        }

        private async void ButtonDownload_ClickActionAsync()
        {
            ChangeFormState(MainFormState.APKLoading);
            await DownloadAPKAsync();
            ChangeFormState(MainFormState.Idle);
        }

        private void ButtonPath_Click(object sender, EventArgs e)
        {
            OpenFileExplorer();
        }

        private void OpenFileExplorer()
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = DownloadAPKFolder;
                openFileDialog.Filter = "APK Files (*.apk)|*.apk";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            InputPath.Text = filePath;
        }

        private void ButtonInstall_Click(object sender, EventArgs e)
        {
            ButtonInstall_ClickActionAsync();
        }

        private async void ButtonInstall_ClickActionAsync()
        {
            await InstallAPKAsync();
        }

        private void ButtonDownloadInstall_Click(object sender, EventArgs e)
        {
            ButtonDownloadInstall_ClickActionAsync();
        }

        private async void ButtonDownloadInstall_ClickActionAsync()
        {
            ChangeFormState(MainFormState.APKLoading);
            await DownloadAPKAsync(false);
            await InstallAPKAsync();
            ChangeFormState(MainFormState.Idle);
        }

        private void ButtonSettings_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void ButtonSaveLogToFile_Click(object sender, EventArgs e)
        {
            ButtonSaveLogToFile_ClickActionAsync();
        }

        private async void ButtonSaveLogToFile_ClickActionAsync()
        {
            string folderPath = deviceLogFolderPath;
            var currentDateTime = DateTime.UtcNow;
            var fileName = $"{Directory.GetFiles(folderPath, "*.log").Length}_{currentDateTime.ToFileTimeUtc()}.log";

            var path = Path.Combine(folderPath, fileName);
            var status = await cmdToolsProvider.TrySaveLogToFileAsync(path, null);

            MessageBox.Show(status ? $"Saved to {path}!" : "Save error...",
                "Save log status", MessageBoxButtons.OK);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #region Download && Install APK
        private async Task DownloadAPKAsync(bool showStatus = true)
        {
            var url = InputDownload.Text;
            var data = await gdDownloadHelper.DownloadFileAsync(url,
                (str) => OutputDownload.Text = str,
                (progress) => ProgressBar.Value = progress);

            if (showStatus)
            {
                MessageBox.Show(data.status ? "Download is completed!" : "Fail with download...",
                    "File download status", MessageBoxButtons.OK);
            }

            OutputDownload.ResetText();
            ProgressBar.Value = 0;

            // Update APK path field
            if (data.status)
            {
                InputPath.Text = data.path;
            }

        }

        private async Task InstallAPKAsync()
        {
            if (AutoDelPrevApp)
            {
                ProgressBar.Value = 10;
                OutputDownload.Text = "Uninstall previous version...";
                await cmdToolsProvider.TryUninstallAPKByPathAsync(InputPath.Text, (str) => OutputDownload.Text = str);
            }

            ProgressBar.Value = 50;
            OutputDownload.Text = "Install new version...";
            var status = await cmdToolsProvider.TryInstallAPKAsync(InputPath.Text, (str) => OutputDownload.Text = str);

            if (DeviceLogEnabled)
            {
                OutputDownload.Text = "Set log buffer size...";
                await cmdToolsProvider.TrySetLogBufferSizeAsync(DeviceLogBufferSize, (str) => OutputDownload.Text = str);
            }

            ProgressBar.Value = 100;
            MessageBox.Show(status ? "Install is completed!" : "Fail with install...",
                "Install status", MessageBoxButtons.OK);

            OutputDownload.ResetText();
            ProgressBar.Value = 0;
        }
        #endregion

        #region Devices DropDownList
        private async void InitDevicesDropDownListAsync()
        {
            DropdownListDevices.Items.Clear();
            DropdownListDevices.Items.Add("Null");

            await UpdateListAsync();

            DropdownListDevices.SelectedIndex = DropdownListDevices.Items.Count - 1;
        }

        private async Task UpdateListAsync()
        {
            foreach (var deviceData in await cmdToolsProvider.GetAndroidDevicesAsync())
            {
                DropdownListDevices.Items.Add(deviceData.serialNumber);
            }
        }
        #endregion
    }
}