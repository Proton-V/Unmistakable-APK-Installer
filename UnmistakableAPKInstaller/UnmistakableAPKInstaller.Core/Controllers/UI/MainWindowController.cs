using System.Drawing.Drawing2D;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Helpers.Models.DiskCache;
using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Core.Controllers.UI
{
    public partial class MainWindowController
    {
        string prevDeviceStr;

        #region Init
        public void Init()
        {
            InitComponents();
        }
        #endregion

        #region UI Actions
        public async void ButtonWifiModeUpdate_ClickActionAsync(Action OnCompleteAction)
        {
            var currentStatus = CurrentDevice?.IsActiveWifi;

            if (currentStatus != null)
            {
                await cmdToolsProvider.CreateOrUpdateWifiDeviceByUsb(CurrentDevice);

                // Add or Update Cache value if Wifi mode turn on
                if (!(bool)currentStatus)
                {
                    DiskCache.AddOrUpdateValue(new DeviceCacheData()
                    {
                        CustomName = null,
                        SerialNumber = CurrentDevice.SerialNumber,
                        IPAddressWPort = CurrentDevice.WifiDeviceData.SerialNumber
                    });
                }
            }

            OnCompleteAction?.Invoke();
        }

        public async void DropdownListDevices_SelectedIndexChangedActionAsync(string customName,
            Action OnCompleteAction)
        {
            var deviceDatas = await GetDeviceListAsync();
            CurrentDevice = deviceDatas.FirstOrDefault(x => x.CachedData.CustomName == customName);
            OnCompleteAction?.Invoke();
        }

        public async void ButtonDownload_ClickActionAsync(Func<bool, Task> DownloadAPKTask, Action OnCompleteAction = null)
        {
            await DownloadAPKTask(true);
            OnCompleteAction?.Invoke();
        }

        public async void ButtonInstall_ClickActionAsync(Func<Task> installAPKTask, Action OnCompleteAction = null)
        {
            await installAPKTask();
            OnCompleteAction?.Invoke();
        }

        public async void ButtonDownloadInstall_ClickActionAsync(Func<bool, Task> DownloadAPKTask, Func<Task> installAPKTask,
            Action OnCompleteAction = null)
        {
            await DownloadAPKTask(false);
            await installAPKTask();
            OnCompleteAction?.Invoke();
        }

        public async void ButtonSaveLogToFile_ClickActionAsync(Action<string, string> ShowMsg, Action OnCompleteAction = null)
        {
            if(CurrentDevice == null)
            {
                OnCompleteAction?.Invoke();
                return;
            }

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

            OnCompleteAction?.Invoke();
        }
        #endregion

        public async Task<DeviceData[]> GetDeviceListAsync()
        {
            var deviceDatas = await cmdToolsProvider.GetAndroidDevicesAsync();

            foreach (var data in deviceDatas)
            {
                var newCacheData = new DeviceCacheData()
                {
                    CustomName = null,
                    SerialNumber = data.IsWifiDevice ? null : data.SerialNumber,
                    IPAddressWPort = data.IsWifiDevice ? data.SerialNumber : data.WifiDeviceData?.SerialNumber ?? string.Empty,
                };

                // Add or Update Cache value for each element
                var cacheData = DiskCache.AddOrUpdateValue(newCacheData);
                
                if(cacheData != null)
                {
                    data.SetCachedData(cacheData);
                }
            }


            if (deviceDatas.Length == 0)
            {
                deviceDatas =
                    new[] { DeviceData.Default };
            }

            return deviceDatas;
        }

        public async Task<bool> HasNewDeviceList()
        {
            var newDeviceStr = await cmdToolsProvider.GetAndroidDevicesStrAsync();
            var result = !newDeviceStr.Equals(prevDeviceStr);

            prevDeviceStr = newDeviceStr;
            return result;
        }
    }
}
