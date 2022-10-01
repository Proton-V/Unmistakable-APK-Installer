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
                isSuccessful = isSuccessful && await tool.TryDownloadAsync(outText, outProgress);
            }

            return isSuccessful;
        }

        public async Task<bool> TryUninstallAPKByPathAsync(string path, Action<string> outText)
        {
            if (!await ContainsAnyDevicesAsync())
            {
                return false;
            }

            var bundleName = await TryGetAPKBundleNameAsync(path);
            return await TryUninstallAPKAsync(bundleName, outText);
        }

        public async Task<string> TryGetAPKBundleNameAsync(string path) => 
            await GetTool<Aapt2Tool>()?.TryGetAPKBundleNameAsync(path);
        public async Task<bool> ContainsAnyDevicesAsync() => 
            await GetTool<AndroidPlatformTools>()?.ContainsAnyDevicesAsync();
        public async Task<DeviceData[]> GetAndroidDevicesAsync() => 
            await GetTool<AndroidPlatformTools>()?.GetAndroidDevicesAsync();
        public async Task<bool> TryUninstallAPKAsync(string bundleName, Action<string> outText) => 
            await GetTool<AndroidPlatformTools>()?.TryUninstallAPKAsync(bundleName, outText);
        public async Task<bool> TryInstallAPKAsync(string path, Action<string> outText) => 
            await GetTool<AndroidPlatformTools>()?.TryInstallAPKAsync(path, outText);
        public async Task<bool> TrySetLogBufferSizeAsync(int sizeInMb, Action<string> outText) => 
            await GetTool<AndroidPlatformTools>()?.TrySetLogBufferSizeAsync(sizeInMb, outText);
        public async Task<bool> TrySaveLogToFileAsync(string path, Action<string>? outText) => 
            await GetTool<AndroidPlatformTools>()?.TrySaveLogToFileAsync(path, outText);
    }
}