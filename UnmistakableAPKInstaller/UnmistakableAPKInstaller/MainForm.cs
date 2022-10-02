using System.Configuration;
using System.Timers;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools;
using UnmistakableAPKInstaller.Tools.Android;
using UnmistakableAPKInstaller.Tools.Android.Models;
using Timer = System.Timers.Timer;

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
            InitTimers();
            InitHandlers();
        }

        bool AutoDelPrevApp => Convert.ToBoolean(ConfigurationManager.AppSettings["AutoDelPrevApp"]);
        bool DeviceLogEnabled => Convert.ToBoolean(ConfigurationManager.AppSettings["DeviceLogEnabled"]);
        int DeviceLogBufferSize => Convert.ToInt32(ConfigurationManager.AppSettings["DeviceLogBufferSize"]);

        string AppDirectory => Environment.CurrentDirectory;
        string DownloadedAPKFolderPath => $"{Path.Combine(AppDirectory, "GoogleDrive")}";

        CmdToolsProvider cmdToolsProvider;
        GoogleDriveDownloadHelper gdDownloadHelper;

        string deviceLogFolderPath;

        // TODO: move to DeviceManager class
        private DeviceData currentDevice;
        private string CurrentDeviceSerialNumber
        {
            get
            {
                if (currentDevice == null)
                {
                    return null;
                }

                return currentDevice.WifiDeviceData?.SerialNumber ?? currentDevice.SerialNumber;
            }
        }

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

            Directory.CreateDirectory(DownloadedAPKFolderPath);

            var platformToolsDownloadLink = ConfigurationManager.AppSettings["PlatformToolsDownloadLink"];
            var platformToolsFolderPath = ConfigurationManager.AppSettings["AndroidPlatformToolsFolderPath"];
            var platformTools = new AndroidPlatformTools(platformToolsDownloadLink, GetFullPath(platformToolsFolderPath));

            var aapt2DownloadLink = ConfigurationManager.AppSettings["Aapt2DownloadLink"];
            var aapt2FolderPath = ConfigurationManager.AppSettings["Aapt2FolderPath"];
            var aapt2Tool = new Aapt2Tool(aapt2DownloadLink, GetFullPath(aapt2FolderPath));

            var gdApiKey = ConfigurationManager.AppSettings["GoogleDriveApiKey"];
            gdDownloadHelper = new GoogleDriveDownloadHelper(gdApiKey, DownloadedAPKFolderPath);

            cmdToolsProvider = new CmdToolsProvider()
                .AddTool(platformTools)
                .AddTool(aapt2Tool);

            if (!cmdToolsProvider.CheckExistsTools())
            {
                DownloadToolsAsync();
            }

            InitInternalComponents();
        }

        #region Form Handlers
        private void InitHandlers()
        {
            this.FormClosed += FormClosedHandler;
        }

        protected void FormClosedHandler(object sender, EventArgs e)
        {
            foreach(var activeTimer in activeTimers)
            {
                activeTimer.Stop();
            }
        }
        #endregion

        // TODO: move to DeviceManager class
        #region Timers
        private List<Timer> activeTimers = new List<Timer>();

        private void InitTimers()
        {
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(TimerUpdateDeviceListAction);
            timer.Interval = 5000;
            timer.Start();

            activeTimers.Add(timer);
        }

        private void TimerUpdateDeviceListAction(object sender, ElapsedEventArgs e)
        {
            this.Invoke(InitDevicesDropDownListAsync, currentDevice?.SerialNumber);
        }
        #endregion

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
            ButtonDownload.Visible = value;
            InputDownload.Visible = value;
            LabelDownload.Visible = value;
            OutputDownload.Visible = value;
            ProgressBar.Visible = value;
            LabelPath.Visible = value;
            InputPath.Visible = value;
            ButtonPath.Visible = value;
            LabelDevices.Visible = value;
            ButtonInstall.Visible = value;
            ButtonDownloadInstall.Visible = value;
            ButtonSettings.Visible = value;
            ButtonSaveLogToFile.Visible = value;
            DropdownListDevices.Visible = value;
            ButtonWifiModeUpdate.Visible = value;
            ButtonDeviceListUpdate.Visible = value;
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
                openFileDialog.InitialDirectory = DownloadedAPKFolderPath;
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
            var fileName = $"{currentDevice.Model} {Directory.GetFiles(folderPath, "*.log").Length}_{currentDateTime.ToFileTimeUtc()}.log";

            var path = Path.Combine(folderPath, fileName);
            var status = await cmdToolsProvider.TrySaveLogToFileAsync(CurrentDeviceSerialNumber, path, null);

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
                await cmdToolsProvider.TryUninstallAPKByPathAsync(CurrentDeviceSerialNumber, InputPath.Text, (str) => OutputDownload.Text = str);
            }

            ProgressBar.Value = 50;
            OutputDownload.Text = "Install new version...";
            var status = await cmdToolsProvider.TryInstallAPKAsync(CurrentDeviceSerialNumber, InputPath.Text, (str) => OutputDownload.Text = str);

            if (DeviceLogEnabled)
            {
                OutputDownload.Text = "Set log buffer size...";
                await cmdToolsProvider.TrySetLogBufferSizeAsync(CurrentDeviceSerialNumber, DeviceLogBufferSize, (str) => OutputDownload.Text = str);
            }

            ProgressBar.Value = 100;
            MessageBox.Show(status ? "Install is completed!" : "Fail with install...",
                "Install status", MessageBoxButtons.OK);

            OutputDownload.ResetText();
            ProgressBar.Value = 0;
        }
        #endregion

        #region Devices DropDownList
        private async void InitDevicesDropDownListAsync(string selectedSerialNumber = default)
        {
            var datas = await GetDeviceListAsync();
            var newDropDownDatas = datas.Select(x => x.SerialNumber).ToList();

            if (DropdownListDevices.Items.Count == newDropDownDatas.Count()
                && newDropDownDatas.All(x => DropdownListDevices.Items.Contains(x)))
            {
                return;
            }

            DropdownListDevices.Items.Clear();

            foreach (var data in newDropDownDatas)
            {
                DropdownListDevices.Items.Add(data);
            }

            if (string.IsNullOrEmpty(selectedSerialNumber)
                || !DropdownListDevices.Items.Contains(selectedSerialNumber))
            {
                DropdownListDevices.SelectedIndex = DropdownListDevices.Items.Count - 1;
            }
            else
            {
                DropdownListDevices.SelectedItem = selectedSerialNumber;
            }
        }

        private async Task<DeviceData[]> GetDeviceListAsync()
        {
            var deviceDatas = await cmdToolsProvider.GetAndroidDevicesAsync();

            if(deviceDatas.Length == 0)
            {
                deviceDatas = 
                    new[] { DeviceData.Default };
            }

            return deviceDatas;
        }
        #endregion

        private void DropdownListDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropdownListDevices_SelectedIndexChangedActionAsync();
        }

        private async void DropdownListDevices_SelectedIndexChangedActionAsync()
        {
            var serialNumber = $"{DropdownListDevices.SelectedItem}";
            var deviceDatas = await cmdToolsProvider.GetAndroidDevicesAsync();
            currentDevice = deviceDatas.FirstOrDefault(x => x.SerialNumber == serialNumber);

            UpdateCurrentDeviceLayout();
        }

        private void UpdateCurrentDeviceLayout()
        {
            if (currentDevice != null)
            {
                this.LabelStatusDevice.Text = 
                    $"{currentDevice.Model} {currentDevice.Status}";

                var isUsbDevice = !currentDevice.IsWifiDevice;

                this.LabelUsbMode.Visible = isUsbDevice;
                this.PictureBoxUsbMode.Visible = isUsbDevice;
                this.LabelWifiMode.Visible = isUsbDevice;
                this.PictureBoxWifiMode.Visible = isUsbDevice;
                this.ButtonWifiModeUpdate.Visible = isUsbDevice;

                this.PictureBoxUsbMode.BackColor = currentDevice.IsActive ? Color.Green : Color.Red;
                this.PictureBoxWifiMode.BackColor = currentDevice.IsActiveWifi ? Color.Green : Color.Red;

                var wifiModeButtonStateStr = currentDevice.IsActiveWifi ? "Off" : "On";
                this.ButtonWifiModeUpdate.Text = $"{wifiModeButtonStateStr} Wifi Mode";
            }
        }

        private async void ButtonWifiModeUpdate_ClickActionAsync()
        {
            var currentStatus = currentDevice?.IsActiveWifi;
            if (currentStatus != null)
            {
                await cmdToolsProvider.CreateOrUpdateWifiDeviceByUsb(currentDevice);
                UpdateCurrentDeviceLayout();
            }
        }

        private void ButtonDeviceListUpdate_Click(object sender, EventArgs e)
        {
            InitDevicesDropDownListAsync(currentDevice?.SerialNumber);
        }

        private void ButtonWifiModeUpdate_Click(object sender, EventArgs e)
        {
            ButtonWifiModeUpdate_ClickActionAsync();
        }
    }
}
