using Serilog;
using System.IO.Compression;
using System.Net;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Tools.Android
{
    /// <summary>
    /// AndroidPlatformTools cmd tool class.
    /// Includes ADB, etc.
    /// </summary>
    public class AndroidPlatformTools : BaseCmdTool
    {
        public const string ADB_PROCESS_NAME = "adb";
        public const int DEFAULT_TCP_PORT = 5555;
        public const string DEFAULT_TCP_PORT_PROP_NAME = "service.adb.tcp.port";

        public AndroidPlatformTools(string downloadLink, string toolFolderPath) : base(downloadLink, toolFolderPath)
        {
        }

        public string AdbPath => $"{toolFolderPath}/{ADB_PROCESS_NAME}";
        string ZipPath => $"{toolFolderPath}/PlatformTools.zip";

        string GetSpecialAdbSerialNumberArg(string serialNumber)
        {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    return string.Empty;
                }

                return $"-s {serialNumber}";
        }

        public override bool Exists()
        {
            Directory.CreateDirectory(toolFolderPath);
            return Directory.GetFiles(toolFolderPath, $"{ADB_PROCESS_NAME}.*").Length > 0;
        }

        public override async Task<bool> TryDownloadToolAsync(Action<string> outText, Action<int> outProgress)
        {
            if (Exists())
            {
                return false;
            }

            Log.Debug($"Tool {GetType().AssemblyQualifiedName} is loading:\n" +
                $"folderPath - {toolFolderPath}\ndownloadLink - {downloadLink}");
            outText("Android PlatformTools is loading...");

            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += (sender, args) =>
                {
                    outProgress(args.ProgressPercentage);
                };

                try
                {
                    await wc.DownloadFileTaskAsync(new Uri(downloadLink), ZipPath);

                    ZipFile.ExtractToDirectory(ZipPath, toolFolderPath, true);
                    File.Delete(ZipPath);

                    var internalZipDirectory = $"{toolFolderPath}/platform-tools";
                    Directory
                        .GetFiles(internalZipDirectory)
                        .ToList()
                        .ForEach(x => File.Move(x, $"{toolFolderPath}/{Path.GetFileName(x)}"));
                    Directory.Delete(internalZipDirectory, true);

                    Log.Debug("Android PlatformTools is loaded!");
                    outText("Android PlatformTools is loaded!");
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error("Android platform tools: {0}", e.ToString());
                    outText(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Quick check for active devices
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ContainsAnyDevicesAsync()
        {
            var str = await GetAndroidDevicesStrAsync();
            var hasDevice = str.Replace("devices", "").Contains("device");
            if (!hasDevice)
            {
                Log.Debug("Device list is empty!");
            }

            return hasDevice;
        }

        /// <summary>
        /// Get all connected device datas as <see cref="DeviceData"/> array
        /// </summary>
        /// <returns></returns>
        public async Task<DeviceData[]> GetAndroidDevicesAsync()
        {
            var results = new DeviceData[] {};

            var deviceListDataStr = await GetAndroidDevicesStrAsync();

            if (!string.IsNullOrEmpty(deviceListDataStr))
            {
                results = await NormalizeDeviceList(deviceListDataStr);
            }

            return results;
        }

        /// <summary>
        /// Convert <paramref name="deviceListDataStr"/> to <see cref="DeviceData"/> array
        /// </summary>
        /// <param name="deviceListDataStr"></param>
        /// <returns></returns>
        private async Task<DeviceData[]> NormalizeDeviceList(string deviceListDataStr)
        {
            List<DeviceData> results = new List<DeviceData>();

            var baseDatas = deviceListDataStr
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)     
                .Select(x => new BaseDeviceData(x))
                .ToArray();

            var usbDeviceDatas =
                baseDatas
                // Skip devices with ip serial number
                .Where(x => !x.IsWifiDevice)
                .ToList();

            var wifiDeviceDatas =
                baseDatas
                .Where(x => !usbDeviceDatas.Contains(x))
                .ToList();

            for (int i = 0; i < usbDeviceDatas.Count; i++)
            {
                var deviceData = new DeviceData(usbDeviceDatas[i]);
                var deviceIpAddress = await GetDeviceIpAddressAsync(deviceData);

                var wifiData = wifiDeviceDatas
                    .FirstOrDefault(x => 
                    {
                        if (IPEndPoint.TryParse(x.SerialNumber, out IPEndPoint endpoint))
                        {
                            return endpoint.Address.ToString() == deviceIpAddress;
                        };
                        return false;
                    });

                if (wifiData != null)
                {
                    deviceData.SetWifiDeviceData(new WifiDeviceData(wifiData));
                    wifiDeviceDatas.Remove(wifiData);
                }

                results.Add(deviceData);
            }

            // Add all self-hosted wifi devices
            results.AddRange(wifiDeviceDatas.Select(x => new DeviceData(x)));

            return results
                .Where(x => x != null)
                .ToArray();
        }

        /// <summary>
        /// Get all connected devices string
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAndroidDevicesStrAsync()
        {
            var args = "devices -l";
            var processData = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(processData.error))
            {
                Log.Warning("Android platform tools: {0}", processData.error);
                return string.Empty;
            }
            else
            {
                var arr = processData.data
                    .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Where(x =>
                        !x.StartsWith("List") && !x.StartsWith("*"));
                return string.Join(Environment.NewLine, arr);
            }
        }

        /// <summary>
        /// Get basic data for special device by serial number
        /// without wifi device data
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public async Task<BaseDeviceData> GetAndroidDeviceDataAsync(string serialNumber)
        {
            var devicesStr = await GetAndroidDevicesStrAsync();
            var deviceStr = devicesStr
                .Split(Environment.NewLine.ToCharArray())
                .FirstOrDefault(x => x.StartsWith(serialNumber));

            if (!string.IsNullOrEmpty(deviceStr))
            {
                return new BaseDeviceData(deviceStr);
            }
            return null;
        }

        /// <summary>
        /// Unistall actual APK by <paramref name="bundleName"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="bundleName"></param>
        /// <param name="outText"></param>
        /// <returns></returns>
        public async Task<bool> TryUninstallAPKAsync(string serialNumber, string bundleName, Action<string> outText)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var args = $"{GetSpecialAdbSerialNumberArg(serialNumber)} uninstall {bundleName}";
            var data = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(data.error))
            {
                Log.Warning("Android platform tools: {0}", data.error);
                outText(data.error);
            }
            else
            {
                outText(data.data);
            }

            return data.data != null;
        }

        /// <summary>
        /// Install APK by <paramref name="path"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="path"></param>
        /// <param name="outText"></param>
        /// <returns></returns>
        public async Task<bool> TryInstallAPKAsync(string serialNumber, string path, Action<string> outText)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var args = $"{GetSpecialAdbSerialNumberArg(serialNumber)} install {path}";
            var data = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(data.error))
            {
                Log.Warning("Android platform tools: {0}", data.error);
                outText(data.error);
            }
            else
            {
                outText(data.data);
            }

            return !string.IsNullOrEmpty(data.data) && data.data.Contains("Success");
        }

        /// <summary>
        /// Set LogBuffer size to device with <paramref name="serialNumber"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="sizeInMb"></param>
        /// <param name="outText"></param>
        /// <returns></returns>
        public async Task<bool> TrySetLogBufferSizeAsync(string serialNumber, int sizeInMb, Action<string> outText)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var args = $"{GetSpecialAdbSerialNumberArg(serialNumber)} logcat -G {sizeInMb}M";
            var data = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(data.error))
            {
                Log.Warning("Android platform tools: {0}", data.error);
                outText(data.error);
            }
            else
            {
                outText(data.data);
            }

            return string.IsNullOrEmpty(data.error);
        }

        /// <summary>
        /// Save current device log to <paramref name="path"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="path"></param>
        /// <param name="outText"></param>
        /// <returns></returns>
        public async Task<bool> TrySaveLogToFileAsync(string serialNumber, string path, Action<string>? outText)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var args = $"{GetSpecialAdbSerialNumberArg(serialNumber)} logcat -d";
            var data = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(data.error))
            {
                Log.Warning("Android platform tools: {0}", data.error);
                outText?.Invoke(data.error);
            }
            else
            {
                outText?.Invoke(data.data);
                await File.WriteAllTextAsync(path, data.data);
            }

            return string.IsNullOrEmpty(data.error);
        }

        /// <summary>
        /// Get device IP address by <paramref name="deviceData"/>
        /// </summary>
        /// <param name="deviceData"></param>
        /// <returns></returns>
        public async Task<string> GetDeviceIpAddressAsync(BaseDeviceData deviceData)
        {
            var args = $"{GetSpecialAdbSerialNumberArg(deviceData.SerialNumber)} shell ip route";
            var processData = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(processData.error))
            {
                Log.Warning("Android platform tools: {0}", processData.error);
                return string.Empty;
            }
            else
            {
                var ipAddressStr = processData.data
                    .Split(null)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Last();
                return ipAddressStr;
            }
        }

        /// <summary>
        /// Opent port on Device with <paramref name="serialNumber"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task<bool> TryOpenPortAsync(string serialNumber, int port = 5555)
        {
            if (!(await ContainsAnyDevicesAsync()))
            {
                return false;
            }

            var args = $"{GetSpecialAdbSerialNumberArg(serialNumber)} tcpip {port}";
            var data = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(data.error))
            {
                Log.Warning("Android platform tools: {0}", data.error);
            }

            return string.IsNullOrEmpty(data.error);
        }

        /// <summary>
        /// Set prop <paramref name="propName"/> to device with <paramref name="serialNumber"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public async Task<bool> SetTempPropAsync(string serialNumber, string propName, string propValue)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var args = $"{GetSpecialAdbSerialNumberArg(serialNumber)} shell setprop {propName} {propValue}";
            var data = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(data.error))
            {
                Log.Warning("Android platform tools: {0}", data.error);
            }

            return string.IsNullOrEmpty(data.error);
        }

        /// <summary>
        /// Connect/Disconnect from device using <paramref name="ipAddress"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task<bool> TryUpdateConnectToDeviceAsync(bool value, string ipAddress, int port = 5555)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var connectStr = value ? "connect" : "disconnect";
            var args = $" {connectStr} {ipAddress}:{port}";
            var data = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(data.error))
            {
                Log.Warning("Android platform tools: {0}", data.error);
            }

            return string.IsNullOrEmpty(data.error);
        }
    }
}
