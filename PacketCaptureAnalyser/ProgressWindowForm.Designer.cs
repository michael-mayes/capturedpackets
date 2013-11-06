//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser
{
    partial class ProgressWindowForm
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
            this.AnalysingPacketCaptureProgressBar = new System.Windows.Forms.ProgressBar();
            this.AnalysingPacketCaptureLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AnalysingPacketCaptureProgressBar
            // 
            this.AnalysingPacketCaptureProgressBar.Enabled = false;
            this.AnalysingPacketCaptureProgressBar.Location = new System.Drawing.Point(13, 28);
            this.AnalysingPacketCaptureProgressBar.Name = "AnalysingPacketCaptureProgressBar";
            this.AnalysingPacketCaptureProgressBar.Size = new System.Drawing.Size(240, 23);
            this.AnalysingPacketCaptureProgressBar.TabIndex = 21;
            this.AnalysingPacketCaptureProgressBar.Tag = "";
            // 
            // AnalysingPacketCaptureLabel
            // 
            this.AnalysingPacketCaptureLabel.Location = new System.Drawing.Point(-3, 4);
            this.AnalysingPacketCaptureLabel.Name = "AnalysingPacketCaptureLabel";
            this.AnalysingPacketCaptureLabel.Size = new System.Drawing.Size(271, 18);
            this.AnalysingPacketCaptureLabel.TabIndex = 25;
            this.AnalysingPacketCaptureLabel.Text = "<Default Text>";
            this.AnalysingPacketCaptureLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 58);
            this.ControlBox = false;
            this.Controls.Add(this.AnalysingPacketCaptureLabel);
            this.Controls.Add(this.AnalysingPacketCaptureProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressWindowForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Analysis Progress";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ProgressBar AnalysingPacketCaptureProgressBar;
        public System.Windows.Forms.Label AnalysingPacketCaptureLabel;
    }
}