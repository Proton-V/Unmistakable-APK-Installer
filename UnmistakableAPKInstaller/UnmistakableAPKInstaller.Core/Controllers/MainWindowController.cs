using System.Configuration;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools;
using UnmistakableAPKInstaller.Tools.Android;
using UnmistakableAPKInstaller.Tools.Android.Models;
using System.Timers;
using Timer = System.Timers.Timer;

namespace UnmistakableAPKInstaller.Core.Controllers
{
    public class MainWindowController
    {
        // TODO: move to DeviceManager class
        public DeviceData currentDevice;
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

        private string GetFullPath(string relativePath) => $"{AppDirectory}/{relativePath}";

        public string DownloadedAPKFolderPath => $"{Path.Combine(AppDirectory, "GoogleDrive")}";
        public bool AutoDelPrevApp => Convert.ToBoolean(ConfigurationManager.AppSettings["AutoDelPrevApp"]);
        public bool DeviceLogEnabled => Convert.ToBoolean(ConfigurationManager.AppSettings["DeviceLogEnabled"]);
        int DeviceLogBufferSize => Convert.ToInt32(ConfigurationManager.AppSettings["DeviceLogBufferSize"]);

        string AppDirectory => Environment.CurrentDirectory;

        CmdToolsProvider cmdToolsProvider;
        GoogleDriveDownloadHelper gdDownloadHelper;

        string deviceLogFolderPath;

        List<Timer> activeTimers = new List<Timer>();

        #region Init
        public void Init(Action DownloadToolsAsync)
        {
            var deviceLogFolderPath = ConfigurationManager.AppSettings["DeviceLogFolderPath"];
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
        }

        public void InitTimers(Action<object, ElapsedEventArgs> UpdateDeviceListAction)
        {
            // Set timer intervals
            Timer updateDeviceListTimer = CreateUpdateDeviceListTimer(5000);
            activeTimers.Add(updateDeviceListTimer);

            foreach(var timer in activeTimers)
            {
                timer.Start();
            }

            Timer CreateUpdateDeviceListTimer(double interval)
            {
                Timer timer = new Timer();
                timer.Elapsed += new ElapsedEventHandler(UpdateDeviceListAction);
                timer.Interval = interval;
                return timer;
            }
        }
        #endregion

        #region UI Actions
        public async void ButtonWifiModeUpdate_ClickActionAsync(Action OnCompleteAction)
        {
            var currentStatus = currentDevice?.IsActiveWifi;
            if (currentStatus != null)
            {
                await cmdToolsProvider.CreateOrUpdateWifiDeviceByUsb(currentDevice);
            }

            OnCompleteAction();
        }

        public async void DropdownListDevices_SelectedIndexChangedActionAsync(string serialNumber, 
            Action OnCompleteAction)
        {
            var deviceDatas = await cmdToolsProvider.GetAndroidDevicesAsync();
            currentDevice = deviceDatas.FirstOrDefault(x => x.SerialNumber == serialNumber);
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
            var fileName = $"{currentDevice.Model} {Directory.GetFiles(folderPath, "*.log").Length}_{currentDateTime.ToFileTimeUtc()}.log";

            var path = Path.Combine(folderPath, fileName);
            var status = await cmdToolsProvider.TrySaveLogToFileAsync(CurrentDeviceSerialNumber, path, null);

            ShowMsg(status ? $"Saved to {path}!" : "Save error...",
                "Save log status");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        #endregion

        public void StopAllTimers()
        {
            foreach (var activeTimer in activeTimers)
            {
                activeTimer.Stop();
            }
        }

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

        public async Task<bool> TryDownloadToolsAsync(Action<string> outText, Action<int> outProgress) =>
            await cmdToolsProvider.TryDownloadToolsAsync(outText, outProgress);

        public async Task<bool> TryUninstallAPKByPathAsync(string path, Action<string> outText) =>
            await cmdToolsProvider.TryUninstallAPKByPathAsync(CurrentDeviceSerialNumber, path, outText);

        public async Task<bool> TryInstallAPKAsync(string path, Action<string> outText) =>
            await cmdToolsProvider.TryInstallAPKAsync(CurrentDeviceSerialNumber, path, outText);

        public async Task<bool> TrySetLogBufferSizeAsync(Action<string> outText) =>
            await cmdToolsProvider.TrySetLogBufferSizeAsync(CurrentDeviceSerialNumber, DeviceLogBufferSize, outText);

        public async Task<(bool status, string path)> DownloadFileAsync(string url,
            Action<string> outText, Action<int> outProgress) =>
            await gdDownloadHelper.DownloadFileAsync(url, outText, outProgress);
    }
}
