namespace UnmistakableAPKInstaller
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonDownload = new System.Windows.Forms.Button();
            this.InputDownload = new System.Windows.Forms.TextBox();
            this.LabelDownload = new System.Windows.Forms.Label();
            this.OutputDownload = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.LabelPath = new System.Windows.Forms.Label();
            this.InputPath = new System.Windows.Forms.TextBox();
            this.ButtonPath = new System.Windows.Forms.Button();
            this.LabelDevices = new System.Windows.Forms.Label();
            this.ButtonInstall = new System.Windows.Forms.Button();
            this.ButtonDownloadInstall = new System.Windows.Forms.Button();
            this.ButtonSettings = new System.Windows.Forms.Button();
            this.ButtonSaveLogToFile = new System.Windows.Forms.Button();
            this.DropdownListDevices = new System.Windows.Forms.ComboBox();
            this.LabelWifiMode = new System.Windows.Forms.Label();
            this.LabelUsbMode = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonDownload
            // 
            this.ButtonDownload.Location = new System.Drawing.Point(556, 67);
            this.ButtonDownload.Name = "ButtonDownload";
            this.ButtonDownload.Size = new System.Drawing.Size(103, 23);
            this.ButtonDownload.TabIndex = 0;
            this.ButtonDownload.Text = "Download APK";
            this.ButtonDownload.UseVisualStyleBackColor = true;
            this.ButtonDownload.Click += new System.EventHandler(this.ButtonDownload_Click);
            // 
            // InputDownload
            // 
            this.InputDownload.Location = new System.Drawing.Point(189, 67);
            this.InputDownload.Name = "InputDownload";
            this.InputDownload.Size = new System.Drawing.Size(347, 23);
            this.InputDownload.TabIndex = 1;
            // 
            // LabelDownload
            // 
            this.LabelDownload.AutoSize = true;
            this.LabelDownload.Location = new System.Drawing.Point(83, 70);
            this.LabelDownload.Name = "LabelDownload";
            this.LabelDownload.Size = new System.Drawing.Size(100, 15);
            this.LabelDownload.TabIndex = 2;
            this.LabelDownload.Text = "Google Drive Link";
            // 
            // OutputDownload
            // 
            this.OutputDownload.Location = new System.Drawing.Point(12, 394);
            this.OutputDownload.Name = "OutputDownload";
            this.OutputDownload.Size = new System.Drawing.Size(776, 47);
            this.OutputDownload.TabIndex = 3;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(12, 368);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(776, 23);
            this.ProgressBar.TabIndex = 4;
            // 
            // LabelPath
            // 
            this.LabelPath.AutoSize = true;
            this.LabelPath.Location = new System.Drawing.Point(83, 38);
            this.LabelPath.Name = "LabelPath";
            this.LabelPath.Size = new System.Drawing.Size(56, 15);
            this.LabelPath.TabIndex = 7;
            this.LabelPath.Text = "APK Path";
            // 
            // InputPath
            // 
            this.InputPath.Location = new System.Drawing.Point(189, 35);
            this.InputPath.Name = "InputPath";
            this.InputPath.ReadOnly = true;
            this.InputPath.Size = new System.Drawing.Size(347, 23);
            this.InputPath.TabIndex = 6;
            // 
            // ButtonPath
            // 
            this.ButtonPath.Location = new System.Drawing.Point(556, 35);
            this.ButtonPath.Name = "ButtonPath";
            this.ButtonPath.Size = new System.Drawing.Size(233, 23);
            this.ButtonPath.TabIndex = 5;
            this.ButtonPath.Text = "Select Manually";
            this.ButtonPath.UseVisualStyleBackColor = true;
            this.ButtonPath.Click += new System.EventHandler(this.ButtonPath_Click);
            // 
            // LabelDevices
            // 
            this.LabelDevices.AutoSize = true;
            this.LabelDevices.Location = new System.Drawing.Point(83, 280);
            this.LabelDevices.Name = "LabelDevices";
            this.LabelDevices.Size = new System.Drawing.Size(50, 15);
            this.LabelDevices.TabIndex = 8;
            this.LabelDevices.Text = "Devices:";
            // 
            // ButtonInstall
            // 
            this.ButtonInstall.Location = new System.Drawing.Point(665, 67);
            this.ButtonInstall.Name = "ButtonInstall";
            this.ButtonInstall.Size = new System.Drawing.Size(124, 23);
            this.ButtonInstall.TabIndex = 10;
            this.ButtonInstall.Text = "Install APK";
            this.ButtonInstall.UseVisualStyleBackColor = true;
            this.ButtonInstall.Click += new System.EventHandler(this.ButtonInstall_Click);
            // 
            // ButtonDownloadInstall
            // 
            this.ButtonDownloadInstall.Location = new System.Drawing.Point(606, 96);
            this.ButtonDownloadInstall.Name = "ButtonDownloadInstall";
            this.ButtonDownloadInstall.Size = new System.Drawing.Size(124, 23);
            this.ButtonDownloadInstall.TabIndex = 12;
            this.ButtonDownloadInstall.Text = "Download && Install";
            this.ButtonDownloadInstall.UseVisualStyleBackColor = true;
            this.ButtonDownloadInstall.Click += new System.EventHandler(this.ButtonDownloadInstall_Click);
            // 
            // ButtonSettings
            // 
            this.ButtonSettings.Location = new System.Drawing.Point(1, 2);
            this.ButtonSettings.Name = "ButtonSettings";
            this.ButtonSettings.Size = new System.Drawing.Size(75, 32);
            this.ButtonSettings.TabIndex = 13;
            this.ButtonSettings.Text = "Settings";
            this.ButtonSettings.UseVisualStyleBackColor = true;
            this.ButtonSettings.Click += new System.EventHandler(this.ButtonSettings_Click);
            // 
            // ButtonSaveLogToFile
            // 
            this.ButtonSaveLogToFile.Location = new System.Drawing.Point(530, 306);
            this.ButtonSaveLogToFile.Name = "ButtonSaveLogToFile";
            this.ButtonSaveLogToFile.Size = new System.Drawing.Size(134, 34);
            this.ButtonSaveLogToFile.TabIndex = 14;
            this.ButtonSaveLogToFile.Text = "Save Log to File";
            this.ButtonSaveLogToFile.UseVisualStyleBackColor = true;
            this.ButtonSaveLogToFile.Click += new System.EventHandler(this.ButtonSaveLogToFile_Click);
            // 
            // DropdownListDevices
            // 
            this.DropdownListDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DropdownListDevices.FormattingEnabled = true;
            this.DropdownListDevices.Location = new System.Drawing.Point(142, 277);
            this.DropdownListDevices.Name = "DropdownListDevices";
            this.DropdownListDevices.Size = new System.Drawing.Size(192, 23);
            this.DropdownListDevices.TabIndex = 15;
            this.DropdownListDevices.SelectedIndexChanged += new System.EventHandler(this.DropdownListDevices_SelectedIndexChanged);
            // 
            // LabelWifiMode
            // 
            this.LabelWifiMode.AutoSize = true;
            this.LabelWifiMode.Location = new System.Drawing.Point(269, 316);
            this.LabelWifiMode.Name = "LabelWifiMode";
            this.LabelWifiMode.Size = new System.Drawing.Size(30, 15);
            this.LabelWifiMode.TabIndex = 17;
            this.LabelWifiMode.Text = "WIFI";
            // 
            // LabelUsbMode
            // 
            this.LabelUsbMode.AutoSize = true;
            this.LabelUsbMode.Location = new System.Drawing.Point(207, 316);
            this.LabelUsbMode.Name = "LabelUsbMode";
            this.LabelUsbMode.Size = new System.Drawing.Size(28, 15);
            this.LabelUsbMode.TabIndex = 16;
            this.LabelUsbMode.Text = "USB";
            this.LabelUsbMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(83, 316);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 15);
            this.label1.TabIndex = 18;
            this.label1.Text = "{Status}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LabelWifiMode);
            this.Controls.Add(this.LabelUsbMode);
            this.Controls.Add(this.DropdownListDevices);
            this.Controls.Add(this.ButtonSaveLogToFile);
            this.Controls.Add(this.ButtonSettings);
            this.Controls.Add(this.ButtonDownloadInstall);
            this.Controls.Add(this.ButtonInstall);
            this.Controls.Add(this.LabelDevices);
            this.Controls.Add(this.LabelPath);
            this.Controls.Add(this.InputPath);
            this.Controls.Add(this.ButtonPath);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.OutputDownload);
            this.Controls.Add(this.LabelDownload);
            this.Controls.Add(this.InputDownload);
            this.Controls.Add(this.ButtonDownload);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Unmistakable APK Installer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button ButtonDownload;
        private TextBox InputDownload;
        private Label LabelDownload;
        private Label OutputDownload;
        private ProgressBar ProgressBar;
        private Label LabelPath;
        private TextBox InputPath;
        private Button ButtonPath;
        private Label LabelDevices;
        private Button ButtonInstall;
        private Button ButtonDownloadInstall;
        private Button ButtonSettings;
        private Button ButtonSaveLogToFile;
        private ComboBox DropdownListDevices;
        private Label LabelWifiMode;
        private Label LabelUsbMode;
        private Label label1;
    }
}