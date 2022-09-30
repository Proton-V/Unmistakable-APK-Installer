using System.IO.Compression;
using System.Net;
using UnmistakableAPKInstaller.Helpers;

namespace UnmistakableAPKInstaller.Tools
{
    public class AndroidPlatformTools : BaseCmdTool
    {
        public AndroidPlatformTools(string downloadLink, string toolFolderPath) : base(downloadLink, toolFolderPath)
        {
        }

        public string AdbPath => $"{toolFolderPath}/adb.exe";
        string ZipPath => $"{toolFolderPath}/PlatformTools.zip";

        public override bool Exists()
        {
            return File.Exists(AdbPath);
        }

        public override async Task<bool> TryDownload(Action<string> outText, Action<int> outProgress)
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

        public async Task<bool> ContainsAnyDevices()
        {
            var str = await GetAndroidDevices();
            var hasDevice = str.Replace("devices", "").Contains("device");
            if (!hasDevice)
            {
                CustomLogger.Log("Device list is empty!");
            }

            return hasDevice;
        }

        public async Task<string> GetAndroidDevices()
        {
            var args = "devices";
            var data = await CmdHelper.StartProcess(AdbPath, args);
            if (!string.IsNullOrEmpty(data.error))
            {
                CustomLogger.Log("Android platform tools: {0}", data.error);
                return data.error;
            }
            else
            {
                return data.data;
            }
        }

        public async Task<bool> TryUninstallAPK(string bundleName, Action<string> outText)
        {
            if (!await ContainsAnyDevices())
            {
                return false;
            }

            var args = $"uninstall {bundleName}";
            var data = await CmdHelper.StartProcess(AdbPath, args);
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

        public async Task<bool> TryInstallAPK(string path, Action<string> outText)
        {
            if (!await ContainsAnyDevices())
            {
                return false;
            }

            var args = $"install {path}";
            var data = await CmdHelper.StartProcess(AdbPath, args);
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

        public async Task<bool> TrySetLogBufferSize(int sizeInMb, Action<string> outText)
        {
            if (!await ContainsAnyDevices())
            {
                return false;
            }

            var args = $"logcat -G {sizeInMb}M";
            var data = await CmdHelper.StartProcess(AdbPath, args);
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

        public async Task<bool> TrySaveLogToFile(string path, Action<string>? outText)
        {
            if (!await ContainsAnyDevices())
            {
                return false;
            }

            var args = $"logcat -d";
            var data = await CmdHelper.StartProcess(AdbPath, args);

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
    }
}
