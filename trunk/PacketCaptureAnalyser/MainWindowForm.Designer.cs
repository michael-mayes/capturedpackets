// $Id$
// $URL$
// <copyright file="MainWindowForm.Designer.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser
{
    /// <summary>
    /// This class provides the main window form
    /// </summary>
    public partial class MainWindowForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.Button theRunAnalysisOnSelectedPackageCaptureButton;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.Button theExitButton;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.OpenFileDialog theSelectedPacketCaptureForAnalysisDialog;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.Button theSelectPacketCaptureButton;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.TextBox theSelectedPacketCapturePathTextBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.Label theSelectedPacketCapturePathLabel;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.Label theSelectedPacketCaptureNameLabel;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.TextBox theSelectedPacketCaptureNameTextBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.Label theSelectedPacketCaptureTypeLabel;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.TextBox theSelectedPacketCaptureTypeTextBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.Button theClearSelectedPacketCaptureButton;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.Button theOpenSelectedPackageCaptureButton;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.CheckBox thePerformLatencyAnalysisCheckBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.CheckBox thePerformTimeAnalysisCheckBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.CheckBox theOutputLatencyAnalysisDebugCheckBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.CheckBox theOutputTimeAnalysisDebugCheckBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.CheckBox theMinimiseMemoryUsageCheckBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.CheckBox theRedirectDebugInformationToOutputCheckBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.CheckBox theEnableInformationEventsInDebugInformationCheckBox;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.CheckBox theEnableDebugInformationCheckBox;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.theRunAnalysisOnSelectedPackageCaptureButton = new System.Windows.Forms.Button();
            this.theExitButton = new System.Windows.Forms.Button();
            this.theSelectedPacketCaptureForAnalysisDialog = new System.Windows.Forms.OpenFileDialog();
            this.theSelectPacketCaptureButton = new System.Windows.Forms.Button();
            this.theSelectedPacketCapturePathTextBox = new System.Windows.Forms.TextBox();
            this.theSelectedPacketCapturePathLabel = new System.Windows.Forms.Label();
            this.theSelectedPacketCaptureNameLabel = new System.Windows.Forms.Label();
            this.theSelectedPacketCaptureNameTextBox = new System.Windows.Forms.TextBox();
            this.theSelectedPacketCaptureTypeLabel = new System.Windows.Forms.Label();
            this.theSelectedPacketCaptureTypeTextBox = new System.Windows.Forms.TextBox();
            this.theClearSelectedPacketCaptureButton = new System.Windows.Forms.Button();
            this.theOpenSelectedPackageCaptureButton = new System.Windows.Forms.Button();
            this.thePerformLatencyAnalysisCheckBox = new System.Windows.Forms.CheckBox();
            this.thePerformTimeAnalysisCheckBox = new System.Windows.Forms.CheckBox();
            this.theOutputLatencyAnalysisDebugCheckBox = new System.Windows.Forms.CheckBox();
            this.theOutputTimeAnalysisDebugCheckBox = new System.Windows.Forms.CheckBox();
            this.theMinimiseMemoryUsageCheckBox = new System.Windows.Forms.CheckBox();
            this.theRedirectDebugInformationToOutputCheckBox = new System.Windows.Forms.CheckBox();
            this.theEnableInformationEventsInDebugInformationCheckBox = new System.Windows.Forms.CheckBox();
            this.theEnableDebugInformationCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // theRunAnalysisOnSelectedPackageCaptureButton
            //
            this.theRunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
            this.theRunAnalysisOnSelectedPackageCaptureButton.Location = new System.Drawing.Point(12, 403);
            this.theRunAnalysisOnSelectedPackageCaptureButton.Name = "RunAnalysisOnSelectedPackageCaptureButton";
            this.theRunAnalysisOnSelectedPackageCaptureButton.Size = new System.Drawing.Size(340, 23);
            this.theRunAnalysisOnSelectedPackageCaptureButton.TabIndex = 1;
            this.theRunAnalysisOnSelectedPackageCaptureButton.Text = "Run Analysis On Selected Packet Capture";
            this.theRunAnalysisOnSelectedPackageCaptureButton.UseVisualStyleBackColor = true;
            this.theRunAnalysisOnSelectedPackageCaptureButton.Click += new System.EventHandler(this.RunAnalysisOnPacketCaptureButton_Click);
            //
            // theExitButton
            //
            this.theExitButton.Location = new System.Drawing.Point(12, 432);
            this.theExitButton.Name = "ExitButton";
            this.theExitButton.Size = new System.Drawing.Size(340, 23);
            this.theExitButton.TabIndex = 2;
            this.theExitButton.Text = "Exit";
            this.theExitButton.UseVisualStyleBackColor = true;
            this.theExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            //
            // theSelectedPacketCaptureForAnalysisDialog
            //
            this.theSelectedPacketCaptureForAnalysisDialog.Filter = "Packet Captures (*pcapng,*.pcap,*.libpcap,*.enc,*.cap,*.ntar)|*.pcapng;*.pcap;*.l" +
    "ibpcap;*.enc;*.cap;*.ntar";
            this.theSelectedPacketCaptureForAnalysisDialog.Title = "Select Packet Capture For Analysis";
            //
            // theSelectPacketCaptureButton
            //
            this.theSelectPacketCaptureButton.Location = new System.Drawing.Point(12, 12);
            this.theSelectPacketCaptureButton.Name = "SelectPacketCaptureButton";
            this.theSelectPacketCaptureButton.Size = new System.Drawing.Size(340, 23);
            this.theSelectPacketCaptureButton.TabIndex = 0;
            this.theSelectPacketCaptureButton.Text = "Select Packet Capture";
            this.theSelectPacketCaptureButton.UseVisualStyleBackColor = true;
            this.theSelectPacketCaptureButton.Click += new System.EventHandler(this.SelectPacketCaptureForAnalysisButton_Click);
            //
            // theSelectedPacketCapturePathTextBox
            //
            this.theSelectedPacketCapturePathTextBox.Location = new System.Drawing.Point(12, 54);
            this.theSelectedPacketCapturePathTextBox.Name = "SelectedPacketCapturePathTextBox";
            this.theSelectedPacketCapturePathTextBox.ReadOnly = true;
            this.theSelectedPacketCapturePathTextBox.Size = new System.Drawing.Size(340, 20);
            this.theSelectedPacketCapturePathTextBox.TabIndex = 3;
            this.theSelectedPacketCapturePathTextBox.TabStop = false;
            this.theSelectedPacketCapturePathTextBox.Text = "<No Packet Capture Selected>";
            //
            // theSelectedPacketCapturePathLabel
            //
            this.theSelectedPacketCapturePathLabel.AutoSize = true;
            this.theSelectedPacketCapturePathLabel.Location = new System.Drawing.Point(12, 38);
            this.theSelectedPacketCapturePathLabel.Name = "SelectedPacketCapturePathLabel";
            this.theSelectedPacketCapturePathLabel.Size = new System.Drawing.Size(168, 13);
            this.theSelectedPacketCapturePathLabel.TabIndex = 4;
            this.theSelectedPacketCapturePathLabel.Text = "Path Of Selected Packet Capture:";
            //
            // theSelectedPacketCaptureNameLabel
            //
            this.theSelectedPacketCaptureNameLabel.AutoSize = true;
            this.theSelectedPacketCaptureNameLabel.Location = new System.Drawing.Point(12, 77);
            this.theSelectedPacketCaptureNameLabel.Name = "SelectedPacketCaptureNameLabel";
            this.theSelectedPacketCaptureNameLabel.Size = new System.Drawing.Size(174, 13);
            this.theSelectedPacketCaptureNameLabel.TabIndex = 6;
            this.theSelectedPacketCaptureNameLabel.Text = "Name Of Selected Packet Capture:";
            //
            // theSelectedPacketCaptureNameTextBox
            //
            this.theSelectedPacketCaptureNameTextBox.Location = new System.Drawing.Point(12, 93);
            this.theSelectedPacketCaptureNameTextBox.Name = "SelectedPacketCaptureNameTextBox";
            this.theSelectedPacketCaptureNameTextBox.ReadOnly = true;
            this.theSelectedPacketCaptureNameTextBox.Size = new System.Drawing.Size(340, 20);
            this.theSelectedPacketCaptureNameTextBox.TabIndex = 5;
            this.theSelectedPacketCaptureNameTextBox.TabStop = false;
            this.theSelectedPacketCaptureNameTextBox.Text = "<No Packet Capture Selected>";
            //
            // theSelectedPacketCaptureTypeLabel
            //
            this.theSelectedPacketCaptureTypeLabel.AutoSize = true;
            this.theSelectedPacketCaptureTypeLabel.Location = new System.Drawing.Point(12, 116);
            this.theSelectedPacketCaptureTypeLabel.Name = "SelectedPacketCaptureTypeLabel";
            this.theSelectedPacketCaptureTypeLabel.Size = new System.Drawing.Size(170, 13);
            this.theSelectedPacketCaptureTypeLabel.TabIndex = 8;
            this.theSelectedPacketCaptureTypeLabel.Text = "Type Of Selected Packet Capture:";
            //
            // theSelectedPacketCaptureTypeTextBox
            //
            this.theSelectedPacketCaptureTypeTextBox.Location = new System.Drawing.Point(12, 132);
            this.theSelectedPacketCaptureTypeTextBox.Name = "SelectedPacketCaptureTypeTextBox";
            this.theSelectedPacketCaptureTypeTextBox.ReadOnly = true;
            this.theSelectedPacketCaptureTypeTextBox.Size = new System.Drawing.Size(340, 20);
            this.theSelectedPacketCaptureTypeTextBox.TabIndex = 7;
            this.theSelectedPacketCaptureTypeTextBox.TabStop = false;
            this.theSelectedPacketCaptureTypeTextBox.Text = "<No Packet Capture Selected>";
            //
            // theClearSelectedPacketCaptureButton
            //
            this.theClearSelectedPacketCaptureButton.Enabled = false;
            this.theClearSelectedPacketCaptureButton.Location = new System.Drawing.Point(12, 160);
            this.theClearSelectedPacketCaptureButton.Name = "ClearSelectedPacketCaptureButton";
            this.theClearSelectedPacketCaptureButton.Size = new System.Drawing.Size(340, 23);
            this.theClearSelectedPacketCaptureButton.TabIndex = 9;
            this.theClearSelectedPacketCaptureButton.Text = "Clear Selected Packet Capture";
            this.theClearSelectedPacketCaptureButton.UseVisualStyleBackColor = true;
            this.theClearSelectedPacketCaptureButton.Click += new System.EventHandler(this.ClearSelectedPacketCaptureButton_Click);
            //
            // theOpenSelectedPackageCaptureButton
            //
            this.theOpenSelectedPackageCaptureButton.Enabled = false;
            this.theOpenSelectedPackageCaptureButton.Location = new System.Drawing.Point(12, 189);
            this.theOpenSelectedPackageCaptureButton.Name = "OpenSelectedPackageCaptureButton";
            this.theOpenSelectedPackageCaptureButton.Size = new System.Drawing.Size(340, 23);
            this.theOpenSelectedPackageCaptureButton.TabIndex = 10;
            this.theOpenSelectedPackageCaptureButton.Text = "Open Selected Packet Capture";
            this.theOpenSelectedPackageCaptureButton.UseVisualStyleBackColor = true;
            this.theOpenSelectedPackageCaptureButton.Click += new System.EventHandler(this.OpenSelectedPackageCaptureButton_Click);
            //
            // thePerformLatencyAnalysisCheckBox
            //
            this.thePerformLatencyAnalysisCheckBox.AutoSize = true;
            this.thePerformLatencyAnalysisCheckBox.Enabled = false;
            this.thePerformLatencyAnalysisCheckBox.Location = new System.Drawing.Point(12, 288);
            this.thePerformLatencyAnalysisCheckBox.Name = "PerformLatencyAnalysisCheckBox";
            this.thePerformLatencyAnalysisCheckBox.Size = new System.Drawing.Size(144, 17);
            this.thePerformLatencyAnalysisCheckBox.TabIndex = 16;
            this.thePerformLatencyAnalysisCheckBox.Text = "Perform Latency Analysis";
            this.thePerformLatencyAnalysisCheckBox.UseVisualStyleBackColor = true;
            this.thePerformLatencyAnalysisCheckBox.CheckedChanged += new System.EventHandler(this.PerformLatencyAnalysisCheckBox_CheckedChanged);
            //
            // thePerformTimeAnalysisCheckBox
            //
            this.thePerformTimeAnalysisCheckBox.AutoSize = true;
            this.thePerformTimeAnalysisCheckBox.Enabled = false;
            this.thePerformTimeAnalysisCheckBox.Location = new System.Drawing.Point(12, 334);
            this.thePerformTimeAnalysisCheckBox.Name = "PerformTimeAnalysisCheckBox";
            this.thePerformTimeAnalysisCheckBox.Size = new System.Drawing.Size(129, 17);
            this.thePerformTimeAnalysisCheckBox.TabIndex = 17;
            this.thePerformTimeAnalysisCheckBox.Text = "Perform Time Analysis";
            this.thePerformTimeAnalysisCheckBox.UseVisualStyleBackColor = true;
            this.thePerformTimeAnalysisCheckBox.CheckedChanged += new System.EventHandler(this.PerformTimeAnalysisCheckBox_CheckedChanged);
            //
            // theOutputLatencyAnalysisDebugCheckBox
            //
            this.theOutputLatencyAnalysisDebugCheckBox.AutoSize = true;
            this.theOutputLatencyAnalysisDebugCheckBox.Enabled = false;
            this.theOutputLatencyAnalysisDebugCheckBox.Location = new System.Drawing.Point(12, 311);
            this.theOutputLatencyAnalysisDebugCheckBox.Name = "OutputLatencyAnalysisDebugCheckBox";
            this.theOutputLatencyAnalysisDebugCheckBox.Size = new System.Drawing.Size(227, 17);
            this.theOutputLatencyAnalysisDebugCheckBox.TabIndex = 18;
            this.theOutputLatencyAnalysisDebugCheckBox.Text = "Output Latency Analysis Debug Infomation";
            this.theOutputLatencyAnalysisDebugCheckBox.UseVisualStyleBackColor = true;
            this.theOutputLatencyAnalysisDebugCheckBox.CheckedChanged += new System.EventHandler(this.OutputLatencyAnalysisDebugCheckBox_CheckedChanged);
            //
            // theOutputTimeAnalysisDebugCheckBox
            //
            this.theOutputTimeAnalysisDebugCheckBox.AutoSize = true;
            this.theOutputTimeAnalysisDebugCheckBox.Enabled = false;
            this.theOutputTimeAnalysisDebugCheckBox.Location = new System.Drawing.Point(12, 357);
            this.theOutputTimeAnalysisDebugCheckBox.Name = "OutputTimeAnalysisDebugCheckBox";
            this.theOutputTimeAnalysisDebugCheckBox.Size = new System.Drawing.Size(212, 17);
            this.theOutputTimeAnalysisDebugCheckBox.TabIndex = 19;
            this.theOutputTimeAnalysisDebugCheckBox.Text = "Output Time Analysis Debug Infomation";
            this.theOutputTimeAnalysisDebugCheckBox.UseVisualStyleBackColor = true;
            this.theOutputTimeAnalysisDebugCheckBox.CheckedChanged += new System.EventHandler(this.OutputTimeAnalysisDebugCheckBox_CheckedChanged);
            //
            // theMinimiseMemoryUsageCheckBox
            //
            this.theMinimiseMemoryUsageCheckBox.AutoSize = true;
            this.theMinimiseMemoryUsageCheckBox.Enabled = false;
            this.theMinimiseMemoryUsageCheckBox.Location = new System.Drawing.Point(12, 380);
            this.theMinimiseMemoryUsageCheckBox.Name = "minimiseMemoryUsageCheckBox";
            this.theMinimiseMemoryUsageCheckBox.Size = new System.Drawing.Size(346, 17);
            this.theMinimiseMemoryUsageCheckBox.TabIndex = 20;
            this.theMinimiseMemoryUsageCheckBox.Text = "Minimise Memory Usage (Potentially Necessary For Very Large Files)";
            this.theMinimiseMemoryUsageCheckBox.UseVisualStyleBackColor = true;
            //
            // theRedirectDebugInformationToOutputCheckBox
            //
            this.theRedirectDebugInformationToOutputCheckBox.AutoSize = true;
            this.theRedirectDebugInformationToOutputCheckBox.Enabled = false;
            this.theRedirectDebugInformationToOutputCheckBox.Location = new System.Drawing.Point(12, 265);
            this.theRedirectDebugInformationToOutputCheckBox.Name = "RedirectDebugInformationToOutputCheckBox";
            this.theRedirectDebugInformationToOutputCheckBox.Size = new System.Drawing.Size(342, 17);
            this.theRedirectDebugInformationToOutputCheckBox.TabIndex = 21;
            this.theRedirectDebugInformationToOutputCheckBox.Text = "Redirect Debug Information To Output (Potentially Helpful On Error)";
            this.theRedirectDebugInformationToOutputCheckBox.UseVisualStyleBackColor = true;
            this.theRedirectDebugInformationToOutputCheckBox.CheckedChanged += new System.EventHandler(this.RedirectDebugInformationToOutputCheckBox_CheckedChanged);
            //
            // theEnableInformationEventsInDebugInformationCheckBox
            //
            this.theEnableInformationEventsInDebugInformationCheckBox.AutoSize = true;
            this.theEnableInformationEventsInDebugInformationCheckBox.Enabled = false;
            this.theEnableInformationEventsInDebugInformationCheckBox.Location = new System.Drawing.Point(12, 242);
            this.theEnableInformationEventsInDebugInformationCheckBox.Name = "EnableInformationEventsInDebugInformationCheckBox";
            this.theEnableInformationEventsInDebugInformationCheckBox.Size = new System.Drawing.Size(252, 17);
            this.theEnableInformationEventsInDebugInformationCheckBox.TabIndex = 22;
            this.theEnableInformationEventsInDebugInformationCheckBox.Text = "Enable Information Events In Debug Information";
            this.theEnableInformationEventsInDebugInformationCheckBox.UseVisualStyleBackColor = true;
            this.theEnableInformationEventsInDebugInformationCheckBox.CheckedChanged += new System.EventHandler(this.EnableInformationEventsInDebugInformationCheckBox_CheckedChanged);
            //
            // theEnableDebugInformationCheckBox
            //
            this.theEnableDebugInformationCheckBox.AutoSize = true;
            this.theEnableDebugInformationCheckBox.Enabled = false;
            this.theEnableDebugInformationCheckBox.Location = new System.Drawing.Point(12, 219);
            this.theEnableDebugInformationCheckBox.Name = "EnableDebugInformationCheckBox";
            this.theEnableDebugInformationCheckBox.Size = new System.Drawing.Size(149, 17);
            this.theEnableDebugInformationCheckBox.TabIndex = 23;
            this.theEnableDebugInformationCheckBox.Text = "Enable Debug Information";
            this.theEnableDebugInformationCheckBox.UseVisualStyleBackColor = true;
            this.theEnableDebugInformationCheckBox.CheckedChanged += new System.EventHandler(this.EnableDebugInformationCheckBox_CheckedChanged);
            //
            // MainWindowForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 465);
            this.Controls.Add(this.theEnableDebugInformationCheckBox);
            this.Controls.Add(this.theEnableInformationEventsInDebugInformationCheckBox);
            this.Controls.Add(this.theRedirectDebugInformationToOutputCheckBox);
            this.Controls.Add(this.theMinimiseMemoryUsageCheckBox);
            this.Controls.Add(this.theOutputTimeAnalysisDebugCheckBox);
            this.Controls.Add(this.theOutputLatencyAnalysisDebugCheckBox);
            this.Controls.Add(this.thePerformTimeAnalysisCheckBox);
            this.Controls.Add(this.thePerformLatencyAnalysisCheckBox);
            this.Controls.Add(this.theOpenSelectedPackageCaptureButton);
            this.Controls.Add(this.theClearSelectedPacketCaptureButton);
            this.Controls.Add(this.theSelectedPacketCaptureTypeLabel);
            this.Controls.Add(this.theSelectedPacketCaptureTypeTextBox);
            this.Controls.Add(this.theSelectedPacketCaptureNameLabel);
            this.Controls.Add(this.theSelectedPacketCaptureNameTextBox);
            this.Controls.Add(this.theSelectedPacketCapturePathLabel);
            this.Controls.Add(this.theSelectedPacketCapturePathTextBox);
            this.Controls.Add(this.theSelectPacketCaptureButton);
            this.Controls.Add(this.theExitButton);
            this.Controls.Add(this.theRunAnalysisOnSelectedPackageCaptureButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainWindowForm";
            this.Text = "Packet Capture Analyser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
