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
using Avalonia.Threading;
using MessageBox.Avalonia.DTO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using Serilog;

namespace UnmistakableAPKInstaller.AvaloniaUI
{
    /// <summary>
    /// AvaloniaUI MainWindow class
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// MainWindow states
        /// </summary>
        enum MainFormState
        {
            Idle,
            APKLoading,
            AppLoading,
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

        /// <summary>
        /// Current active device (selected)
        /// </summary>
        DeviceData CurrentDevice => controller.CurrentDevice;

        /// <summary>
        /// Controller instance
        /// </summary>
        MainWindowController controller;

        /// <summary>
        /// Method to force update controller or window (update internal fields)
        /// </summary>
        public void ForceUpdate()
        {
            controller.Init();
        }

        #region Init

        /// <summary>
        /// Method to init internal components
        /// </summary>
        private async void InitInternalComponents()
        {
            ChangeFormState(MainFormState.AppLoading);

            controller = new MainWindowController();

            controller.Init();
            var hasAllTools = controller.HasAllTools;
            if (!hasAllTools)
            {
                await DownloadToolsAsync();
            }
            InitHandlers();

            OutStrOpText.Text = "Initialize devices...";
            ProgressBar.IsIndeterminate = true;

            await InitDevicesDropDownListAsync();

            controller.InitTimers(
                UpdateDeviceListAction: TimerUpdateDeviceListAction);

            ChangeFormState(MainFormState.Idle);

            OutStrOpText.Text = string.Empty;
            ProgressBar.IsIndeterminate = false;
        }

        /// <summary>
        /// Update device list for timer (call on UIThread)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerUpdateDeviceListAction(object sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(async () => await InitDevicesDropDownListAsync(CurrentDevice?.CustomCachedData.CustomName, null));
        }

        /// <summary>
        /// Initialize handlers for UI elements
        /// </summary>
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

        /// <summary>
        /// Call when this window closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Window_OnClose(object sender, EventArgs e)
        {
            controller.StopAllTimers();
        }

        #region Form state methods

        /// <summary>
        /// Update window state
        /// </summary>
        /// <param name="state">next state</param>
        private void ChangeFormState(MainFormState state)
        {
            switch (state)
            {
                case MainFormState.Idle:
                    ChangeVisibility(true);
                    break;
                case MainFormState.AppLoading:
                    ChangeVisibility(false);
                    ImageBanner.IsVisible = true;

                    Panel7.IsVisible = true;
                    OutStrOpText.IsVisible = true;
                    ProgressBar.IsVisible = true;
                    break;
                case MainFormState.APKLoading:
                    ChangeVisibility(true);
                    break;
            }
        }

        /// <summary>
        /// Update UI elements visibility
        /// </summary>
        /// <param name="value"></param>
        private void ChangeVisibility(bool value)
        {
            // Default image banner state
            ImageBanner.IsVisible = false;

            Panel1.IsVisible = value;
            Panel2.IsVisible = value;
            Panel3.IsVisible = value;
            Panel4.IsVisible = value;
            Panel5.IsVisible = value;
            Panel6.IsVisible = value;
            Panel7.IsVisible = value;

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

        /// <summary>
        /// Update device layout elements with <see cref="CurrentDevice"/>
        /// </summary>
        private void UpdateCurrentDeviceLayout()
        {
            if (CurrentDevice != null)
            {
                this.LabelStatusDevice.Content =
                    $"{CurrentDevice.Model} {CurrentDevice.Status}";

                var isNullDevice = CurrentDevice.Status == DeviceData.NULL_VALUE;
                this.LabelUsbMode.IsVisible = !isNullDevice;
                this.PictureBoxUsbMode.IsVisible = !isNullDevice;
                this.LabelWifiMode.IsVisible = !isNullDevice;
                this.PictureBoxWifiMode.IsVisible = !isNullDevice;
                this.ButtonWifiModeUpdate.IsVisible = !isNullDevice;

                this.PictureBoxUsbMode.Background = GetDefaultSolidColorBrush(!CurrentDevice.IsWifiDevice && CurrentDevice.IsActive);
                this.PictureBoxWifiMode.Background = GetDefaultSolidColorBrush(CurrentDevice.IsActiveWifi);

                var wifiModeButtonStateStr = CurrentDevice.IsActiveWifi ? "Off" : "On";
                this.ButtonWifiModeUpdate.Content = $"{wifiModeButtonStateStr} Wifi Mode";
            }
        }

        /// <summary>
        /// Method to get <see cref="SolidColorBrush"/> for active/deactivated state
        /// </summary>
        /// <param name="value">is active state</param>
        /// <returns></returns>
        private SolidColorBrush GetDefaultSolidColorBrush(bool value)
        {
            var color = value ? Colors.Green : Colors.Red;
            return new SolidColorBrush(color);
        }

        /// <summary>
        /// Activate/deactivate root element
        /// </summary>
        /// <param name="value"></param>
        private void EnableThisForm(bool value)
        {
            Root.IsEnabled = value;
        }

        #endregion

        #region Form events

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
            var customName = $"{DropdownListDevices.SelectedItem}";
            controller.DropdownListDevices_SelectedIndexChangedActionAsync(customName, () =>
            {
                UpdateCurrentDeviceLayout();
                EnableThisForm(true);
            });
        }

