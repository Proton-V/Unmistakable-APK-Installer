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
            this.OutputDevices = new System.Windows.Forms.RichTextBox();
            this.ButtonInstall = new System.Windows.Forms.Button();
            this.ButtonDevices = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonDownload
            // 
            this.ButtonDownload.Location = new System.Drawing.Point(555, 48);
            this.ButtonDownload.Name = "ButtonDownload";
            this.ButtonDownload.Size = new System.Drawing.Size(124, 23);
            this.ButtonDownload.TabIndex = 0;
            this.ButtonDownload.Text = "Download APK";
            this.ButtonDownload.UseVisualStyleBackColor = true;
            this.ButtonDownload.Click += new System.EventHandler(this.ButtonDownload_Click);
            // 
            // InputDownload
            // 
            this.InputDownload.Location = new System.Drawing.Point(188, 48);
            this.InputDownload.Name = "InputDownload";
            this.InputDownload.Size = new System.Drawing.Size(347, 23);
            this.InputDownload.TabIndex = 1;
            // 
            // LabelDownload
            // 
            this.LabelDownload.AutoSize = true;
            this.LabelDownload.Location = new System.Drawing.Point(82, 51);
            this.LabelDownload.Name = "LabelDownload";
            this.LabelDownload.Size = new System.Drawing.Size(100, 15);
            this.LabelDownload.TabIndex = 2;
            this.LabelDownload.Text = "Google Drive Link";
            // 
            // OutputDownload
            // 
            this.OutputDownload.AutoSize = true;
            this.OutputDownload.Location = new System.Drawing.Point(12, 353);
            this.OutputDownload.Name = "OutputDownload";
            this.OutputDownload.Size = new System.Drawing.Size(0, 15);
            this.OutputDownload.TabIndex = 3;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(12, 317);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(776, 23);
            this.ProgressBar.TabIndex = 4;
            // 
            // LabelPath
            // 
            this.LabelPath.AutoSize = true;
            this.LabelPath.Location = new System.Drawing.Point(82, 19);
            this.LabelPath.Name = "LabelPath";
            this.LabelPath.Size = new System.Drawing.Size(56, 15);
            this.LabelPath.TabIndex = 7;
            this.LabelPath.Text = "APK Path";
            // 
            // InputPath
            // 
            this.InputPath.Location = new System.Drawing.Point(188, 16);
            this.InputPath.Name = "InputPath";
            this.InputPath.ReadOnly = true;
            this.InputPath.Size = new System.Drawing.Size(347, 23);
            this.InputPath.TabIndex = 6;
            // 
            // ButtonPath
            // 
            this.ButtonPath.Location = new System.Drawing.Point(555, 16);
            this.ButtonPath.Name = "ButtonPath";
            this.ButtonPath.Size = new System.Drawing.Size(124, 23);
            this.ButtonPath.TabIndex = 5;
            this.ButtonPath.Text = "Select Manually";
            this.ButtonPath.UseVisualStyleBackColor = true;
            this.ButtonPath.Click += new System.EventHandler(this.ButtonPath_Click);
            // 
            // LabelDevices
            // 
            this.LabelDevices.AutoSize = true;
            this.LabelDevices.Location = new System.Drawing.Point(82, 108);
            this.LabelDevices.Name = "LabelDevices";
            this.LabelDevices.Size = new System.Drawing.Size(50, 15);
            this.LabelDevices.TabIndex = 8;
            this.LabelDevices.Text = "Devices:";
            // 
            // OutputDevices
            // 
            this.OutputDevices.Location = new System.Drawing.Point(158, 105);
            this.OutputDevices.Name = "OutputDevices";
            this.OutputDevices.ReadOnly = true;
            this.OutputDevices.Size = new System.Drawing.Size(521, 125);
            this.OutputDevices.TabIndex = 9;
            this.OutputDevices.Text = "";
            // 
            // ButtonInstall
            // 
            this.ButtonInstall.Location = new System.Drawing.Point(388, 258);
            this.ButtonInstall.Name = "ButtonInstall";
            this.ButtonInstall.Size = new System.Drawing.Size(75, 23);
            this.ButtonInstall.TabIndex = 10;
            this.ButtonInstall.Text = "Install APK";
            this.ButtonInstall.UseVisualStyleBackColor = true;
            this.ButtonInstall.Click += new System.EventHandler(this.ButtonInstall_Click);
            // 
            // ButtonDevices
            // 
            this.ButtonDevices.Location = new System.Drawing.Point(701, 128);
            this.ButtonDevices.Name = "ButtonDevices";
            this.ButtonDevices.Size = new System.Drawing.Size(75, 78);
            this.ButtonDevices.TabIndex = 11;
            this.ButtonDevices.Text = "Update devices";
            this.ButtonDevices.UseVisualStyleBackColor = true;
            this.ButtonDevices.Click += new System.EventHandler(this.ButtonDevices_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ButtonDevices);
            this.Controls.Add(this.ButtonInstall);
            this.Controls.Add(this.OutputDevices);
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
        private RichTextBox OutputDevices;
        private Button ButtonInstall;
        private Button ButtonDevices;
    }
}