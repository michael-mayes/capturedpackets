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

namespace WiresharkRecordingAnalyser
{
    public partial class MainWindowForm : System.Windows.Forms.Form
    {
        //Enumerated list of the types of recordings supported by the main window form
        private enum MainWindowFormRecordingType
        {
            WiresharkTcpdumpRecording = 0,
            NASnifferDOSRecording = 1,
            UnknownRecording = 2
        }

        private static MainWindowFormRecordingType TheMainWindowFormRecordingType;

        public MainWindowForm()
        {
            InitializeComponent();

            //Clear the selected recording on window form creation
            ClearSelectedRecording();
        }

        private void SelectRecordingForAnalysisButton_Click(object sender, System.EventArgs e)
        {
            //Open the recording selection dialog box
            System.Windows.Forms.DialogResult TheSelectRecordingDialogResult = SelectRecordingForAnalysisDialog.ShowDialog();

            if (TheSelectRecordingDialogResult == System.Windows.Forms.DialogResult.OK)
            {
                //Populate the path and name of the selected recording, not necessarily a Wireshark recording at this stage
                SelectedRecordingPathTextBox.Text = System.IO.Path.GetDirectoryName(SelectRecordingForAnalysisDialog.FileName);
                SelectedRecordingNameTextBox.Text = System.IO.Path.GetFileName(SelectRecordingForAnalysisDialog.FileName);

                //Determine the type of the recording from the extension
                switch(System.IO.Path.GetExtension(SelectRecordingForAnalysisDialog.FileName))
                {
                    case ".pcap":
                    case ".libpcap":
                        {
                            //This is a Wireshark/tcpdump recording
                            TheMainWindowFormRecordingType = MainWindowFormRecordingType.WiresharkTcpdumpRecording;
                            break;
                        }

                    case ".cap":
                    case ".enc":
                        {
                            //This is an NA Sniffer (DOS) recording
                            TheMainWindowFormRecordingType = MainWindowFormRecordingType.NASnifferDOSRecording;
                            break;
                        }

                    default:
                        {
                            //This recording is either an unsupported form of Wireshark recording or is another type of recording
                            TheMainWindowFormRecordingType = MainWindowFormRecordingType.UnknownRecording;
                            break;
                        }
                }

                //Update the window to reflect the selected recording
                ReflectSelectedRecording();

            }
        }

        private void RunAnalysisOnRecordingButton_Click(object sender, System.EventArgs e)
        {
            bool TheResult = true;

            switch (TheMainWindowFormRecordingType)
            {
                case MainWindowFormRecordingType.WiresharkTcpdumpRecording:
                    {
                        RecordingProcessingNamespace.PCAPRecordingProcessing ThePCAPRecordingProcessing = new RecordingProcessingNamespace.PCAPRecordingProcessing();

                        TheResult = ThePCAPRecordingProcessing.ProcessRecording(SelectRecordingForAnalysisDialog.FileName);

                        break;
                    }

                case MainWindowFormRecordingType.NASnifferDOSRecording:
                    {
                        RecordingProcessingNamespace.ENCRecordingProcessing TheENCRecordingProcessing = new RecordingProcessingNamespace.ENCRecordingProcessing();

                        TheResult = TheENCRecordingProcessing.ProcessRecording(SelectRecordingForAnalysisDialog.FileName);

                        break;
                    }

                case MainWindowFormRecordingType.UnknownRecording:
                default:
                    {
                        System.Diagnostics.Debug.WriteLine("Unknown Wireshark recording type!!!");

                        TheResult = false;

                        break;
                    }
            }

            //Dependent on the result of the processing above, display a message box to indicate success or otherwise
            if (TheResult)
            {
                //Display a message box to indicate analysis of the recording is complete
                System.Windows.Forms.MessageBox.Show("Analysis of " + SelectRecordingForAnalysisDialog.FileName + " completed!", "Run Analysis On Wireshark Recording Selected");
            }
            else
            {
                //Display a message box to indicate analysis of the recording failed
                System.Windows.Forms.MessageBox.Show("Analysis of " + SelectRecordingForAnalysisDialog.FileName + " failed!!!", "Run Analysis On Wireshark Recording Selected");
            }

            //Clear the selected recording after analysis
            ClearSelectedRecording();
        }

        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void ClearSelectedRecordingButton_Click(object sender, System.EventArgs e)
        {
            //Clear the selected recording on user request
            ClearSelectedRecording();
        }

        private void ReflectSelectedRecording()
        {
            switch (TheMainWindowFormRecordingType)
            {
                case MainWindowFormRecordingType.WiresharkTcpdumpRecording:
                    {
                        //This is a Wireshark/tcpdump recording
                        SelectedRecordingTypeTextBox.Text = "Wireshark/tcpdump";

                        //Analysis of a Wireshark/tcpdump recording is supported so enable the button to start the analysis
                        RunAnalysisOnRecordingButton.Enabled = true;
                        break;
                    }

                case MainWindowFormRecordingType.NASnifferDOSRecording:
                    {
                        //This is an NA Sniffer (DOS) recording
                        SelectedRecordingTypeTextBox.Text = "NA Sniffer (DOS)";

                        //Analysis of an NA Sniffer (DOS) recording is supported so enable the button to start the analysis
                        RunAnalysisOnRecordingButton.Enabled = true;
                        break;
                    }

                case MainWindowFormRecordingType.UnknownRecording:
                default:
                    {
                        //This recording is either an unsupported form of Wireshark recording or is another type of recording
                        SelectedRecordingTypeTextBox.Text = "<Unknown Wireshark Recording Type>";

                        //Analysis of this recording is not supported so disable the button to start the analysis
                        RunAnalysisOnRecordingButton.Enabled = false;
                        break;
                    }
            }
        }

        private void ClearSelectedRecording()
        {
            TheMainWindowFormRecordingType = MainWindowFormRecordingType.UnknownRecording;

            SelectedRecordingPathTextBox.Text = "<No Wireshark Recording Selected>";
            SelectedRecordingNameTextBox.Text = "<No Wireshark Recording Selected>";
            SelectedRecordingTypeTextBox.Text = "<No Wireshark Recording Selected>";

            RunAnalysisOnRecordingButton.Enabled = false;
        }
    }
}
