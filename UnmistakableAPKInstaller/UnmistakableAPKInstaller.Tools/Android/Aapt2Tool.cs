using System.IO.Compression;
using System.Net;
using UnmistakableAPKInstaller.Helpers;

namespace UnmistakableAPKInstaller.Tools.Android
{
    public class Aapt2Tool : BaseCmdTool
    {
        public Aapt2Tool(string downloadLink, string toolFolderPath) : base(downloadLink, toolFolderPath)
        {
        }

        public string Aapt2Path => $"{toolFolderPath}/aapt2.exe";
        string ZipPath => $"{toolFolderPath}/Aapt2.zip";

        public override bool Exists()
        {
            return File.Exists(Aapt2Path);
        }

        public override async Task<bool> TryDownloadToolAsync(Action<string> outText, Action<int> outProgress)
        {
            if (Exists())
            {
                return false;
            }

            outText("Aapt2 is loading...");

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

                    outText("Aapt2 is loaded!");
                    return true;
                }
                catch (Exception e)
                {
                    outText(e.Message);
                    CustomLogger.Log("Aapt2: {0}", e.ToString());
                    return false;
                }
            }
        }

        public async Task<string> TryGetAPKBundleNameAsync(string path)
        {
            try
            {
                var args = $"dump {path} | findstr -n \"package: name = \"";
                var data = await CmdHelper.StartProcessAsync(Aapt2Path, args);
                if (!string.IsNullOrEmpty(data.data))
                {
                    return data.data;
                }
                else
                {
                    throw new Exception(data.error);
                }
            }
            catch (Exception e)
            {
                CustomLogger.Log("Aapt2: {0}", e.ToString());
                return e.Message;
            }
        }
    }
}
