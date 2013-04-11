//This is free and unencumbered software released into the public domain.

//Anyone is free to copy, modify, publish, use, compile, sell, or
//distribute this software, either in source code form or as a compiled
//binary, for any purpose, commercial or non-commercial, and by any
//means.

//In jurisdictions that recognize copyright laws, the author or authors
//of this software dedicate any and all copyright interest in the
//software to the public domain. We make this dedication for the benefit
//of the public at large and to the detriment of our heirs and
//successors. We intend this dedication to be an overt act of
//relinquishment in perpetuity of all present and future rights to this
//software under copyright law.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.

//For more information, please refer to <http://unlicense.org/>

namespace PacketCaptureAnalyser
{
    public partial class MainWindowForm : System.Windows.Forms.Form
    {
        //Enumerated list of the types of packet captures supported by the main window form
        private enum MainWindowFormPacketCaptureTypeEnumeration
        {
            LibpcapTcpdump = 0,
            NASnifferDOS = 1,
            Unknown = 2
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

            //Delete any existing output files with the selected name to ape the clearing of all text from the output window
            if (System.IO.File.Exists(SelectedOutputFileForAnalysisDialog.FileName))
            {
                System.IO.File.Delete(SelectedOutputFileForAnalysisDialog.FileName);
            }

            //Remove the output window from the list of listeners to debug output as all text will go to the output file
            System.Diagnostics.Debug.Listeners.Clear();

