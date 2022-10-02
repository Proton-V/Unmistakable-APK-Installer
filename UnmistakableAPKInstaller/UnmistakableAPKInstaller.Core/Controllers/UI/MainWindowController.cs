using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Core.Controllers.UI
{
    public partial class MainWindowController
    {
        #region Init
        public void Init(Action DownloadToolsAsync)
        {
            InitComponents();
            if (!cmdToolsProvider.CheckExistsTools())
            {
                DownloadToolsAsync();
            }
        }
        #endregion

        #region UI Actions
        public async void ButtonWifiModeUpdate_ClickActionAsync(Action OnCompleteAction)
        {
            var currentStatus = CurrentDevice?.IsActiveWifi;
            if (currentStatus != null)
            {
                await cmdToolsProvider.CreateOrUpdateWifiDeviceByUsb(CurrentDevice);
            }

            OnCompleteAction();
        }

        public async void DropdownListDevices_SelectedIndexChangedActionAsync(string serialNumber,
            Action OnCompleteAction)
        {
            var deviceDatas = await cmdToolsProvider.GetAndroidDevicesAsync();
            CurrentDevice = deviceDatas.FirstOrDefault(x => x.SerialNumber == serialNumber);
            OnCompleteAction();
        }

        public async void ButtonDownload_ClickActionAsync(Func<bool, Task> DownloadAPKTask)
        {
            await DownloadAPKTask(true);
        }

        public async void ButtonInstall_ClickActionAsync(Func<Task> installAPKTask)
        {
            await installAPKTask();
        }

        public async void ButtonDownloadInstall_ClickActionAsync(Func<bool, Task> DownloadAPKTask, Func<Task> installAPKTask,
            Action OnCompleteAction)
        {
            await DownloadAPKTask(false);
            await installAPKTask();
            OnCompleteAction();
        }

        public async void ButtonSaveLogToFile_ClickActionAsync(Action<string, string> ShowMsg)
        {
            string folderPath = deviceLogFolderPath;
            var currentDateTime = DateTime.UtcNow;
            var fileName = $"{CurrentDevice.Model} {Directory.GetFiles(folderPath, "*.log").Length}_{currentDateTime.ToFileTimeUtc()}.log";

            var path = Path.Combine(folderPath, fileName);
            var status = await cmdToolsProvider.TrySaveLogToFileAsync(CurrentDeviceSerialNumber, path, null);

            ShowMsg(status ? $"Saved to {path}!" : "Save error...",
                "Save log status");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        #endregion

        public async Task<DeviceData[]> GetDeviceListAsync()
        {
            var deviceDatas = await cmdToolsProvider.GetAndroidDevicesAsync();

            if (deviceDatas.Length == 0)
            {
                deviceDatas =
                    new[] { DeviceData.Default };
            }

            return deviceDatas;
        }
    }
}
