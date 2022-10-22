using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Tools.Android
{
    public class AndroidPlatformTools : BaseCmdTool
    {
        public const string ADB_PROCESS_NAME = "adb";
        public const int DEFAULT_TCP_PORT = 5555;
        public const string DEFAULT_TCP_PORT_PROP_NAME = "service.adb.tcp.port";

        public AndroidPlatformTools(string downloadLink, string toolFolderPath) : base(downloadLink, toolFolderPath)
        {
        }

        public string AdbPath => $"{toolFolderPath}/{ADB_PROCESS_NAME}.exe";
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
            return File.Exists(AdbPath);
        }

        public override async Task<bool> TryDownloadToolAsync(Action<string> outText, Action<int> outProgress)
        {
            if (Exists())
            {
                return false;
            }

            outText("Android PlatformTools is loading...");

            Directory.CreateDirectory(toolFolderPath);

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

                    outText("Android PlatformTools is loaded!");
                    return true;
                }
                catch (Exception e)
                {
                    outText(e.Message);
                    CustomLogger.Log("Android platform tools: {0}", e.ToString());
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
                CustomLogger.Log("Device list is empty!");
            }

            return hasDevice;
        }

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

        private async Task<DeviceData[]> NormalizeDeviceList(string deviceListDataStr)
        {
            List<DeviceData> results = new List<DeviceData>();

            var datas = deviceListDataStr
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(x => new DeviceData(x))
                .ToArray();

            var usbDeviceDatas =
                datas
                // Skip devices with ip serial number
                .Where(x => !IPEndPoint.TryParse(x.SerialNumber, out IPEndPoint endpoint))
                .ToList();

            var wifiDeviceDatas = datas.ToList();
            wifiDeviceDatas.RemoveAll(x => usbDeviceDatas.Contains(x));

            for (int i = 0; i < usbDeviceDatas.Count; i++)
            {
                var deviceData = datas[i];
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
                    deviceData.SetWifiDeviceData(wifiData);
                    wifiDeviceDatas.Remove(wifiData);
                }

                results.Add(deviceData);
            }

            // Add all self-hosted wifi devices
            results.AddRange(wifiDeviceDatas);

            return results
                .Where(x => x != null)
                .ToArray();
        } 

        private async Task<string> GetAndroidDevicesStrAsync()
        {
            var args = "devices -l";
            var processData = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(processData.error))
            {
                CustomLogger.Log("Android platform tools: {0}", processData.error);
                return string.Empty;
            }
            else
            {
                return processData.data;
            }
        }

        /// <summary>
        /// Get basic data for special device by serial number
        /// without wifi device data
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public async Task<DeviceData> GetAndroidDeviceDataAsync(string serialNumber)
        {
            var devicesStr = await GetAndroidDevicesStrAsync();
            var deviceStr = devicesStr
                .Split(Environment.NewLine.ToCharArray())
                .FirstOrDefault(x => x.StartsWith(serialNumber));

            if (!string.IsNullOrEmpty(deviceStr))
            {
                return new DeviceData(deviceStr);
            }
            return null;
        }

        public async Task<bool> TryUninstallAPKAsync(string serialNumber, string bundleName, Action<string> outText)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var args = $"{GetSpecialAdbSerialNumberArg(serialNumber)} uninstall {bundleName}";
            var data = await CmdHelper.StartProcessAsync(AdbPath, args);
            outText(data.data ?? data.error);
            if (!string.IsNullOrEmpty(data.error))
            {
                CustomLogger.Log("Android platform tools: {0}", data.error);
                outText(data.error);
            }
            else
            {
                outText(data.data);
            }

            return data.data != null;
        }

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
                CustomLogger.Log("Android platform tools: {0}", data.error);
                outText(data.error);
            }
            else
            {
                outText(data.data);
            }

            return !string.IsNullOrEmpty(data.data) && data.data.Contains("Success");
        }

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
                CustomLogger.Log("Android platform tools: {0}", data.error);
                outText(data.error);
            }
            else
            {
                outText(data.data);
            }

            return string.IsNullOrEmpty(data.error);
        }

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
                CustomLogger.Log("Android platform tools: {0}", data.error);
                outText?.Invoke(data.error);
            }
            else
            {
                outText?.Invoke(data.data);
                await File.WriteAllTextAsync(path, data.data);
            }

            return string.IsNullOrEmpty(data.error);
        }

        public async Task<string> GetDeviceIpAddressAsync(DeviceData deviceData)
        {
            var args = $"{GetSpecialAdbSerialNumberArg(deviceData.SerialNumber)} shell ip route";
            var processData = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(processData.error))
            {
                CustomLogger.Log("Android platform tools: {0}", processData.error);
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

        public async Task<bool> TryOpenPortAsync(string serialNumber, int port = 5555)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var args = $"{GetSpecialAdbSerialNumberArg(serialNumber)} tcpip {port}";
            var data = await CmdHelper.StartProcessAsync(AdbPath, args);

            if (!string.IsNullOrEmpty(data.error))
            {
                CustomLogger.Log("Android platform tools: {0}", data.error);
            }

            return string.IsNullOrEmpty(data.error);
        }

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
                CustomLogger.Log("Android platform tools: {0}", data.error);
            }

            return string.IsNullOrEmpty(data.error);
        }

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
                CustomLogger.Log("Android platform tools: {0}", data.error);
            }

            return string.IsNullOrEmpty(data.error);
        }
    }
}
