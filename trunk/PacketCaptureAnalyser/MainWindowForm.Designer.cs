namespace PacketCaptureAnalyser
{
    partial class MainWindowForm
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
            this.RunAnalysisOnPackageSelectedCaptureButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.SelectedPacketCaptureForAnalysisDialog = new System.Windows.Forms.OpenFileDialog();
            this.SelectPacketCaptureButton = new System.Windows.Forms.Button();
            this.SelectedPacketCapturePathTextBox = new System.Windows.Forms.TextBox();
            this.SelectedPacketCapturePathLabel = new System.Windows.Forms.Label();
            this.SelectedPacketCaptureNameLabel = new System.Windows.Forms.Label();
            this.SelectedPacketCaptureNameTextBox = new System.Windows.Forms.TextBox();
            this.SelectedPacketCaptureTypeLabel = new System.Windows.Forms.Label();
            this.SelectedPacketCaptureTypeTextBox = new System.Windows.Forms.TextBox();
            this.ClearSelectedPacketCaptureButton = new System.Windows.Forms.Button();
            this.OpenSelectedPackageCaptureButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RunAnalysisOnPackageSelectedCaptureButton
            // 
            this.RunAnalysisOnPackageSelectedCaptureButton.Enabled = false;
            this.RunAnalysisOnPackageSelectedCaptureButton.Location = new System.Drawing.Point(11, 187);
            this.RunAnalysisOnPackageSelectedCaptureButton.Name = "RunAnalysisOnPackageSelectedCaptureButton";
            this.RunAnalysisOnPackageSelectedCaptureButton.Size = new System.Drawing.Size(242, 23);
            this.RunAnalysisOnPackageSelectedCaptureButton.TabIndex = 1;
            this.RunAnalysisOnPackageSelectedCaptureButton.Text = "Run Analysis On Selected Packet Capture";
            this.RunAnalysisOnPackageSelectedCaptureButton.UseVisualStyleBackColor = true;
            this.RunAnalysisOnPackageSelectedCaptureButton.Click += new System.EventHandler(this.RunAnalysisOnPacketCaptureButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(11, 246);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(242, 23);
            this.ExitButton.TabIndex = 2;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // SelectedPacketCaptureForAnalysisDialog
            // 
            this.SelectedPacketCaptureForAnalysisDialog.Filter = "Wireshark Packet Captures|*.pcap;*.libpcap;*.enc";
            this.SelectedPacketCaptureForAnalysisDialog.Title = "Select Packet Capture For Analysis";
            // 
            // SelectPacketCaptureButton
            // 
            this.SelectPacketCaptureButton.Location = new System.Drawing.Point(12, 12);
            this.SelectPacketCaptureButton.Name = "SelectPacketCaptureButton";
            this.SelectPacketCaptureButton.Size = new System.Drawing.Size(242, 23);
            this.SelectPacketCaptureButton.TabIndex = 0;
            this.SelectPacketCaptureButton.Text = "Select Packet Capture";
            this.SelectPacketCaptureButton.UseVisualStyleBackColor = true;
            this.SelectPacketCaptureButton.Click += new System.EventHandler(this.SelectPacketCaptureForAnalysisButton_Click);
            // 
            // SelectedPacketCapturePathTextBox
            // 
            this.SelectedPacketCapturePathTextBox.Location = new System.Drawing.Point(12, 83);
            this.SelectedPacketCapturePathTextBox.Name = "SelectedPacketCapturePathTextBox";
            this.SelectedPacketCapturePathTextBox.ReadOnly = true;
            this.SelectedPacketCapturePathTextBox.Size = new System.Drawing.Size(242, 20);
            this.SelectedPacketCapturePathTextBox.TabIndex = 3;
            this.SelectedPacketCapturePathTextBox.Text = "<No Packet Capture Selected>";
            // 
            // SelectedPacketCapturePathLabel
            // 
            this.SelectedPacketCapturePathLabel.AutoSize = true;
            this.SelectedPacketCapturePathLabel.Location = new System.Drawing.Point(12, 67);
            this.SelectedPacketCapturePathLabel.Name = "SelectedPacketCapturePathLabel";
            this.SelectedPacketCapturePathLabel.Size = new System.Drawing.Size(168, 13);
            this.SelectedPacketCapturePathLabel.TabIndex = 4;
            this.SelectedPacketCapturePathLabel.Text = "Path Of Selected Packet Capture:";
            // 
            // SelectedPacketCaptureNameLabel
            // 
            this.SelectedPacketCaptureNameLabel.AutoSize = true;
            this.SelectedPacketCaptureNameLabel.Location = new System.Drawing.Point(12, 106);
            this.SelectedPacketCaptureNameLabel.Name = "SelectedPacketCaptureNameLabel";
            this.SelectedPacketCaptureNameLabel.Size = new System.Drawing.Size(174, 13);
            this.SelectedPacketCaptureNameLabel.TabIndex = 6;
            this.SelectedPacketCaptureNameLabel.Text = "Name Of Selected Packet Capture:";
            // 
            // SelectedPacketCaptureNameTextBox
            // 
            this.SelectedPacketCaptureNameTextBox.Location = new System.Drawing.Point(12, 122);
            this.SelectedPacketCaptureNameTextBox.Name = "SelectedPacketCaptureNameTextBox";
            this.SelectedPacketCaptureNameTextBox.ReadOnly = true;
            this.SelectedPacketCaptureNameTextBox.Size = new System.Drawing.Size(242, 20);
            this.SelectedPacketCaptureNameTextBox.TabIndex = 5;
            this.SelectedPacketCaptureNameTextBox.Text = "<No Packet Capture Selected>";
            // 
            // SelectedPacketCaptureTypeLabel
            // 
            this.SelectedPacketCaptureTypeLabel.AutoSize = true;
            this.SelectedPacketCaptureTypeLabel.Location = new System.Drawing.Point(12, 145);
            this.SelectedPacketCaptureTypeLabel.Name = "SelectedPacketCaptureTypeLabel";
            this.SelectedPacketCaptureTypeLabel.Size = new System.Drawing.Size(170, 13);
            this.SelectedPacketCaptureTypeLabel.TabIndex = 8;
            this.SelectedPacketCaptureTypeLabel.Text = "Type Of Selected Packet Capture:";
            // 
            // SelectedPacketCaptureTypeTextBox
            // 
            this.SelectedPacketCaptureTypeTextBox.Location = new System.Drawing.Point(12, 161);
            this.SelectedPacketCaptureTypeTextBox.Name = "SelectedPacketCaptureTypeTextBox";
            this.SelectedPacketCaptureTypeTextBox.ReadOnly = true;
            this.SelectedPacketCaptureTypeTextBox.Size = new System.Drawing.Size(242, 20);
            this.SelectedPacketCaptureTypeTextBox.TabIndex = 7;
            this.SelectedPacketCaptureTypeTextBox.Text = "<No Packet Capture Selected>";
            // 
            // ClearSelectedPacketCaptureButton
            // 
            this.ClearSelectedPacketCaptureButton.Location = new System.Drawing.Point(12, 41);
            this.ClearSelectedPacketCaptureButton.Name = "ClearSelectedPacketCaptureButton";
            this.ClearSelectedPacketCaptureButton.Size = new System.Drawing.Size(242, 23);
            this.ClearSelectedPacketCaptureButton.TabIndex = 9;
            this.ClearSelectedPacketCaptureButton.Text = "Clear Selected Packet Capture";
            this.ClearSelectedPacketCaptureButton.UseVisualStyleBackColor = true;
            this.ClearSelectedPacketCaptureButton.Click += new System.EventHandler(this.ClearSelectedPacketCaptureButton_Click);
            // 
            // OpenSelectedPackageCaptureButton
            // 
            this.OpenSelectedPackageCaptureButton.Enabled = false;
            this.OpenSelectedPackageCaptureButton.Location = new System.Drawing.Point(12, 217);
            this.OpenSelectedPackageCaptureButton.Name = "OpenSelectedPackageCaptureButton";
            this.OpenSelectedPackageCaptureButton.Size = new System.Drawing.Size(242, 23);
            this.OpenSelectedPackageCaptureButton.TabIndex = 10;
            this.OpenSelectedPackageCaptureButton.Text = "Open Selected Packet Capture";
            this.OpenSelectedPackageCaptureButton.UseVisualStyleBackColor = true;
            this.OpenSelectedPackageCaptureButton.Click += new System.EventHandler(this.OpenSelectedPackageCaptureButton_Click);
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 279);
            this.Controls.Add(this.OpenSelectedPackageCaptureButton);
            this.Controls.Add(this.ClearSelectedPacketCaptureButton);
            this.Controls.Add(this.SelectedPacketCaptureTypeLabel);
            this.Controls.Add(this.SelectedPacketCaptureTypeTextBox);
            this.Controls.Add(this.SelectedPacketCaptureNameLabel);
            this.Controls.Add(this.SelectedPacketCaptureNameTextBox);
            this.Controls.Add(this.SelectedPacketCapturePathLabel);
            this.Controls.Add(this.SelectedPacketCapturePathTextBox);
            this.Controls.Add(this.SelectPacketCaptureButton);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.RunAnalysisOnPackageSelectedCaptureButton);
            this.Name = "MainWindowForm";
            this.Text = "Packet Capture Analyser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RunAnalysisOnPackageSelectedCaptureButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.OpenFileDialog SelectedPacketCaptureForAnalysisDialog;
        private System.Windows.Forms.Button SelectPacketCaptureButton;
        private System.Windows.Forms.TextBox SelectedPacketCapturePathTextBox;
        private System.Windows.Forms.Label SelectedPacketCapturePathLabel;
        private System.Windows.Forms.Label SelectedPacketCaptureNameLabel;
        private System.Windows.Forms.TextBox SelectedPacketCaptureNameTextBox;
        private System.Windows.Forms.Label SelectedPacketCaptureTypeLabel;
        private System.Windows.Forms.TextBox SelectedPacketCaptureTypeTextBox;
        private System.Windows.Forms.Button ClearSelectedPacketCaptureButton;
        private System.Windows.Forms.Button OpenSelectedPackageCaptureButton;
    }
}