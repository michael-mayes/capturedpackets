// $Id$
// $URL$
// <copyright file="DebugInformation.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace Analysis
{
    // This class will implement the Disposable class so as to be able to clean up after the datatables it creates which themselves implement the Disposable class
    class DebugInformation : System.IDisposable
    {
        private System.Diagnostics.TextWriterTraceListener theOutputWindowListener;

        private string theOutputFilePath;

        private bool enableDebugInformation;

        private bool enableInformationEvents;

        public DebugInformation(string theOutputFilePath, bool enableDebugInformation, bool enableInformationEvents, bool redirectDebugInformationToOutput)
        {
            // Delete any existing output files with the selected name to ape the clearing of all text from the output window
            if (System.IO.File.Exists(theOutputFilePath))
            {
                // Reset the attributes of the existing output file to ensure that it can be deleted
                System.IO.File.SetAttributes(theOutputFilePath, System.IO.FileAttributes.Normal);

                // Then delete the existing output file
                System.IO.File.Delete(theOutputFilePath);
            }

            this.theOutputFilePath = theOutputFilePath;

            this.enableDebugInformation = enableDebugInformation;

            this.enableInformationEvents = enableInformationEvents;

            if (enableDebugInformation)
            {
                this.theOutputWindowListener =
                    new System.Diagnostics.TextWriterTraceListener(theOutputFilePath);

                // Unless instructed otherwise, remove the output window from the list of listeners to debug output as all text will go to the output file
                if (enableDebugInformation &&
                    !redirectDebugInformationToOutput)
                {
                    System.Diagnostics.Debug.Listeners.Clear();
                }

                // Redirect any text added to the output window to the output file
                System.Diagnostics.Trace.Listeners.Add(this.theOutputWindowListener);
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.enableDebugInformation)
                {
                    // Flush output to the output file and then close it
                    this.theOutputWindowListener.Flush();
                    this.theOutputWindowListener.Close();

                    System.Diagnostics.Debug.Listeners.Remove(this.theOutputWindowListener);

                    // Dispose any resources allocated to the trace listener if instructed
                    this.theOutputWindowListener.Dispose();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        public void WriteTestRunEvent(string theTestRunEvent)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.WriteLine("Test:  " +
                    theTestRunEvent);
            }
        }

        public void WriteInformationEvent(string theInformationEvent)
        {
            if (this.enableDebugInformation && this.enableInformationEvents)
            {
                System.Diagnostics.Trace.WriteLine("Info:  " +
                    theInformationEvent);
            }
        }

        public void WriteErrorEvent(string theErrorEvent)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.WriteLine("Error: " +
                    theErrorEvent);
            }
        }

        public void WriteTextElement(string theTextElement)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.Write(theTextElement);
            }
        }

        public void WriteTextLine(string theTextLine)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.WriteLine(theTextLine);
            }
        }

        public void WriteBlankLine()
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.Write(System.Environment.NewLine);
            }
        }

        public void Open()
        {
            if (this.enableDebugInformation)
            {
                // Flush output to the output file and then close it
                this.theOutputWindowListener.Flush();

                if (System.IO.File.Exists(this.theOutputFilePath))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(this.theOutputFilePath);
                    }
                    catch (System.ComponentModel.Win32Exception f)
                    {
                        this.WriteErrorEvent("The exception " +
                            f.GetType().Name +
                            " with the following message: " +
                            f.Message +
                            " was raised as there is no application registered that can open the " +
                            System.IO.Path.GetFileName(this.theOutputFilePath) +
                            " output file!!!");
                    }
                }
            }
        }
    }
}
