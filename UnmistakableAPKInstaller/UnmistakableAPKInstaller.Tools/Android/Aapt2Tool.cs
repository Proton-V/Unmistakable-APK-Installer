using Serilog;
using System.IO.Compression;
using System.Net;
using UnmistakableAPKInstaller.Helpers;

namespace UnmistakableAPKInstaller.Tools.Android
{
    public class Aapt2Tool : BaseCmdTool
    {
        public const string AAPT2_FILE_NAME = "aapt2";

        public Aapt2Tool(string downloadLink, string toolFolderPath) : base(downloadLink, toolFolderPath)
        {
        }

        public string Aapt2Path => $"{toolFolderPath}/{AAPT2_FILE_NAME}";
        string ZipPath => $"{toolFolderPath}/Aapt2.zip";

        public override bool Exists()
        {
            Directory.CreateDirectory(toolFolderPath);
            return Directory.GetFiles(toolFolderPath, $"{AAPT2_FILE_NAME}.*").Length > 0;
        }

        public override async Task<bool> TryDownloadToolAsync(Action<string> outText, Action<int> outProgress)
        {
            if (Exists())
            {
                return false;
            }

            Log.Debug($"Tool {GetType().AssemblyQualifiedName} is loading:\n" +
                $"folderPath - {toolFolderPath}\ndownloadLink - {downloadLink}");
            outText("Aapt2 is loading...");

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

                    Log.Debug("Aapt2 is loaded!");
                    outText("Aapt2 is loaded!");
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error("Aapt2: {0}", e.ToString());
                    outText(e.Message);
                    return false;
                }
            }
        }

        public async Task<string> TryGetAPKBundleNameAsync(string path)
        {
            try
            {
                var args = $"dump {path}";
                var data = await CmdHelper.StartProcessAsync(Aapt2Path, args);
                if (!string.IsNullOrEmpty(data.data))
                {
                    var packageNameStartStr = "Package name=";

                    var packageName = data.data
                        .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .First(x => x.StartsWith(packageNameStartStr))
                        ?.Split("id=")
                        ?.ElementAtOrDefault(0)
                        ?.Replace(packageNameStartStr, string.Empty)
                        .Trim();

                    Log.Debug($"Found bundle name {packageName} for {path}");

                    return packageName;
                }
                else
                {
                    throw new Exception(data.error);
                }
            }
            catch (Exception e)
            {
                Log.Error("Aapt2: {0}", e.ToString());
                return e.Message;
            }
        }
    }
}
