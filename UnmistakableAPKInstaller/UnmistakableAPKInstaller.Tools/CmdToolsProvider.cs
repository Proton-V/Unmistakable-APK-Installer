using UnmistakableAPKInstaller.Tools.Android;
using UnmistakableAPKInstaller.Tools.Android.Data;

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

        public async Task<bool> TryDownloadTools(Action<string> outText, Action<int> outProgress)
        {
            var requiredTools = tools.Values.Where(x => !x.Exists());
            var isSuccessful = true;

            foreach(var tool in requiredTools)
            {
                isSuccessful = isSuccessful && await tool.TryDownload(outText, outProgress);
            }

            return isSuccessful;
        }

        public async Task<bool> TryUninstallAPKByPath(string path, Action<string> outText)
        {
            if (!await ContainsAnyDevices())
            {
                return false;
            }

            var bundleName = await TryGetAPKBundleName(path);
            return await TryUninstallAPK(bundleName, outText);
        }

        public async Task<string> TryGetAPKBundleName(string path) => 
            await GetTool<Aapt2Tool>()?.TryGetAPKBundleName(path);
        public async Task<bool> ContainsAnyDevices() => 
            await GetTool<AndroidPlatformTools>()?.ContainsAnyDevices();
        public async Task<DeviceData[]> GetAndroidDevices() => 
            await GetTool<AndroidPlatformTools>()?.GetAndroidDevices();
        public async Task<bool> TryUninstallAPK(string bundleName, Action<string> outText) => 
            await GetTool<AndroidPlatformTools>()?.TryUninstallAPK(bundleName, outText);
        public async Task<bool> TryInstallAPK(string path, Action<string> outText) => 
            await GetTool<AndroidPlatformTools>()?.TryInstallAPK(path, outText);
        public async Task<bool> TrySetLogBufferSize(int sizeInMb, Action<string> outText) => 
            await GetTool<AndroidPlatformTools>()?.TrySetLogBufferSize(sizeInMb, outText);
        public async Task<bool> TrySaveLogToFile(string path, Action<string>? outText) => 
            await GetTool<AndroidPlatformTools>()?.TrySaveLogToFile(path, outText);
    }
}