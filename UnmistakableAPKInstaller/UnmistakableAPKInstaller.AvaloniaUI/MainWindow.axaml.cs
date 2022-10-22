using Avalonia.Controls;
using System.Threading.Tasks;
using System.Timers;
using System;
using UnmistakableAPKInstaller.Core.Controllers.UI;
using UnmistakableAPKInstaller.Tools.Android.Models;
using Avalonia.Media;
using System.Linq;
using System.Collections.Generic;
using MessageBox.Avalonia.BaseWindows.Base;
using MessageBox.Avalonia.Enums;
using Avalonia;
using System.Diagnostics;

namespace UnmistakableAPKInstaller.AvaloniaUI
{
    public partial class MainWindow : Window
    {
        enum MainFormState
        {
            Idle,
            APKLoading,
            ToolsLoading,
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(bool initInternalComponents) : this()
        {
            if (initInternalComponents)
            {
                InitInternalComponents();
            }
        }

        DeviceData CurrentDevice => controller.CurrentDevice;

        MainWindowController controller;

        public void ForceUpdate()
        {
            controller.Init(DownloadToolsAsync);
        }

        #region Init
        private void InitInternalComponents()
        {
            controller = new MainWindowController();

            controller.Init(DownloadToolsAsync);
            InitDevicesDropDownListAsync();
            controller.InitTimers(
                UpdateDeviceListAction: TimerUpdateDeviceListAction);
            InitHandlers();
        }

        void TimerUpdateDeviceListAction(object sender, ElapsedEventArgs e)
        {
            //this.Invoke(InitDevicesDropDownListAsync, CurrentDevice?.SerialNumber, null);
            InitDevicesDropDownListAsync(CurrentDevice?.SerialNumber, null);
        }

        private void InitHandlers()
        {
            this.Closed += Window_OnClose;

            ButtonSettings.Click += ButtonSettings_Click;
            ButtonDownload.Click += ButtonDownload_Click;
            ButtonPath.Click += ButtonPath_Click;
            ButtonInstall.Click += ButtonInstall_Click;
            ButtonDownloadInstall.Click += ButtonDownloadInstall_Click;
            ButtonSaveLogToFile.Click += ButtonSaveLogToFile_Click;
            DropdownListDevices.SelectionChanged += DropdownListDevices_SelectedIndexChanged;
            ButtonWifiModeUpdate.Click += ButtonWifiModeUpdate_Click;
            ButtonDeviceListUpdate.Click += ButtonDeviceListUpdate_Click;
        }
        #endregion

        void Window_OnClose(object sender, EventArgs e)
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
                    OutStrOpText.IsVisible = true;
                    ProgressBar.IsVisible = true;
                    break;
                case MainFormState.APKLoading:
                    ChangeVisibility(true);
                    ButtonDownload.IsVisible = false;
                    ButtonDownloadInstall.IsVisible = false;
                    ButtonInstall.IsVisible = false;
                    ButtonSettings.IsVisible = false;
                    ButtonSaveLogToFile.IsVisible = false;
                    break;
            }
        }

        private void ChangeVisibility(bool value)
        {
            ButtonDownload.IsVisible = value;
            InputDownload.IsVisible = value;
            LabelDownload.IsVisible = value;
            OutStrOpText.IsVisible = value;
            ProgressBar.IsVisible = value;
            LabelPath.IsVisible = value;
            InputPath.IsVisible = value;
            ButtonPath.IsVisible = value;
            LabelDevices.IsVisible = value;
            ButtonInstall.IsVisible = value;
            ButtonDownloadInstall.IsVisible = value;
            ButtonSettings.IsVisible = value;
            ButtonSaveLogToFile.IsVisible = value;
            DropdownListDevices.IsVisible = value;
            ButtonDeviceListUpdate.IsVisible = value;

            // Device status group
            LabelWifiMode.IsVisible = value;
            LabelUsbMode.IsVisible = value;
            LabelStatusDevice.IsVisible = value;
            PictureBoxWifiMode.IsVisible = value;
            PictureBoxUsbMode.IsVisible = value;
            ButtonWifiModeUpdate.IsVisible = value;
        }

