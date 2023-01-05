namespace UnmistakableAPKInstaller.Tools
{
    /// <summary>
    /// Base class for downloadable cmd tools.
    /// </summary>
    public abstract class BaseCmdTool
    {
        public BaseCmdTool(string downloadLink, string toolFolderPath)
        {
            this.toolFolderPath = toolFolderPath;
            this.downloadLink = downloadLink;
        }

        /// <summary>
        /// Default folder
        /// </summary>
        protected string toolFolderPath;

        /// <summary>
        /// Tool download link
        /// </summary>
        protected string downloadLink;

        /// <summary>
        /// Check tool exists
        /// </summary>
        /// <returns></returns>
        public abstract bool Exists();

        /// <summary>
        /// Try download tool
        /// </summary>
        /// <param name="outText"></param>
        /// <param name="outProgress"></param>
        /// <returns></returns>
        public abstract Task<bool> TryDownloadToolAsync(Action<string> outText, Action<int> outProgress);
    }
}
