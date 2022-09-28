namespace UnmistakableAPKInstaller.Tools
{
    public class CmdToolsProvider
    {
        public CmdToolsProvider(AndroidPlatformTools platformTools, Aapt2Tool aapt2Tool)
        {
            this.platformTools = platformTools;
            this.aapt2Tool = aapt2Tool;
        }

        AndroidPlatformTools platformTools;
        Aapt2Tool aapt2Tool;

        BaseCmdTool[] Tools => new BaseCmdTool[]
        {
            platformTools,
            aapt2Tool
        };

        public bool CheckExistsTools()
        {
            return Tools.All(x => x.Exists());
        }

        public async Task<bool> TryDownloadTools(Action<string> outText, Action<int> outProgress)
        {
            var requiredTools = Tools.Where(x => !x.Exists());
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

        public async Task<string> TryGetAPKBundleName(string path) => await aapt2Tool.TryGetAPKBundleName(path);
        public async Task<bool> ContainsAnyDevices() => await platformTools.ContainsAnyDevices();
        public async Task<string> GetAndroidDevices() => await platformTools.GetAndroidDevices();
        public async Task<bool> TryUninstallAPK(string bundleName, Action<string> outText) => await platformTools.TryUninstallAPK(bundleName, outText);
        public async Task<bool> TryInstallAPK(string path, Action<string> outText) => await platformTools.TryInstallAPK(path, outText);
        public async Task<bool> TrySetLogBufferSize(int sizeInMb, Action<string> outText) => await platformTools.TrySetLogBufferSize(sizeInMb, outText);
        public async Task<bool> TrySaveLogToFile(string path, Action<string>? outText) => await platformTools.TrySaveLogToFile(path, outText);
    }
}