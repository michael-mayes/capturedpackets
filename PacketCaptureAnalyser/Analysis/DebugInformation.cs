// $Id$
// $URL$
// <copyright file="DebugInformation.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace Analysis
{
    /// <summary>
    /// This class provides processing for logging of debug information including errors and information text
    /// This class will implement the Disposable class so as to be able to clean up after the data tables it creates which themselves implement the Disposable class
    /// </summary>
    public class DebugInformation : System.IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private System.Diagnostics.TextWriterTraceListener theOutputWindowListener;

        /// <summary>
        /// 
        /// </summary>
        private string theOutputFilePath;

        /// <summary>
        /// 
        /// </summary>
        private bool enableDebugInformation;

        /// <summary>
        /// 
        /// </summary>
        private bool enableInformationEvents;

        /// <summary>
        /// Initializes a new instance of the DebugInformation class
        /// </summary>
        /// <param name="theOutputFilePath"></param>
        /// <param name="enableDebugInformation"></param>
        /// <param name="enableInformationEvents"></param>
        /// <param name="redirectDebugInformationToOutput"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theTestRunEvent"></param>
        public void WriteTestRunEvent(string theTestRunEvent)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.WriteLine("Test:  " +
                    theTestRunEvent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theInformationEvent"></param>
        public void WriteInformationEvent(string theInformationEvent)
        {
            if (this.enableDebugInformation && this.enableInformationEvents)
            {
                System.Diagnostics.Trace.WriteLine("Info:  " +
                    theInformationEvent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theErrorEvent"></param>
        public void WriteErrorEvent(string theErrorEvent)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.WriteLine("Error: " +
                    theErrorEvent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theTextElement"></param>
        public void WriteTextElement(string theTextElement)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.Write(theTextElement);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theTextLine"></param>
        public void WriteTextLine(string theTextLine)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.WriteLine(theTextLine);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteBlankLine()
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.Write(System.Environment.NewLine);
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
