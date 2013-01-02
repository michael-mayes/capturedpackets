namespace WiresharkRecordingAnalyser
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
            this.RunAnalysisOnRecordingButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.SelectRecordingForAnalysisDialog = new System.Windows.Forms.OpenFileDialog();
            this.SelectRecordingForAnalysisButton = new System.Windows.Forms.Button();
            this.SelectedRecordingPathTextBox = new System.Windows.Forms.TextBox();
            this.SelectedRecordingPathLabel = new System.Windows.Forms.Label();
            this.SelectedRecordingNameLabel = new System.Windows.Forms.Label();
            this.SelectedRecordingNameTextBox = new System.Windows.Forms.TextBox();
            this.SelectedRecordingTypeLabel = new System.Windows.Forms.Label();
            this.SelectedRecordingTypeTextBox = new System.Windows.Forms.TextBox();
            this.ClearSelectedRecordingButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RunAnalysisOnRecordingButton
            // 
            this.RunAnalysisOnRecordingButton.Enabled = false;
            this.RunAnalysisOnRecordingButton.Location = new System.Drawing.Point(12, 187);
            this.RunAnalysisOnRecordingButton.Name = "RunAnalysisOnRecordingButton";
            this.RunAnalysisOnRecordingButton.Size = new System.Drawing.Size(242, 23);
            this.RunAnalysisOnRecordingButton.TabIndex = 1;
            this.RunAnalysisOnRecordingButton.Text = "Run Analysis On Wireshark Recording Selected";
            this.RunAnalysisOnRecordingButton.UseVisualStyleBackColor = true;
            this.RunAnalysisOnRecordingButton.Click += new System.EventHandler(this.RunAnalysisOnRecordingButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(12, 216);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(242, 23);
            this.ExitButton.TabIndex = 2;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // SelectRecordingForAnalysisDialog
            // 
            this.SelectRecordingForAnalysisDialog.Filter = "Wireshark Recordings|*.pcap;*.libpcap;*.enc";
            this.SelectRecordingForAnalysisDialog.Title = "Select Wireshark Recording For Analysis";
            // 
            // SelectRecordingForAnalysisButton
            // 
            this.SelectRecordingForAnalysisButton.Location = new System.Drawing.Point(12, 12);
            this.SelectRecordingForAnalysisButton.Name = "SelectRecordingForAnalysisButton";
            this.SelectRecordingForAnalysisButton.Size = new System.Drawing.Size(242, 23);
            this.SelectRecordingForAnalysisButton.TabIndex = 0;
            this.SelectRecordingForAnalysisButton.Text = "Select Wireshark Recording For Analysis";
            this.SelectRecordingForAnalysisButton.UseVisualStyleBackColor = true;
            this.SelectRecordingForAnalysisButton.Click += new System.EventHandler(this.SelectRecordingForAnalysisButton_Click);
            // 
            // SelectedRecordingPathTextBox
            // 
            this.SelectedRecordingPathTextBox.Location = new System.Drawing.Point(12, 83);
            this.SelectedRecordingPathTextBox.Name = "SelectedRecordingPathTextBox";
            this.SelectedRecordingPathTextBox.ReadOnly = true;
            this.SelectedRecordingPathTextBox.Size = new System.Drawing.Size(242, 20);
            this.SelectedRecordingPathTextBox.TabIndex = 3;
            this.SelectedRecordingPathTextBox.Text = "<No Wireshark Recording Selected>";
            // 
            // SelectedRecordingPathLabel
            // 
            this.SelectedRecordingPathLabel.AutoSize = true;
            this.SelectedRecordingPathLabel.Location = new System.Drawing.Point(12, 67);
            this.SelectedRecordingPathLabel.Name = "SelectedRecordingPathLabel";
            this.SelectedRecordingPathLabel.Size = new System.Drawing.Size(194, 13);
            this.SelectedRecordingPathLabel.TabIndex = 4;
            this.SelectedRecordingPathLabel.Text = "Path Of Wireshark Recording Selected:";
            // 
            // SelectedRecordingNameLabel
            // 
            this.SelectedRecordingNameLabel.AutoSize = true;
            this.SelectedRecordingNameLabel.Location = new System.Drawing.Point(12, 106);
            this.SelectedRecordingNameLabel.Name = "SelectedRecordingNameLabel";
            this.SelectedRecordingNameLabel.Size = new System.Drawing.Size(200, 13);
            this.SelectedRecordingNameLabel.TabIndex = 6;
            this.SelectedRecordingNameLabel.Text = "Name Of Wireshark Recording Selected:";
            // 
            // SelectedRecordingNameTextBox
            // 
            this.SelectedRecordingNameTextBox.Location = new System.Drawing.Point(12, 122);
            this.SelectedRecordingNameTextBox.Name = "SelectedRecordingNameTextBox";
            this.SelectedRecordingNameTextBox.ReadOnly = true;
            this.SelectedRecordingNameTextBox.Size = new System.Drawing.Size(242, 20);
            this.SelectedRecordingNameTextBox.TabIndex = 5;
            this.SelectedRecordingNameTextBox.Text = "<No Wireshark Recording Selected>";
            // 
            // SelectedRecordingTypeLabel
            // 
            this.SelectedRecordingTypeLabel.AutoSize = true;
            this.SelectedRecordingTypeLabel.Location = new System.Drawing.Point(12, 145);
            this.SelectedRecordingTypeLabel.Name = "SelectedRecordingTypeLabel";
            this.SelectedRecordingTypeLabel.Size = new System.Drawing.Size(196, 13);
            this.SelectedRecordingTypeLabel.TabIndex = 8;
            this.SelectedRecordingTypeLabel.Text = "Type Of Wireshark Recording Selected:";
            // 
            // SelectedRecordingTypeTextBox
            // 
            this.SelectedRecordingTypeTextBox.Location = new System.Drawing.Point(12, 161);
            this.SelectedRecordingTypeTextBox.Name = "SelectedRecordingTypeTextBox";
            this.SelectedRecordingTypeTextBox.ReadOnly = true;
            this.SelectedRecordingTypeTextBox.Size = new System.Drawing.Size(242, 20);
            this.SelectedRecordingTypeTextBox.TabIndex = 7;
            this.SelectedRecordingTypeTextBox.Text = "<No Wireshark Recording Selected>";
            // 
            // ClearSelectedRecordingButton
            // 
            this.ClearSelectedRecordingButton.Location = new System.Drawing.Point(12, 41);
            this.ClearSelectedRecordingButton.Name = "ClearSelectedRecordingButton";
            this.ClearSelectedRecordingButton.Size = new System.Drawing.Size(242, 23);
            this.ClearSelectedRecordingButton.TabIndex = 9;
            this.ClearSelectedRecordingButton.Text = "Clear Wireshark Recording Selected";
            this.ClearSelectedRecordingButton.UseVisualStyleBackColor = true;
            this.ClearSelectedRecordingButton.Click += new System.EventHandler(this.ClearSelectedRecordingButton_Click);
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 252);
            this.Controls.Add(this.ClearSelectedRecordingButton);
            this.Controls.Add(this.SelectedRecordingTypeLabel);
            this.Controls.Add(this.SelectedRecordingTypeTextBox);
            this.Controls.Add(this.SelectedRecordingNameLabel);
            this.Controls.Add(this.SelectedRecordingNameTextBox);
            this.Controls.Add(this.SelectedRecordingPathLabel);
            this.Controls.Add(this.SelectedRecordingPathTextBox);
            this.Controls.Add(this.SelectRecordingForAnalysisButton);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.RunAnalysisOnRecordingButton);
            this.Name = "MainWindowForm";
            this.Text = "Wireshark Recording Analyser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RunAnalysisOnRecordingButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.OpenFileDialog SelectRecordingForAnalysisDialog;
        private System.Windows.Forms.Button SelectRecordingForAnalysisButton;
        private System.Windows.Forms.TextBox SelectedRecordingPathTextBox;
        private System.Windows.Forms.Label SelectedRecordingPathLabel;
        private System.Windows.Forms.Label SelectedRecordingNameLabel;
        private System.Windows.Forms.TextBox SelectedRecordingNameTextBox;
        private System.Windows.Forms.Label SelectedRecordingTypeLabel;
        private System.Windows.Forms.TextBox SelectedRecordingTypeTextBox;
        private System.Windows.Forms.Button ClearSelectedRecordingButton;
    }
}