            //Redirect any text added to the output window to the output file
            using (System.Diagnostics.TextWriterTraceListener TheOutputWindowListener =
                new System.Diagnostics.TextWriterTraceListener(SelectedOutputFileForAnalysisDialog.FileName))
            {
                System.Diagnostics.Trace.Listeners.Add(TheOutputWindowListener);

                //Start the analysis of the packet capture
                System.Diagnostics.Trace.WriteLine
                    (
                    "Analysis of the " +
                    System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
                    " packet capture started"
                    );

                AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing =
                    new AnalysisNamespace.LatencyAnalysisProcessing();

                AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing =
                    new AnalysisNamespace.TimeAnalysisProcessing();

                //Initialise the functionality to perform latency analysis on the messages found
                TheLatencyAnalysisProcessing.Create();

                //Initialise the functionality to perform time analysis on the messages found
                TheTimeAnalysisProcessing.Create();

                switch (TheMainWindowFormPacketCaptureType)
                {
                    case MainWindowFormPacketCaptureTypeEnumeration.LibpcapTcpdump:
                        {
                            PacketCaptureProcessingNamespace.PCAPPackageCaptureProcessing ThePCAPPackageCaptureProcessing =
                                new PacketCaptureProcessingNamespace.PCAPPackageCaptureProcessing();

                            TheResult = ThePCAPPackageCaptureProcessing.Process(TheLatencyAnalysisProcessing, TheTimeAnalysisProcessing, SelectedPacketCaptureForAnalysisDialog.FileName);

                            break;
                        }

                    case MainWindowFormPacketCaptureTypeEnumeration.NASnifferDOS:
                        {
                            PacketCaptureProcessingNamespace.SnifferPackageCaptureProcessing TheSnifferPackageCaptureProcessing =
                                new PacketCaptureProcessingNamespace.SnifferPackageCaptureProcessing();

                            TheResult = TheSnifferPackageCaptureProcessing.Process(TheLatencyAnalysisProcessing, TheTimeAnalysisProcessing, SelectedPacketCaptureForAnalysisDialog.FileName);

                            break;
                        }

                    case MainWindowFormPacketCaptureTypeEnumeration.Unknown:
                    default:
                        {
                            System.Diagnostics.Trace.WriteLine
                                (
                                "The" +
                                System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
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
                        System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
                        " packet capture completed successfully!"
                        );

                    System.Diagnostics.Trace.Write(System.Environment.NewLine);

                    //Finalise the latency analysis on the messages found including printing the results to debug output
                    //Only perform this action if the analysis of the packet capture completed successfully

                    //Read the start time to allow later calculation of the duration of the latency analysis finalisation
                    System.DateTime TheLatencyAnalysisStartTime = System.DateTime.Now;

                    System.Diagnostics.Trace.WriteLine
                        (
                        "Latency analysis for the " +
                        System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
                        " packet capture started"
                        );

                    TheLatencyAnalysisProcessing.Finalise();

                    //Compute the duration between the start and the end times

                    System.DateTime TheLatencyAnalysisEndTime = System.DateTime.Now;

                    System.TimeSpan TheLatencyAnalysisDuration =
                        TheLatencyAnalysisEndTime - TheLatencyAnalysisStartTime;

                    System.Diagnostics.Trace.WriteLine
                        (
                        "Latency analysis for the " +
                        System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
                        " packet capture completed in " +
                        TheLatencyAnalysisDuration.TotalSeconds.ToString() +
                        " seconds"
                        );

                    System.Diagnostics.Trace.Write(System.Environment.NewLine);

                    //Finalise the time analysis on the messages found including printing the results to debug output
                    //Only perform this action if the analysis of the packet capture completed successfully

                    //Read the start time to allow later calculation of the duration of the time analysis finalisation
                    System.DateTime TheTimeAnalysisStartTime = System.DateTime.Now;

                    System.Diagnostics.Trace.WriteLine
                        (
                        "Time analysis for the " +
                        System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
                        " packet capture started"
                        );

                    TheTimeAnalysisProcessing.Finalise();

                    //Compute the duration between the start and the end times

                    System.DateTime TheTimeAnalysisEndTime = System.DateTime.Now;

                    System.TimeSpan TheTimeAnalysisDuration =
                        TheTimeAnalysisEndTime - TheTimeAnalysisStartTime;

                    System.Diagnostics.Trace.WriteLine
                        (
                        "Time analysis for the " +
                        System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
                        " packet capture completed in " +
                        TheTimeAnalysisDuration.TotalSeconds.ToString() +
                        " seconds"
                        );
                }
                else
                {
                    //Display a debug message to indicate analysis of the packet capture failed
                    System.Diagnostics.Trace.WriteLine
                        (
                        "Analysis of the " +
                        System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
                        " packet capture failed!!!"
                        );
                }

                //Flush output to the output file and then close it
                TheOutputWindowListener.Flush();
                TheOutputWindowListener.Close();

                System.Diagnostics.Debug.Listeners.Remove(TheOutputWindowListener);
            }

            //Dependent on the result of the processing above, display a message box to indicate success or otherwise

            System.Windows.Forms.DialogResult TheMessageBoxResult;

            if (TheResult)
            {
                //Display a message box to indicate analysis of the packet capture is complete and ask whether to open the output file
                TheMessageBoxResult =
                    System.Windows.Forms.MessageBox.Show
                    (
                    "Analysis of the " +
                    System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
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
                    System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName) +
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
                if (System.IO.File.Exists(SelectedOutputFileForAnalysisDialog.FileName))
                {
                    System.Diagnostics.Process.Start(SelectedOutputFileForAnalysisDialog.FileName);
                }
            }

            //Update the display after completion of the analysis
            ReflectCompletionOfPacketCaptureAnalysis();
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
                case MainWindowFormPacketCaptureTypeEnumeration.LibpcapTcpdump:
                    {
                        //This is a libpcap/tcpdump packet capture
                        SelectedPacketCaptureTypeTextBox.Text = "libpcap/tcpdump";

                        //Analysis of a libpcap/tcpdump packet capture is supported:
                        //1) Enable the button to clear the selected package capture
                        //2) Enable the button to open the package capture
                        //3) Enable the button to select the output file
                        //4) Disable the button to start the analysis on the packet capture - enabled on selection of the output file
                        ClearSelectedPacketCaptureButton.Enabled = true;
                        OpenSelectedPackageCaptureButton.Enabled = true;
                        SelectOutputFileButton.Enabled = true;
                        RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
                        break;
                    }

                case MainWindowFormPacketCaptureTypeEnumeration.NASnifferDOS:
                    {
                        //This is an NA Sniffer (DOS) packet capture
                        SelectedPacketCaptureTypeTextBox.Text = "NA Sniffer (DOS)";

                        //Analysis of an NA Sniffer (DOS) packet capture is supported:
                        //1) Enable the button to clear the selected package capture
                        //2) Enable the button to open the package capture
                        //3) Enable the button to select the output file
                        //4) Disable the button to start the analysis on the packet capture - enabled on selection of the output file
                        ClearSelectedPacketCaptureButton.Enabled = true;
                        OpenSelectedPackageCaptureButton.Enabled = true;
                        SelectOutputFileButton.Enabled = true;
                        RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
                        break;
                    }

                case MainWindowFormPacketCaptureTypeEnumeration.Unknown:
                default:
                    {
                        //This packet capture is either an unsupported form of packet capture or is another type of file
                        SelectedPacketCaptureTypeTextBox.Text = "<Unknown Packet Capture Type>";

                        //Analysis of this packet capture is not supported:
                        //1) Enable the button to clear the selected package capture
                        //2) Disable the button to open the package capture - not valid for opening
                        //3) Disable the button to select the output file - not needed if no analysis
                        //4) Disable the button to start the analysis on the packet capture - not needed if no analysis
                        ClearSelectedPacketCaptureButton.Enabled = true;
                        OpenSelectedPackageCaptureButton.Enabled = false;
                        SelectOutputFileButton.Enabled = false;
                        RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
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
        }

        private void ReflectSelectedOutputFile()
        {
            switch (TheMainWindowFormPacketCaptureType)
            {
                case MainWindowFormPacketCaptureTypeEnumeration.LibpcapTcpdump:
                case MainWindowFormPacketCaptureTypeEnumeration.NASnifferDOS:
                    {
                        //Analysis of a libpcap/tcpdump packet capture or an NA Sniffer (DOS) packet capture is supported so enable the button to start the analysis on it
                        RunAnalysisOnSelectedPackageCaptureButton.Enabled = true;
                        break;
                    }

                case MainWindowFormPacketCaptureTypeEnumeration.Unknown:
                default:
                    {
                        //Analysis of this packet capture is not supported so disable the button to start the analysis on it
                        RunAnalysisOnSelectedPackageCaptureButton.Enabled = false;
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
        }

        private void OpenSelectedPackageCaptureButton_Click(object sender, System.EventArgs e)
        {
            if (System.IO.File.Exists(SelectedPacketCaptureForAnalysisDialog.FileName))
            {
                System.Diagnostics.Process.Start(SelectedPacketCaptureForAnalysisDialog.FileName);
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
    }
}
