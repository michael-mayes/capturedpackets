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
        private enum MainWindowFormPacketCaptureType
        {
            LibpcapTcpdumpPacketCapture = 0,
            SnifferPacketCapture = 1,
            UnknownPacketCapture = 2
        }

        private static MainWindowFormPacketCaptureType TheMainWindowFormPacketCaptureType;

        public MainWindowForm()
        {
            InitializeComponent();

            //Clear the selected packet capture on window form creation
            ClearSelectedRecording();
        }

        private void SelectRecordingForAnalysisButton_Click(object sender, System.EventArgs e)
        {
            //Open the packet capture selection dialog box
            System.Windows.Forms.DialogResult TheSelectRecordingDialogResult = SelectedPacketCaptureForAnalysisDialog.ShowDialog();

            if (TheSelectRecordingDialogResult == System.Windows.Forms.DialogResult.OK)
            {
                //Populate the path and name of the selected packet capture, not necessarily a packet capture at this stage
                SelectedPacketCapturePathTextBox.Text = System.IO.Path.GetDirectoryName(SelectedPacketCaptureForAnalysisDialog.FileName);
                SelectedPacketCaptureNameTextBox.Text = System.IO.Path.GetFileName(SelectedPacketCaptureForAnalysisDialog.FileName);

                //Determine the type of the packet capture from the extension
                switch(System.IO.Path.GetExtension(SelectedPacketCaptureForAnalysisDialog.FileName))
                {
                    case ".pcap":
                    case ".libpcap":
                        {
                            //This is a libpcap/tcpdump packet capture
                            TheMainWindowFormPacketCaptureType = MainWindowFormPacketCaptureType.LibpcapTcpdumpPacketCapture;
                            break;
                        }

                    case ".cap":
                    case ".enc":
                        {
                            //This is an NA Sniffer (DOS) packet capture
                            TheMainWindowFormPacketCaptureType = MainWindowFormPacketCaptureType.SnifferPacketCapture;
                            break;
                        }

                    default:
                        {
                            //This packet capture is either an unsupported form of packet capture or is another type of file
                            TheMainWindowFormPacketCaptureType = MainWindowFormPacketCaptureType.UnknownPacketCapture;
                            break;
                        }
                }

                //Update the window to reflect the selected packet capture
                ReflectSelectedRecording();

            }
        }

        private void RunAnalysisOnRecordingButton_Click(object sender, System.EventArgs e)
        {
            bool TheResult = true;

            switch (TheMainWindowFormPacketCaptureType)
            {
                case MainWindowFormPacketCaptureType.LibpcapTcpdumpPacketCapture:
                    {
                        PacketCaptureProcessingNamespace.PCAPPackageCaptureProcessing ThePCAPPackageCaptureProcessing = new PacketCaptureProcessingNamespace.PCAPPackageCaptureProcessing();

                        TheResult = ThePCAPPackageCaptureProcessing.ProcessRecording(SelectedPacketCaptureForAnalysisDialog.FileName);

                        break;
                    }

                case MainWindowFormPacketCaptureType.SnifferPacketCapture:
                    {
                        PacketCaptureProcessingNamespace.SnifferPackageCaptureProcessing TheSnifferPackageCaptureProcessing = new PacketCaptureProcessingNamespace.SnifferPackageCaptureProcessing();

                        TheResult = TheSnifferPackageCaptureProcessing.ProcessRecording(SelectedPacketCaptureForAnalysisDialog.FileName);

                        break;
                    }

                case MainWindowFormPacketCaptureType.UnknownPacketCapture:
                default:
                    {
                        System.Diagnostics.Debug.WriteLine("Unknown packet capture type!!!");

                        TheResult = false;

                        break;
                    }
            }

            //Dependent on the result of the processing above, display a message box to indicate success or otherwise
            if (TheResult)
            {
                //Display a message box to indicate analysis of the packet capture is complete
                System.Windows.Forms.MessageBox.Show("Analysis of " + SelectedPacketCaptureForAnalysisDialog.FileName + " completed!", "Run Analysis On Selected Packet Capture");
            }
            else
            {
                //Display a message box to indicate analysis of the packet capture failed
                System.Windows.Forms.MessageBox.Show("Analysis of " + SelectedPacketCaptureForAnalysisDialog.FileName + " failed!!!", "Run Analysis On Selected Packet Capture");
            }

            //Clear the selected packet capture after analysis
            ClearSelectedRecording();
        }

        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void ClearSelectedRecordingButton_Click(object sender, System.EventArgs e)
        {
            //Clear the selected packet capture on user request
            ClearSelectedRecording();
        }

        private void ReflectSelectedRecording()
        {
            switch (TheMainWindowFormPacketCaptureType)
            {
                case MainWindowFormPacketCaptureType.LibpcapTcpdumpPacketCapture:
                    {
                        //This is a libpcap/tcpdump packet capture
                        SelectedPacketCaptureTypeTextBox.Text = "libpcap/tcpdump";

                        //Analysis of a libpcap/tcpdump packet capture is supported so enable the button to open the package capture or start the analysis on it
                        OpenSelectedPackageCaptureButton.Enabled = true;
                        RunAnalysisOnPackageSelectedCaptureButton.Enabled = true;
                        break;
                    }

                case MainWindowFormPacketCaptureType.SnifferPacketCapture:
                    {
                        //This is an NA Sniffer (DOS) packet capture
                        SelectedPacketCaptureTypeTextBox.Text = "NA Sniffer (DOS)";

                        //Analysis of an NA Sniffer (DOS) packet capture is supported so enable the button to open the package capture or start the analysis on it
                        OpenSelectedPackageCaptureButton.Enabled = true;
                        RunAnalysisOnPackageSelectedCaptureButton.Enabled = true;
                        break;
                    }

                case MainWindowFormPacketCaptureType.UnknownPacketCapture:
                default:
                    {
                        //This packet capture is either an unsupported form of packet capture or is another type of file
                        SelectedPacketCaptureTypeTextBox.Text = "<Unknown Packet Capture Type>";

                        //Analysis of this packet capture is not supported so disable the button to open the package capture or start the analysis on it
                        OpenSelectedPackageCaptureButton.Enabled = false;
                        RunAnalysisOnPackageSelectedCaptureButton.Enabled = false;
                        break;
                    }
            }
        }

        private void ClearSelectedRecording()
        {
            TheMainWindowFormPacketCaptureType = MainWindowFormPacketCaptureType.UnknownPacketCapture;

            SelectedPacketCapturePathTextBox.Text = "<No Packet Capture Selected>";
            SelectedPacketCaptureNameTextBox.Text = "<No Packet Capture Selected>";
            SelectedPacketCaptureTypeTextBox.Text = "<No Packet Capture Selected>";

            OpenSelectedPackageCaptureButton.Enabled = false;
            RunAnalysisOnPackageSelectedCaptureButton.Enabled = false;
        }

        private void OpenSelectedPackageCaptureButton_Click(object sender, System.EventArgs e)
        {
            if (System.IO.File.Exists(SelectedPacketCaptureForAnalysisDialog.FileName))
            {
                System.Diagnostics.Process.Start(SelectedPacketCaptureForAnalysisDialog.FileName);
            }
        }
    }
}
