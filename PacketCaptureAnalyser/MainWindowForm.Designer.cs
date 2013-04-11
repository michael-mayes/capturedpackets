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
            this.RunAnalysisOnSelectedPackageCaptureButton = new System.Windows.Forms.Button();
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
            this.SelectedOutputFileForAnalysisDialog = new System.Windows.Forms.SaveFileDialog();
            this.SelectOutputFileButton = new System.Windows.Forms.Button();
            this.SelectedOutputFileNameLabel = new System.Windows.Forms.Label();
            this.SelectedOutputFileNameTextBox = new System.Windows.Forms.TextBox();
            this.SelectedOutputFilePathLabel = new System.Windows.Forms.Label();
            this.SelectedOutputFilePathTextBox = new System.Windows.Forms.TextBox();
            this.PerformLatencyAnalysisCheckBox = new System.Windows.Forms.CheckBox();
            this.PerformTimeAnalysisCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // RunAnalysisOnSelectedPackageCaptureButton
            // 
            this.RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
            this.RunAnalysisOnSelectedPackageCaptureButton.Location = new System.Drawing.Point(12, 368);
            this.RunAnalysisOnSelectedPackageCaptureButton.Name = "RunAnalysisOnSelectedPackageCaptureButton";
            this.RunAnalysisOnSelectedPackageCaptureButton.Size = new System.Drawing.Size(241, 23);
            this.RunAnalysisOnSelectedPackageCaptureButton.TabIndex = 1;
            this.RunAnalysisOnSelectedPackageCaptureButton.Text = "Run Analysis On Selected Packet Capture";
            this.RunAnalysisOnSelectedPackageCaptureButton.UseVisualStyleBackColor = true;
            this.RunAnalysisOnSelectedPackageCaptureButton.Click += new System.EventHandler(this.RunAnalysisOnPacketCaptureButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(12, 397);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(241, 23);
            this.ExitButton.TabIndex = 2;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // SelectedPacketCaptureForAnalysisDialog
            // 
            this.SelectedPacketCaptureForAnalysisDialog.Filter = "Packet Captures (*.pcap,*.libpcap,*.enc,*.cap)|*.pcap;*.libpcap;*.enc;*.cap";
            this.SelectedPacketCaptureForAnalysisDialog.Title = "Select Packet Capture For Analysis";
            // 
            // SelectPacketCaptureButton
            // 
            this.SelectPacketCaptureButton.Location = new System.Drawing.Point(12, 12);
            this.SelectPacketCaptureButton.Name = "SelectPacketCaptureButton";
            this.SelectPacketCaptureButton.Size = new System.Drawing.Size(241, 23);
            this.SelectPacketCaptureButton.TabIndex = 0;
            this.SelectPacketCaptureButton.Text = "Select Packet Capture";
            this.SelectPacketCaptureButton.UseVisualStyleBackColor = true;
            this.SelectPacketCaptureButton.Click += new System.EventHandler(this.SelectPacketCaptureForAnalysisButton_Click);
            // 
            // SelectedPacketCapturePathTextBox
            // 
            this.SelectedPacketCapturePathTextBox.Location = new System.Drawing.Point(12, 54);
            this.SelectedPacketCapturePathTextBox.Name = "SelectedPacketCapturePathTextBox";
            this.SelectedPacketCapturePathTextBox.ReadOnly = true;
            this.SelectedPacketCapturePathTextBox.Size = new System.Drawing.Size(241, 20);
            this.SelectedPacketCapturePathTextBox.TabIndex = 3;
            this.SelectedPacketCapturePathTextBox.TabStop = false;
            this.SelectedPacketCapturePathTextBox.Text = "<No Packet Capture Selected>";
            // 
            // SelectedPacketCapturePathLabel
            // 
            this.SelectedPacketCapturePathLabel.AutoSize = true;
            this.SelectedPacketCapturePathLabel.Location = new System.Drawing.Point(12, 38);
            this.SelectedPacketCapturePathLabel.Name = "SelectedPacketCapturePathLabel";
            this.SelectedPacketCapturePathLabel.Size = new System.Drawing.Size(168, 13);
            this.SelectedPacketCapturePathLabel.TabIndex = 4;
            this.SelectedPacketCapturePathLabel.Text = "Path Of Selected Packet Capture:";
            // 
            // SelectedPacketCaptureNameLabel
            // 
            this.SelectedPacketCaptureNameLabel.AutoSize = true;
            this.SelectedPacketCaptureNameLabel.Location = new System.Drawing.Point(12, 77);
            this.SelectedPacketCaptureNameLabel.Name = "SelectedPacketCaptureNameLabel";
            this.SelectedPacketCaptureNameLabel.Size = new System.Drawing.Size(174, 13);
            this.SelectedPacketCaptureNameLabel.TabIndex = 6;
            this.SelectedPacketCaptureNameLabel.Text = "Name Of Selected Packet Capture:";
            // 
            // SelectedPacketCaptureNameTextBox
            // 
            this.SelectedPacketCaptureNameTextBox.Location = new System.Drawing.Point(12, 93);
            this.SelectedPacketCaptureNameTextBox.Name = "SelectedPacketCaptureNameTextBox";
            this.SelectedPacketCaptureNameTextBox.ReadOnly = true;
            this.SelectedPacketCaptureNameTextBox.Size = new System.Drawing.Size(241, 20);
            this.SelectedPacketCaptureNameTextBox.TabIndex = 5;
            this.SelectedPacketCaptureNameTextBox.TabStop = false;
            this.SelectedPacketCaptureNameTextBox.Text = "<No Packet Capture Selected>";
            // 
            // SelectedPacketCaptureTypeLabel
            // 
            this.SelectedPacketCaptureTypeLabel.AutoSize = true;
            this.SelectedPacketCaptureTypeLabel.Location = new System.Drawing.Point(12, 116);
            this.SelectedPacketCaptureTypeLabel.Name = "SelectedPacketCaptureTypeLabel";
            this.SelectedPacketCaptureTypeLabel.Size = new System.Drawing.Size(170, 13);
            this.SelectedPacketCaptureTypeLabel.TabIndex = 8;
            this.SelectedPacketCaptureTypeLabel.Text = "Type Of Selected Packet Capture:";
            // 
            // SelectedPacketCaptureTypeTextBox
            // 
            this.SelectedPacketCaptureTypeTextBox.Location = new System.Drawing.Point(12, 132);
            this.SelectedPacketCaptureTypeTextBox.Name = "SelectedPacketCaptureTypeTextBox";
            this.SelectedPacketCaptureTypeTextBox.ReadOnly = true;
            this.SelectedPacketCaptureTypeTextBox.Size = new System.Drawing.Size(241, 20);
            this.SelectedPacketCaptureTypeTextBox.TabIndex = 7;
            this.SelectedPacketCaptureTypeTextBox.TabStop = false;
            this.SelectedPacketCaptureTypeTextBox.Text = "<No Packet Capture Selected>";
            // 
            // ClearSelectedPacketCaptureButton
            // 
            this.ClearSelectedPacketCaptureButton.Enabled = false;
            this.ClearSelectedPacketCaptureButton.Location = new System.Drawing.Point(11, 158);
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
            this.OpenSelectedPackageCaptureButton.Location = new System.Drawing.Point(11, 187);
            this.OpenSelectedPackageCaptureButton.Name = "OpenSelectedPackageCaptureButton";
            this.OpenSelectedPackageCaptureButton.Size = new System.Drawing.Size(242, 23);
            this.OpenSelectedPackageCaptureButton.TabIndex = 10;
            this.OpenSelectedPackageCaptureButton.Text = "Open Selected Packet Capture";
            this.OpenSelectedPackageCaptureButton.UseVisualStyleBackColor = true;
            this.OpenSelectedPackageCaptureButton.Click += new System.EventHandler(this.OpenSelectedPackageCaptureButton_Click);
            // 
            // SelectedOutputFileForAnalysisDialog
            // 
            this.SelectedOutputFileForAnalysisDialog.Filter = "Text File (*.txt)|*.txt";
            // 
            // SelectOutputFileButton
            // 
            this.SelectOutputFileButton.Location = new System.Drawing.Point(11, 216);
            this.SelectOutputFileButton.Name = "SelectOutputFileButton";
            this.SelectOutputFileButton.Size = new System.Drawing.Size(242, 23);
            this.SelectOutputFileButton.TabIndex = 11;
            this.SelectOutputFileButton.Text = "Select Output File";
            this.SelectOutputFileButton.UseVisualStyleBackColor = true;
            this.SelectOutputFileButton.Click += new System.EventHandler(this.SelectOuputFileButton_Click);
            // 
            // SelectedOutputFileNameLabel
            // 
            this.SelectedOutputFileNameLabel.AutoSize = true;
            this.SelectedOutputFileNameLabel.Location = new System.Drawing.Point(11, 281);
            this.SelectedOutputFileNameLabel.Name = "SelectedOutputFileNameLabel";
            this.SelectedOutputFileNameLabel.Size = new System.Drawing.Size(151, 13);
            this.SelectedOutputFileNameLabel.TabIndex = 15;
            this.SelectedOutputFileNameLabel.Text = "Name Of Selected Output File:";
            // 
            // SelectedOutputFileNameTextBox
            // 
            this.SelectedOutputFileNameTextBox.Location = new System.Drawing.Point(13, 297);
            this.SelectedOutputFileNameTextBox.Name = "SelectedOutputFileNameTextBox";
            this.SelectedOutputFileNameTextBox.ReadOnly = true;
            this.SelectedOutputFileNameTextBox.Size = new System.Drawing.Size(240, 20);
            this.SelectedOutputFileNameTextBox.TabIndex = 14;
            this.SelectedOutputFileNameTextBox.TabStop = false;
            this.SelectedOutputFileNameTextBox.Text = "<No Output File Selected>";
            // 
            // SelectedOutputFilePathLabel
            // 
            this.SelectedOutputFilePathLabel.AutoSize = true;
            this.SelectedOutputFilePathLabel.Location = new System.Drawing.Point(11, 242);
            this.SelectedOutputFilePathLabel.Name = "SelectedOutputFilePathLabel";
            this.SelectedOutputFilePathLabel.Size = new System.Drawing.Size(145, 13);
            this.SelectedOutputFilePathLabel.TabIndex = 13;
            this.SelectedOutputFilePathLabel.Text = "Path Of Selected Output File:";
            // 
            // SelectedOutputFilePathTextBox
            // 
            this.SelectedOutputFilePathTextBox.Location = new System.Drawing.Point(12, 258);
            this.SelectedOutputFilePathTextBox.Name = "SelectedOutputFilePathTextBox";
            this.SelectedOutputFilePathTextBox.ReadOnly = true;
            this.SelectedOutputFilePathTextBox.Size = new System.Drawing.Size(241, 20);
            this.SelectedOutputFilePathTextBox.TabIndex = 12;
            this.SelectedOutputFilePathTextBox.TabStop = false;
            this.SelectedOutputFilePathTextBox.Text = "<No Output File Selected>";
            // 
            // PerformLatencyAnalysisCheckBox
            // 
            this.PerformLatencyAnalysisCheckBox.AutoSize = true;
            this.PerformLatencyAnalysisCheckBox.Checked = true;
            this.PerformLatencyAnalysisCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PerformLatencyAnalysisCheckBox.Enabled = false;
            this.PerformLatencyAnalysisCheckBox.Location = new System.Drawing.Point(15, 324);
            this.PerformLatencyAnalysisCheckBox.Name = "PerformLatencyAnalysisCheckBox";
            this.PerformLatencyAnalysisCheckBox.Size = new System.Drawing.Size(144, 17);
            this.PerformLatencyAnalysisCheckBox.TabIndex = 16;
            this.PerformLatencyAnalysisCheckBox.Text = "Perform Latency Analysis";
            this.PerformLatencyAnalysisCheckBox.UseVisualStyleBackColor = true;
            // 
            // PerformTimeAnalysisCheckBox
            // 
            this.PerformTimeAnalysisCheckBox.AutoSize = true;
            this.PerformTimeAnalysisCheckBox.Enabled = false;
            this.PerformTimeAnalysisCheckBox.Location = new System.Drawing.Point(15, 348);
            this.PerformTimeAnalysisCheckBox.Name = "PerformTimeAnalysisCheckBox";
            this.PerformTimeAnalysisCheckBox.Size = new System.Drawing.Size(129, 17);
            this.PerformTimeAnalysisCheckBox.TabIndex = 17;
            this.PerformTimeAnalysisCheckBox.Text = "Perform Time Analysis";
            this.PerformTimeAnalysisCheckBox.UseVisualStyleBackColor = true;
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 432);
            this.Controls.Add(this.PerformTimeAnalysisCheckBox);
            this.Controls.Add(this.PerformLatencyAnalysisCheckBox);
            this.Controls.Add(this.SelectedOutputFileNameLabel);
            this.Controls.Add(this.SelectedOutputFileNameTextBox);
            this.Controls.Add(this.SelectedOutputFilePathLabel);
            this.Controls.Add(this.SelectedOutputFilePathTextBox);
            this.Controls.Add(this.SelectOutputFileButton);
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
            this.Controls.Add(this.RunAnalysisOnSelectedPackageCaptureButton);
            this.Name = "MainWindowForm";
            this.Text = "Packet Capture Analyser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RunAnalysisOnSelectedPackageCaptureButton;
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
        private System.Windows.Forms.SaveFileDialog SelectedOutputFileForAnalysisDialog;
        private System.Windows.Forms.Button SelectOutputFileButton;
        private System.Windows.Forms.Label SelectedOutputFileNameLabel;
        private System.Windows.Forms.TextBox SelectedOutputFileNameTextBox;
        private System.Windows.Forms.Label SelectedOutputFilePathLabel;
        private System.Windows.Forms.TextBox SelectedOutputFilePathTextBox;
        private System.Windows.Forms.CheckBox PerformLatencyAnalysisCheckBox;
        private System.Windows.Forms.CheckBox PerformTimeAnalysisCheckBox;
    }
}