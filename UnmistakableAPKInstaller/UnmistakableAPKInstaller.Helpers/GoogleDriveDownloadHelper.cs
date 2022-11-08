using Serilog;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using UnmistakableAPKInstaller.Helpers.Models.GoogleDrive;

namespace UnmistakableAPKInstaller.Helpers
{
    /// <summary>
    /// Google Drive API helper class
    /// </summary>
    public class GoogleDriveDownloadHelper
    {
        public GoogleDriveDownloadHelper(string apiKey, string extractFolder)
        {
            this.apiKey = apiKey;
            this.extractFolder = extractFolder;
        }

        /// <summary>
        /// API key
        /// </summary>
        string apiKey;

        /// <summary>
        /// Folder to save GD files
        /// </summary>
        string extractFolder;

        /// <summary>
        /// Check GD link
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool ValidateUrl(string url)
        {
            if(url is null)
            {
                return false;
            }

            return url.StartsWith("https://drive.google.com/file/d/") && url.Contains("/view");
        }

        /// <summary>
        /// Download file asynchronously
        /// </summary>
        /// <param name="url"></param>
        /// <param name="outText"></param>
        /// <param name="outProgress"></param>
        /// <returns></returns>
        public async Task<(bool status, string path)> DownloadFileAsync(string url,
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
                    var fileName = await GetFilenameAsync(fileDataUrl);

                    var gdUrl = $"{fileDataUrl}&alt=media";

                    var path = $"{extractFolder}/{fileName}";
                    await wc.DownloadFileTaskAsync(new Uri(gdUrl), path);

                    outText("GD file is loaded!");
                    return (true, path);
                }
                catch (Exception e)
                {
                    outText(e.Message);
                    Log.Error("GD Download Helper: {0}", e.ToString());
                    return (false, string.Empty);
                }
            }
        }

        /// <summary>
        /// Find file id by <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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
                Log.Error("GD Download Helper: {0}", e.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Get FileData URL by <paramref name="fileId"/>
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        private string GetDownloadFileDataUrl(string fileId)
        {
            return $"https://www.googleapis.com/drive/v3/files/{fileId}?key={apiKey}";
        }

        /// <summary>
        /// Get filename by <paramref name="fileDataUrl"/>
        /// </summary>
        /// <param name="fileDataUrl"></param>
        /// <returns></returns>
        private async Task<string> GetFilenameAsync(string fileDataUrl)
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
                    Log.Error("GD Download Helper: {0}", e.ToString());
                    return null;
                }
            }
        }
    }
}
