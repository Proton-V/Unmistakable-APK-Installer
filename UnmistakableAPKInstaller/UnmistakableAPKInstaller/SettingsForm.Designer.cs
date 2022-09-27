namespace UnmistakableAPKInstaller
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.CheckBoxSetBufferSizeOnInstallAPK = new System.Windows.Forms.CheckBox();
            this.TextBoxBuffSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.InputDeviceLogFolderPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ButtonLogFolderSelect = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.InputGDapiKey = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ButtonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(54, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Device log buffer settings";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CheckBoxSetBufferSizeOnInstallAPK
            // 
            this.CheckBoxSetBufferSizeOnInstallAPK.AutoSize = true;
            this.CheckBoxSetBufferSizeOnInstallAPK.Location = new System.Drawing.Point(54, 41);
            this.CheckBoxSetBufferSizeOnInstallAPK.Name = "CheckBoxSetBufferSizeOnInstallAPK";
            this.CheckBoxSetBufferSizeOnInstallAPK.Size = new System.Drawing.Size(168, 19);
            this.CheckBoxSetBufferSizeOnInstallAPK.TabIndex = 1;
            this.CheckBoxSetBufferSizeOnInstallAPK.Text = "Set Buff Size On Install APK";
            this.CheckBoxSetBufferSizeOnInstallAPK.UseVisualStyleBackColor = true;
            // 
            // TextBoxBuffSize
            // 
            this.TextBoxBuffSize.Location = new System.Drawing.Point(122, 66);
            this.TextBoxBuffSize.MaxLength = 2;
            this.TextBoxBuffSize.Name = "TextBoxBuffSize";
            this.TextBoxBuffSize.Size = new System.Drawing.Size(100, 23);
            this.TextBoxBuffSize.TabIndex = 2;
            this.TextBoxBuffSize.Validating += this.TextBoxBuffSize_Validating;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(36, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Buff size (MB)";
            // 
            // InputDeviceLogFolderPath
            // 
            this.InputDeviceLogFolderPath.Location = new System.Drawing.Point(12, 127);
            this.InputDeviceLogFolderPath.Name = "InputDeviceLogFolderPath";
            this.InputDeviceLogFolderPath.ReadOnly = true;
            this.InputDeviceLogFolderPath.Size = new System.Drawing.Size(133, 23);
            this.InputDeviceLogFolderPath.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(27, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Device Log Folder";
            // 
            // ButtonLogFolderSelect
            // 
            this.ButtonLogFolderSelect.Location = new System.Drawing.Point(151, 127);
            this.ButtonLogFolderSelect.Name = "ButtonLogFolderSelect";
            this.ButtonLogFolderSelect.Size = new System.Drawing.Size(104, 23);
            this.ButtonLogFolderSelect.TabIndex = 9;
            this.ButtonLogFolderSelect.Text = "Select Manually";
            this.ButtonLogFolderSelect.UseVisualStyleBackColor = true;
            this.ButtonLogFolderSelect.Click += new System.EventHandler(this.ButtonLogFolderSelect_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(12, 210);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "Google Drive API key";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InputGDapiKey
            // 
            this.InputGDapiKey.Location = new System.Drawing.Point(135, 207);
            this.InputGDapiKey.Name = "InputGDapiKey";
            this.InputGDapiKey.Size = new System.Drawing.Size(121, 23);
            this.InputGDapiKey.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(66, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "Google Drive settings";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ButtonSave
            // 
            this.ButtonSave.Location = new System.Drawing.Point(91, 260);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(75, 23);
            this.ButtonSave.TabIndex = 13;
            this.ButtonSave.Text = "Save";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 295);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.InputGDapiKey);
            this.Controls.Add(this.ButtonLogFolderSelect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.InputDeviceLogFolderPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBoxBuffSize);
            this.Controls.Add(this.CheckBoxSetBufferSizeOnInstallAPK);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void TextBoxBuffSize_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            int intVal;
            if(!Int32.TryParse(textBox.Text, out intVal))
            {
                MessageBox.Show("Please enter number between 1-99", "Buffer Size");
                e.Cancel = true;
            }
        }

        #endregion

        private Label label1;
        private CheckBox CheckBoxSetBufferSizeOnInstallAPK;
        private TextBox TextBoxBuffSize;
        private Label label2;
        private TextBox InputDeviceLogFolderPath;
        private Label label3;
        private Button ButtonLogFolderSelect;
        private Label label4;
        private TextBox InputGDapiKey;
        private Label label5;
        private Button ButtonSave;
    }
}