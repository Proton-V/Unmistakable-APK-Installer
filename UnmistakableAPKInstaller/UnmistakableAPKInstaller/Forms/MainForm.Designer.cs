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
            this.LabelStatusDevice = new System.Windows.Forms.Label();
            this.PictureBoxWifiMode = new System.Windows.Forms.PictureBox();
            this.PictureBoxUsbMode = new System.Windows.Forms.PictureBox();
            this.ButtonWifiModeUpdate = new System.Windows.Forms.Button();
            this.ButtonDeviceListUpdate = new System.Windows.Forms.Button();
            this.GroupBoxFormRoot = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxWifiMode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxUsbMode)).BeginInit();
            this.GroupBoxFormRoot.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonDownload
            // 
            this.ButtonDownload.Location = new System.Drawing.Point(561, 98);
            this.ButtonDownload.Name = "ButtonDownload";
            this.ButtonDownload.Size = new System.Drawing.Size(103, 23);
            this.ButtonDownload.TabIndex = 0;
            this.ButtonDownload.Text = "Download APK";
            this.ButtonDownload.UseVisualStyleBackColor = true;
            this.ButtonDownload.Click += new System.EventHandler(this.ButtonDownload_Click);
            // 
            // InputDownload
            // 
            this.InputDownload.Location = new System.Drawing.Point(194, 98);
            this.InputDownload.Name = "InputDownload";
            this.InputDownload.Size = new System.Drawing.Size(347, 23);
            this.InputDownload.TabIndex = 1;
            // 
            // LabelDownload
            // 
            this.LabelDownload.AutoSize = true;
            this.LabelDownload.Location = new System.Drawing.Point(88, 101);
            this.LabelDownload.Name = "LabelDownload";
            this.LabelDownload.Size = new System.Drawing.Size(100, 15);
            this.LabelDownload.TabIndex = 2;
            this.LabelDownload.Text = "Google Drive Link";
            // 
            // OutputDownload
            // 
            this.OutputDownload.BackColor = System.Drawing.Color.Silver;
            this.OutputDownload.Location = new System.Drawing.Point(17, 406);
            this.OutputDownload.Name = "OutputDownload";
            this.OutputDownload.Size = new System.Drawing.Size(776, 66);
            this.OutputDownload.TabIndex = 3;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(17, 369);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(776, 23);
            this.ProgressBar.TabIndex = 4;
            // 
            // LabelPath
            // 
            this.LabelPath.AutoSize = true;
            this.LabelPath.Location = new System.Drawing.Point(88, 69);
            this.LabelPath.Name = "LabelPath";
            this.LabelPath.Size = new System.Drawing.Size(56, 15);
            this.LabelPath.TabIndex = 7;
            this.LabelPath.Text = "APK Path";
            // 
            // InputPath
            // 
            this.InputPath.Location = new System.Drawing.Point(194, 66);
            this.InputPath.Name = "InputPath";
            this.InputPath.ReadOnly = true;
            this.InputPath.Size = new System.Drawing.Size(347, 23);
            this.InputPath.TabIndex = 6;
            // 
            // ButtonPath
            // 
            this.ButtonPath.Location = new System.Drawing.Point(561, 66);
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
            this.LabelDevices.Location = new System.Drawing.Point(194, 200);
            this.LabelDevices.Name = "LabelDevices";
            this.LabelDevices.Size = new System.Drawing.Size(50, 15);
            this.LabelDevices.TabIndex = 8;
            this.LabelDevices.Text = "Devices:";
            // 
            // ButtonInstall
            // 
            this.ButtonInstall.Location = new System.Drawing.Point(670, 98);
            this.ButtonInstall.Name = "ButtonInstall";
            this.ButtonInstall.Size = new System.Drawing.Size(124, 23);
            this.ButtonInstall.TabIndex = 10;
            this.ButtonInstall.Text = "Install APK";
            this.ButtonInstall.UseVisualStyleBackColor = true;
            this.ButtonInstall.Click += new System.EventHandler(this.ButtonInstall_Click);
            // 
            // ButtonDownloadInstall
            // 
            this.ButtonDownloadInstall.Location = new System.Drawing.Point(611, 127);
            this.ButtonDownloadInstall.Name = "ButtonDownloadInstall";
            this.ButtonDownloadInstall.Size = new System.Drawing.Size(124, 23);
            this.ButtonDownloadInstall.TabIndex = 12;
            this.ButtonDownloadInstall.Text = "Download && Install";
            this.ButtonDownloadInstall.UseVisualStyleBackColor = true;
            this.ButtonDownloadInstall.Click += new System.EventHandler(this.ButtonDownloadInstall_Click);
            // 
            // ButtonSettings
            // 
            this.ButtonSettings.Location = new System.Drawing.Point(6, 33);
            this.ButtonSettings.Name = "ButtonSettings";
            this.ButtonSettings.Size = new System.Drawing.Size(75, 32);
            this.ButtonSettings.TabIndex = 13;
            this.ButtonSettings.Text = "Settings";
            this.ButtonSettings.UseVisualStyleBackColor = true;
            this.ButtonSettings.Click += new System.EventHandler(this.ButtonSettings_Click);
            // 
            // ButtonSaveLogToFile
            // 
            this.ButtonSaveLogToFile.Location = new System.Drawing.Point(296, 293);
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
            this.DropdownListDevices.Location = new System.Drawing.Point(253, 197);
            this.DropdownListDevices.Name = "DropdownListDevices";
            this.DropdownListDevices.Size = new System.Drawing.Size(192, 23);
            this.DropdownListDevices.TabIndex = 15;
            this.DropdownListDevices.SelectedIndexChanged += new System.EventHandler(this.DropdownListDevices_SelectedIndexChanged);
            // 
            // LabelWifiMode
            // 
            this.LabelWifiMode.AutoSize = true;
            this.LabelWifiMode.Location = new System.Drawing.Point(380, 236);
            this.LabelWifiMode.Name = "LabelWifiMode";
            this.LabelWifiMode.Size = new System.Drawing.Size(30, 15);
            this.LabelWifiMode.TabIndex = 17;
            this.LabelWifiMode.Text = "WIFI";
            // 
            // LabelUsbMode
            // 
            this.LabelUsbMode.AutoSize = true;
            this.LabelUsbMode.Location = new System.Drawing.Point(318, 236);
            this.LabelUsbMode.Name = "LabelUsbMode";
            this.LabelUsbMode.Size = new System.Drawing.Size(28, 15);
            this.LabelUsbMode.TabIndex = 16;
            this.LabelUsbMode.Text = "USB";
            this.LabelUsbMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LabelStatusDevice
            // 
            this.LabelStatusDevice.AutoSize = true;
            this.LabelStatusDevice.Location = new System.Drawing.Point(194, 236);
            this.LabelStatusDevice.Name = "LabelStatusDevice";
            this.LabelStatusDevice.Size = new System.Drawing.Size(47, 15);
            this.LabelStatusDevice.TabIndex = 18;
            this.LabelStatusDevice.Text = "{Status}";
            this.LabelStatusDevice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PictureBoxWifiMode
            // 
            this.PictureBoxWifiMode.BackColor = System.Drawing.Color.Red;
            this.PictureBoxWifiMode.Location = new System.Drawing.Point(416, 236);
            this.PictureBoxWifiMode.Name = "PictureBoxWifiMode";
            this.PictureBoxWifiMode.Size = new System.Drawing.Size(14, 15);
            this.PictureBoxWifiMode.TabIndex = 19;
            this.PictureBoxWifiMode.TabStop = false;
            // 
            // PictureBoxUsbMode
            // 
            this.PictureBoxUsbMode.BackColor = System.Drawing.Color.Red;
            this.PictureBoxUsbMode.Location = new System.Drawing.Point(352, 236);
            this.PictureBoxUsbMode.Name = "PictureBoxUsbMode";
            this.PictureBoxUsbMode.Size = new System.Drawing.Size(14, 15);
            this.PictureBoxUsbMode.TabIndex = 20;
            this.PictureBoxUsbMode.TabStop = false;
            // 
            // ButtonWifiModeUpdate
            // 
            this.ButtonWifiModeUpdate.Location = new System.Drawing.Point(446, 232);
            this.ButtonWifiModeUpdate.Name = "ButtonWifiModeUpdate";
            this.ButtonWifiModeUpdate.Size = new System.Drawing.Size(115, 23);
            this.ButtonWifiModeUpdate.TabIndex = 21;
            this.ButtonWifiModeUpdate.Text = "Update Wifi Mode";
            this.ButtonWifiModeUpdate.UseVisualStyleBackColor = true;
            this.ButtonWifiModeUpdate.Click += new System.EventHandler(this.ButtonWifiModeUpdate_Click);
            // 
            // ButtonDeviceListUpdate
            // 
            this.ButtonDeviceListUpdate.Location = new System.Drawing.Point(465, 197);
            this.ButtonDeviceListUpdate.Name = "ButtonDeviceListUpdate";
            this.ButtonDeviceListUpdate.Size = new System.Drawing.Size(115, 23);
            this.ButtonDeviceListUpdate.TabIndex = 22;
            this.ButtonDeviceListUpdate.Text = "Update Device List";
            this.ButtonDeviceListUpdate.UseVisualStyleBackColor = true;
            this.ButtonDeviceListUpdate.Click += new System.EventHandler(this.ButtonDeviceListUpdate_Click);
            // 
            // GroupBoxFormRoot
            // 
            this.GroupBoxFormRoot.Controls.Add(this.ButtonDeviceListUpdate);
            this.GroupBoxFormRoot.Controls.Add(this.ButtonSettings);
            this.GroupBoxFormRoot.Controls.Add(this.ButtonWifiModeUpdate);
            this.GroupBoxFormRoot.Controls.Add(this.ButtonDownload);
            this.GroupBoxFormRoot.Controls.Add(this.PictureBoxUsbMode);
            this.GroupBoxFormRoot.Controls.Add(this.InputDownload);
            this.GroupBoxFormRoot.Controls.Add(this.PictureBoxWifiMode);
            this.GroupBoxFormRoot.Controls.Add(this.LabelDownload);
            this.GroupBoxFormRoot.Controls.Add(this.LabelStatusDevice);
            this.GroupBoxFormRoot.Controls.Add(this.OutputDownload);
            this.GroupBoxFormRoot.Controls.Add(this.LabelWifiMode);
            this.GroupBoxFormRoot.Controls.Add(this.ProgressBar);
            this.GroupBoxFormRoot.Controls.Add(this.LabelUsbMode);
            this.GroupBoxFormRoot.Controls.Add(this.ButtonPath);
            this.GroupBoxFormRoot.Controls.Add(this.DropdownListDevices);
            this.GroupBoxFormRoot.Controls.Add(this.InputPath);
            this.GroupBoxFormRoot.Controls.Add(this.ButtonSaveLogToFile);
            this.GroupBoxFormRoot.Controls.Add(this.LabelPath);
            this.GroupBoxFormRoot.Controls.Add(this.LabelDevices);
            this.GroupBoxFormRoot.Controls.Add(this.ButtonDownloadInstall);
            this.GroupBoxFormRoot.Controls.Add(this.ButtonInstall);
            this.GroupBoxFormRoot.Location = new System.Drawing.Point(-5, -29);
            this.GroupBoxFormRoot.Name = "GroupBoxFormRoot";
            this.GroupBoxFormRoot.Size = new System.Drawing.Size(814, 489);
            this.GroupBoxFormRoot.TabIndex = 23;
            this.GroupBoxFormRoot.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.GroupBoxFormRoot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Unmistakable APK Installer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxWifiMode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxUsbMode)).EndInit();
            this.GroupBoxFormRoot.ResumeLayout(false);
            this.GroupBoxFormRoot.PerformLayout();
            this.ResumeLayout(false);

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
        private Label LabelStatusDevice;
        private PictureBox PictureBoxWifiMode;
        private PictureBox PictureBoxUsbMode;
        private Button ButtonWifiModeUpdate;
        private Button ButtonDeviceListUpdate;
        private GroupBox GroupBoxFormRoot;
    }
}