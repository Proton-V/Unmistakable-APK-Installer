using System.Configuration;
using System.Timers;
using UnmistakableAPKInstaller.Core.Managers;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools;
using UnmistakableAPKInstaller.Tools.Android;
using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Core.Controllers.UI
{
    public partial class MainWindowController
    {
        public DeviceData CurrentDevice
        {
            get => deviceManager.CurrentDevice;
            private set => deviceManager.UpdateCurrentDevice(value);
        }

        string CurrentDeviceSerialNumber
            => deviceManager.CurrentDeviceSerialNumber;

        public bool HasAllTools => cmdToolsProvider.CheckExistsTools();
        public string DownloadedAPKFolderPath => $"{Path.Combine(AppDirectory, "GoogleDrive")}";
        public bool AutoDelPrevApp => Convert.ToBoolean(ConfigurationManager.AppSettings["AutoDelPrevApp"]);
        public bool DeviceLogEnabled => Convert.ToBoolean(ConfigurationManager.AppSettings["DeviceLogEnabled"]);
        int DeviceLogBufferSize => Convert.ToInt32(ConfigurationManager.AppSettings["DeviceLogBufferSize"]);

        string AppDirectory => Environment.CurrentDirectory;
        private string GetFullAppDataPath(string relativePath) => $"{DiskCache.AppDataDirectory}/{relativePath}";

        CmdToolsProvider cmdToolsProvider;
        GoogleDriveDownloadHelper gdDownloadHelper;
        string deviceLogFolderPath;

        TimerController timerController;
        DeviceManager deviceManager;

        public void InitComponents()
        {
            timerController = new TimerController();
            deviceManager = new DeviceManager();

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
            var platformTools = new AndroidPlatformTools(platformToolsDownloadLink, GetFullAppDataPath(platformToolsFolderPath));

            var aapt2DownloadLink = ConfigurationManager.AppSettings["Aapt2DownloadLink"];
            var aapt2FolderPath = ConfigurationManager.AppSettings["Aapt2FolderPath"];
            var aapt2Tool = new Aapt2Tool(aapt2DownloadLink, GetFullAppDataPath(aapt2FolderPath));

            var gdApiKey = ConfigurationManager.AppSettings["GoogleDriveApiKey"];
            gdDownloadHelper = new GoogleDriveDownloadHelper(gdApiKey, DownloadedAPKFolderPath);

            cmdToolsProvider = new CmdToolsProvider()
                .AddTool(platformTools)
                .AddTool(platformTools)
                .AddTool(aapt2Tool);
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

        public void InitTimers(Action<object, ElapsedEventArgs> UpdateDeviceListAction) =>
            timerController.InitTimers(UpdateDeviceListAction);
        public void StopAllTimers() =>
            timerController.StopAllTimers();
    }
}
