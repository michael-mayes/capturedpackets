// <copyright file="MainWindowForm.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer
{
    using System.Linq; // Required to be able to use Any method

    /// <summary>
    /// This class provides the main window form
    /// </summary>
    public partial class MainWindowForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// The path of the selected packet capture
        /// </summary>
        private string theSelectedPacketCapturePath;

        /// <summary>
        /// The type of the selected packet capture
        /// </summary>
        private MainWindowFormConstants.PacketCaptureType theSelectedPacketCaptureType;

        /// <summary>
        /// The class provides the latency analysis processing
        /// </summary>
        private Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing = null;

        /// <summary>
        /// The class provides the burst analysis processing
        /// </summary>
        private Analysis.BurstAnalysis.Processing theBurstAnalysisProcessing = null;

        /// <summary>
        /// The class provides the time analysis processing
        /// </summary>
        private Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing = null;

        /// <summary>
        /// Initializes a new instance of the MainWindowForm class
        /// </summary>
        public MainWindowForm()
        {
            this.InitializeComponent();

            // Clear the selected packet capture on window form creation
            this.ClearSelectedPacketCapture(this);
        }

        //// Packet capture type

        /// <summary>
        /// Checks if the indicated packet capture has an expected type
        /// </summary>
        /// <param name="thePath">The path of the packet capture</param>
        /// <returns>>Boolean flag that indicates whether the indicated packet capture has an expected type</returns>
        private static bool IsExpectedPacketCaptureType(string thePath)
        {
            bool theResult = true;

            string theFileExtension = System.IO.Path.GetExtension(thePath);

            // Check if this is an expected type for a packet capture
            // Determine the type of the packet capture from the file extension
            // Does the file extension match one of the expected values for a packet capture?
            if (MainWindowFormConstants.ExpectedPCAPNextGenerationPacketCaptureFileExtensions.Any(theFileExtension.Equals) ||
                MainWindowFormConstants.ExpectedPCAPPacketCaptureFileExtensions.Any(theFileExtension.Equals) ||
                MainWindowFormConstants.ExpectedNASnifferDOSPacketCaptureFileExtensions.Any(theFileExtension.Equals))
            {
                // This file is a supported type for a packet capture
                // Any changes to this set of supported types should also be reflected to the Selected Packet Capture For Analysis dialog!
            }
            else
            {
                // This file is an unsupported type for a packet capture
                theResult = false;
            }

            return theResult;
        }

        //// Button and checkbox click actions

        /// <summary>
        /// Processes the button click event for the "Select Packet Capture" button
        /// </summary>
        /// <param name="sender">The sender for the button click event</param>
        /// <param name="e">The arguments for the button click event</param>
        private void SelectPacketCaptureButton_Click(object sender, System.EventArgs e)
        {
            // Open the packet capture selection dialog box
            System.Windows.Forms.DialogResult theSelectedPacketCaptureDialogResult =
                this.theSelectPacketCaptureForAnalysisDialog.ShowDialog();

            if (theSelectedPacketCaptureDialogResult == System.Windows.Forms.DialogResult.OK)
            {
                // Update the window to reflect the selected packet capture
                this.ReflectSelectedPacketCapture(this.theSelectPacketCaptureForAnalysisDialog.FileName, sender);
            }
        }

        /// <summary>
        /// Processes the button click event for the "Clear Selected Packet Capture" button
        /// </summary>
        /// <param name="sender">The sender for the button click event</param>
        /// <param name="e">The arguments for the button click event</param>
        private void ClearSelectedPacketCaptureButton_Click(object sender, System.EventArgs e)
        {
            // Clear the selected packet capture on user request
            this.ClearSelectedPacketCapture(sender);
        }

        /// <summary>
        /// Processes the button click event for the "Open Selected Packet Capture" button
        /// </summary>
        /// <param name="sender">The sender for the button click event</param>
        /// <param name="e">The arguments for the button click event</param>
        private void OpenSelectedPackageCaptureButton_Click(object sender, System.EventArgs e)
        {
            if (System.IO.File.Exists(this.theSelectedPacketCapturePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(this.theSelectedPacketCapturePath);
                }
                catch (System.ComponentModel.Win32Exception f)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "The exception " +
                        f.GetType().Name +
                        " with the following message: " +
                        f.Message +
                        " was raised as there is no application registered that can open the " +
                        System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                        " packet capture!!!");
                }
            }
        }

        /// <summary>
        /// Processes the check changed event for the "Enable" check box in the "Debug Information" group box
        /// </summary>
        /// <param name="sender">The sender for the check changed event</param>
        /// <param name="e">The arguments for the check changed event</param>
        private void EnableDebugInformationCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            // Guard against infinite recursion
            if ((sender as System.Windows.Forms.Control).ContainsFocus)
            {
                if (this.theEnableDebugInformationCheckBox.Checked)
                {
                    this.EnablePacketCaptureAnalysisCheckBoxes(sender);
                }
                else
                {
                    this.DisablePacketCaptureAnalysisCheckBoxes(sender);
                }
            }
        }

        /// <summary>
        /// Processes the check changed event for the "Perform" check box in the "Latency Analysis" group box
        /// </summary>
        /// <param name="sender">The sender for the check changed event</param>
        /// <param name="e">The arguments for the check changed event</param>
        private void PerformLatencyAnalysisCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            // Guard against infinite recursion
            if ((sender as System.Windows.Forms.Control).ContainsFocus)
            {
                if (this.thePerformLatencyAnalysisCheckBox.Checked)
                {
                    this.theOutputLatencyAnalysisHistogramCheckBox.Checked = false;
                    this.theOutputLatencyAnalysisHistogramCheckBox.Enabled = true;

                    this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Checked = false;
                    this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Enabled = true;
                }
                else
                {
                    this.theOutputLatencyAnalysisHistogramCheckBox.Checked = false;
                    this.theOutputLatencyAnalysisHistogramCheckBox.Enabled = false;

                    this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Checked = false;
                    this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Processes the check changed event for the "Perform" check box in the "Burst Analysis" group box
        /// </summary>
        /// <param name="sender">The sender for the check changed event</param>
        /// <param name="e">The arguments for the check changed event</param>
        private void PerformBurstAnalysisCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            // Guard against infinite recursion
            if ((sender as System.Windows.Forms.Control).ContainsFocus)
            {
                if (this.thePerformBurstAnalysisCheckBox.Checked)
                {
                    this.theOutputBurstAnalysisHistogramCheckBox.Checked = false;
                    this.theOutputBurstAnalysisHistogramCheckBox.Enabled = true;

                    this.theOutputAdditionalBurstAnalysisInformationCheckBox.Checked = false;
                    this.theOutputAdditionalBurstAnalysisInformationCheckBox.Enabled = true;
                }
                else
                {
                    this.theOutputBurstAnalysisHistogramCheckBox.Checked = false;
                    this.theOutputBurstAnalysisHistogramCheckBox.Enabled = false;

                    this.theOutputAdditionalBurstAnalysisInformationCheckBox.Checked = false;
                    this.theOutputAdditionalBurstAnalysisInformationCheckBox.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Processes the check changed event for the "Perform" check box in the "Time Analysis" group box
        /// </summary>
        /// <param name="sender">The sender for the check changed event</param>
        /// <param name="e">The arguments for the check changed event</param>
        private void PerformTimeAnalysisCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            // Guard against infinite recursion
            if ((sender as System.Windows.Forms.Control).ContainsFocus)
            {
                if (this.thePerformTimeAnalysisCheckBox.Checked)
                {
                    this.theOutputTimeAnalysisTimestampHistogramCheckBox.Checked = false;
                    this.theOutputTimeAnalysisTimestampHistogramCheckBox.Enabled = true;

                    this.theOutputTimeAnalysisTimeHistogramCheckBox.Checked = false;
                    this.theOutputTimeAnalysisTimeHistogramCheckBox.Enabled = true;

                    this.theOutputAdditionalTimeAnalysisInformationCheckBox.Checked = false;
                    this.theOutputAdditionalTimeAnalysisInformationCheckBox.Enabled = true;
                }
                else
                {
                    this.theOutputTimeAnalysisTimestampHistogramCheckBox.Checked = false;
                    this.theOutputTimeAnalysisTimestampHistogramCheckBox.Enabled = false;

                    this.theOutputTimeAnalysisTimeHistogramCheckBox.Checked = false;
                    this.theOutputTimeAnalysisTimeHistogramCheckBox.Enabled = false;

                    this.theOutputAdditionalTimeAnalysisInformationCheckBox.Checked = false;
                    this.theOutputAdditionalTimeAnalysisInformationCheckBox.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Processes the button click event for the "Run Analysis On Packet Capture" button
        /// </summary>
        /// <param name="sender">The sender for the button click event</param>
        /// <param name="e">The arguments for the button click event</param>
        private void RunAnalysisOnPacketCaptureButton_Click(object sender, System.EventArgs e)
        {
            this.AnalysePacketCapture();
        }

        /// <summary>
        /// Processes the button click event for the "Exit" button
        /// </summary>
        /// <param name="sender">The sender for the button click event</param>
        /// <param name="e">The arguments for the button click event</param>
        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        //// Drag and drop actions

        /// <summary>
        /// Processes the drag enter event on dragging a file into the main window form
        /// </summary>
        /// <param name="sender">The sender for the drag enter event</param>
        /// <param name="e">The arguments for the drag enter event</param>
        private void MainWindowForm_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Get the set of the files dragged and dropped into the main window form
            // Will only use the first of the set as single file processing is the existing pattern for this application
            string[] theFiles = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);

            // Check if this is an expected type for a packet capture
            if (IsExpectedPacketCaptureType(theFiles[0]))
            {
                // This file is a supported type for a packet capture so start so allow the drag into the main window form
                if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
                {
                    e.Effect = System.Windows.Forms.DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = System.Windows.Forms.DragDropEffects.None;
                }
            }
            else
            {
                // This file is either an unsupported type for a packet capture or is another type of file so prevent the drag into the main window form
                e.Effect = System.Windows.Forms.DragDropEffects.None;
            }
        }

        /// <summary>
        /// Processes the drag drop event on dropping a file into the main window form
        /// </summary>
        /// <param name="sender">The sender for the drag drop event</param>
        /// <param name="e">The arguments for the drag drop event</param>
        private void MainWindowForm_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Get the set of the files dragged and dropped into the main window form
            // Will only use the first of the set as single file processing is the existing pattern for this application
            string[] theFiles = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);

            // Check if this is an expected type for a packet capture
            if (IsExpectedPacketCaptureType(theFiles[0]))
            {
                // This file is a supported type for a packet capture so start processing it

                // Update the window to reflect the selected packet capture
                this.ReflectSelectedPacketCapture(theFiles[0], sender);
            }
        }

        //// Packet capture support functions

        /// <summary>
        /// Reflects the selection of the indicated packet capture by setting up the associated items accordingly
        /// </summary>
        /// <param name="thePath">The path of the selected packet capture</param>
        /// <param name="sender">The sender for the initiating event</param>
        private void ReflectSelectedPacketCapture(string thePath, object sender)
        {
            // Store the path of the indicated packet capture for use by later processing
            this.theSelectedPacketCapturePath = thePath;

            System.Diagnostics.Debug.WriteLine(
                "Selection of the " +
                System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                " packet capture");

            // Determine the type of the packet capture
            this.DeterminePacketCaptureType();

            switch (this.theSelectedPacketCaptureType)
            {
                case MainWindowFormConstants.PacketCaptureType.PCAPNextGeneration:
                    {
                        //// Analysis of a PCAP Next Generation packet capture is supported

                        // Enable the "Selected Packet Capture" group box
                        this.theSelectedPacketCaptureGroupBox.Enabled = true;

                        // Populate the path for the selected packet capture
                        this.theSelectedPacketCapturePathTextBox.Text =
                            System.IO.Path.GetDirectoryName(
                            this.theSelectedPacketCapturePath);

                        // Populate the name for the selected packet capture
                        this.theSelectedPacketCaptureNameTextBox.Text =
                            System.IO.Path.GetFileName(
                            this.theSelectedPacketCapturePath);

                        // This is a PCAP Next Generation packet capture
                        this.theSelectedPacketCaptureTypeTextBox.Text =
                            Properties.Resources.PCAPNextGeneration;

                        // Enable the buttons
                        this.EnablePacketCaptureAnalysisButtons();

                        // Reset and enable the check boxes
                        this.EnablePacketCaptureAnalysisCheckBoxes(sender);

                        break;
                    }

                case MainWindowFormConstants.PacketCaptureType.PCAP:
                    {
                        //// Analysis of a PCAP packet capture is supported

                        // Enable the "Selected Packet Capture" group box
                        this.theSelectedPacketCaptureGroupBox.Enabled = true;

                        // Populate the path for the selected packet capture
                        this.theSelectedPacketCapturePathTextBox.Text =
                            System.IO.Path.GetDirectoryName(
                            this.theSelectedPacketCapturePath);

                        // Populate the name for the selected packet capture
                        this.theSelectedPacketCaptureNameTextBox.Text =
                            System.IO.Path.GetFileName(
                            this.theSelectedPacketCapturePath);

                        // This is a PCAP packet capture
                        this.theSelectedPacketCaptureTypeTextBox.Text =
                            Properties.Resources.PCAP;

                        // Enable the buttons
                        this.EnablePacketCaptureAnalysisButtons();

                        // Reset and enable the check boxes
                        this.EnablePacketCaptureAnalysisCheckBoxes(sender);

                        break;
                    }

                case MainWindowFormConstants.PacketCaptureType.NASnifferDOS:
                    {
                        //// Analysis of an NA Sniffer (DOS) packet capture is supported

                        // Enable the "Selected Packet Capture" group box
                        this.theSelectedPacketCaptureGroupBox.Enabled = true;

                        // Populate the path for the selected packet capture
                        this.theSelectedPacketCapturePathTextBox.Text =
                            System.IO.Path.GetDirectoryName(
                            this.theSelectedPacketCapturePath);

                        // Populate the name for the selected packet capture
                        this.theSelectedPacketCaptureNameTextBox.Text =
                            System.IO.Path.GetFileName(
                            this.theSelectedPacketCapturePath);

                        // This is an NA Sniffer (DOS) packet capture
                        this.theSelectedPacketCaptureTypeTextBox.Text =
                            Properties.Resources.NASnifferDOS;

                        // Enable the buttons
                        this.EnablePacketCaptureAnalysisButtons();

                        // Reset and enable the check boxes
                        this.EnablePacketCaptureAnalysisCheckBoxes(sender);

                        break;
                    }

                case MainWindowFormConstants.PacketCaptureType.Incorrect:
                    {
                        //// Analysis of this packet capture is not supported

                        // Enable the "Selected Packet Capture" group box
                        this.theSelectedPacketCaptureGroupBox.Enabled = true;

                        // Populate the path for the selected packet capture
                        this.theSelectedPacketCapturePathTextBox.Text =
                            System.IO.Path.GetDirectoryName(
                            this.theSelectedPacketCapturePath);

                        // Populate the name for the selected packet capture
                        this.theSelectedPacketCaptureNameTextBox.Text =
                            System.IO.Path.GetFileName(
                            this.theSelectedPacketCapturePath);

                        // This packet capture has an incorrect file extension for the form of packet capture it contains
                        this.theSelectedPacketCaptureTypeTextBox.Text =
                            "<Incorrect Packet Capture Type For File Extension>";

                        // Reset the buttons
                        this.ResetPacketCaptureAnalysisButtons();

                        // Reset and disable the check boxes
                        this.DisablePacketCaptureAnalysisCheckBoxes(sender);

                        break;
                    }

                case MainWindowFormConstants.PacketCaptureType.Unknown:
                default:
                    {
                        //// Analysis of this packet capture is not supported

                        // Enable the "Selected Packet Capture" group box
                        this.theSelectedPacketCaptureGroupBox.Enabled = true;

                        // Populate the path for the selected packet capture
                        this.theSelectedPacketCapturePathTextBox.Text =
                            System.IO.Path.GetDirectoryName(
                            this.theSelectedPacketCapturePath);

                        // Populate the name for the selected packet capture
                        this.theSelectedPacketCaptureNameTextBox.Text =
                            System.IO.Path.GetFileName(
                            this.theSelectedPacketCapturePath);

                        // This packet capture is either an unsupported form of packet capture or is another type of file
                        this.theSelectedPacketCaptureTypeTextBox.Text =
                            "<Unknown Packet Capture Type>";

                        // Reset the buttons
                        this.ResetPacketCaptureAnalysisButtons();

                        // Reset and disable the check boxes
                        this.DisablePacketCaptureAnalysisCheckBoxes(sender);

                        break;
                    }
            }
        }

        /// <summary>
        /// Clears the currently selected packet capture and resets the associated items
        /// </summary>
        /// <param name="sender">The sender for the initiating event</param>
        private void ClearSelectedPacketCapture(object sender)
        {
            this.theSelectedPacketCapturePath = null;

            this.theSelectedPacketCaptureType =
                MainWindowFormConstants.PacketCaptureType.Unknown;

            // Disable the "Selected Packet Capture" group box
            this.theSelectedPacketCaptureGroupBox.Enabled = false;

            // Clear the path, name and type for the selected packet capture
            this.theSelectedPacketCapturePathTextBox.Text = string.Empty;
            this.theSelectedPacketCaptureNameTextBox.Text = string.Empty;
            this.theSelectedPacketCaptureTypeTextBox.Text = string.Empty;

            // Disable the button to clear the selected package capture
            this.theClearSelectedPacketCaptureButton.Enabled = false;

            // Disable the button to open the package capture
            this.theOpenSelectedPackageCaptureButton.Enabled = false;

            // Disable the button to start the analysis on the packet capture
            this.theRunAnalysisOnSelectedPackageCaptureButton.Enabled = false;

            // Reset and disable the check boxes
            this.DisablePacketCaptureAnalysisCheckBoxes(sender);
        }

        //// Packet capture type

        /// <summary>
        /// Determines the type of the packet capture from the initial bytes and ensure that it is of the expected type for the file extension
        /// </summary>
        private void DeterminePacketCaptureType()
        {
            //// Determine the type of the packet capture

            // Declare a file stream for the packet capture for reading
            System.IO.FileStream theFileStream = null;

            // Use a try/finally pattern to avoid multiple disposals of a resource
            try
            {
                // Open a file stream for the packet capture for reading
                theFileStream = System.IO.File.OpenRead(this.theSelectedPacketCapturePath);

                // Open a binary reader for the file stream for the packet capture
                using (System.IO.BinaryReader theBinaryReader =
                    new System.IO.BinaryReader(theFileStream))
                {
                    // The resources for the file stream for the packet capture will be disposed of by the binary reader so set it back to null here to prevent the finally clause performing an additional disposal
                    theFileStream = null;

                    string theFileExtension = System.IO.Path.GetExtension(this.theSelectedPacketCapturePath);

                    switch (theBinaryReader.ReadUInt32())
                    {
                        case (uint)PacketCapture.PCAPNGPackageCapture.Constants.BlockType.SectionHeaderBlock:
                            {
                                if (MainWindowFormConstants.ExpectedPCAPNextGenerationPacketCaptureFileExtensions.Any(theFileExtension.Equals))
                                {
                                    // This is a PCAP Next Generation packet capture
                                    this.theSelectedPacketCaptureType =
                                        MainWindowFormConstants.PacketCaptureType.PCAPNextGeneration;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine(
                                        "The " +
                                        System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                                        " packet capture should be a PCAP Next Generation packet capture based on its initial bytes, but its file extension " +
                                        theFileExtension +
                                        " is incorrect for that type of packet capture!!!");

                                    this.theSelectedPacketCaptureType =
                                        MainWindowFormConstants.PacketCaptureType.Incorrect;
                                }

                                break;
                            }

                        case (uint)PacketCapture.PCAPPackageCapture.Constants.LittleEndianMagicNumber:
                        case (uint)PacketCapture.PCAPPackageCapture.Constants.BigEndianMagicNumber:
                            {
                                if (MainWindowFormConstants.ExpectedPCAPPacketCaptureFileExtensions.Any(theFileExtension.Equals))
                                {
                                    // This is a PCAP packet capture
                                    this.theSelectedPacketCaptureType =
                                        MainWindowFormConstants.PacketCaptureType.PCAP;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine(
                                        "The " +
                                        System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                                        " packet capture should be a PCAP packet capture based on its initial bytes, but its file extension " +
                                        theFileExtension +
                                        " is incorrect for that type of packet capture!!!");

                                    this.theSelectedPacketCaptureType =
                                        MainWindowFormConstants.PacketCaptureType.Incorrect;
                                }

                                break;
                            }

                        case (uint)PacketCapture.SnifferPackageCapture.Constants.ExpectedMagicNumberHighest:
                            {
                                if (MainWindowFormConstants.ExpectedNASnifferDOSPacketCaptureFileExtensions.Any(theFileExtension.Equals))
                                {
                                    // This is an NA Sniffer (DOS) packet capture
                                    this.theSelectedPacketCaptureType =
                                        MainWindowFormConstants.PacketCaptureType.NASnifferDOS;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine(
                                        "The " +
                                        System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                                        " packet capture should be an NA Sniffer (DOS) packet capture based on its initial bytes, but its file extension " +
                                        theFileExtension +
                                        " is incorrect for that type of packet capture!!!");

                                    this.theSelectedPacketCaptureType =
                                        MainWindowFormConstants.PacketCaptureType.Incorrect;
                                }

                                break;
                            }

                        default:
                            {
                                // This packet capture is either an unsupported form of packet capture or is another type of file
                                this.theSelectedPacketCaptureType =
                                    MainWindowFormConstants.PacketCaptureType.Unknown;

                                break;
                            }
                    }
                }
            }
            finally
            {
                // Dispose of the resources for the file stream for the packet capture if this action has not already taken place above
                if (theFileStream != null)
                {
                    theFileStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Performs analysis of the currently selected packet capture
        /// </summary>
        private void AnalysePacketCapture()
        {
            bool theResult = true;

            // Declare an instance of the progress window form
            ProgressWindowForm theProgressWindowForm = null;

            // Use a try/finally pattern to avoid multiple disposals of a resource
            try
            {
                // Create an instance of the progress window form
                theProgressWindowForm = new PacketCaptureAnalyzer.ProgressWindowForm();

                // Show the progress window form now the analysis has started
                theProgressWindowForm.Show();
                theProgressWindowForm.Activate();

                // Update the label now the reading of the packet capture has started - the progress bar will stay at zero
                theProgressWindowForm.ProgressBarLabel = "Reading Packet Capture Into System Memory";
                theProgressWindowForm.ProgressBar = 0;
                theProgressWindowForm.Refresh();

                theProgressWindowForm.ProgressBar = 5;

                // Obtain the name of the packet capture
                string thePacketCaptureFileName =
                    System.IO.Path.GetFileName(
                    this.theSelectedPacketCapturePath);

                theProgressWindowForm.ProgressBar = 10;

                using (Analysis.DebugInformation theDebugInformation =
                    new Analysis.DebugInformation(
                        this.theSelectedPacketCapturePath + ".txt",
                        this.theEnableDebugInformationCheckBox.Checked,
                        this.theEnableInformationEventsInDebugInformationCheckBox.Checked,
                        this.theRedirectDebugInformationToOutputCheckBox.Checked))
                {
                    theProgressWindowForm.ProgressBar = 20;

                    // Start the processing of the packet capture
                    theDebugInformation.WriteTestRunEvent(
                        "Processing of the " +
                        thePacketCaptureFileName +
                        " packet capture started");

                    theDebugInformation.WriteBlankLine();

                    theDebugInformation.WriteTextLine(
                        new string('=', 144));

                    theDebugInformation.WriteBlankLine();

                    // Perform the actions to initialize the analysis of the packet capture
                    theResult = this.InitialiseAnalysisForPacketCapture(
                        theDebugInformation,
                        theProgressWindowForm,
                        thePacketCaptureFileName);

                    // Dependent on the result of the processing above, display a debug message to indicate success or otherwise
                    if (theResult)
                    {
                        // Display a debug message to indicate parsing of the packet capture completed successfully
                        theDebugInformation.WriteTestRunEvent(
                            "Parsing of the " +
                            thePacketCaptureFileName +
                            " packet capture completed successfully");

                        // Perform the actions to analyze the packet capture
                        theResult = this.PerformAnalysisForPacketCapture(
                            theDebugInformation,
                            theProgressWindowForm,
                            thePacketCaptureFileName);

                        theDebugInformation.WriteBlankLine();

                        theDebugInformation.WriteTextLine(
                            new string('=', 144));

                        theDebugInformation.WriteBlankLine();

                        // Completed the processing of the packet capture
                        theDebugInformation.WriteTestRunEvent(
                            "Processing of the " +
                            thePacketCaptureFileName +
                            " packet capture completed");
                    }
                    else
                    {
                        theDebugInformation.WriteBlankLine();

                        theDebugInformation.WriteTextLine(
                            new string('=', 144));

                        theDebugInformation.WriteBlankLine();

                        // Display a debug message to indicate processing of the packet capture failed
                        theDebugInformation.WriteErrorEvent(
                            "Processing of the " +
                            thePacketCaptureFileName +
                            " packet capture failed!!!");
                    }

                    if (theProgressWindowForm != null)
                    {
                        // Hide and close the progress window form now the analysis has completed
                        theProgressWindowForm.Hide();
                        theProgressWindowForm.Close();

                        // The resources for the progress window form will have been disposed by closing it so prevent the finally clause performing an additional disposal
                        theProgressWindowForm = null;
                    }

                    //// Dependent on the result of the processing above, display a message box to indicate success or otherwise

                    System.Windows.Forms.DialogResult theMessageBoxResult;

                    System.Windows.Forms.MessageBoxOptions theMessageBoxOptions =
                        new System.Windows.Forms.MessageBoxOptions();

                    if (System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
                    {
                        theMessageBoxOptions |=
                            System.Windows.Forms.MessageBoxOptions.RtlReading |
                            System.Windows.Forms.MessageBoxOptions.RightAlign;
                    }

                    if (theResult)
                    {
                        if (this.theEnableDebugInformationCheckBox.Checked)
                        {
                            // Display a message box to indicate analysis of the packet capture completed successfully and ask whether to open the output file
                            theMessageBoxResult =
                                System.Windows.Forms.MessageBox.Show(
                                Properties.Resources.AnalysisOfThePacketCaptureCompletedSuccessfully +
                                System.Environment.NewLine +
                                System.Environment.NewLine +
                                Properties.Resources.DoYouWantToOpenTheOutputFile,
                                Properties.Resources.RunAnalysisOnSelectedPacketCapture,
                                System.Windows.Forms.MessageBoxButtons.YesNo,
                                System.Windows.Forms.MessageBoxIcon.Question,
                                System.Windows.Forms.MessageBoxDefaultButton.Button1,
                                theMessageBoxOptions);
                        }
                        else
                        {
                            // Display a message box to indicate analysis of the packet capture completed successfully
                            theMessageBoxResult =
                                System.Windows.Forms.MessageBox.Show(
                                Properties.Resources.AnalysisOfThePacketCaptureCompletedSuccessfully,
                                Properties.Resources.RunAnalysisOnSelectedPacketCapture,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information,
                                System.Windows.Forms.MessageBoxDefaultButton.Button1,
                                theMessageBoxOptions);
                        }
                    }
                    else
                    {
                        if (this.theEnableDebugInformationCheckBox.Checked)
                        {
                            // Display a message box to indicate analysis of the packet capture failed and ask whether to open the output file
                            theMessageBoxResult =
                                System.Windows.Forms.MessageBox.Show(
                                Properties.Resources.AnalysisOfThePacketCaptureFailed +
                                System.Environment.NewLine +
                                System.Environment.NewLine +
                                Properties.Resources.DoYouWantToOpenTheOutputFile,
                                Properties.Resources.RunAnalysisOnSelectedPacketCapture,
                                System.Windows.Forms.MessageBoxButtons.YesNo,
                                System.Windows.Forms.MessageBoxIcon.Error,
                                System.Windows.Forms.MessageBoxDefaultButton.Button1,
                                theMessageBoxOptions);
                        }
                        else
                        {
                            // Display a message box to indicate analysis of the packet capture failed
                            theMessageBoxResult =
                                System.Windows.Forms.MessageBox.Show(
                                Properties.Resources.AnalysisOfThePacketCaptureFailed,
                                Properties.Resources.RunAnalysisOnSelectedPacketCapture,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Error,
                                System.Windows.Forms.MessageBoxDefaultButton.Button1,
                                theMessageBoxOptions);
                        }
                    }

                    //// Dispose of the resources for the analysis processing if these actions have not already taken place above

                    if (this.theLatencyAnalysisProcessing != null)
                    {
                        this.theLatencyAnalysisProcessing.Dispose();
                    }

                    if (this.theBurstAnalysisProcessing != null)
                    {
                        this.theBurstAnalysisProcessing.Dispose();
                    }

                    if (this.theTimeAnalysisProcessing != null)
                    {
                        this.theTimeAnalysisProcessing.Dispose();
                    }

                    // Dependent on the button selection at the message box, open the output file
                    if (theMessageBoxResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        theDebugInformation.Open();
                    }
                }
            }
            finally
            {
                // Dispose of the resources for the progress window form if this action has not already taken place above
                if (theProgressWindowForm != null)
                {
                    theProgressWindowForm.Dispose();
                }
            }
        }

        /// <summary>
        /// Performs the actions to initialize the analysis of the packet capture
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theProgressWindowForm">The progress window form</param>
        /// <param name="thePacketCaptureFileName">The name of the packet capture</param>
        /// <returns>Boolean flag that indicates whether the actions to initialize the analysis of the packet capture was successful</returns>
        private bool InitialiseAnalysisForPacketCapture(Analysis.DebugInformation theDebugInformation, ProgressWindowForm theProgressWindowForm, string thePacketCaptureFileName)
        {
            bool theResult = true;

            theProgressWindowForm.ProgressBar = 30;

            // Only perform the latency analysis if the check box was selected for it on the main window form
            if (this.thePerformLatencyAnalysisCheckBox.Checked)
            {
                this.theLatencyAnalysisProcessing =
                    new Analysis.LatencyAnalysis.Processing(
                        theDebugInformation,
                        this.theOutputLatencyAnalysisHistogramCheckBox.Checked,
                        this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Checked,
                        this.theSelectedPacketCapturePath);

                // Initialise the functionality to perform latency analysis on the messages found
                this.theLatencyAnalysisProcessing.Create();
            }

            theProgressWindowForm.ProgressBar = 35;

            // Only perform the burst analysis if the check box was selected for it on the main window form
            if (this.thePerformBurstAnalysisCheckBox.Checked)
            {
                this.theBurstAnalysisProcessing =
                    new Analysis.BurstAnalysis.Processing(
                        theDebugInformation,
                        this.theOutputBurstAnalysisHistogramCheckBox.Checked,
                        this.theOutputAdditionalBurstAnalysisInformationCheckBox.Checked,
                        this.theSelectedPacketCapturePath);

                // Initialise the functionality to perform burst analysis on the messages found
                this.theBurstAnalysisProcessing.Create();
            }

            theProgressWindowForm.ProgressBar = 40;

            // Only perform the time analysis if the check box was selected for it on the main window form
            if (this.thePerformTimeAnalysisCheckBox.Checked)
            {
                this.theTimeAnalysisProcessing =
                    new Analysis.TimeAnalysis.Processing(
                        theDebugInformation,
                        this.theOutputTimeAnalysisTimestampHistogramCheckBox.Checked,
                        this.theOutputTimeAnalysisTimeHistogramCheckBox.Checked,
                        this.theOutputAdditionalTimeAnalysisInformationCheckBox.Checked,
                        this.theSelectedPacketCapturePath);

                // Initialise the functionality to perform time analysis on the messages found
                this.theTimeAnalysisProcessing.Create();
            }

            theProgressWindowForm.ProgressBar = 45;

            switch (this.theSelectedPacketCaptureType)
            {
                case MainWindowFormConstants.PacketCaptureType.PCAPNextGeneration:
                    {
                        theDebugInformation.WriteInformationEvent(
                            "This is a PCAP Next Generation packet capture");

                        PacketCapture.PCAPNGPackageCapture.Processing thePCAPNGPackageCaptureProcessing =
                            new PacketCapture.PCAPNGPackageCapture.Processing(
                                theProgressWindowForm,
                                theDebugInformation,
                                this.thePerformLatencyAnalysisCheckBox.Checked,
                                this.theLatencyAnalysisProcessing,
                                this.thePerformBurstAnalysisCheckBox.Checked,
                                this.theBurstAnalysisProcessing,
                                this.thePerformTimeAnalysisCheckBox.Checked,
                                this.theTimeAnalysisProcessing,
                                this.theSelectedPacketCapturePath,
                                this.theUseAlternativeSequenceNumberCheckBox.Checked,
                                this.theMinimizeMemoryUsageCheckBox.Checked);

                        theProgressWindowForm.ProgressBar = 50;

                        theResult = thePCAPNGPackageCaptureProcessing.ProcessPacketCapture();

                        break;
                    }

                case MainWindowFormConstants.PacketCaptureType.PCAP:
                    {
                        theDebugInformation.WriteInformationEvent(
                            "This is a PCAP packet capture");

                        PacketCapture.PCAPPackageCapture.Processing thePCAPPackageCaptureProcessing =
                            new PacketCapture.PCAPPackageCapture.Processing(
                                theProgressWindowForm,
                                theDebugInformation,
                                this.thePerformLatencyAnalysisCheckBox.Checked,
                                this.theLatencyAnalysisProcessing,
                                this.thePerformBurstAnalysisCheckBox.Checked,
                                this.theBurstAnalysisProcessing,
                                this.thePerformTimeAnalysisCheckBox.Checked,
                                this.theTimeAnalysisProcessing,
                                this.theSelectedPacketCapturePath,
                                this.theUseAlternativeSequenceNumberCheckBox.Checked,
                                this.theMinimizeMemoryUsageCheckBox.Checked);

                        theProgressWindowForm.ProgressBar = 50;

                        theResult = thePCAPPackageCaptureProcessing.ProcessPacketCapture();

                        break;
                    }

                case MainWindowFormConstants.PacketCaptureType.NASnifferDOS:
                    {
                        theDebugInformation.WriteInformationEvent(
                            "This is an NA Sniffer (DOS) packet capture");

                        PacketCapture.SnifferPackageCapture.Processing theSnifferPackageCaptureProcessing =
                            new PacketCapture.SnifferPackageCapture.Processing(
                                theProgressWindowForm,
                                theDebugInformation,
                                this.thePerformLatencyAnalysisCheckBox.Checked,
                                this.theLatencyAnalysisProcessing,
                                this.thePerformBurstAnalysisCheckBox.Checked,
                                this.theBurstAnalysisProcessing,
                                this.thePerformTimeAnalysisCheckBox.Checked,
                                this.theTimeAnalysisProcessing,
                                this.theSelectedPacketCapturePath,
                                this.theUseAlternativeSequenceNumberCheckBox.Checked,
                                this.theMinimizeMemoryUsageCheckBox.Checked);

                        theProgressWindowForm.ProgressBar = 50;

                        theResult = theSnifferPackageCaptureProcessing.ProcessPacketCapture();

                        break;
                    }

                default:
                    {
                        theDebugInformation.WriteErrorEvent(
                            "The" +
                            thePacketCaptureFileName +
                            " packet capture is of an unknown or incorrect type!!!");

                        theResult = false;

                        break;
                    }
            }

            return theResult;
        }

        /// <summary>
        /// Performs the actions to analyze the packet capture
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theProgressWindowForm">The progress window form</param>
        /// <param name="thePacketCaptureFileName">The name of the packet capture</param>
        /// <returns>Boolean flag that indicates whether performing the actions to analyze the packet capture was successful</returns>
        private bool PerformAnalysisForPacketCapture(Analysis.DebugInformation theDebugInformation, ProgressWindowForm theProgressWindowForm, string thePacketCaptureFileName)
        {
            bool theResult = true;

            int theScaling = 0;

            if (this.thePerformLatencyAnalysisCheckBox.Checked ||
                this.thePerformTimeAnalysisCheckBox.Checked ||
                this.thePerformBurstAnalysisCheckBox.Checked)
            {
                // Update the label now the analysis of the packet capture has started - the progress bar will stay at zero
                theProgressWindowForm.ProgressBarLabel = "Performing Analysis Of Packet Capture";
                theProgressWindowForm.ProgressBar = 0;
                theProgressWindowForm.Refresh();

                // Calculate the scaling to use for the progress bar
                if (this.thePerformLatencyAnalysisCheckBox.Checked &&
                    !this.thePerformTimeAnalysisCheckBox.Checked)
                {
                    // Only one set of analysis will be run
                    theScaling = 1;
                }
                else if (!this.thePerformLatencyAnalysisCheckBox.Checked &&
                    this.thePerformTimeAnalysisCheckBox.Checked)
                {
                    // Only one set of analysis will be run
                    theScaling = 1;
                }
                else
                {
                    // Both sets of analysis will be run
                    theScaling = 2;
                }
            }

            // Only perform the latency analysis if the check box was selected for it on the main window form
            if (this.thePerformLatencyAnalysisCheckBox.Checked)
            {
                theDebugInformation.WriteBlankLine();

                theDebugInformation.WriteTextLine(
                    new string('=', 144));

                theDebugInformation.WriteBlankLine();

                //// Finalise the latency analysis on the messages found including printing the results to debug output
                //// Only perform this action if the analysis of the packet capture completed successfully

                // Read the start time to allow later calculation of the duration of the latency analysis finalisation
                System.DateTime theLatencyAnalysisStartTime = System.DateTime.Now;

                theDebugInformation.WriteTestRunEvent(
                    "Latency analysis for the " +
                    thePacketCaptureFileName +
                    " packet capture started");

                theProgressWindowForm.ProgressBar += 20 / theScaling;

                this.theLatencyAnalysisProcessing.Finalise();

                theProgressWindowForm.ProgressBar += 60 / theScaling;

                //// Compute the duration between the start and the end times

                System.DateTime theLatencyAnalysisEndTime = System.DateTime.Now;

                System.TimeSpan theLatencyAnalysisDuration =
                    theLatencyAnalysisEndTime - theLatencyAnalysisStartTime;

                theDebugInformation.WriteTestRunEvent(
                    "Latency analysis for the " +
                    thePacketCaptureFileName +
                    " packet capture completed in " +
                    theLatencyAnalysisDuration.TotalSeconds.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " seconds");

                theProgressWindowForm.ProgressBar += 20 / theScaling;
            }

            // Only perform the burst analysis if the check box was selected for it on the main window form
            if (this.thePerformBurstAnalysisCheckBox.Checked)
            {
                theDebugInformation.WriteBlankLine();

                theDebugInformation.WriteTextLine(
                    new string('=', 144));

                theDebugInformation.WriteBlankLine();

                //// Finalise the burst analysis on the messages found including printing the results to debug output
                //// Only perform this action if the analysis of the packet capture completed successfully

                // Read the start time to allow later calculation of the duration of the burst analysis finalisation
                System.DateTime theBurstAnalysisStartTime = System.DateTime.Now;

                theDebugInformation.WriteTestRunEvent(
                    "Burst analysis for the " +
                    thePacketCaptureFileName +
                    " packet capture started");

                //// theProgressWindowForm.ProgressBar += 20 / theScaling;

                this.theBurstAnalysisProcessing.Finalise();

                //// theProgressWindowForm.ProgressBar += 60 / theScaling;

                //// Compute the duration between the start and the end times

                System.DateTime theBurstAnalysisEndTime = System.DateTime.Now;

                System.TimeSpan theBurstAnalysisDuration =
                    theBurstAnalysisEndTime - theBurstAnalysisStartTime;

                theDebugInformation.WriteTestRunEvent(
                    "Burst analysis for the " +
                    thePacketCaptureFileName +
                    " packet capture completed in " +
                    theBurstAnalysisDuration.TotalSeconds.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " seconds");

                //// theProgressWindowForm.ProgressBar += 20 / theScaling;
            }

            // Only perform the time analysis if the check box was selected for it on the main window form
            if (this.thePerformTimeAnalysisCheckBox.Checked)
            {
                theDebugInformation.WriteBlankLine();

                theDebugInformation.WriteTextLine(
                    new string('=', 144));

                theDebugInformation.WriteBlankLine();

                //// Finalise the time analysis on the messages found including printing the results to debug output
                //// Only perform this action if the analysis of the packet capture completed successfully

                // Read the start time to allow later calculation of the duration of the time analysis finalisation
                System.DateTime theTimeAnalysisStartTime = System.DateTime.Now;

                theDebugInformation.WriteTestRunEvent(
                    "Time analysis for the " +
                    thePacketCaptureFileName +
                    " packet capture started");

                theProgressWindowForm.ProgressBar += 20 / theScaling;

                this.theTimeAnalysisProcessing.Finalise();

                theProgressWindowForm.ProgressBar += 60 / theScaling;

                //// Compute the duration between the start and the end times

                System.DateTime theTimeAnalysisEndTime = System.DateTime.Now;

                System.TimeSpan theTimeAnalysisDuration =
                    theTimeAnalysisEndTime - theTimeAnalysisStartTime;

                theDebugInformation.WriteTestRunEvent(
                    "Time analysis for the " +
                    thePacketCaptureFileName +
                    " packet capture completed in " +
                    theTimeAnalysisDuration.TotalSeconds.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " seconds");

                theProgressWindowForm.ProgressBar += 20 / theScaling;
            }

            return theResult;
        }

        //// Button support functions

        /// <summary>
        /// Enables the buttons concerned with packet capture analysis on the main window form
        /// </summary>
        private void EnablePacketCaptureAnalysisButtons()
        {
            //// Enable the buttons

            // 1) Enable the button to clear the selected package capture
            this.theClearSelectedPacketCaptureButton.Enabled = true;

            // 2) Enable the button to open the package capture
            this.theOpenSelectedPackageCaptureButton.Enabled = true;

            // 3) Disable the button to start the analysis on the packet capture - enabled on selection of the output file
            this.theRunAnalysisOnSelectedPackageCaptureButton.Enabled = true;
        }

        /// <summary>
        /// Resets the buttons concerned with packet capture analysis on the main window form
        /// </summary>
        private void ResetPacketCaptureAnalysisButtons()
        {
            //// Reset the buttons

            this.theClearSelectedPacketCaptureButton.Enabled = true;
            this.theOpenSelectedPackageCaptureButton.Enabled = false;
            this.theRunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
        }

        //// Check box support functions

        /// <summary>
        /// Resets and then enables the check boxes (and group boxes) concerned with packet capture analysis on the main window form
        /// </summary>
        /// <param name="sender">The sender for the initiating event</param>
        private void EnablePacketCaptureAnalysisCheckBoxes(object sender)
        {
            //// Enable the check boxes

            // Guard against infinite recursion
            if (sender != this.theEnableDebugInformationCheckBox)
            {
                this.theEnableDebugInformationCheckBox.Checked = true;
                this.theEnableDebugInformationCheckBox.Enabled = true;

                this.theDebugInformationGroupBox.Enabled = true;
            }

            // Match the state of the "Enable Information Events" check box to the state of the "Enable" check box
            this.theEnableInformationEventsInDebugInformationCheckBox.Checked =
                this.theEnableDebugInformationCheckBox.Checked;

            this.theEnableInformationEventsInDebugInformationCheckBox.Enabled = true;

            this.theRedirectDebugInformationToOutputCheckBox.Checked = false;
            this.theRedirectDebugInformationToOutputCheckBox.Enabled = true;

            this.theLatencyAnalysisGroupBox.Enabled = true;

            this.thePerformLatencyAnalysisCheckBox.Checked = false;
            this.thePerformLatencyAnalysisCheckBox.Enabled = true;

            this.theOutputLatencyAnalysisHistogramCheckBox.Checked = false;
            this.theOutputLatencyAnalysisHistogramCheckBox.Enabled = false;

            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Checked = false;
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Enabled = false;

            this.theBurstAnalysisGroupBox.Enabled = true;

            this.thePerformBurstAnalysisCheckBox.Checked = false;
            this.thePerformBurstAnalysisCheckBox.Enabled = true;

            this.theOutputBurstAnalysisHistogramCheckBox.Checked = false;
            this.theOutputBurstAnalysisHistogramCheckBox.Enabled = false;

            this.theOutputAdditionalBurstAnalysisInformationCheckBox.Checked = false;
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.Enabled = false;

            this.theTimeAnalysisGroupBox.Enabled = true;

            this.thePerformTimeAnalysisCheckBox.Checked = false;
            this.thePerformTimeAnalysisCheckBox.Enabled = true;

            this.theOutputTimeAnalysisTimestampHistogramCheckBox.Checked = false;
            this.theOutputTimeAnalysisTimestampHistogramCheckBox.Enabled = false;

            this.theOutputTimeAnalysisTimeHistogramCheckBox.Checked = false;
            this.theOutputTimeAnalysisTimeHistogramCheckBox.Enabled = false;

            this.theOutputAdditionalTimeAnalysisInformationCheckBox.Checked = false;
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.Enabled = false;

            // Do not change the "Minimize Memory Usage" check box on a change to the "Enable" check box in the "Debug Information" group box
            if (sender != this.theEnableDebugInformationCheckBox)
            {
                this.theUseAlternativeSequenceNumberCheckBox.Checked = false;
                this.theUseAlternativeSequenceNumberCheckBox.Enabled = true;

                this.theMinimizeMemoryUsageCheckBox.Checked = false;
                this.theMinimizeMemoryUsageCheckBox.Enabled = true;
            }
        }

        /// <summary>
        /// Resets and then disables the check boxes (and group boxes) concerned with packet capture analysis on the main window form
        /// </summary>
        /// <param name="sender">The sender for the initiating event</param>
        private void DisablePacketCaptureAnalysisCheckBoxes(object sender)
        {
            //// Disable the check boxes

            // Guard against infinite recursion
            if (sender != this.theEnableDebugInformationCheckBox)
            {
                this.theEnableDebugInformationCheckBox.Checked = false;
                this.theEnableDebugInformationCheckBox.Enabled = false;

                this.theDebugInformationGroupBox.Enabled = false;
            }

            // Match the state of the "Enable Information Events" check box to the state of the "Enable" check box
            this.theEnableInformationEventsInDebugInformationCheckBox.Checked =
                this.theEnableDebugInformationCheckBox.Checked;

            this.theEnableInformationEventsInDebugInformationCheckBox.Enabled = false;

            this.theRedirectDebugInformationToOutputCheckBox.Checked = false;
            this.theRedirectDebugInformationToOutputCheckBox.Enabled = false;

            this.theLatencyAnalysisGroupBox.Enabled = false;

            this.thePerformLatencyAnalysisCheckBox.Checked = false;
            this.thePerformLatencyAnalysisCheckBox.Enabled = false;

            this.theOutputLatencyAnalysisHistogramCheckBox.Checked = false;
            this.theOutputLatencyAnalysisHistogramCheckBox.Enabled = false;

            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Checked = false;
            this.theOutputAdditionalLatencyAnalysisInformationCheckBox.Enabled = false;

            this.theBurstAnalysisGroupBox.Enabled = false;

            this.thePerformBurstAnalysisCheckBox.Checked = false;
            this.thePerformBurstAnalysisCheckBox.Enabled = false;

            this.theOutputBurstAnalysisHistogramCheckBox.Checked = false;
            this.theOutputBurstAnalysisHistogramCheckBox.Enabled = false;

            this.theOutputAdditionalBurstAnalysisInformationCheckBox.Checked = false;
            this.theOutputAdditionalBurstAnalysisInformationCheckBox.Enabled = false;

            this.theTimeAnalysisGroupBox.Enabled = false;

            this.thePerformTimeAnalysisCheckBox.Checked = false;
            this.thePerformTimeAnalysisCheckBox.Enabled = false;

            this.theOutputTimeAnalysisTimestampHistogramCheckBox.Checked = false;
            this.theOutputTimeAnalysisTimestampHistogramCheckBox.Enabled = false;

            this.theOutputTimeAnalysisTimeHistogramCheckBox.Checked = false;
            this.theOutputTimeAnalysisTimeHistogramCheckBox.Enabled = false;

            this.theOutputAdditionalTimeAnalysisInformationCheckBox.Checked = false;
            this.theOutputAdditionalTimeAnalysisInformationCheckBox.Enabled = false;

            // Do not change the "Minimize Memory Usage" check box on a change to the "Enable" check box in the "Debug Information" group box
            if (sender != this.theEnableDebugInformationCheckBox)
            {
                this.theUseAlternativeSequenceNumberCheckBox.Checked = false;
                this.theUseAlternativeSequenceNumberCheckBox.Enabled = false;

                this.theMinimizeMemoryUsageCheckBox.Checked = false;
                this.theMinimizeMemoryUsageCheckBox.Enabled = false;
            }
        }
    }
}
