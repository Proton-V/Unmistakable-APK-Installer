using UnmistakableAPKInstaller.Tools.Android;
using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Tools
{
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

        internal Dictionary<Type, BaseCmdTool> tools;

        private T GetTool<T>() where T : BaseCmdTool
        {
            return (T)tools.Values.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        public bool CheckExistsTools()
        {
            return tools.All(x => x.Value.Exists());
        }

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

        public async Task<bool> TryUninstallAPKByPathAsync(string serialNumber, string path, Action<string> outText)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var bundleName = await TryGetAPKBundleNameAsync(path);
            return await TryUninstallAPKAsync(serialNumber, bundleName, outText);
        }

        public async Task<bool> CreateOrUpdateWifiDeviceByUsb(DeviceData deviceData)
        {
            var ipAddress = await GetDeviceIpAddressAsync(deviceData);
            if(await TryOpenPortAsync(deviceData.SerialNumber))
            {
                var nextVal = !deviceData.IsActiveWifi;
                var result = await TryUpdateConnectToDeviceAsync(nextVal, ipAddress);

                DeviceData newWifiDeviceData = null;
                if (nextVal && result)
                {
                    await SetTempPropAsync(deviceData.SerialNumber,
                        AndroidPlatformTools.DEFAULT_TCP_PORT_PROP_NAME,
                        AndroidPlatformTools.DEFAULT_TCP_PORT.ToString());

                    newWifiDeviceData = await GetAndroidDeviceDataAsync(ipAddress);
                }
                deviceData.SetWifiDeviceData(newWifiDeviceData);

                return result;
            }

            return false;
        }

        public async Task<string> TryGetAPKBundleNameAsync(string path) => 
            await Aapt2Tool?.TryGetAPKBundleNameAsync(path);

        public async Task<bool> ContainsAnyDevicesAsync() => 
            await AndroidPlatformTools?.ContainsAnyDevicesAsync();
        public async Task<DeviceData[]> GetAndroidDevicesAsync() => 
            await AndroidPlatformTools?.GetAndroidDevicesAsync();
        public async Task<bool> TryUninstallAPKAsync(string serialNumber, string bundleName, Action<string> outText) => 
            await AndroidPlatformTools?.TryUninstallAPKAsync(serialNumber, bundleName, outText);
        public async Task<bool> TryInstallAPKAsync(string serialNumber, string path, Action<string> outText) => 
            await AndroidPlatformTools?.TryInstallAPKAsync(serialNumber, path, outText);
        public async Task<bool> TrySetLogBufferSizeAsync(string serialNumber, int sizeInMb, Action<string> outText) => 
            await AndroidPlatformTools?.TrySetLogBufferSizeAsync(serialNumber, sizeInMb, outText);
        public async Task<bool> TrySaveLogToFileAsync(string serialNumber, string path, Action<string>? outText) => 
            await AndroidPlatformTools?.TrySaveLogToFileAsync(serialNumber, path, outText);
        public async Task<string> GetDeviceIpAddressAsync(DeviceData deviceData) =>
            await AndroidPlatformTools?.GetDeviceIpAddressAsync(deviceData);
        public async Task<bool> TryOpenPortAsync(string serialNumber, int port = 5555) =>
            await AndroidPlatformTools?.TryOpenPortAsync(serialNumber, port);
        public async Task<bool> SetTempPropAsync(string serialNumber, string propName, string propValue) =>
            await AndroidPlatformTools?.SetTempPropAsync(serialNumber, propName, propValue);
        public async Task<bool> TryUpdateConnectToDeviceAsync(bool value, string ipAddress, int port = 5555) =>
            await AndroidPlatformTools?.TryUpdateConnectToDeviceAsync(value, ipAddress, port);
        public async Task<DeviceData> GetAndroidDeviceDataAsync(string serialNumber) =>
            await AndroidPlatformTools?.GetAndroidDeviceDataAsync(serialNumber);

        Aapt2Tool Aapt2Tool => GetTool<Aapt2Tool>();
        AndroidPlatformTools AndroidPlatformTools => GetTool<AndroidPlatformTools>();
    }
}