        private async void ButtonDeviceListUpdate_Click(object sender, EventArgs e)
        {
            EnableThisForm(false);
            await InitDevicesDropDownListAsync(CurrentDevice?.CustomCachedData.CustomName,
                OnCompleteAction: () => EnableThisForm(true));
        }

        private void ButtonWifiModeUpdate_Click(object sender, EventArgs e)
        {
            EnableThisForm(false);
            controller.ButtonWifiModeUpdate_ClickActionAsync((res) =>
            {
                if (res)
                {
                    PictureBoxWifiMode.PropertyChanged += PictureBoxWifiMode_PropertyChanged;
                }
                else
                {
                    EnableThisForm(true);
                }
            });
        }

        private void PictureBoxWifiMode_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            PictureBoxWifiMode.PropertyChanged -= PictureBoxWifiMode_PropertyChanged;
            EnableThisForm(true);
        }
        #endregion

        #region Additional UI controller methods

        /// <summary>
        /// Init/Update UI device list
        /// </summary>
        /// <param name="selectedCustomName">selected device</param>
        /// <param name="OnCompleteAction"></param>
        /// <returns></returns>
        private async Task InitDevicesDropDownListAsync(string selectedCustomName = default, Action OnCompleteAction = null)
        {
            var hasNewDeviceList = await controller.HasNewDeviceList();
            if (!hasNewDeviceList)
            {
                OnCompleteAction?.Invoke();
                return;
            }

            var datas = await controller.GetDeviceListAsync();
            var newDropDownDatas = datas.Select(x => x.CustomCachedData.CustomName).ToList();

            DropdownListDevices.Items = newDropDownDatas;

            if (string.IsNullOrEmpty(selectedCustomName)
                || !DropdownListDevices.Items.Cast<string>().Contains(selectedCustomName))
            {
                DropdownListDevices.SelectedIndex = DropdownListDevices.ItemCount - 1;
            }
            else
            {
                DropdownListDevices.SelectedItem = selectedCustomName;
            }

            OnCompleteAction?.Invoke();
        }

        /// <summary>
        /// Open APK File Dialog
        /// </summary>
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

        /// <summary>
        /// Download tools method (with UI logic)
        /// </summary>
        /// <returns></returns>
        private async Task DownloadToolsAsync()
        {
            var status = await controller.TryDownloadToolsAsync(
                (str) => OutStrOpText.Text = str,
                (progress) => ProgressBar.Value = progress);

            await GetDefaultMessageBox(status ? "Download is completed!" : "Fail with download...",
                "Download status").ShowDialog(this);

            OutStrOpText.Text = string.Empty;
            ProgressBar.Value = 0;
        }

        /// <summary>
        /// Install APK with logic to uninstall current bundle && enable buffer
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Download APK from GD link
        /// </summary>
        /// <param name="showStatus"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Method to open MSG dialog
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private IMsBoxWindow<ButtonResult> GetDefaultMessageBox(string text, string title = default)
        {
            var standardParams = new MessageBoxStandardParams();
            standardParams.ContentTitle = title;
            standardParams.ContentMessage = text;
            standardParams.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var iconPath = "avares://UnmistakableAPKInstaller/Assets/logo-min.ico";

            try
            {
                var bitmap = new Bitmap(assetLoader.Open(new Uri(iconPath)));
                standardParams.WindowIcon = new WindowIcon(bitmap);
            }
            catch (Exception e)
            {
                Log.Error($"Fail with open icon path: {iconPath} - {e}");
            }

            return MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow(standardParams);
        }
        #endregion;
    }
}