        private void UpdateCurrentDeviceLayout()
        {
            if (CurrentDevice != null)
            {
                this.LabelStatusDevice.Name =
                    $"{CurrentDevice.Model} {CurrentDevice.Status}";

                var isUsbDevice = !CurrentDevice.IsWifiDevice;

                this.LabelUsbMode.IsVisible = isUsbDevice;
                this.PictureBoxUsbMode.IsVisible = isUsbDevice;
                this.LabelWifiMode.IsVisible = isUsbDevice;
                this.PictureBoxWifiMode.IsVisible = isUsbDevice;
                this.ButtonWifiModeUpdate.IsVisible = isUsbDevice;

                this.PictureBoxUsbMode.Background = GetDefaultSolidColorBrush(CurrentDevice.IsActive);
                this.PictureBoxWifiMode.Background = GetDefaultSolidColorBrush(CurrentDevice.IsActiveWifi);

                var wifiModeButtonStateStr = CurrentDevice.IsActiveWifi ? "Off" : "On";
                this.ButtonWifiModeUpdate.Name = $"{wifiModeButtonStateStr} Wifi Mode";
            }
        }

        private SolidColorBrush GetDefaultSolidColorBrush(bool value)
        {
            var color = value ? Colors.Green : Colors.Red;
            return new SolidColorBrush(color);
        }

        private void EnableThisForm(bool value)
        {
            Root.IsEnabled = value;
        }
        #endregion

