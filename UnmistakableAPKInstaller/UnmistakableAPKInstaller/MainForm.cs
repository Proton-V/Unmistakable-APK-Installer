using System.Configuration;
using System.Diagnostics;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Tools;

namespace UnmistakableAPKInstaller
{
    public partial class MainForm : Form
    {
        enum MainFormState
        {
            Idle,
            APKLoading,
            ToolsLoading,
        }

        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        string AppDomainDirectory => AppDomain.CurrentDomain.BaseDirectory;
        string DownloadAPKFolder => $"{AppDomainDirectory}GoogleDrive";

        CmdToolsProvider cmdToolsProvider;
        GoogleDriveDownloadHelper gdDownloadHelper;

        private void Init()
        {
            Directory.CreateDirectory(DownloadAPKFolder);

            var platformToolsDownloadLink = ConfigurationManager.AppSettings["PlatformToolsDownloadLink"];
            var platformToolsFolderPath = ConfigurationManager.AppSettings["AndroidPlatformToolsFolderPath"];
            var platformTools = new AndroidPlatformTools(platformToolsDownloadLink, GetFullPath(platformToolsFolderPath));

            var aapt2DownloadLink = ConfigurationManager.AppSettings["Aapt2DownloadLink"];
            var aapt2FolderPath = ConfigurationManager.AppSettings["Aapt2FolderPath"];
            var aapt2Tool = new Aapt2Tool(aapt2DownloadLink, GetFullPath(aapt2FolderPath));

            var gdApiKey = ConfigurationManager.AppSettings["GoogleDriveApiKey"];
            gdDownloadHelper = new GoogleDriveDownloadHelper(gdApiKey, DownloadAPKFolder);

            cmdToolsProvider = new CmdToolsProvider(
                platformTools,
                aapt2Tool);

            if (!cmdToolsProvider.CheckExistsTools())
            {
                DownloadTools();
            }
        }

        private string GetFullPath(string relativePath) => $"{AppDomainDirectory}/{relativePath}";

        private async void DownloadTools()
        {
            ChangeFormState(MainFormState.ToolsLoading);

            var status = await cmdToolsProvider.TryDownloadTools(
                (str) => OutputDownload.Text = str,
                (progress) => ProgressBar.Value = progress);

            MessageBox.Show(status ? "Download is completed!" : "Fail with download...",
                "Download status", MessageBoxButtons.OK);

            OutputDownload.ResetText();
            ProgressBar.Value = 0;

            ChangeFormState(MainFormState.Idle);
        }

        private void ChangeFormState(MainFormState state)
        {
            switch (state)
            {
                case MainFormState.Idle:
                    ChangeVisibility(true);
                    break;
                case MainFormState.ToolsLoading:
                    ChangeVisibility(false);
                    OutputDownload.Visible = true;
                    ProgressBar.Visible = true;
                    break;
                case MainFormState.APKLoading:
                    ChangeVisibility(true);
                    ButtonDownload.Visible = false;
                    ButtonDownloadInstall.Visible = false;
                    ButtonInstall.Visible = false;
                    break;
            }
        }

        private void ChangeVisibility(bool value)
        {
            ButtonDownload.Visible = value;
            InputDownload.Visible = value;
            LabelDownload.Visible = value;
            OutputDownload.Visible = value;
            ProgressBar.Visible = value;
            LabelPath.Visible = value;
            InputPath.Visible = value;
            ButtonPath.Visible = value;
            LabelDevices.Visible = value;
            OutputDevices.Visible = value;
            ButtonInstall.Visible = value;
            ButtonDevices.Visible = value;
            ButtonDownloadInstall.Visible = value;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private async void ButtonDownload_Click(object sender, EventArgs e)
        {
            ChangeFormState(MainFormState.APKLoading);
            await DownloadAPK();
            ChangeFormState(MainFormState.Idle);
        }

        private void ButtonPath_Click(object sender, EventArgs e)
        {
            OpenFileExplorer();
        }

        private void OpenFileExplorer()
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = DownloadAPKFolder;
                openFileDialog.Filter = "APK Files (*.apk)|*.apk";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            InputPath.Text = filePath;
        }

        private async void ButtonInstall_Click(object sender, EventArgs e)
        {
            await InstallAPK();
        }

        private async void ButtonDevices_Click(object sender, EventArgs e)
        {
            OutputDevices.Text = await cmdToolsProvider.GetAndroidDevices();
        }

        private async void ButtonDownloadInstall_Click(object sender, EventArgs e)
        {
            ChangeFormState(MainFormState.APKLoading);
            await DownloadAPK(false);
            await InstallAPK();
            ChangeFormState(MainFormState.Idle);
        }

        #region Download && Install APK
        private async Task DownloadAPK(bool showStatus = true)
        {
            var url = InputDownload.Text;
            var data = await gdDownloadHelper.DownloadFile(url,
                (str) => OutputDownload.Text = str,
                (progress) => ProgressBar.Value = progress);

            if (showStatus)
            {
                MessageBox.Show(data.status ? "Download is completed!" : "Fail with download...",
                    "File download status", MessageBoxButtons.OK);
            }

            OutputDownload.ResetText();
            ProgressBar.Value = 0;

            // Update APK path field
            if (data.status)
            {
                InputPath.Text = data.path;
            }

        }

        private async Task InstallAPK()
        {
            ProgressBar.Value = 10;
            OutputDownload.Text = "Uninstall previous version...";
            await cmdToolsProvider.TryUninstallAPKByPath(InputPath.Text, (str) => OutputDownload.Text = str);

            ProgressBar.Value = 50;
            OutputDownload.Text = "Install new version...";
            var status = await cmdToolsProvider.TryInstallAPK(InputPath.Text, (str) => OutputDownload.Text = str);

            ProgressBar.Value = 100;
            MessageBox.Show(status ? "Install is completed!" : "Fail with install...",
                "Install status", MessageBoxButtons.OK);

            OutputDownload.ResetText();
            ProgressBar.Value = 0;
        }
        #endregion
    }
}