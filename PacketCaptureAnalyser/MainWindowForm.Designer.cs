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

        //// Button

        /// <summary>
        /// The "Select Packet Capture" button
        /// </summary>
        private System.Windows.Forms.Button theSelectPacketCaptureButton;

        //// "Selected Packet Capture" group box

        /// <summary>
        /// The group box that contains items related to the selected packet capture
        /// </summary>
        private System.Windows.Forms.GroupBox theSelectedPacketCaptureGroupBox;

        /// <summary>
        /// The label for the path of the selected packet capture
        /// </summary>
        private System.Windows.Forms.Label theSelectedPacketCapturePathLabel;

        /// <summary>
        /// The text box that displays the path of the selected packet capture
        /// </summary>
        private System.Windows.Forms.TextBox theSelectedPacketCapturePathTextBox;

        /// <summary>
        /// The label for the name of the selected packet capture
        /// </summary>
        private System.Windows.Forms.Label theSelectedPacketCaptureNameLabel;

        /// <summary>
        /// The text box that displays the name of the selected packet capture
        /// </summary>
        private System.Windows.Forms.TextBox theSelectedPacketCaptureNameTextBox;

        /// <summary>
        /// The label for the type of the selected packet capture
        /// </summary>
        private System.Windows.Forms.Label theSelectedPacketCaptureTypeLabel;

        /// <summary>
        /// The text box that displays the type of the selected packet capture
        /// </summary>
        private System.Windows.Forms.TextBox theSelectedPacketCaptureTypeTextBox;

        //// Buttons

        /// <summary>
        /// The "Clear Selected Packet Capture" button
        /// </summary>
        private System.Windows.Forms.Button theClearSelectedPacketCaptureButton;

        /// <summary>
        /// The "Open Selected Packet Capture" button
        /// </summary>
        private System.Windows.Forms.Button theOpenSelectedPackageCaptureButton;

        //// "Debug Information" group box

        /// <summary>
        /// The group box that contains items related to the debug information
        /// </summary>
        private System.Windows.Forms.GroupBox theDebugInformationGroupBox;

        /// <summary>
        /// The check box that indicates whether to enable the debug information
        /// </summary>
        private System.Windows.Forms.CheckBox theEnableDebugInformationCheckBox;

        /// <summary>
        /// The check box that indicates whether to redirect debug information to the output window
        /// </summary>
        private System.Windows.Forms.CheckBox theRedirectDebugInformationToOutputCheckBox;

        /// <summary>
        /// The check box that indicates whether to include information events in the debug information
        /// </summary>
        private System.Windows.Forms.CheckBox theEnableInformationEventsInDebugInformationCheckBox;

        //// "Latency Analysis" group box

        /// <summary>
        /// The group box that contains items related to latency analysis
        /// </summary>
        private System.Windows.Forms.GroupBox theLatencyAnalysisGroupBox;

        /// <summary>
        /// The check box that indicates whether to perform latency analysis for the selected packet capture
        /// </summary>
        private System.Windows.Forms.CheckBox thePerformLatencyAnalysisCheckBox;

        /// <summary>
        /// The check box that indicates whether to output histograms from the latency analysis for the selected packet capture
        /// </summary>
        private System.Windows.Forms.CheckBox theOutputLatencyAnalysisHistogramCheckBox;

        /// <summary>
        /// The check box that indicates whether to output additional information from the latency analysis for the selected packet capture
        /// </summary>
        private System.Windows.Forms.CheckBox theOutputAdditionalLatencyAnalysisInformationCheckBox;

        //// "Burst Analysis" group box

        /// <summary>
        /// The group box that contains items related to burst analysis
        /// </summary>
        private System.Windows.Forms.GroupBox theBurstAnalysisGroupBox;

        /// <summary>
        /// The check box that indicates whether to perform burst analysis for the selected packet capture
        /// </summary>
        private System.Windows.Forms.CheckBox thePerformBurstAnalysisCheckBox;

        /// <summary>
        /// The check box that indicates whether to output histograms from the burst analysis for the selected packet capture
        /// </summary>
        private System.Windows.Forms.CheckBox theOutputBurstAnalysisHistogramCheckBox;

        /// <summary>
        /// The check box that indicates whether to output additional information from the burst analysis for the selected packet capture
        /// </summary>
        private System.Windows.Forms.CheckBox theOutputAdditionalBurstAnalysisInformationCheckBox;

        //// "Time Analysis" group box

        /// <summary>
        /// The group box that contains items related to time analysis
        /// </summary>
        private System.Windows.Forms.GroupBox theTimeAnalysisGroupBox;

        /// <summary>
        /// The check box that indicates whether to perform time analysis for the selected packet capture
        /// </summary>
        private System.Windows.Forms.CheckBox thePerformTimeAnalysisCheckBox;

        /// <summary>
        /// The check box that indicates whether to output histograms from the time analysis for the selected packet capture
        /// </summary>
        private System.Windows.Forms.CheckBox theOutputTimeAnalysisHistogramCheckBox;

        /// <summary>
        /// The check box that indicates whether to output additional information from the time analysis for the selected packet capture
        /// </summary>
        private System.Windows.Forms.CheckBox theOutputAdditionalTimeAnalysisInformationCheckBox;

        //// Check Boxes

        /// <summary>
        /// The check box that indicates whether to use the alternative sequence number in the data read from the packet capture, required for legacy recordings
        /// </summary>
        private System.Windows.Forms.CheckBox theUseAlternativeSequenceNumberCheckBox;

        /// <summary>
        /// The check box that indicates whether to perform reading from the packet capture using a method that will minimize memory usage, possibly at the expense of increased processing time
        /// </summary>
        private System.Windows.Forms.CheckBox theMinimizeMemoryUsageCheckBox;

        //// Buttons

        /// <summary>
        /// The "Run Analysis On Packet Capture" button
        /// </summary>
        private System.Windows.Forms.Button theRunAnalysisOnSelectedPackageCaptureButton;

        /// <summary>
        /// The "Exit" button
        /// </summary>
        private System.Windows.Forms.Button theExitButton;

        //// Dialogue Box

        /// <summary>
        /// The dialog box used to select a packet capture for analysis
        /// </summary>
        private System.Windows.Forms.OpenFileDialog theSelectedPacketCaptureForAnalysisDialog;

        /// <summary>
        /// Clean up any resources used by the main window form
        /// </summary>
        /// <param name="disposing">Boolean flag that indicates whether the method call comes from a Dispose method (its value is true) or from the garbage collector (its value is false)</param>
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
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox = new System.Windows.Forms.CheckBox();
            this.theOutputAdditionalTimeAnalysisInformationCheckBox = new System.Windows.Forms.CheckBox();
            this.theMinimizeMemoryUsageCheckBox = new System.Windows.Forms.CheckBox();
            this.theRedirectDebugInformationToOutputCheckBox = new System.Windows.Forms.CheckBox();
            this.theEnableInformationEventsInDebugInformationCheckBox = new System.Windows.Forms.CheckBox();
            this.theEnableDebugInformationCheckBox = new System.Windows.Forms.CheckBox();
            this.theUseAlternativeSequenceNumberCheckBox = new System.Windows.Forms.CheckBox();
            this.theLatencyAnalysisGroupBox = new System.Windows.Forms.GroupBox();
            this.theOutputLatencyAnalysisHistogramCheckBox = new System.Windows.Forms.CheckBox();
            this.theDebugInformationGroupBox = new System.Windows.Forms.GroupBox();
            this.theTimeAnalysisGroupBox = new System.Windows.Forms.GroupBox();
            this.theOutputTimeAnalysisHistogramCheckBox = new System.Windows.Forms.CheckBox();
            this.theSelectedPacketCaptureGroupBox = new System.Windows.Forms.GroupBox();
            this.theBurstAnalysisGroupBox = new System.Windows.Forms.GroupBox();
            this.theOutputBurstAnalysisHistogramCheckBox = new System.Windows.Forms.CheckBox();
            this.theOutputAdditionalBurstAnalysisInformationCheckBox = new System.Windows.Forms.CheckBox();
            this.thePerformBurstAnalysisCheckBox = new System.Windows.Forms.CheckBox();
            this.theLatencyAnalysisGroupBox.SuspendLayout();
            this.theDebugInformationGroupBox.SuspendLayout();
            this.theTimeAnalysisGroupBox.SuspendLayout();
            this.theSelectedPacketCaptureGroupBox.SuspendLayout();
            this.theBurstAnalysisGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // theRunAnalysisOnSelectedPackageCaptureButton
            // 
            this.theRunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
            this.theRunAnalysisOnSelectedPackageCaptureButton.Location = new System.Drawing.Point(12, 535);
            this.theRunAnalysisOnSelectedPackageCaptureButton.Name = "theRunAnalysisOnSelectedPackageCaptureButton";
            this.theRunAnalysisOnSelectedPackageCaptureButton.Size = new System.Drawing.Size(357, 23);
            this.theRunAnalysisOnSelectedPackageCaptureButton.TabIndex = 10;
            this.theRunAnalysisOnSelectedPackageCaptureButton.Text = "Run Analysis On Selected Packet Capture";
            this.theRunAnalysisOnSelectedPackageCaptureButton.UseVisualStyleBackColor = true;
            this.theRunAnalysisOnSelectedPackageCaptureButton.Click += new System.EventHandler(this.RunAnalysisOnPacketCaptureButton_Click);
            // 
            // theExitButton
            // 
            this.theExitButton.Location = new System.Drawing.Point(12, 564);
            this.theExitButton.Name = "theExitButton";
            this.theExitButton.Size = new System.Drawing.Size(357, 23);
            this.theExitButton.TabIndex = 11;
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
            this.theSelectPacketCaptureButton.Name = "theSelectPacketCaptureButton";
            this.theSelectPacketCaptureButton.Size = new System.Drawing.Size(357, 23);
            this.theSelectPacketCaptureButton.TabIndex = 0;
            this.theSelectPacketCaptureButton.Text = "Select Packet Capture";
            this.theSelectPacketCaptureButton.UseVisualStyleBackColor = true;
            this.theSelectPacketCaptureButton.Click += new System.EventHandler(this.SelectPacketCaptureButton_Click);
            // 
            // theSelectedPacketCapturePathTextBox
            // 
            this.theSelectedPacketCapturePathTextBox.Location = new System.Drawing.Point(38, 16);
            this.theSelectedPacketCapturePathTextBox.Name = "theSelectedPacketCapturePathTextBox";
            this.theSelectedPacketCapturePathTextBox.ReadOnly = true;
            this.theSelectedPacketCapturePathTextBox.Size = new System.Drawing.Size(311, 20);
            this.theSelectedPacketCapturePathTextBox.TabIndex = 1;
            this.theSelectedPacketCapturePathTextBox.TabStop = false;
            this.theSelectedPacketCapturePathTextBox.Text = "<No Packet Capture Selected>";
            // 
            // theSelectedPacketCapturePathLabel
            // 
            this.theSelectedPacketCapturePathLabel.AutoSize = true;
            this.theSelectedPacketCapturePathLabel.Location = new System.Drawing.Point(3, 19);
            this.theSelectedPacketCapturePathLabel.Name = "theSelectedPacketCapturePathLabel";
            this.theSelectedPacketCapturePathLabel.Size = new System.Drawing.Size(29, 13);
            this.theSelectedPacketCapturePathLabel.TabIndex = 0;
            this.theSelectedPacketCapturePathLabel.Text = "Path";
            // 
            // theSelectedPacketCaptureNameLabel
            // 
            this.theSelectedPacketCaptureNameLabel.AutoSize = true;
            this.theSelectedPacketCaptureNameLabel.Location = new System.Drawing.Point(3, 45);
            this.theSelectedPacketCaptureNameLabel.Name = "theSelectedPacketCaptureNameLabel";
            this.theSelectedPacketCaptureNameLabel.Size = new System.Drawing.Size(35, 13);
            this.theSelectedPacketCaptureNameLabel.TabIndex = 2;
            this.theSelectedPacketCaptureNameLabel.Text = "Name";
            // 
            // theSelectedPacketCaptureNameTextBox
            // 
            this.theSelectedPacketCaptureNameTextBox.Location = new System.Drawing.Point(38, 42);
            this.theSelectedPacketCaptureNameTextBox.Name = "theSelectedPacketCaptureNameTextBox";
            this.theSelectedPacketCaptureNameTextBox.ReadOnly = true;
            this.theSelectedPacketCaptureNameTextBox.Size = new System.Drawing.Size(311, 20);
            this.theSelectedPacketCaptureNameTextBox.TabIndex = 3;
            this.theSelectedPacketCaptureNameTextBox.TabStop = false;
            this.theSelectedPacketCaptureNameTextBox.Text = "<No Packet Capture Selected>";
            // 
            // theSelectedPacketCaptureTypeLabel
            // 
            this.theSelectedPacketCaptureTypeLabel.AutoSize = true;
            this.theSelectedPacketCaptureTypeLabel.Location = new System.Drawing.Point(3, 70);
            this.theSelectedPacketCaptureTypeLabel.Name = "theSelectedPacketCaptureTypeLabel";
            this.theSelectedPacketCaptureTypeLabel.Size = new System.Drawing.Size(31, 13);
            this.theSelectedPacketCaptureTypeLabel.TabIndex = 4;
            this.theSelectedPacketCaptureTypeLabel.Text = "Type";
            // 
            // theSelectedPacketCaptureTypeTextBox
            // 
            this.theSelectedPacketCaptureTypeTextBox.Location = new System.Drawing.Point(38, 67);
            this.theSelectedPacketCaptureTypeTextBox.Name = "theSelectedPacketCaptureTypeTextBox";
            this.theSelectedPacketCaptureTypeTextBox.ReadOnly = true;
            this.theSelectedPacketCaptureTypeTextBox.Size = new System.Drawing.Size(311, 20);
            this.theSelectedPacketCaptureTypeTextBox.TabIndex = 5;
            this.theSelectedPacketCaptureTypeTextBox.TabStop = false;
            this.theSelectedPacketCaptureTypeTextBox.Text = "<No Packet Capture Selected>";
            // 
            // theClearSelectedPacketCaptureButton
            // 
            this.theClearSelectedPacketCaptureButton.Enabled = false;
            this.theClearSelectedPacketCaptureButton.Location = new System.Drawing.Point(12, 143);
            this.theClearSelectedPacketCaptureButton.Name = "theClearSelectedPacketCaptureButton";
            this.theClearSelectedPacketCaptureButton.Size = new System.Drawing.Size(357, 23);
            this.theClearSelectedPacketCaptureButton.TabIndex = 2;
            this.theClearSelectedPacketCaptureButton.Text = "Clear Selected Packet Capture";
            this.theClearSelectedPacketCaptureButton.UseVisualStyleBackColor = true;
            this.theClearSelectedPacketCaptureButton.Click += new System.EventHandler(this.ClearSelectedPacketCaptureButton_Click);
            // 
            // theOpenSelectedPackageCaptureButton
            // 
            this.theOpenSelectedPackageCaptureButton.Enabled = false;
            this.theOpenSelectedPackageCaptureButton.Location = new System.Drawing.Point(12, 172);
            this.theOpenSelectedPackageCaptureButton.Name = "theOpenSelectedPackageCaptureButton";
            this.theOpenSelectedPackageCaptureButton.Size = new System.Drawing.Size(357, 23);
            this.theOpenSelectedPackageCaptureButton.TabIndex = 3;
            this.theOpenSelectedPackageCaptureButton.Text = "Open Selected Packet Capture";
            this.theOpenSelectedPackageCaptureButton.UseVisualStyleBackColor = true;
            this.theOpenSelectedPackageCaptureButton.Click += new System.EventHandler(this.OpenSelectedPackageCaptureButton_Click);
            // 
            // thePerformLatencyAnalysisCheckBox
            // 
            this.thePerformLatencyAnalysisCheckBox.AutoSize = true;
            this.thePerformLatencyAnalysisCheckBox.Enabled = false;
            this.thePerformLatencyAnalysisCheckBox.Location = new System.Drawing.Point(6, 19);
            this.thePerformLatencyAnalysisCheckBox.Name = "thePerformLatencyAnalysisCheckBox";
            this.thePerformLatencyAnalysisCheckBox.Size = new System.Drawing.Size(62, 17);
            this.thePerformLatencyAnalysisCheckBox.TabIndex = 0;
            this.thePerformLatencyAnalysisCheckBox.Text = "Perform";
            this.thePerformLatencyAnalysisCheckBox.UseVisualStyleBackColor = true;
            this.thePerformLatencyAnalysisCheckBox.CheckedChanged += new System.EventHandler(this.PerformLatencyAnalysisCheckBox_CheckedChanged);
            // 
            // thePerformTimeAnalysisCheckBox
            // 
            this.thePerformTimeAnalysisCheckBox.AutoSize = true;
            this.thePerformTimeAnalysisCheckBox.Enabled = false;
            this.thePerformTimeAnalysisCheckBox.Location = new System.Drawing.Point(6, 19);
            this.thePerformTimeAnalysisCheckBox.Name = "thePerformTimeAnalysisCheckBox";
            this.thePerformTimeAnalysisCheckBox.Size = new System.Drawing.Size(62, 17);
            this.thePerformTimeAnalysisCheckBox.TabIndex = 0;
            this.thePerformTimeAnalysisCheckBox.Text = "Perform";
            this.thePerformTimeAnalysisCheckBox.UseVisualStyleBackColor = true;
            this.thePerformTimeAnalysisCheckBox.CheckedChanged += new System.EventHandler(this.PerformTimeAnalysisCheckBox_CheckedChanged);
            // 
            // theOutputAdditionalLatencyAnalysisInformationCheckBox
            // 
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.AutoSize = true;
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Enabled = false;
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Location = new System.Drawing.Point(187, 42);
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Name = "theOutputAdditionalLatencyAnalysisInformationCheckBox";
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Size = new System.Drawing.Size(162, 17);
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.TabIndex = 2;
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Text = "Output Additional Information";
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.UseVisualStyleBackColor = true;
            // 
            // theOutputAdditionalTimeAnalysisInformationCheckBox
            // 
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.AutoSize = true;
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.Enabled = false;
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.Location = new System.Drawing.Point(187, 41);
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.Name = "theOutputAdditionalTimeAnalysisInformationCheckBox";
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.Size = new System.Drawing.Size(162, 17);
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.TabIndex = 2;
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.Text = "Output Additional Information";
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.UseVisualStyleBackColor = true;
            // 
            // theMinimizeMemoryUsageCheckBox
            // 
            this.theMinimizeMemoryUsageCheckBox.AutoSize = true;
            this.theMinimizeMemoryUsageCheckBox.Enabled = false;
            this.theMinimizeMemoryUsageCheckBox.Location = new System.Drawing.Point(12, 512);
            this.theMinimizeMemoryUsageCheckBox.Name = "theMinimizeMemoryUsageCheckBox";
            this.theMinimizeMemoryUsageCheckBox.Size = new System.Drawing.Size(346, 17);
            this.theMinimizeMemoryUsageCheckBox.TabIndex = 9;
            this.theMinimizeMemoryUsageCheckBox.Text = "Minimize Memory Usage (Potentially Necessary For Very Large Files)";
            this.theMinimizeMemoryUsageCheckBox.UseVisualStyleBackColor = true;
            // 
            // theRedirectDebugInformationToOutputCheckBox
            // 
            this.theRedirectDebugInformationToOutputCheckBox.AutoSize = true;
            this.theRedirectDebugInformationToOutputCheckBox.Enabled = false;
            this.theRedirectDebugInformationToOutputCheckBox.Location = new System.Drawing.Point(187, 42);
            this.theRedirectDebugInformationToOutputCheckBox.Name = "theRedirectDebugInformationToOutputCheckBox";
            this.theRedirectDebugInformationToOutputCheckBox.Size = new System.Drawing.Size(117, 17);
            this.theRedirectDebugInformationToOutputCheckBox.TabIndex = 2;
            this.theRedirectDebugInformationToOutputCheckBox.Text = "Redirect To Output";
            this.theRedirectDebugInformationToOutputCheckBox.UseVisualStyleBackColor = true;
            // 
            // theEnableInformationEventsInDebugInformationCheckBox
            // 
            this.theEnableInformationEventsInDebugInformationCheckBox.AutoSize = true;
            this.theEnableInformationEventsInDebugInformationCheckBox.Enabled = false;
            this.theEnableInformationEventsInDebugInformationCheckBox.Location = new System.Drawing.Point(31, 42);
            this.theEnableInformationEventsInDebugInformationCheckBox.Name = "theEnableInformationEventsInDebugInformationCheckBox";
            this.theEnableInformationEventsInDebugInformationCheckBox.Size = new System.Drawing.Size(150, 17);
            this.theEnableInformationEventsInDebugInformationCheckBox.TabIndex = 1;
            this.theEnableInformationEventsInDebugInformationCheckBox.Text = "Enable Information Events";
            this.theEnableInformationEventsInDebugInformationCheckBox.UseVisualStyleBackColor = true;
            // 
            // theEnableDebugInformationCheckBox
            // 
            this.theEnableDebugInformationCheckBox.AutoSize = true;
            this.theEnableDebugInformationCheckBox.Enabled = false;
            this.theEnableDebugInformationCheckBox.Location = new System.Drawing.Point(6, 19);
            this.theEnableDebugInformationCheckBox.Name = "theEnableDebugInformationCheckBox";
            this.theEnableDebugInformationCheckBox.Size = new System.Drawing.Size(59, 17);
            this.theEnableDebugInformationCheckBox.TabIndex = 0;
            this.theEnableDebugInformationCheckBox.Text = "Enable";
            this.theEnableDebugInformationCheckBox.UseVisualStyleBackColor = true;
            this.theEnableDebugInformationCheckBox.CheckedChanged += new System.EventHandler(this.EnableDebugInformationCheckBox_CheckedChanged);
            // 
            // theUseAlternativeSequenceNumberCheckBox
            // 
            this.theUseAlternativeSequenceNumberCheckBox.AutoSize = true;
            this.theUseAlternativeSequenceNumberCheckBox.Enabled = false;
            this.theUseAlternativeSequenceNumberCheckBox.Location = new System.Drawing.Point(12, 489);
            this.theUseAlternativeSequenceNumberCheckBox.Name = "theUseAlternativeSequenceNumberCheckBox";
            this.theUseAlternativeSequenceNumberCheckBox.Size = new System.Drawing.Size(309, 17);
            this.theUseAlternativeSequenceNumberCheckBox.TabIndex = 8;
            this.theUseAlternativeSequenceNumberCheckBox.Text = "Use Alternative Sequence Number (For Legacy Recordings)";
            this.theUseAlternativeSequenceNumberCheckBox.UseVisualStyleBackColor = true;
            // 
            // theLatencyAnalysisGroupBox
            // 
            this.theLatencyAnalysisGroupBox.Controls.Add(this.theOutputLatencyAnalysisHistogramCheckBox);
            this.theLatencyAnalysisGroupBox.Controls.Add(this.thePerformLatencyAnalysisCheckBox);
            this.theLatencyAnalysisGroupBox.Controls.Add(this.theOutputAdditionalLatencyAnalysisInformationCheckBox);
            this.theLatencyAnalysisGroupBox.Enabled = false;
            this.theLatencyAnalysisGroupBox.Location = new System.Drawing.Point(12, 274);
            this.theLatencyAnalysisGroupBox.Name = "theLatencyAnalysisGroupBox";
            this.theLatencyAnalysisGroupBox.Size = new System.Drawing.Size(357, 67);
            this.theLatencyAnalysisGroupBox.TabIndex = 5;
            this.theLatencyAnalysisGroupBox.TabStop = false;
            this.theLatencyAnalysisGroupBox.Text = "Latency Analysis";
            // 
            // theOutputLatencyAnalysisHistogramCheckBox
            // 
            this.theOutputLatencyAnalysisHistogramCheckBox.AutoSize = true;
            this.theOutputLatencyAnalysisHistogramCheckBox.Enabled = false;
            this.theOutputLatencyAnalysisHistogramCheckBox.Location = new System.Drawing.Point(31, 42);
            this.theOutputLatencyAnalysisHistogramCheckBox.Name = "theOutputLatencyAnalysisHistogramCheckBox";
            this.theOutputLatencyAnalysisHistogramCheckBox.Size = new System.Drawing.Size(108, 17);
            this.theOutputLatencyAnalysisHistogramCheckBox.TabIndex = 1;
            this.theOutputLatencyAnalysisHistogramCheckBox.Text = "Output Histogram";
            this.theOutputLatencyAnalysisHistogramCheckBox.UseVisualStyleBackColor = true;
            // 
            // theDebugInformationGroupBox
            // 
            this.theDebugInformationGroupBox.Controls.Add(this.theEnableDebugInformationCheckBox);
            this.theDebugInformationGroupBox.Controls.Add(this.theEnableInformationEventsInDebugInformationCheckBox);
            this.theDebugInformationGroupBox.Controls.Add(this.theRedirectDebugInformationToOutputCheckBox);
            this.theDebugInformationGroupBox.Enabled = false;
            this.theDebugInformationGroupBox.Location = new System.Drawing.Point(12, 201);
            this.theDebugInformationGroupBox.Name = "theDebugInformationGroupBox";
            this.theDebugInformationGroupBox.Size = new System.Drawing.Size(357, 67);
            this.theDebugInformationGroupBox.TabIndex = 4;
            this.theDebugInformationGroupBox.TabStop = false;
            this.theDebugInformationGroupBox.Text = "Debug Information";
            // 
            // theTimeAnalysisGroupBox
            // 
            this.theTimeAnalysisGroupBox.Controls.Add(this.theOutputTimeAnalysisHistogramCheckBox);
            this.theTimeAnalysisGroupBox.Controls.Add(this.thePerformTimeAnalysisCheckBox);
            this.theTimeAnalysisGroupBox.Controls.Add(this.theOutputAdditionalTimeAnalysisInformationCheckBox);
            this.theTimeAnalysisGroupBox.Enabled = false;
            this.theTimeAnalysisGroupBox.Location = new System.Drawing.Point(12, 419);
            this.theTimeAnalysisGroupBox.Name = "theTimeAnalysisGroupBox";
            this.theTimeAnalysisGroupBox.Size = new System.Drawing.Size(357, 64);
            this.theTimeAnalysisGroupBox.TabIndex = 7;
            this.theTimeAnalysisGroupBox.TabStop = false;
            this.theTimeAnalysisGroupBox.Text = "Time Analysis";
            // 
            // theOutputTimeAnalysisHistogramCheckBox
            // 
            this.theOutputTimeAnalysisHistogramCheckBox.AutoSize = true;
            this.theOutputTimeAnalysisHistogramCheckBox.Enabled = false;
            this.theOutputTimeAnalysisHistogramCheckBox.Location = new System.Drawing.Point(31, 42);
            this.theOutputTimeAnalysisHistogramCheckBox.Name = "theOutputTimeAnalysisHistogramCheckBox";
            this.theOutputTimeAnalysisHistogramCheckBox.Size = new System.Drawing.Size(108, 17);
            this.theOutputTimeAnalysisHistogramCheckBox.TabIndex = 1;
            this.theOutputTimeAnalysisHistogramCheckBox.Text = "Output Histogram";
            this.theOutputTimeAnalysisHistogramCheckBox.UseVisualStyleBackColor = true;
            // 
            // theSelectedPacketCaptureGroupBox
            // 
            this.theSelectedPacketCaptureGroupBox.Controls.Add(this.theSelectedPacketCapturePathLabel);
            this.theSelectedPacketCaptureGroupBox.Controls.Add(this.theSelectedPacketCapturePathTextBox);
            this.theSelectedPacketCaptureGroupBox.Controls.Add(this.theSelectedPacketCaptureNameLabel);
            this.theSelectedPacketCaptureGroupBox.Controls.Add(this.theSelectedPacketCaptureNameTextBox);
            this.theSelectedPacketCaptureGroupBox.Controls.Add(this.theSelectedPacketCaptureTypeLabel);
            this.theSelectedPacketCaptureGroupBox.Controls.Add(this.theSelectedPacketCaptureTypeTextBox);
            this.theSelectedPacketCaptureGroupBox.Enabled = false;
            this.theSelectedPacketCaptureGroupBox.Location = new System.Drawing.Point(12, 41);
            this.theSelectedPacketCaptureGroupBox.Name = "theSelectedPacketCaptureGroupBox";
            this.theSelectedPacketCaptureGroupBox.Size = new System.Drawing.Size(357, 96);
            this.theSelectedPacketCaptureGroupBox.TabIndex = 1;
            this.theSelectedPacketCaptureGroupBox.TabStop = false;
            this.theSelectedPacketCaptureGroupBox.Text = "Selected Packet Capture";
            // 
            // theBurstAnalysisGroupBox
            // 
            this.theBurstAnalysisGroupBox.Controls.Add(this.theOutputBurstAnalysisHistogramCheckBox);
            this.theBurstAnalysisGroupBox.Controls.Add(this.theOutputAdditionalBurstAnalysisInformationCheckBox);
            this.theBurstAnalysisGroupBox.Controls.Add(this.thePerformBurstAnalysisCheckBox);
            this.theBurstAnalysisGroupBox.Enabled = false;
            this.theBurstAnalysisGroupBox.Location = new System.Drawing.Point(12, 347);
            this.theBurstAnalysisGroupBox.Name = "theBurstAnalysisGroupBox";
            this.theBurstAnalysisGroupBox.Size = new System.Drawing.Size(357, 66);
            this.theBurstAnalysisGroupBox.TabIndex = 6;
            this.theBurstAnalysisGroupBox.TabStop = false;
            this.theBurstAnalysisGroupBox.Text = "Burst Analysis";
            // 
            // theOutputBurstAnalysisHistogramCheckBox
            // 
            this.theOutputBurstAnalysisHistogramCheckBox.AutoSize = true;
            this.theOutputBurstAnalysisHistogramCheckBox.Enabled = false;
            this.theOutputBurstAnalysisHistogramCheckBox.Location = new System.Drawing.Point(31, 43);
            this.theOutputBurstAnalysisHistogramCheckBox.Name = "theOutputBurstAnalysisHistogramCheckBox";
            this.theOutputBurstAnalysisHistogramCheckBox.Size = new System.Drawing.Size(108, 17);
            this.theOutputBurstAnalysisHistogramCheckBox.TabIndex = 1;
            this.theOutputBurstAnalysisHistogramCheckBox.Text = "Output Histogram";
            this.theOutputBurstAnalysisHistogramCheckBox.UseVisualStyleBackColor = true;
            // 
            // theOutputAdditionalBurstAnalysisInformationCheckBox
            // 
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.AutoSize = true;
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.Enabled = false;
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.Location = new System.Drawing.Point(187, 43);
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.Name = "theOutputAdditionalBurstAnalysisInformationCheckBox";
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.Size = new System.Drawing.Size(162, 17);
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.TabIndex = 2;
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.Text = "Output Additional Information";
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.UseVisualStyleBackColor = true;
            // 
            // thePerformBurstAnalysisCheckBox
            // 
            this.thePerformBurstAnalysisCheckBox.AutoSize = true;
            this.thePerformBurstAnalysisCheckBox.Enabled = false;
            this.thePerformBurstAnalysisCheckBox.Location = new System.Drawing.Point(6, 20);
            this.thePerformBurstAnalysisCheckBox.Name = "thePerformBurstAnalysisCheckBox";
            this.thePerformBurstAnalysisCheckBox.Size = new System.Drawing.Size(62, 17);
            this.thePerformBurstAnalysisCheckBox.TabIndex = 0;
            this.thePerformBurstAnalysisCheckBox.Text = "Perform";
            this.thePerformBurstAnalysisCheckBox.UseVisualStyleBackColor = true;
            this.thePerformBurstAnalysisCheckBox.CheckedChanged += new System.EventHandler(this.PerformBurstAnalysisCheckBox_CheckedChanged);
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 596);
            this.Controls.Add(this.theBurstAnalysisGroupBox);
            this.Controls.Add(this.theSelectedPacketCaptureGroupBox);
            this.Controls.Add(this.theUseAlternativeSequenceNumberCheckBox);
            this.Controls.Add(this.theTimeAnalysisGroupBox);
            this.Controls.Add(this.theDebugInformationGroupBox);
            this.Controls.Add(this.theLatencyAnalysisGroupBox);
            this.Controls.Add(this.theMinimizeMemoryUsageCheckBox);
            this.Controls.Add(this.theOpenSelectedPackageCaptureButton);
            this.Controls.Add(this.theClearSelectedPacketCaptureButton);
            this.Controls.Add(this.theSelectPacketCaptureButton);
            this.Controls.Add(this.theExitButton);
            this.Controls.Add(this.theRunAnalysisOnSelectedPackageCaptureButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainWindowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Packet Capture Analyser";
            this.theLatencyAnalysisGroupBox.ResumeLayout(false);
            this.theLatencyAnalysisGroupBox.PerformLayout();
            this.theDebugInformationGroupBox.ResumeLayout(false);
            this.theDebugInformationGroupBox.PerformLayout();
            this.theTimeAnalysisGroupBox.ResumeLayout(false);
            this.theTimeAnalysisGroupBox.PerformLayout();
            this.theSelectedPacketCaptureGroupBox.ResumeLayout(false);
            this.theSelectedPacketCaptureGroupBox.PerformLayout();
            this.theBurstAnalysisGroupBox.ResumeLayout(false);
            this.theBurstAnalysisGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
