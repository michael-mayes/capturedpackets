//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace Analysis
{
    //This class will implement the Disposable class so as to be able to clean up after the datatables it creates which themselves implement the Disposable class
    class DebugInformation : System.IDisposable
    {
        private System.Diagnostics.TextWriterTraceListener TheOutputWindowListener;

        private string TheOutputFilePath;

        private bool EnableInformationEvents;

        public DebugInformation(string TheOutputFilePath, bool EnableInformationEvents, bool RedirectDebugInformationToOutput)
        {
            //Delete any existing output files with the selected name to ape the clearing of all text from the output window
            if (System.IO.File.Exists(TheOutputFilePath))
            {
                //Reset the attributes of the existing output file to ensure that it can be deleted
                System.IO.File.SetAttributes(TheOutputFilePath, System.IO.FileAttributes.Normal);

                //Then delete the existing output file
                System.IO.File.Delete(TheOutputFilePath);
            }

            TheOutputWindowListener =
                new System.Diagnostics.TextWriterTraceListener(TheOutputFilePath);

            this.TheOutputFilePath = TheOutputFilePath;

            this.EnableInformationEvents = EnableInformationEvents;

            //Unless instructed otherwise, remove the output window from the list of listeners to debug output as all text will go to the output file
            if (!RedirectDebugInformationToOutput)
            {
                System.Diagnostics.Debug.Listeners.Clear();
            }

            //Redirect any text added to the output window to the output file
            System.Diagnostics.Trace.Listeners.Add(TheOutputWindowListener);
        }

        public virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                //Flush output to the output file and then close it
                TheOutputWindowListener.Flush();
                TheOutputWindowListener.Close();

                System.Diagnostics.Debug.Listeners.Remove(TheOutputWindowListener);

                //Dispose any resources allocated to the trace listener if instructed
                TheOutputWindowListener.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        public void WriteTestRunEvent(string TheTestRunEvent)
        {
            System.Diagnostics.Trace.WriteLine
                (
                "Test:  " +
                TheTestRunEvent
                );
        }

        public void WriteInformationEvent(string TheInformationEvent)
        {
            if (EnableInformationEvents)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "Info:  " +
                    TheInformationEvent
                    );
            }
        }

        public void WriteErrorEvent(string TheErrorEvent)
        {
            System.Diagnostics.Trace.WriteLine
                (
                "Error: " +
                TheErrorEvent
                );
        }

        public void WriteTextString(string TheTextString)
        {
            System.Diagnostics.Trace.WriteLine
                (
                TheTextString
                );
        }

        public void WriteBlankLine()
        {
            System.Diagnostics.Trace.Write
                (
                System.Environment.NewLine
                );
        }

        public void Open()
        {
            //Flush output to the output file and then close it
            TheOutputWindowListener.Flush();

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
                        "Error: " +
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
}
