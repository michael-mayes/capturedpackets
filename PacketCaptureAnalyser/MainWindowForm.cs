﻿//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser
{
    public partial class MainWindowForm : System.Windows.Forms.Form
    {
        //Enumerated list of the types of packet captures supported by the main window form
        private enum MainWindowFormPacketCaptureTypeEnumeration
        {
            PcapNG = 0,
            LibpcapTcpdump = 1,
            NASnifferDOS = 2,
            Unknown = 3
        }

        private static MainWindowFormPacketCaptureTypeEnumeration TheMainWindowFormPacketCaptureType;

        public MainWindowForm()
        {
            InitializeComponent();

            //Clear the selected packet capture on window form creation
            ClearSelectedPacketCapture();
        }

        private void SelectPacketCaptureForAnalysisButton_Click(object sender, System.EventArgs e)
        {
            //Open the packet capture selection dialog box
            System.Windows.Forms.DialogResult TheSelectedPacketCaptureDialogResult =
                SelectedPacketCaptureForAnalysisDialog.ShowDialog();

            if (TheSelectedPacketCaptureDialogResult == System.Windows.Forms.DialogResult.OK)
            {
                //Populate the path and name of the selected packet capture, not necessarily a packet capture at this stage
                SelectedPacketCapturePathTextBox.Text =
                    System.IO.Path.GetDirectoryName(SelectedPacketCaptureForAnalysisDialog.FileName);

                SelectedPacketCaptureNameTextBox.Text =
                    System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName);

                //Determine the type of the packet capture from the extension
                switch (System.IO.Path.GetExtension(SelectedPacketCaptureForAnalysisDialog.FileName))
                {
                    case ".pcapng":
                        {
                            //This is a PCAP Next Generation capture
                            TheMainWindowFormPacketCaptureType =
                                MainWindowFormPacketCaptureTypeEnumeration.PcapNG;
                            break;
                        }

                    case ".pcap":
                    case ".libpcap":
                    case ".cap":
                        {
                            //This is a libpcap/tcpdump packet capture
                            TheMainWindowFormPacketCaptureType =
                                MainWindowFormPacketCaptureTypeEnumeration.LibpcapTcpdump;
                            break;
                        }

                    case ".enc":
                        {
                            //This is an NA Sniffer (DOS) packet capture
                            TheMainWindowFormPacketCaptureType =
                                MainWindowFormPacketCaptureTypeEnumeration.NASnifferDOS;
                            break;
                        }

                    default:
                        {
                            //This packet capture is either an unsupported form of packet capture or is another type of file
                            TheMainWindowFormPacketCaptureType =
                                MainWindowFormPacketCaptureTypeEnumeration.Unknown;
                            break;
                        }
                }

                //Update the window to reflect the selected packet capture
                ReflectSelectedPacketCapture();

                System.Diagnostics.Trace.WriteLine
                    (
                    "Selection of the " +
                    System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
                    " packet capture"
                    );
            }
        }

        private void RunAnalysisOnPacketCaptureButton_Click(object sender, System.EventArgs e)
        {
            bool TheResult = true;

            //Create an instance of the progress window form
            var TheProgressWindowForm = new PacketCaptureAnalyser.ProgressWindowForm();

            //Show the progress window form now the analysis has started
            TheProgressWindowForm.Show();
            TheProgressWindowForm.Activate();

            //Update the label now the reading of the packet capture has started - the progress bar will stay at zero
            TheProgressWindowForm.AnalysingPacketCaptureLabel.Text = "Reading Packet Capture Into System Memory";
            TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 0;
            TheProgressWindowForm.Refresh();

            string TheOutputFilePath = SelectedOutputFileForAnalysisDialog.FileName;
            string TheOutputFileName = System.IO.Path.GetFileName(TheOutputFilePath);

            TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 5;

            string ThePacketCaptureFilePath = SelectedPacketCaptureForAnalysisDialog.FileName;
            string ThePacketCaptureFileName = System.IO.Path.GetFileName(ThePacketCaptureFilePath);

            TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 10;

            //Delete any existing output files with the selected name to ape the clearing of all text from the output window
            if (System.IO.File.Exists(TheOutputFilePath))
            {
                System.IO.File.Delete(TheOutputFilePath);
            }

            TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 15;

            //Unless instructed otherwise, remove the output window from the list of listeners to debug output as all text will go to the output file
            if (!OutputDebugToOutputWindowCheckBox.Checked)
            {
                System.Diagnostics.Debug.Listeners.Clear();
            }

            TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 20;

            //Redirect any text added to the output window to the output file
            using (System.Diagnostics.TextWriterTraceListener TheOutputWindowListener =
                new System.Diagnostics.TextWriterTraceListener(TheOutputFilePath))
            {
                TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 25;

                System.Diagnostics.Trace.Listeners.Add(TheOutputWindowListener);

                TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 30;

                //Start the analysis of the packet capture
                System.Diagnostics.Trace.WriteLine
                    (
                    "Analysis of the " +
                    ThePacketCaptureFileName +
                    " packet capture started"
                    );

                AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing = null;
                AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing = null;

                TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 35;

                //Only perform the latency analysis if the check box was selected for it on the main window form
                if (PerformLatencyAnalysisCheckBox.Checked)
                {
                    TheLatencyAnalysisProcessing = new AnalysisNamespace.LatencyAnalysisProcessing(OutputLatencyAnalysisDebugCheckBox.Checked);

                    //Initialise the functionality to perform latency analysis on the messages found
                    TheLatencyAnalysisProcessing.Create();
                }

                TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 40;

                //Only perform the time analysis if the check box was selected for it on the main window form
                if (PerformTimeAnalysisCheckBox.Checked)
                {
                    TheTimeAnalysisProcessing = new AnalysisNamespace.TimeAnalysisProcessing(OutputTimeAnalysisDebugCheckBox.Checked);

                    //Initialise the functionality to perform time analysis on the messages found
                    TheTimeAnalysisProcessing.Create();
                }

                TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 45;

                switch (TheMainWindowFormPacketCaptureType)
                {
                    case MainWindowFormPacketCaptureTypeEnumeration.PcapNG:
                        {
                            PacketCaptureProcessingNamespace.PCAPNGPackageCaptureProcessing ThePCAPNGPackageCaptureProcessing =
                                new PacketCaptureProcessingNamespace.PCAPNGPackageCaptureProcessing();

                            TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 50;

                            TheResult = ThePCAPNGPackageCaptureProcessing.Process(TheProgressWindowForm,
                                                                                  PerformLatencyAnalysisCheckBox.Checked,
                                                                                  TheLatencyAnalysisProcessing,
                                                                                  PerformTimeAnalysisCheckBox.Checked,
                                                                                  TheTimeAnalysisProcessing,
                                                                                  SelectedPacketCaptureForAnalysisDialog.FileName,
                                                                                  MinimiseMemoryUsageCheckBox.Checked);

                            break;
                        }

                    case MainWindowFormPacketCaptureTypeEnumeration.LibpcapTcpdump:
                        {
                            PacketCaptureProcessingNamespace.PCAPPackageCaptureProcessing ThePCAPPackageCaptureProcessing =
                                new PacketCaptureProcessingNamespace.PCAPPackageCaptureProcessing();

                            TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 50;

                            TheResult = ThePCAPPackageCaptureProcessing.Process(TheProgressWindowForm,
                                                                                PerformLatencyAnalysisCheckBox.Checked,
                                                                                TheLatencyAnalysisProcessing,
                                                                                PerformTimeAnalysisCheckBox.Checked,
                                                                                TheTimeAnalysisProcessing,
                                                                                SelectedPacketCaptureForAnalysisDialog.FileName,
                                                                                MinimiseMemoryUsageCheckBox.Checked);

                            break;
                        }

                    case MainWindowFormPacketCaptureTypeEnumeration.NASnifferDOS:
                        {
                            PacketCaptureProcessingNamespace.SnifferPackageCaptureProcessing TheSnifferPackageCaptureProcessing =
                                new PacketCaptureProcessingNamespace.SnifferPackageCaptureProcessing();

                            TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 50;

                            TheResult = TheSnifferPackageCaptureProcessing.Process(TheProgressWindowForm,
                                                                                   PerformLatencyAnalysisCheckBox.Checked,
                                                                                   TheLatencyAnalysisProcessing,
                                                                                   PerformTimeAnalysisCheckBox.Checked,
                                                                                   TheTimeAnalysisProcessing,
                                                                                   SelectedPacketCaptureForAnalysisDialog.FileName,
                                                                                   MinimiseMemoryUsageCheckBox.Checked);

                            break;
                        }

                    case MainWindowFormPacketCaptureTypeEnumeration.Unknown:
                    default:
                        {
                            System.Diagnostics.Trace.WriteLine
                                (
                                "The" +
                                ThePacketCaptureFileName +
                                " packet capture is of an unknown type!!!"
                                );

                            TheResult = false;

                            break;
                        }
                }

                //Dependent on the result of the processing above, display a debug message to indicate success or otherwise
                if (TheResult)
                {
                    //Display a debug message to indicate analysis of the packet capture completed successfully
                    System.Diagnostics.Trace.WriteLine
                        (
                        "Analysis of the " +
                        ThePacketCaptureFileName +
                        " packet capture completed successfully!"
                        );

                    int Scaling = 0;

                    if (PerformLatencyAnalysisCheckBox.Checked || PerformTimeAnalysisCheckBox.Checked)
                    {
                        //Update the label now the analysis of the packet capture has started - the progress bar will stay at zero
                        TheProgressWindowForm.AnalysingPacketCaptureLabel.Text = "Performing Analysis Of Packet Capture";
                        TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value = 0;
                        TheProgressWindowForm.Refresh();

                        //Calculate the scaling to use for the progress bar
                        if (PerformLatencyAnalysisCheckBox.Checked & !PerformTimeAnalysisCheckBox.Checked)
                        {
                            //Only one set of analysis will be run
                            Scaling = 1;
                        }
                        else if (!PerformLatencyAnalysisCheckBox.Checked && PerformTimeAnalysisCheckBox.Checked)
                        {
                            //Only one set of analysis will be run
                            Scaling = 1;
                        }

                        else
                        {
                            //Both sets of analysis will be run
                            Scaling = 2;
                        }
                    }

                    //Only perform the latency analysis if the check box was selected for it on the main window form
                    if (PerformLatencyAnalysisCheckBox.Checked)
                    {
                        System.Diagnostics.Trace.Write(System.Environment.NewLine);

                        //Finalise the latency analysis on the messages found including printing the results to debug output
                        //Only perform this action if the analysis of the packet capture completed successfully

                        //Read the start time to allow later calculation of the duration of the latency analysis finalisation
                        System.DateTime TheLatencyAnalysisStartTime = System.DateTime.Now;

                        System.Diagnostics.Trace.WriteLine
                            (
                            "Latency analysis for the " +
                            ThePacketCaptureFileName +
                            " packet capture started"
                            );

                        TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value += (20 / Scaling);

                        TheLatencyAnalysisProcessing.Finalise();

                        TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value += (60 / Scaling);

                        //Compute the duration between the start and the end times

                        System.DateTime TheLatencyAnalysisEndTime = System.DateTime.Now;

                        System.TimeSpan TheLatencyAnalysisDuration =
                            TheLatencyAnalysisEndTime - TheLatencyAnalysisStartTime;

                        System.Diagnostics.Trace.WriteLine
                            (
                            "Latency analysis for the " +
                            ThePacketCaptureFileName +
                            " packet capture completed in " +
                            TheLatencyAnalysisDuration.TotalSeconds.ToString() +
                            " seconds"
                            );

                        TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value += (20 / Scaling);
                    }

                    //Only perform the time analysis if the check box was selected for it on the main window form
                    if (PerformTimeAnalysisCheckBox.Checked)
                    {
                        System.Diagnostics.Trace.Write(System.Environment.NewLine);

                        //Finalise the time analysis on the messages found including printing the results to debug output
                        //Only perform this action if the analysis of the packet capture completed successfully

                        //Read the start time to allow later calculation of the duration of the time analysis finalisation
                        System.DateTime TheTimeAnalysisStartTime = System.DateTime.Now;

                        System.Diagnostics.Trace.WriteLine
                            (
                            "Time analysis for the " +
                            ThePacketCaptureFileName +
                            " packet capture started"
                            );

                        TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value += (20 / Scaling);

                        TheTimeAnalysisProcessing.Finalise();

                        TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value += (60 / Scaling);

                        //Compute the duration between the start and the end times

                        System.DateTime TheTimeAnalysisEndTime = System.DateTime.Now;

                        System.TimeSpan TheTimeAnalysisDuration =
                            TheTimeAnalysisEndTime - TheTimeAnalysisStartTime;

                        System.Diagnostics.Trace.WriteLine
                            (
                            "Time analysis for the " +
                            ThePacketCaptureFileName +
                            " packet capture completed in " +
                            TheTimeAnalysisDuration.TotalSeconds.ToString() +
                            " seconds"
                            );

                        TheProgressWindowForm.AnalysingPacketCaptureProgressBar.Value += (20 / Scaling);
                    }
                }
                else
                {
                    //Display a debug message to indicate analysis of the packet capture failed
                    System.Diagnostics.Trace.WriteLine
                        (
                        "Analysis of the " +
                        ThePacketCaptureFileName +
                        " packet capture failed!!!"
                        );
                }

                //Flush output to the output file and then close it
                TheOutputWindowListener.Flush();
                TheOutputWindowListener.Close();

                System.Diagnostics.Debug.Listeners.Remove(TheOutputWindowListener);
            }

            if (TheProgressWindowForm != null)
            {
                //Hide and close the progress window form now the analysis has completed
                TheProgressWindowForm.Hide();
                TheProgressWindowForm.Close();
            }

            //Update the display after completion of the analysis
            ReflectCompletionOfPacketCaptureAnalysis();

            //Dependent on the result of the processing above, display a message box to indicate success or otherwise

            System.Windows.Forms.DialogResult TheMessageBoxResult;

            if (TheResult)
            {
                //Display a message box to indicate analysis of the packet capture is complete and ask whether to open the output file
                TheMessageBoxResult =
                    System.Windows.Forms.MessageBox.Show
                    (
                    "Analysis of the " +
                    ThePacketCaptureFileName +
                    " packet capture completed successfully!" +
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    "Do you want to open the output file?",
                    "Run Analysis On Selected Packet Capture",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question
                    );
            }
            else
            {
                //Display a message box to indicate analysis of the packet capture failed and ask whether to open the output file
                TheMessageBoxResult =
                    System.Windows.Forms.MessageBox.Show
                    (
                    "Analysis of the " +
                    ThePacketCaptureFileName +
                    " packet capture failed!!!" +
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    "Do you want to open the output file?",
                    "Run Analysis On Selected Packet Capture",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Error
                    );
            }

            //Dependent on the button selection at the message box, open the output file
            if (TheMessageBoxResult == System.Windows.Forms.DialogResult.Yes)
            {
                if (System.IO.File.Exists(TheOutputFilePath))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(TheOutputFilePath);
                    }

                    catch (System.ComponentModel.Win32Exception f)
                    {
                        System.Diagnostics.Trace.WriteLine
                            (
                            "The exception " +
                            f.GetType().Name +
                            " with the following message: " +
                            f.Message +
                            " was raised as there is no application registered that can open the " +
                            System.IO.Path.GetFileName(TheOutputFilePath) +
                            " output file!!!"
                            );
                    }
                }
            }
        }

        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void ClearSelectedPacketCaptureButton_Click(object sender, System.EventArgs e)
        {
            //Clear the selected packet capture on user request
            ClearSelectedPacketCapture();
        }

        private void ReflectSelectedPacketCapture()
        {
            switch (TheMainWindowFormPacketCaptureType)
            {
                case MainWindowFormPacketCaptureTypeEnumeration.PcapNG:
                    {
                        //This is a PCAP Next Generation packet capture
                        SelectedPacketCaptureTypeTextBox.Text = "PCAP Next Generation";

                        //Analysis of a PCAP Next Generation packet capture is supported

                        //Enable the buttons
                        EnablePacketCaptureAnalysisButtons();

                        //Reset and disable the check boxes
                        ResetPacketCaptureAnalysisCheckBoxes();
                        DisablePacketCaptureAnalysisCheckBoxes();

                        break;
                    }

                case MainWindowFormPacketCaptureTypeEnumeration.LibpcapTcpdump:
                    {
                        //This is a libpcap/tcpdump packet capture
                        SelectedPacketCaptureTypeTextBox.Text = "libpcap/tcpdump";

                        //Analysis of a libpcap/tcpdump packet capture is supported

                        //Enable the buttons
                        EnablePacketCaptureAnalysisButtons();

                        //Reset and disable the check boxes
                        ResetPacketCaptureAnalysisCheckBoxes();
                        DisablePacketCaptureAnalysisCheckBoxes();

                        break;
                    }

                case MainWindowFormPacketCaptureTypeEnumeration.NASnifferDOS:
                    {
                        //This is an NA Sniffer (DOS) packet capture
                        SelectedPacketCaptureTypeTextBox.Text = "NA Sniffer (DOS)";

                        //Analysis of an NA Sniffer (DOS) packet capture is supported

                        //Enable the buttons
                        EnablePacketCaptureAnalysisButtons();

                        //Reset and disable the check boxes
                        ResetPacketCaptureAnalysisCheckBoxes();
                        DisablePacketCaptureAnalysisCheckBoxes();

                        break;
                    }

                case MainWindowFormPacketCaptureTypeEnumeration.Unknown:
                default:
                    {
                        //This packet capture is either an unsupported form of packet capture or is another type of file
                        SelectedPacketCaptureTypeTextBox.Text = "<Unknown Packet Capture Type>";

                        //Analysis of this packet capture is not supported

                        //Reset the buttons
                        ResetPacketCaptureAnalysisButtons();

                        //Reset and disable the check boxes
                        ResetPacketCaptureAnalysisCheckBoxes();
                        DisablePacketCaptureAnalysisCheckBoxes();

                        break;
                    }
            }
        }

        private void ClearSelectedPacketCapture()
        {
            TheMainWindowFormPacketCaptureType =
                MainWindowFormPacketCaptureTypeEnumeration.Unknown;

            SelectedPacketCapturePathTextBox.Text = "<No Packet Capture Selected>";
            SelectedPacketCaptureNameTextBox.Text = "<No Packet Capture Selected>";
            SelectedPacketCaptureTypeTextBox.Text = "<No Packet Capture Selected>";

            SelectedOutputFilePathTextBox.Text = "<No Output File Selected>";
            SelectedOutputFileNameTextBox.Text = "<No Output File Selected>";

            //1) Disable the button to clear the selected package capture
            //2) Disable the button to open the package capture
            //3) Disable the button to select the output file
            //4) Disable the button to start the analysis on the packet capture
            ClearSelectedPacketCaptureButton.Enabled = false;
            OpenSelectedPackageCaptureButton.Enabled = false;
            SelectOutputFileButton.Enabled = false;
            RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;

            //5) Reset and disable the check boxes
            ResetPacketCaptureAnalysisCheckBoxes();
            DisablePacketCaptureAnalysisCheckBoxes();
        }

        private void ReflectSelectedOutputFile()
        {
            switch (TheMainWindowFormPacketCaptureType)
            {
                case MainWindowFormPacketCaptureTypeEnumeration.PcapNG:
                case MainWindowFormPacketCaptureTypeEnumeration.LibpcapTcpdump:
                case MainWindowFormPacketCaptureTypeEnumeration.NASnifferDOS:
                    {
                        //Analysis of a PCAP Next Generation, libpcap/tcpdump or NA Sniffer (DOS) packet capture is supported so enable the button to start the analysis on it
                        RunAnalysisOnSelectedPackageCaptureButton.Enabled = true;

                        //Reset and enable the check boxes
                        ResetPacketCaptureAnalysisCheckBoxes();
                        EnablePacketCaptureAnalysisCheckBoxes();

                        break;
                    }

                case MainWindowFormPacketCaptureTypeEnumeration.Unknown:
                default:
                    {
                        //Analysis of this packet capture is not supported so disable the button to start the analysis on it
                        RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;

                        //Reset and disable the check boxes
                        ResetPacketCaptureAnalysisCheckBoxes();
                        DisablePacketCaptureAnalysisCheckBoxes();

                        break;
                    }
            }
        }

        private void ReflectCompletionOfPacketCaptureAnalysis()
        {
            SelectedOutputFilePathTextBox.Text = "<No Output File Selected>";
            SelectedOutputFileNameTextBox.Text = "<No Output File Selected>";

            //Disable the button to start the analysis on the packet capture
            RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;

            //Reset and disable the check boxes
            ResetPacketCaptureAnalysisCheckBoxes();
            DisablePacketCaptureAnalysisCheckBoxes();
        }

        private void EnablePacketCaptureAnalysisButtons()
        {
            //Enable the buttons

            //1) Enable the button to clear the selected package capture
            //2) Enable the button to open the package capture
            //3) Enable the button to select the output file
            //4) Disable the button to start the analysis on the packet capture - enabled on selection of the output file

            ClearSelectedPacketCaptureButton.Enabled = true;
            OpenSelectedPackageCaptureButton.Enabled = true;
            SelectOutputFileButton.Enabled = true;
            RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
        }

        private void ResetPacketCaptureAnalysisButtons()
        {
            //Reset the buttons
            ClearSelectedPacketCaptureButton.Enabled = true;
            OpenSelectedPackageCaptureButton.Enabled = false;
            SelectOutputFileButton.Enabled = false;
            RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
        }

        private void ResetPacketCaptureAnalysisCheckBoxes()
        {
            //Reset the check boxes
            PerformLatencyAnalysisCheckBox.Checked = true;
            OutputLatencyAnalysisDebugCheckBox.Checked = false;
            PerformTimeAnalysisCheckBox.Checked = false;
            OutputTimeAnalysisDebugCheckBox.Checked = false;
            MinimiseMemoryUsageCheckBox.Checked = false;
            OutputDebugToOutputWindowCheckBox.Checked = false;
        }

        private void EnablePacketCaptureAnalysisCheckBoxes()
        {
            //Enable the check boxes
            PerformLatencyAnalysisCheckBox.Enabled = true;
            OutputLatencyAnalysisDebugCheckBox.Enabled = true;
            PerformTimeAnalysisCheckBox.Enabled = true;
            OutputTimeAnalysisDebugCheckBox.Enabled = true;
            MinimiseMemoryUsageCheckBox.Enabled = true;
            OutputDebugToOutputWindowCheckBox.Enabled = true;
        }

        private void DisablePacketCaptureAnalysisCheckBoxes()
        {
            //Disable the check boxes
            PerformLatencyAnalysisCheckBox.Enabled = false;
            OutputLatencyAnalysisDebugCheckBox.Enabled = false;
            PerformTimeAnalysisCheckBox.Enabled = false;
            OutputTimeAnalysisDebugCheckBox.Enabled = false;
            MinimiseMemoryUsageCheckBox.Enabled = false;
            OutputDebugToOutputWindowCheckBox.Enabled = false;
        }

        private void OpenSelectedPackageCaptureButton_Click(object sender, System.EventArgs e)
        {
            if (System.IO.File.Exists(SelectedPacketCaptureForAnalysisDialog.FileName))
            {
                try
                {
                    System.Diagnostics.Process.Start(SelectedPacketCaptureForAnalysisDialog.FileName);
                }

                catch (System.ComponentModel.Win32Exception f)
                {
                    System.Diagnostics.Trace.WriteLine
                        (
                        "The exception " +
                        f.GetType().Name +
                        " with the following message: " +
                        f.Message +
                        " was raised as there is no application registered that can open the " +
                        System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
                        " packet capture!!!"
                        );
                }
            }
        }

        private void SelectOuputFileButton_Click(object sender, System.EventArgs e)
        {
            //Open the packet capture selection dialog box - set the name to that of the packet capture, but without the file extension

            SelectedOutputFileForAnalysisDialog.FileName =
                SelectedPacketCaptureForAnalysisDialog.FileName.ToString() + ".txt";

            System.Windows.Forms.DialogResult TheSelectOutputFileDialogResult =
                SelectedOutputFileForAnalysisDialog.ShowDialog();

            if (TheSelectOutputFileDialogResult == System.Windows.Forms.DialogResult.OK)
            {
                //Populate the path and name of the selected output file
                SelectedOutputFilePathTextBox.Text =
                    System.IO.Path.GetDirectoryName(SelectedOutputFileForAnalysisDialog.FileName);

                SelectedOutputFileNameTextBox.Text =
                    System.IO.Path.GetFileName(SelectedOutputFileForAnalysisDialog.FileName);

                //Update the window to reflect the selected output file
                ReflectSelectedOutputFile();

                System.Diagnostics.Trace.WriteLine
                    (
                    "Selection of the " +
                    System.IO.Path.GetFileName(SelectedOutputFileForAnalysisDialog.FileName) +
                    " output file"
                    );
            }
        }

        private void OutputLatencyAnalysisDebugCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (OutputLatencyAnalysisDebugCheckBox.Checked)
            {
                PerformLatencyAnalysisCheckBox.Checked = true;
            }
        }

        private void OutputTimeAnalysisDebugCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (OutputTimeAnalysisDebugCheckBox.Checked)
            {
                PerformTimeAnalysisCheckBox.Checked = true;
            }
        }
    }
}
