// <copyright file="DebugInformation.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.Analysis
{
    /// <summary>
    /// This class provides processing for logging of debug information including errors and information text
    /// This class will implement the Disposable class so as to be able to clean up after the data tables it creates which themselves implement the Disposable class
    /// </summary>
    public class DebugInformation : System.IDisposable
    {
        /// <summary>
        /// The list of listeners to receive the debug output
        /// </summary>
        private System.Diagnostics.TextWriterTraceListener theOutputWindowListener;

        /// <summary>
        /// The path of the output file to receive the debug output
        /// </summary>
        private string theOutputFilePath;

        /// <summary>
        /// Boolean flag that indicates whether to enable the debug information
        /// </summary>
        private bool enableDebugInformation;

        /// <summary>
        /// Boolean flag that indicates whether to include information events in the debug information
        /// </summary>
        private bool enableInformationEvents;

        /// <summary>
        /// Initializes a new instance of the DebugInformation class
        /// </summary>
        /// <param name="theOutputFilePath">The path of the output file to receive the debug output</param>
        /// <param name="enableDebugInformation">Boolean flag that indicates whether to enable the debug information</param>
        /// <param name="enableInformationEvents">Boolean flag that indicates whether to include information events in the debug information</param>
        /// <param name="redirectDebugInformationToOutput">Boolean flag that indicates whether to redirect debug information to the output window</param>
        public DebugInformation(string theOutputFilePath, bool enableDebugInformation, bool enableInformationEvents, bool redirectDebugInformationToOutput)
        {
            // Delete any existing output files with the selected name to ape the clearing of all text from the output window
            if (System.IO.File.Exists(
                theOutputFilePath))
            {
                // Reset the attributes of the existing output file to ensure that it can be deleted
                System.IO.File.SetAttributes(
                    theOutputFilePath,
                    System.IO.FileAttributes.Normal);

                // Then delete the existing output file
                System.IO.File.Delete(
                    theOutputFilePath);
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
                System.Diagnostics.Trace.Listeners.Add(
                    this.theOutputWindowListener);
            }
        }

        /// <summary>
        /// Clean up any resources used by the debug information class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Writes the supplied test run event string as a new line to the list of listeners if the debug information is enabled
        /// </summary>
        /// <param name="theTestRunEvent">The test run event string to be written as a new line to the list of listeners</param>
        public void WriteTestRunEvent(string theTestRunEvent)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.WriteLine(
                    "Test:  " +
                    theTestRunEvent);
            }
        }

        /// <summary>
        /// Writes the supplied information string as a new line to the list of listeners if the debug information is enabled and information events are to be included
        /// </summary>
        /// <param name="theInformationEvent">The information string to be written as a new line to the list of listeners</param>
        public void WriteInformationEvent(string theInformationEvent)
        {
            if (this.enableDebugInformation &&
                this.enableInformationEvents)
            {
                System.Diagnostics.Trace.WriteLine(
                    "Info:  " +
                    theInformationEvent);
            }
        }

        /// <summary>
        /// Writes the supplied error string as a new line to the list of listeners if the debug information is enabled
        /// </summary>
        /// <param name="theErrorEvent">The error string to be written as a new line to the list of listeners</param>
        public void WriteErrorEvent(string theErrorEvent)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.WriteLine(
                    "Error: " +
                    theErrorEvent);
            }
        }

        /// <summary>
        /// Writes the supplied text string on the current line for the list of listeners if the debug information is enabled
        /// </summary>
        /// <param name="theTextElement">The text string to be written on the current line for the list of listeners</param>
        public void WriteTextElement(string theTextElement)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.Write(
                    theTextElement);
            }
        }

        /// <summary>
        /// Writes the supplied text string as a new line to the list of listeners if the debug information is enabled
        /// </summary>
        /// <param name="theTextLine">The text string to be written as a new line to the list of listeners</param>
        public void WriteTextLine(string theTextLine)
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.WriteLine(
                    theTextLine);
            }
        }

        /// <summary>
        /// Writes a blank line to the list of listeners if the debug information is enabled
        /// </summary>
        public void WriteBlankLine()
        {
            if (this.enableDebugInformation)
            {
                System.Diagnostics.Trace.Write(
                    System.Environment.NewLine);
            }
        }

        /// <summary>
        /// Opens the output file for viewing if the debug information is enabled
        /// </summary>
        public void Open()
        {
            if (this.enableDebugInformation)
            {
                // Flush output to the output file and then close it
                this.theOutputWindowListener.Flush();

                if (System.IO.File.Exists(
                    this.theOutputFilePath))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(
                            this.theOutputFilePath);
                    }
                    catch (System.ComponentModel.Win32Exception f)
                    {
                        this.WriteErrorEvent(
                            "The exception " +
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

        /// <summary>
        /// Clean up any resources used by the debug information class
        /// </summary>
        /// <param name="disposing">Boolean flag that indicates whether the method call comes from a Dispose method (its value is true) or from the garbage collector (its value is false)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.enableDebugInformation)
                {
                    // Flush output to the output file and then close it
                    this.theOutputWindowListener.Flush();
                    this.theOutputWindowListener.Close();

                    System.Diagnostics.Debug.Listeners.Remove(
                        this.theOutputWindowListener);

                    // Dispose any resources allocated to the trace listener if instructed
                    this.theOutputWindowListener.Dispose();
                }
            }
        }
    }
}
