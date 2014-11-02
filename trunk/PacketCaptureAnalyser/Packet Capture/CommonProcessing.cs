// $Id$
// $URL$
// <copyright file="CommonProcessing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture
{
    /// <summary>
    /// This class provides the common packet capture processing
    /// </summary>
    public abstract class CommonProcessing
    {
        //// Concrete attributes - cannot be overridden by a derived class

        /// <summary>
        /// The instance of the progress window form to use for reporting progress of the processing
        /// </summary>
        private PacketCaptureAnalyser.ProgressWindowForm theProgressWindowForm;

        /// <summary>
        /// The object that provides for the logging of debug information
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// Boolean flag that indicates whether to perform latency analysis processing for data read from the packet capture
        /// </summary>
        private bool performLatencyAnalysisProcessing;

        /// <summary>
        /// The object that provides the latency analysis processing for data read from the packet capture
        /// </summary>
        private Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing;

        /// <summary>
        /// Boolean flag that indicates whether to perform burst analysis processing for data read from the packet capture
        /// </summary>
        private bool performBurstAnalysisProcessing;

        /// <summary>
        /// The object that provides the burst analysis processing for data read from the packet capture
        /// </summary>
        private Analysis.BurstAnalysis.Processing theBurstAnalysisProcessing;

        /// <summary>
        /// Boolean flag that indicates whether to perform time analysis processing for data read from the packet capture
        /// </summary>
        private bool performTimeAnalysisProcessing;

        /// <summary>
        /// The object that provides the time analysis processing for data read from the packet capture
        /// </summary>
        private Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing;

        /// <summary>
        /// The path of the selected packet capture
        /// </summary>
        private string theSelectedPacketCapturePath;

        /// <summary>
        /// Boolean flag that indicates whether to use the alternative sequence number in the data read from the packet capture, required for legacy recordings
        /// </summary>
        private bool useAlternativeSequenceNumber;

        /// <summary>
        /// Boolean flag that indicates whether to perform reading from the packet capture using a method that will minimize memory usage, possibly at the expense of increased processing time
        /// </summary>
        private bool minimizeMemoryUsage;

        //// Concrete methods - cannot be overridden by a derived class

        /// <summary>
        /// Initializes a new instance of the CommonProcessing class
        /// </summary>
        /// <param name="theProgressWindowForm">The instance of the progress window form to use for reporting progress of the processing</param>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="performLatencyAnalysisProcessing">Boolean flag that indicates whether to perform latency analysis processing for data read from the packet capture</param>
        /// <param name="theLatencyAnalysisProcessing">The object that provides the latency analysis processing for data read from the packet capture</param>
        /// <param name="performBurstAnalysisProcessing">Boolean flag that indicates whether to perform burst analysis processing for data read from the packet capture</param>
        /// <param name="theBurstAnalysisProcessing">The object that provides the burst analysis processing for data read from the packet capture</param>
        /// <param name="performTimeAnalysisProcessing">Boolean flag that indicates whether to perform time analysis processing for data read from the packet capture</param>
        /// <param name="theTimeAnalysisProcessing">The object that provides the time analysis processing for data read from the packet capture</param>
        /// <param name="theSelectedPacketCapturePath">The path of the selected packet capture</param>
        /// <param name="useAlternativeSequenceNumber">Boolean flag that indicates whether to use the alternative sequence number in the data read from the packet capture, required for legacy recordings</param>
        /// <param name="minimizeMemoryUsage">Boolean flag that indicates whether to perform reading from the packet capture using a method that will minimize memory usage, possibly at the expense of increased processing time</param>
        protected CommonProcessing(PacketCaptureAnalyser.ProgressWindowForm theProgressWindowForm, Analysis.DebugInformation theDebugInformation, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performBurstAnalysisProcessing, Analysis.BurstAnalysis.Processing theBurstAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing, string theSelectedPacketCapturePath, bool useAlternativeSequenceNumber, bool minimizeMemoryUsage)
        {
            this.theProgressWindowForm = theProgressWindowForm;

            this.theDebugInformation = theDebugInformation;

            this.performLatencyAnalysisProcessing = performLatencyAnalysisProcessing;
            this.theLatencyAnalysisProcessing = theLatencyAnalysisProcessing;

            this.performBurstAnalysisProcessing = performBurstAnalysisProcessing;
            this.theBurstAnalysisProcessing = theBurstAnalysisProcessing;

            this.performTimeAnalysisProcessing = performTimeAnalysisProcessing;
            this.theTimeAnalysisProcessing = theTimeAnalysisProcessing;

            this.theSelectedPacketCapturePath = theSelectedPacketCapturePath;

            this.useAlternativeSequenceNumber = useAlternativeSequenceNumber;

            this.minimizeMemoryUsage = minimizeMemoryUsage;
        }

        /// <summary>
        /// Gets the object that provides for the logging of debug information
        /// </summary>
        protected Analysis.DebugInformation TheDebugInformation
        {
            get
            {
                return this.theDebugInformation;
            }
        }

        /// <summary>
        /// Processes the selected packet capture
        /// </summary>
        /// <returns>Boolean flag that indicates whether the selected packet capture could be processed</returns>
        public bool ProcessPacketCapture()
        {
            bool theResult = true;

            try
            {
                this.theProgressWindowForm.ProgressBar = 55;

                if (System.IO.File.Exists(this.theSelectedPacketCapturePath))
                {
                    this.theProgressWindowForm.ProgressBar = 60;

                    // Read the start time to allow later calculation of the duration of the processing
                    System.DateTime theStartTime = System.DateTime.Now;

                    this.theDebugInformation.WriteInformationEvent(
                        "Reading of all bytes from the " +
                        System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                        " packet capture started");

                    this.theProgressWindowForm.ProgressBar = 65;

                    System.IO.FileStream theFileStream = null;

                    byte[] theBytes = null;

                    // If there is a need to minimize memory usage then sacrifice the significant processing
                    // speed improvements that come from the up front read of the whole file into an array
                    if (this.minimizeMemoryUsage)
                    {
                        // Open a file stream for the packet capture for reading
                        theFileStream = System.IO.File.OpenRead(
                            this.theSelectedPacketCapturePath);
                    }
                    else
                    {
                        // Read all the bytes from the packet capture into an array
                        theBytes = System.IO.File.ReadAllBytes(
                            this.theSelectedPacketCapturePath);
                    }

                    this.theProgressWindowForm.ProgressBar = 70;

                    //// Compute the duration between the start and the end times

                    System.DateTime theEndTime = System.DateTime.Now;

                    System.TimeSpan theDuration = theEndTime - theStartTime;

                    this.theDebugInformation.WriteInformationEvent(
                        "Reading of all bytes from the " +
                        System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                        " packet capture completed in " +
                        theDuration.Seconds.ToString() +
                        " seconds");

                    this.theProgressWindowForm.ProgressBar = 75;

                    // If there is a need to minimize memory usage then open the binary reader directly on the file stream, otherwise
                    // open a memory stream to the array containing the up front read of the file and then open the binary reader on that
                    if (this.minimizeMemoryUsage)
                    {
                        this.theProgressWindowForm.ProgressBar = 80;

                        // Open a binary reader for the file stream for the packet capture
                        using (System.IO.BinaryReader theBinaryReader =
                            new System.IO.BinaryReader(theFileStream))
                        {
                            this.theProgressWindowForm.ProgressBar = 90;

                            // Ensure that the position of the binary reader is set to the beginning of the memory stream
                            theBinaryReader.BaseStream.Position = 0;

                            // Declare an entity to be used for the network data link type extracted from the packet capture global header
                            uint theNetworkDataLinkType = 0;

                            // Declare an entity to be used for the timestamp accuracy extracted from the packet capture global header
                            double theTimestampAccuracy = 0.0;

                            this.theProgressWindowForm.ProgressBar = 95;

                            // Only continue reading from the packet capture if the packet capture global header was read successfully
                            if (this.ProcessPacketCaptureGlobalHeader(
                                theBinaryReader,
                                out theNetworkDataLinkType,
                                out theTimestampAccuracy))
                            {
                                this.theProgressWindowForm.ProgressBar = 100;

                                theResult = this.ProcessPackets(
                                    theBinaryReader,
                                    theNetworkDataLinkType,
                                    theTimestampAccuracy);
                            }
                            else
                            {
                                theResult = false;
                            }
                        }
                    }
                    else
                    {
                        // Create a memory stream to read the packet capture from the byte array
                        using (System.IO.MemoryStream theMemoryStream =
                            new System.IO.MemoryStream(theBytes))
                        {
                            this.theProgressWindowForm.ProgressBar = 80;

                            // Open a binary reader for the memory stream for the packet capture
                            using (System.IO.BinaryReader theBinaryReader =
                                new System.IO.BinaryReader(theMemoryStream))
                            {
                                this.theProgressWindowForm.ProgressBar = 90;

                                // Ensure that the position of the binary reader is set to the beginning of the memory stream
                                theBinaryReader.BaseStream.Position = 0;

                                // Declare an entity to be used for the network data link type extracted from the global header for the packet capture
                                uint thePacketCaptureNetworkDataLinkType = 0;

                                // Declare an entity to be used for the timestamp accuracy extracted from the global header for the packet capture
                                double thePacketCaptureTimestampAccuracy = 0.0;

                                this.theProgressWindowForm.ProgressBar = 95;

                                // Only continue reading from the packet capture if the packet capture global header was read successfully
                                if (this.ProcessPacketCaptureGlobalHeader(
                                    theBinaryReader,
                                    out thePacketCaptureNetworkDataLinkType,
                                    out thePacketCaptureTimestampAccuracy))
                                {
                                    this.theProgressWindowForm.ProgressBar = 100;

                                    theResult = this.ProcessPackets(
                                        theBinaryReader,
                                        thePacketCaptureNetworkDataLinkType,
                                        thePacketCaptureTimestampAccuracy);
                                }
                                else
                                {
                                    theResult = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.theDebugInformation.WriteErrorEvent(
                        "The " +
                        System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                        " packet capture does not exist!!!");

                    theResult = false;
                }
            }
            catch (System.IO.IOException e)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as access to the " +
                    System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                    " packet capture was denied because it is being used by another process or because of insufficient resources!!!");

                theResult = false;
            }
            catch (System.UnauthorizedAccessException e)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as access to the " +
                    System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                    " packet capture was denied because this process was deemed as unauthorised by the OS!!!");

                theResult = false;
            }
            catch (System.OutOfMemoryException e)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as there was insufficient memory to read in all of the " +
                    System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                    " packet capture!!!");

                this.theDebugInformation.WriteInformationEvent(
                    "It is suggested that the analysis is run again with the 'Minimize Memory Usage' check-box checked");

                theResult = false;
            }

            return theResult;
        }

        //// Abstract methods - must be overridden with a concrete implementation by a derived class

        /// <summary>
        /// Processes the packet capture global header
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the packet capture global header could be processed</returns>
        protected abstract bool ProcessPacketCaptureGlobalHeader(System.IO.BinaryReader theBinaryReader, out uint thePacketCaptureNetworkDataLinkType, out double thePacketCaptureTimestampAccuracy);

        /// <summary>
        /// Processes the packet header
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the packet header could be processed</returns>
        protected abstract bool ProcessPacketHeader(System.IO.BinaryReader theBinaryReader, uint thePacketCaptureNetworkDataLinkType, double thePacketCaptureTimestampAccuracy, out long thePacketPayloadLength, out double thePacketTimestamp);

        //// Private methods

        /// <summary>
        /// Processes packets from the selected packet capture
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the packets could be processed from the selected packet capture</returns>
        private bool ProcessPackets(System.IO.BinaryReader theBinaryReader, uint thePacketCaptureNetworkDataLinkType, double thePacketCaptureTimestampAccuracy)
        {
            bool theResult = true;

            ulong theNumberOfPacketsProcessed = 0;

            // Read the start time to allow later calculation of the duration of the processing
            System.DateTime theStartTime =
                System.DateTime.Now;

            // Display a debug message to indicate parsing of the packet capture completed successfully
            this.theDebugInformation.WriteTestRunEvent(
                "Parsing of the " +
                System.IO.Path.GetFileName(this.theSelectedPacketCapturePath) +
                " packet capture started");

            if (this.theProgressWindowForm != null)
            {
                // Update the label and progress bar now the parsing of the packet capture has started
                this.theProgressWindowForm.ProgressBarLabel = "Performing Parsing Of Packet Capture";
                this.theProgressWindowForm.ProgressBar = 0;
                this.theProgressWindowForm.Refresh();
            }

            EthernetFrame.Processing theEthernetFrameProcessing =
                new EthernetFrame.Processing(
                    this.theDebugInformation,
                    theBinaryReader,
                    this.performLatencyAnalysisProcessing,
                    this.theLatencyAnalysisProcessing,
                    this.performBurstAnalysisProcessing,
                    this.theBurstAnalysisProcessing,
                    this.performTimeAnalysisProcessing,
                    this.theTimeAnalysisProcessing,
                    this.useAlternativeSequenceNumber);

            // Attempt to process the packets in the packet capture
            try
            {
                // Store the length of the stream locally - the .NET framework does not cache it so each query requires an expensive read - this is OK so long as not editing the file at the same time as analysing it
                long theStreamLength = theBinaryReader.BaseStream.Length;

                // Declare an entity to be used for each payload length extracted from a packet header for a packet
                long thePacketPayloadLength = 0;

                // Declare an entity to be used for each timestamp extracted from a packet header for a packet
                double thePacketTimestamp = 0.0;

                //// Keep looping through the packet capture, processing each packet header and Ethernet frame in turn, while characters remain in the packet capture and there are no errors
                //// Cannot use the PeekChar method here as some characters in the packet capture may fall outside of the bounds of the character encoding - it is a binary format after all!
                //// Instead use the position of the binary reader within the stream, stopping once the length of stream has been reached

                while (theBinaryReader.BaseStream.Position < theStreamLength && theResult)
                {
                    ++theNumberOfPacketsProcessed;

                    //// Restore the following lines if you want indication of progress through the packet capture or want to tie an error condition to a particular packet

                    // theDebugInformation.WriteInformationEvent
                    //     ("Started processing of packet number " +
                    //     theNumberOfPacketsProcessed.ToString());

                    // Check whether the end of the packet capture has been reached
                    if (theBinaryReader.BaseStream.Position < theStreamLength)
                    {
                        if (this.ProcessPacketHeader(
                            theBinaryReader,
                            thePacketCaptureNetworkDataLinkType,
                            thePacketCaptureTimestampAccuracy,
                            out thePacketPayloadLength,
                            out thePacketTimestamp))
                        {
                            // Check whether the end of the packet capture has been reached
                            if (theBinaryReader.BaseStream.Position < theStreamLength)
                            {
                                switch (thePacketCaptureNetworkDataLinkType)
                                {
                                    case (uint)CommonConstants.NetworkDataLinkType.NullLoopBack:
                                    case (uint)CommonConstants.NetworkDataLinkType.CiscoHDLC:
                                        {
                                            // Just read the bytes off from the packet capture so we can continue
                                            theBinaryReader.ReadBytes(
                                                (int)thePacketPayloadLength);

                                            break;
                                        }

                                    case (uint)CommonConstants.NetworkDataLinkType.Ethernet:
                                        {
                                            theEthernetFrameProcessing.ProcessEthernetFrame(
                                                theNumberOfPacketsProcessed,
                                                thePacketPayloadLength,
                                                thePacketTimestamp);

                                            break;
                                        }

                                    default:
                                        {
                                            theResult = false;

                                            this.theDebugInformation.WriteErrorEvent(
                                                "Processing of the packet number " +
                                                theNumberOfPacketsProcessed.ToString() +
                                                " failed during processing of packet header with unknown datalink type!!!");

                                            break;
                                        }
                                }
                            }
                            else
                            {
                                // Stop looping as have reached the end of the packet capture
                                break;
                            }
                        }
                        else
                        {
                            theResult = false;

                            this.theDebugInformation.WriteErrorEvent(
                                "Processing of the packet number " +
                                theNumberOfPacketsProcessed.ToString() +
                                " failed during processing of Ethernet frame!!!");

                            // Stop looping as there has been an error!!!
                            break;
                        }
                    }
                    else
                    {
                        // Stop looping as have reached the end of the packet capture
                        break;
                    }

                    if (this.theProgressWindowForm != null)
                    {
                        // Update the progress bar to reflect the completion of this iteration of the loop
                        this.theProgressWindowForm.ProgressBar =
                            (int)((theBinaryReader.BaseStream.Position * 100) / theStreamLength);
                    }
                }
            }
            catch (System.IO.EndOfStreamException e)
            {
                //// If the end of the stream is reached while reading the packet capture, ignore the error as there is no more processing to conduct and we don't want to lose the data we have already processed

                this.theDebugInformation.WriteErrorEvent(
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " has been caught and ignored!");

                theResult = true;
            }

            if (theResult)
            {
                //// Compute the duration between the start and the end times

                System.DateTime theEndTime =
                    System.DateTime.Now;

                System.TimeSpan theDuration =
                    theEndTime - theStartTime;

                this.theDebugInformation.WriteInformationEvent(
                    "Parsing of " +
                    theNumberOfPacketsProcessed.ToString() +
                    " packets completed in " +
                    theDuration.TotalSeconds.ToString() +
                    " seconds");
            }

            if (this.theProgressWindowForm != null)
            {
                // Update the label and progress bar now the parsing of the packet capture has completed
                this.theProgressWindowForm.ProgressBarLabel = "Completed Parsing Of Packet Capture";
                this.theProgressWindowForm.ProgressBar = 100;
                this.theProgressWindowForm.Refresh();
            }

            return theResult;
        }
    }
}
