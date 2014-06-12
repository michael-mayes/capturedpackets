// $Id$
// $URL$
// <copyright file="ProgressWindowForm.Designer.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser
{
    /// <summary>
    /// This class provides the progress window form
    /// </summary>
    public partial class ProgressWindowForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// The progress bar
        /// </summary>
        private System.Windows.Forms.ProgressBar progressBar;

        /// <summary>
        /// The label providing additional context for the progress bar
        /// </summary>
        private System.Windows.Forms.Label progressBarLabel;

        /// <summary>
        /// Gets or sets the value of the progress bar
        /// </summary>
        public int ProgressBar
        {
            get { return this.progressBar.Value; }
            set { this.progressBar.Value = value; }
        }

        /// <summary>
        /// Gets or sets the text of the progress bar label
        /// </summary>
        public string ProgressBarLabel
        {
            get { return this.progressBarLabel.Text; }
            set { this.progressBarLabel.Text = value; }
        }

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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressBarLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // ProgressBar
            //
            this.progressBar.Enabled = false;
            this.progressBar.Location = new System.Drawing.Point(13, 28);
            this.progressBar.Name = "ProgressBar";
            this.progressBar.Size = new System.Drawing.Size(240, 23);
            this.progressBar.TabIndex = 21;
            this.progressBar.Tag = string.Empty;
            //
            // ProgressBarLabel
            //
            this.progressBarLabel.Location = new System.Drawing.Point(-3, 4);
            this.progressBarLabel.Name = "ProgressBarLabel";
            this.progressBarLabel.Size = new System.Drawing.Size(271, 18);
            this.progressBarLabel.TabIndex = 25;
            this.progressBarLabel.Text = "<Default Text>";
            this.progressBarLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // ProgressWindowForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 58);
            this.ControlBox = false;
            this.Controls.Add(this.progressBarLabel);
            this.Controls.Add(this.progressBar);
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
    }
}