        #region Form events
        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            EnableThisForm(false);
            ChangeFormState(MainFormState.APKLoading);
            controller.ButtonDownload_ClickActionAsync(DownloadAPKAsync,
                OnCompleteAction: () =>
                {
                    ChangeFormState(MainFormState.Idle);
                    EnableThisForm(true);
                });
        }

        private void ButtonPath_Click(object sender, EventArgs e)
        {
            OpenFileExplorer();
        }

        private void ButtonInstall_Click(object sender, EventArgs e)
        {
            EnableThisForm(false);
            controller.ButtonInstall_ClickActionAsync(InstallAPKAsync,
                OnCompleteAction: () => EnableThisForm(true));
        }

        private void ButtonDownloadInstall_Click(object sender, EventArgs e)
        {
            EnableThisForm(false);
            ChangeFormState(MainFormState.APKLoading);
            controller.ButtonDownloadInstall_ClickActionAsync(DownloadAPKAsync, InstallAPKAsync,
                OnCompleteAction: () =>
                {
                    ChangeFormState(MainFormState.Idle);
                    EnableThisForm(true);
                });
        }

        private void ButtonSettings_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsWindow(true);
            settingsForm.ShowDialog(this);
        }

        private void ButtonSaveLogToFile_Click(object sender, EventArgs e)
        {
            EnableThisForm(false);
            controller.ButtonSaveLogToFile_ClickActionAsync(
                (text, caption) => GetDefaultMessageBox(text, caption).ShowDialog(this),
                OnCompleteAction: () => EnableThisForm(true));
        }

        private void DropdownListDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableThisForm(false);
            var serialNumber = $"{DropdownListDevices.SelectedItem}";
            controller.DropdownListDevices_SelectedIndexChangedActionAsync(serialNumber, () =>
            {
                UpdateCurrentDeviceLayout();
                EnableThisForm(true);
            });
        }

        private void ButtonDeviceListUpdate_Click(object sender, EventArgs e)
        {
            EnableThisForm(false);
            InitDevicesDropDownListAsync(CurrentDevice?.SerialNumber,
                OnCompleteAction: () => EnableThisForm(true));
        }

        private void ButtonWifiModeUpdate_Click(object sender, EventArgs e)
        {
            EnableThisForm(false);
            controller.ButtonWifiModeUpdate_ClickActionAsync(() =>
            {
                UpdateCurrentDeviceLayout();
                EnableThisForm(true);
            });
        }
        #endregion

        #region Additional UI controller methods
        private async void InitDevicesDropDownListAsync(string selectedSerialNumber = default, Action OnCompleteAction = null)
        {
            var datas = await controller.GetDeviceListAsync();
            var newDropDownDatas = datas.Select(x => x.SerialNumber).ToList();

            if (DropdownListDevices.ItemCount == newDropDownDatas.Count()
                && newDropDownDatas.All(x => DropdownListDevices.Items.Cast<string>().Contains(x)))
            {
                OnCompleteAction?.Invoke();
                return;
            }

            var newItems = new List<string>();

            foreach (var data in newDropDownDatas)
            {
                newItems.Add(data);
            }

            DropdownListDevices.Items = newItems;

            if (string.IsNullOrEmpty(selectedSerialNumber)
                || !DropdownListDevices.Items.Cast<string>().Contains(selectedSerialNumber))
            {
                DropdownListDevices.SelectedIndex = DropdownListDevices.ItemCount - 1;
            }
            else
            {
                DropdownListDevices.SelectedItem = selectedSerialNumber;
            }

            OnCompleteAction?.Invoke();
        }

        private async void OpenFileExplorer()
        {
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            openFileDialog.InitialDirectory = controller.DownloadedAPKFolderPath;

            var apkFilter = new FileDialogFilter();
            apkFilter.Name = "APK Files (*.apk)";
            apkFilter.Extensions = new List<string>() { "apk" };
            openFileDialog.Filters = new List<FileDialogFilter> { apkFilter };
            openFileDialog.AllowMultiple = false;

            var res = await openFileDialog.ShowAsync(this);
            if (res != null)
            {
                filePath = res.FirstOrDefault();
            }

            InputPath.Text = filePath;
        }

        private async void DownloadToolsAsync()
        {
            ChangeFormState(MainFormState.ToolsLoading);

            var status = await controller.TryDownloadToolsAsync(
                (str) => OutStrOpText.Text = str,
                (progress) => ProgressBar.Value = progress);

            await GetDefaultMessageBox(status ? "Download is completed!" : "Fail with download...",
                "Download status").ShowDialog(this);

            OutStrOpText.Text = string.Empty;
            ProgressBar.Value = 0;

            ChangeFormState(MainFormState.Idle);
        }

        public async Task InstallAPKAsync()
        {
            if (controller.AutoDelPrevApp)
            {
                ProgressBar.Value = 10;
                OutStrOpText.Text = "Uninstall previous version...";
                await controller.TryUninstallAPKByPathAsync(InputPath.Text, (str) => OutStrOpText.Text = str);
            }

            ProgressBar.Value = 50;
            OutStrOpText.Text = "Install new version...";
            var status = await controller.TryInstallAPKAsync(InputPath.Text, (str) => OutStrOpText.Text = str);

            if (controller.DeviceLogEnabled)
            {
                OutStrOpText.Text = "Set log buffer size...";
                await controller.TrySetLogBufferSizeAsync((str) => OutStrOpText.Text = str);
            }

            ProgressBar.Value = 100;

            var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("title", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed...");
            await GetDefaultMessageBox(status ? "Install is completed!" : "Fail with install...",
                "Install status").ShowDialog(this);

            OutStrOpText.Text = string.Empty;
            ProgressBar.Value = 0;
        }

        public async Task DownloadAPKAsync(bool showStatus = true)
        {
            var url = InputDownload.Text;
            var data = await controller.DownloadFileAsync(url,
                (str) => OutStrOpText.Text = str,
                (progress) => ProgressBar.Value = progress);

            if (showStatus)
            {
                await GetDefaultMessageBox(data.status ? "Download is completed!" : "Fail with download...",
                    "File download status").ShowDialog(this);
            }

            OutStrOpText.Text = string.Empty;
            ProgressBar.Value = 0;

            // Update APK path field
            if (data.status)
            {
                InputPath.Text = data.path;
            }
        }

        private IMsBoxWindow<ButtonResult> GetDefaultMessageBox(string text, string title = default)
        {
            return MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow(title, text);
        }
        #endregion;
    }
}
