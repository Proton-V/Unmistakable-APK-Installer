namespace UnmistakableAPKInstaller.Tools
{
    public abstract class BaseCmdTool
    {
        public BaseCmdTool(string downloadLink, string toolFolderPath)
        {
            this.toolFolderPath = toolFolderPath;
            this.downloadLink = downloadLink;
        }

        protected string toolFolderPath;
        protected string downloadLink;

        public abstract bool Exists();
        public abstract Task<bool> TryDownloadAsync(Action<string> outText, Action<int> outProgress);
    }
}
