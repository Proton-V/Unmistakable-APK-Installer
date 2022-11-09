using UnmistakableAPKInstaller.Tools.Android;
using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Tools
{
    /// <summary>
    /// Provider class for all <see cref="BaseCmdTool"/>
    /// </summary>
    public class CmdToolsProvider
    {
        public CmdToolsProvider(params BaseCmdTool[] defaultTools)
        {
            tools = new Dictionary<Type, BaseCmdTool>();
            foreach (var tool in defaultTools)
            {
                tools.Add(tool.GetType(), tool);
            }
        }

        /// <summary>
        /// Tools container
        /// </summary>
        internal Dictionary<Type, BaseCmdTool> tools;

        /// <summary>
        /// Get tool with type of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T GetTool<T>() where T : BaseCmdTool
        {
            return (T)tools.Values.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        /// <summary>
        /// Check if all tools exists
        /// </summary>
        /// <returns></returns>
        public bool CheckExistsTools()
        {
            return tools.All(x => x.Value.Exists());
        }

        /// <summary>
        /// Download all tools that don't exist
        /// </summary>
        /// <param name="outText"></param>
        /// <param name="outProgress"></param>
        /// <returns></returns>
        public async Task<bool> TryDownloadToolsAsync(Action<string> outText, Action<int> outProgress)
        {
            var requiredTools = tools.Values.Where(x => !x.Exists());
            var isSuccessful = true;

            foreach(var tool in requiredTools)
            {
                isSuccessful = isSuccessful && await tool.TryDownloadToolAsync(outText, outProgress);
            }

            return isSuccessful;
        }

        /// <summary>
        /// Unistall current APK version using <paramref name="path"/> apk file
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="path"></param>
        /// <param name="outText"></param>
        /// <returns></returns>
        public async Task<bool> TryUninstallAPKByPathAsync(string serialNumber, string path, Action<string> outText)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var bundleName = await TryGetAPKBundleNameAsync(path);
            return await TryUninstallAPKAsync(serialNumber, bundleName, outText);
        }

        /// <summary>
        /// Upsert Wi-Fi mode status for USB connected device.
        /// Set next active status.
        /// </summary>
        /// <param name="deviceData"></param>
        /// <returns></returns>
        public async Task<bool> CreateOrUpdateWifiDeviceByUsb(DeviceData deviceData)
        {
            var ipAddress = await GetDeviceIpAddressAsync(deviceData);
            if(await TryOpenPortAsync(deviceData.SerialNumber))
            {
                var nextVal = !deviceData.IsActiveWifi;
                var result = await TryUpdateConnectToDeviceAsync(nextVal, ipAddress);

                WifiDeviceData newWifiDeviceData = null;
                if (nextVal && result)
                {
                    await SetTempPropAsync(deviceData.SerialNumber,
                        AndroidPlatformTools.DEFAULT_TCP_PORT_PROP_NAME,
                        AndroidPlatformTools.DEFAULT_TCP_PORT.ToString());

                    newWifiDeviceData = new WifiDeviceData(await GetAndroidDeviceDataAsync(ipAddress));
                }
                deviceData.SetWifiDeviceData(newWifiDeviceData);

                return result;
            }

            return false;
        }

        /// <summary>
        /// Get bundle name from APK file
        /// </summary>
        /// <param name="path">APK file path</param>
        /// <returns></returns>
        public async Task<string> TryGetAPKBundleNameAsync(string path) => 
            await Aapt2Tool?.TryGetAPKBundleNameAsync(path);

        /// <summary>
        /// Quick check for active devices
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ContainsAnyDevicesAsync() => 
            await AndroidPlatformTools?.ContainsAnyDevicesAsync();

        /// <summary>
        /// Get all connected devices string
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAndroidDevicesStrAsync() =>
            await AndroidPlatformTools?.GetAndroidDevicesStrAsync();

        /// <summary>
        /// Get all connected device datas as <see cref="DeviceData"/> array
        /// </summary>
        /// <returns></returns>
        public async Task<DeviceData[]> GetAndroidDevicesAsync() => 
            await AndroidPlatformTools?.GetAndroidDevicesAsync();

        /// <summary>
        /// Unistall actual APK by <paramref name="bundleName"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="bundleName"></param>
        /// <param name="outText"></param>
        /// <returns></returns>
        public async Task<bool> TryUninstallAPKAsync(string serialNumber, string bundleName, Action<string> outText) => 
            await AndroidPlatformTools?.TryUninstallAPKAsync(serialNumber, bundleName, outText);

        /// <summary>
        /// Install APK by <paramref name="path"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="path"></param>
        /// <param name="outText"></param>
        /// <returns></returns>
        public async Task<bool> TryInstallAPKAsync(string serialNumber, string path, Action<string> outText) => 
            await AndroidPlatformTools?.TryInstallAPKAsync(serialNumber, path, outText);

        /// <summary>
        /// Set LogBuffer size to device with <paramref name="serialNumber"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="sizeInMb"></param>
        /// <param name="outText"></param>
        /// <returns></returns>
        public async Task<bool> TrySetLogBufferSizeAsync(string serialNumber, int sizeInMb, Action<string> outText) => 
            await AndroidPlatformTools?.TrySetLogBufferSizeAsync(serialNumber, sizeInMb, outText);

        /// <summary>
        /// Save current device log to <paramref name="path"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="path"></param>
        /// <param name="outText"></param>
        /// <returns></returns>
        public async Task<bool> TrySaveLogToFileAsync(string serialNumber, string path, Action<string>? outText) => 
            await AndroidPlatformTools?.TrySaveLogToFileAsync(serialNumber, path, outText);

        /// <summary>
        /// Get device IP address by <paramref name="deviceData"/>
        /// </summary>
        /// <param name="deviceData"></param>
        /// <returns></returns>
        public async Task<string> GetDeviceIpAddressAsync(BaseDeviceData deviceData) =>
            await AndroidPlatformTools?.GetDeviceIpAddressAsync(deviceData);

        /// <summary>
        /// Opent port on Device with <paramref name="serialNumber"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task<bool> TryOpenPortAsync(string serialNumber, int port = 5555) =>
            await AndroidPlatformTools?.TryOpenPortAsync(serialNumber, port);

        /// <summary>
        /// Set prop <paramref name="propName"/> to device with <paramref name="serialNumber"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public async Task<bool> SetTempPropAsync(string serialNumber, string propName, string propValue) =>
            await AndroidPlatformTools?.SetTempPropAsync(serialNumber, propName, propValue);

        /// <summary>
        /// Connect/Disconnect from device using <paramref name="ipAddress"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task<bool> TryUpdateConnectToDeviceAsync(bool value, string ipAddress, int port = 5555) =>
            await AndroidPlatformTools?.TryUpdateConnectToDeviceAsync(value, ipAddress, port);

        /// <summary>
        /// Get basic data for special device by serial number
        /// without wifi device data
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public async Task<BaseDeviceData> GetAndroidDeviceDataAsync(string serialNumber) =>
            await AndroidPlatformTools?.GetAndroidDeviceDataAsync(serialNumber);

        Aapt2Tool Aapt2Tool => GetTool<Aapt2Tool>();
        AndroidPlatformTools AndroidPlatformTools => GetTool<AndroidPlatformTools>();
    }
}