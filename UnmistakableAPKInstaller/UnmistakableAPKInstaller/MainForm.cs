using System.Timers;
using UnmistakableAPKInstaller.Core.Controllers;
using UnmistakableAPKInstaller.Tools.Android.Models;

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
            controller = new MainWindowController();
            InitializeComponent();
            InitInternalComponents();
            InitHandlers();
        }

        DeviceData CurrentDevice => controller.currentDevice;

        MainWindowController controller;

        public void ForceUpdate()
        {
            controller.Init(DownloadToolsAsync);
        }
        #region Init
        private void InitInternalComponents()
        {
            controller.Init(DownloadToolsAsync);
            InitDevicesDropDownListAsync();
            controller.InitTimers(
                UpdateDeviceListAction: TimerUpdateDeviceListAction);
            InitHandlers();
        }

        void TimerUpdateDeviceListAction(object sender, ElapsedEventArgs e)
        {
            this.Invoke(InitDevicesDropDownListAsync, CurrentDevice?.SerialNumber);
        }

        private void InitHandlers()
        {
            this.FormClosed += Form_OnClose;
        }
        #endregion

        void Form_OnClose(object sender, EventArgs e)
        {
            controller.StopAllTimers();
        }

        #region Form state methods
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

        private void UpdateCurrentDeviceLayout()
        {
            if (CurrentDevice != null)
            {
                this.LabelStatusDevice.Text =
                    $"{CurrentDevice.Model} {CurrentDevice.Status}";

                var isUsbDevice = !CurrentDevice.IsWifiDevice;

                this.LabelUsbMode.Visible = isUsbDevice;
                this.PictureBoxUsbMode.Visible = isUsbDevice;
                this.LabelWifiMode.Visible = isUsbDevice;
                this.PictureBoxWifiMode.Visible = isUsbDevice;
                this.ButtonWifiModeUpdate.Visible = isUsbDevice;

                this.PictureBoxUsbMode.BackColor = CurrentDevice.IsActive ? Color.Green : Color.Red;
                this.PictureBoxWifiMode.BackColor = CurrentDevice.IsActiveWifi ? Color.Green : Color.Red;

                var wifiModeButtonStateStr = CurrentDevice.IsActiveWifi ? "Off" : "On";
                this.ButtonWifiModeUpdate.Text = $"{wifiModeButtonStateStr} Wifi Mode";
            }
        }

        private void EnableThisForm(bool value)
        {
            this.Enabled = value;
        }
        #endregion

        #region Form events
        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            ChangeFormState(MainFormState.APKLoading);
            controller.ButtonDownload_ClickActionAsync(DownloadAPKAsync);
            ChangeFormState(MainFormState.Idle);
        }

        private void ButtonPath_Click(object sender, EventArgs e)
        {
            OpenFileExplorer();
        }

        private void ButtonInstall_Click(object sender, EventArgs e)
        {
            controller.ButtonInstall_ClickActionAsync(InstallAPKAsync);
        }

        private void ButtonDownloadInstall_Click(object sender, EventArgs e)
        {
            ChangeFormState(MainFormState.APKLoading);
            controller.ButtonDownloadInstall_ClickActionAsync(DownloadAPKAsync, InstallAPKAsync, 
                OnCompleteAction: () => ChangeFormState(MainFormState.Idle));
        }

        private void ButtonSettings_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void ButtonSaveLogToFile_Click(object sender, EventArgs e)
        {
            controller.ButtonSaveLogToFile_ClickActionAsync(
                (text, caption) => MessageBox.Show(text, caption, MessageBoxButtons.OK));
        }

        private void DropdownListDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            var serialNumber = $"{DropdownListDevices.SelectedItem}";
            controller.DropdownListDevices_SelectedIndexChangedActionAsync(serialNumber, UpdateCurrentDeviceLayout);
        }

        private void ButtonDeviceListUpdate_Click(object sender, EventArgs e)
        {
            InitDevicesDropDownListAsync(CurrentDevice?.SerialNumber);
        }

        private void ButtonWifiModeUpdate_Click(object sender, EventArgs e)
        {
            controller.ButtonWifiModeUpdate_ClickActionAsync(UpdateCurrentDeviceLayout);
        }
        #endregion

        #region Additional UI controller methods
        private async void InitDevicesDropDownListAsync(string selectedSerialNumber = default)
        {
            var datas = await controller.GetDeviceListAsync();
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

        private void OpenFileExplorer()
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = controller.DownloadedAPKFolderPath;
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

        private async void DownloadToolsAsync()
        {
            ChangeFormState(MainFormState.ToolsLoading);

            var status = await controller.TryDownloadToolsAsync(
                (str) => OutputDownload.Text = str,
                (progress) => ProgressBar.Value = progress);

            MessageBox.Show(status ? "Download is completed!" : "Fail with download...",
                "Download status", MessageBoxButtons.OK);

            OutputDownload.ResetText();
            ProgressBar.Value = 0;

            ChangeFormState(MainFormState.Idle);
        }

        public async Task InstallAPKAsync()
        {
            if (controller.AutoDelPrevApp)
            {
                ProgressBar.Value = 10;
                OutputDownload.Text = "Uninstall previous version...";
                await controller.TryUninstallAPKByPathAsync(InputPath.Text, (str) => OutputDownload.Text = str);
            }

            ProgressBar.Value = 50;
            OutputDownload.Text = "Install new version...";
            var status = await controller.TryInstallAPKAsync(InputPath.Text, (str) => OutputDownload.Text = str);

            if (controller.DeviceLogEnabled)
            {
                OutputDownload.Text = "Set log buffer size...";
                await controller.TrySetLogBufferSizeAsync((str) => OutputDownload.Text = str);
            }

            ProgressBar.Value = 100;
            MessageBox.Show(status ? "Install is completed!" : "Fail with install...",
                "Install status", MessageBoxButtons.OK);

            OutputDownload.ResetText();
            ProgressBar.Value = 0;
        }

        public async Task DownloadAPKAsync(bool showStatus = true)
        {
            var url = InputDownload.Text;
            var data = await controller.DownloadFileAsync(url,
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
        #endregion;
    }
}
