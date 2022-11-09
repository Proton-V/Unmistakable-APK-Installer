using Serilog;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Timers;
using UnmistakableAPKInstaller.Core.Managers;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools;
using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Core.Controllers.UI
{
    public partial class MainWindowController
    {
        /// <summary>
        /// Get/Set selected device
        /// </summary>
        public DeviceData CurrentDevice
        {
            get => deviceManager.CurrentDevice;
            private set => deviceManager.UpdateCurrentDevice(value);
        }

        /// <summary>
        /// Get serial number of Current Device
        /// </summary>
        string CurrentDeviceSerialNumber
            => deviceManager.CurrentDeviceSerialNumber;

        /// <summary>
        /// Check existence of all tools
        /// </summary>
        public bool HasAllTools => cmdToolsProvider.CheckExistsTools();

        /// <summary>
        /// Path to folder with downloaded APK
        /// </summary>
        public string DownloadedAPKFolderPath => $"{Path.Combine(AppManager.AppDirectory, "GoogleDrive")}";

        /// <summary>
        /// Auto-delete previous version app
        /// </summary>
        public bool AutoDelPrevApp => Convert.ToBoolean(ConfigurationManager.AppSettings["AutoDelPrevApp"]);

        /// <summary>
        /// DeviceLog enable status
        /// </summary>
        public bool DeviceLogEnabled => Convert.ToBoolean(ConfigurationManager.AppSettings["DeviceLogEnabled"]);

        /// <summary>
        /// Buffer Size for DeviceLog
        /// </summary>
        int DeviceLogBufferSize => Convert.ToInt32(ConfigurationManager.AppSettings["DeviceLogBufferSize"]);

        /// <summary>
        /// Get full app path from relative path
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        private string GetFullAppDataPath(string relativePath) => $"{DiskCache.AppDataDirectory}/{relativePath}";

        CmdToolsProvider cmdToolsProvider;
        GoogleDriveDownloadHelper gdDownloadHelper;
        string deviceLogFolderPath;

        TimerController timerController;
        DeviceManager deviceManager;

        /// <summary>
        /// Method to get section key name for current platform
        /// </summary>
        /// <returns></returns>
        private string GetCurrentPlatformSectionKeyName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "OSx";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }

            Log.Warning("Section key name not found for current platform");
            return string.Empty;
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
