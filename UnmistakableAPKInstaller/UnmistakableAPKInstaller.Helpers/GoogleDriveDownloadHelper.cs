using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using UnmistakableAPKInstaller.Helpers.Models.GoogleDrive;

namespace UnmistakableAPKInstaller.Helpers
{
    public class GoogleDriveDownloadHelper
    {
        public GoogleDriveDownloadHelper(string apiKey, string extractFolder)
        {
            this.apiKey = apiKey;
            this.extractFolder = extractFolder;
        }

        string apiKey;
        string extractFolder;

        public bool ValidateUrl(string url)
        {
            return url.StartsWith("https://drive.google.com/file/d/") && url.Contains("/view");
        }

        public async Task<(bool status, string path)> DownloadFile(string url,
            Action<string> outText, Action<int> outProgress)
        {
            if (!ValidateUrl(url))
            {
                outText("URL NOT VALID");
                return (false, string.Empty);
            }

            outText("GD file is loading...");

            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += (sender, args) =>
                {
                    outProgress(args.ProgressPercentage);
                };

                try
                {
                    var getFileId = GetFileId(url);
                    var fileDataUrl = GetDownloadFileDataUrl(getFileId);
                    var fileName = await GetFilename(fileDataUrl);

                    var gdUrl = $"{fileDataUrl}&alt=media";

                    var path = $"{extractFolder}/{fileName}";
                    await wc.DownloadFileTaskAsync(new Uri(gdUrl), path);

                    outText("GD file is loaded!");
                    return (true, path);
                }
                catch (Exception e)
                {
                    outText(e.Message);
                    CustomLogger.WriteToLog("GD Download Helper: {0}", e.ToString());
                    return (false, string.Empty);
                }
            }
        }

        private string GetFileId(string url)
        {
            try
            {
                var regex = new Regex("file\\/d\\/(.*)\\/view");
                var fileId = regex.Match(url).Groups[1].Value;
                return fileId;
            }
            catch (Exception e)
            {
                CustomLogger.WriteToLog("GD Download Helper: {0}", e.ToString());
                return string.Empty;
            }
        }

        private string GetDownloadFileDataUrl(string fileId)
        {
            return $"https://www.googleapis.com/drive/v3/files/{fileId}?key={apiKey}";
        }

        private async Task<string> GetFilename(string fileDataUrl)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    var str = await wc.DownloadStringTaskAsync(new Uri(fileDataUrl));
                    return JsonSerializer.Deserialize<FileData>(str).name;
                }
                catch (Exception e)
                {
                    CustomLogger.WriteToLog("GD Download Helper: {0}", e.ToString());
                    return null;
                }
            }
        }
    }
}
