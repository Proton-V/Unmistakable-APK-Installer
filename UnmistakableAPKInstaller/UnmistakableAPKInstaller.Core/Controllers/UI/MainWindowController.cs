using Serilog;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using UnmistakableAPKInstaller.Core.Managers;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Helpers.Models.DiskCache;
using UnmistakableAPKInstaller.Tools.Android;
using UnmistakableAPKInstaller.Tools;
using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Core.Controllers.UI
{
    public partial class MainWindowController
    {
        public MainWindowController()
        {
            timerController = new TimerController();
            deviceManager = new DeviceManager();
        }

        string prevDeviceStr;

        #region Init
        public void Init()
        {
            InitComponents();
        }

        public void InitComponents()
        {
            deviceLogFolderPath = ConfigurationManager.AppSettings["DeviceLogFolderPath"];
            if (deviceLogFolderPath == string.Empty)
            {
                deviceLogFolderPath = Path.Combine(AppManager.AppDirectory,
                    ConfigurationManager.AppSettings["DeviceLogDefaultFolderName"]);
            }
            Directory.CreateDirectory(deviceLogFolderPath);

            Directory.CreateDirectory(DownloadedAPKFolderPath);

            var currentPlatformKeyName = GetCurrentPlatformSectionKeyName();

            var platformToolsDownloadLinkSection = ConfigurationManager.GetSection("AndroidPlatformToolsDownloadLink") as NameValueCollection;
            var aapt2DownloadLinkSection = ConfigurationManager.GetSection("Aapt2DownloadLink") as NameValueCollection;

            var platformToolsDownloadLink = platformToolsDownloadLinkSection[currentPlatformKeyName];
            var platformToolsFolderPath = ConfigurationManager.AppSettings["AndroidPlatformToolsFolderPath"];
            var platformTools = new AndroidPlatformTools(platformToolsDownloadLink, GetFullAppDataPath(platformToolsFolderPath));

            var aapt2DownloadLink = aapt2DownloadLinkSection[currentPlatformKeyName];
            var aapt2FolderPath = ConfigurationManager.AppSettings["Aapt2FolderPath"];
            var aapt2Tool = new Aapt2Tool(aapt2DownloadLink, GetFullAppDataPath(aapt2FolderPath));

            var gdApiKey = ConfigurationManager.AppSettings["GoogleDriveApiKey"];
            gdDownloadHelper = new GoogleDriveDownloadHelper(gdApiKey, DownloadedAPKFolderPath);

            cmdToolsProvider = new CmdToolsProvider()
                .AddTool(platformTools)
                .AddTool(aapt2Tool);
        }
        #endregion

        #region UI Actions
        public async Task ButtonWifiModeUpdate_ClickActionAsync(Action<bool> OnCompleteAction)
        {
            var currentStatus = CurrentDevice?.IsActiveWifi;
            var res = currentStatus != null;

            if (res)
            {
                await cmdToolsProvider.CreateOrUpdateWifiDeviceByUsb(CurrentDevice);

                // Add or Update Cache value if Wifi mode turn on
                DiskCache.AddOrUpdateValue(new DeviceCacheData()
                {
                    CustomName = null,
                    SerialNumber = CurrentDevice?.SerialNumber,
                    IPAddressWPort = CurrentDevice?.WifiDeviceData?.SerialNumber
                });
            }

            OnCompleteAction?.Invoke(res);
        }

        public async void DropdownListDevices_SelectedIndexChangedActionAsync(string customName,
            Action OnCompleteAction)
        {
            var deviceDatas = await GetDeviceListAsync();
            CurrentDevice = deviceDatas.FirstOrDefault(x => x.CustomCachedData.CustomName == customName);
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
