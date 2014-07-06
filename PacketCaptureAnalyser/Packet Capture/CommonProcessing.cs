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

        protected PacketCaptureAnalyser.ProgressWindowForm theProgressWindowForm;
        protected Analysis.DebugInformation theDebugInformation;

        protected bool performLatencyAnalysisProcessing;
        protected Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing;

        protected bool performTimeAnalysisProcessing;
        protected Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing;

        protected string thePacketCapture;
        protected bool minimiseMemoryUsage;

        //// Concrete methods - cannot be overridden by a derived class

        /// <summary>
        /// Initializes a new instance of the CommonProcessing class
        /// </summary>
        /// <param name="theProgressWindowForm"></param>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="performLatencyAnalysisProcessing">The flag that indicates whether to perform latency analysis processing for data read from the packet capture</param>
        /// <param name="theLatencyAnalysisProcessing">The object that provides the latency analysis processing for data read from the packet capture</param>
        /// <param name="performTimeAnalysisProcessing">The flag that indicates whether to perform time analysis processing for data read from the packet capture</param>
        /// <param name="theTimeAnalysisProcessing">The object that provides the time analysis processing for data read from the packet capture</param>
        /// <param name="thePacketCapture"></param>
        /// <param name="minimiseMemoryUsage"></param>
        protected CommonProcessing(PacketCaptureAnalyser.ProgressWindowForm theProgressWindowForm, Analysis.DebugInformation theDebugInformation, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing, string thePacketCapture, bool minimiseMemoryUsage)
        {
            this.theProgressWindowForm = theProgressWindowForm;
            this.theDebugInformation = theDebugInformation;
            this.performLatencyAnalysisProcessing = performLatencyAnalysisProcessing;
            this.theLatencyAnalysisProcessing = theLatencyAnalysisProcessing;
            this.performTimeAnalysisProcessing = performTimeAnalysisProcessing;
            this.theTimeAnalysisProcessing = theTimeAnalysisProcessing;
            this.thePacketCapture = thePacketCapture;
            this.minimiseMemoryUsage = minimiseMemoryUsage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Process()
        {
            bool theResult = true;

            try
            {
                this.theProgressWindowForm.ProgressBar = 55;

                if (System.IO.File.Exists(this.thePacketCapture))
                {
                    this.theProgressWindowForm.ProgressBar = 60;

                    // Read the start time to allow later calculation of the duration of the processing
                    System.DateTime theStartTime = System.DateTime.Now;

                    this.theDebugInformation.WriteInformationEvent("Starting read of all bytes from the " +
                        System.IO.Path.GetFileName(this.thePacketCapture) +
                        " packet capture");

                    this.theProgressWindowForm.ProgressBar = 65;

                    System.IO.FileStream theFileStream = null;

                    byte[] theBytes = null;

                    // If there is a need to minimise memory usage then sacrifice the significant processing
                    // speed improvements that come from the up front read of the whole file into an array
                    if (this.minimiseMemoryUsage)
                    {
                        // Open a file stream for the packet capture for reading
                        theFileStream = System.IO.File.OpenRead(this.thePacketCapture);
                    }
                    else
                    {
                        // Read all the bytes from the packet capture into an array
                        theBytes = System.IO.File.ReadAllBytes(this.thePacketCapture);
                    }

                    this.theProgressWindowForm.ProgressBar = 70;

                    //// Compute the duration between the start and the end times

                    System.DateTime theEndTime = System.DateTime.Now;

                    System.TimeSpan theDuration = theEndTime - theStartTime;

                    this.theDebugInformation.WriteInformationEvent("Finished read of all bytes from the " +
                        System.IO.Path.GetFileName(this.thePacketCapture) +
                        " packet capture in " +
                        theDuration.Seconds.ToString() +
                        " seconds");

                    this.theProgressWindowForm.ProgressBar = 75;

                    // If there is a need to minimise memory usage then open the binary reader directly on the file stream, otherwise
                    // open a memory stream to the array containing the up front read of the file and then open the binary reader on that
                    if (this.minimiseMemoryUsage)
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
                            if (this.ProcessGlobalHeader(theBinaryReader, out theNetworkDataLinkType, out theTimestampAccuracy))
                            {
                                this.theProgressWindowForm.ProgressBar = 100;

                                theResult = this.ProcessPackets(theBinaryReader, theNetworkDataLinkType, theTimestampAccuracy);
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

                                // Declare an entity to be used for the network data link type extracted from the packet capture global header
                                uint theNetworkDataLinkType = 0;

                                // Declare an entity to be used for the timestamp accuracy extracted from the packet capture global header
                                double theTimestampAccuracy = 0.0;

                                this.theProgressWindowForm.ProgressBar = 95;

                                // Only continue reading from the packet capture if the packet capture global header was read successfully
                                if (this.ProcessGlobalHeader(theBinaryReader, out theNetworkDataLinkType, out theTimestampAccuracy))
                                {
                                    this.theProgressWindowForm.ProgressBar = 100;

                                    theResult = this.ProcessPackets(theBinaryReader, theNetworkDataLinkType, theTimestampAccuracy);
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
                    this.theDebugInformation.WriteErrorEvent("The " +
                        System.IO.Path.GetFileName(this.thePacketCapture) +
                        " packet capture does not exist!!!");

                    theResult = false;
                }
            }
            catch (System.IO.IOException e)
            {
                this.theDebugInformation.WriteErrorEvent("The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as access to the " +
                    System.IO.Path.GetFileName(this.thePacketCapture) +
                    " packet capture was denied because it is being used by another process or because of insufficient resources!!!");

                theResult = false;
            }
            catch (System.UnauthorizedAccessException e)
            {
                this.theDebugInformation.WriteErrorEvent("The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as access to the " +
                    System.IO.Path.GetFileName(this.thePacketCapture) +
                    " packet capture was denied because this process was deemed as unauthorised by the OS!!!");

                theResult = false;
            }
            catch (System.OutOfMemoryException e)
            {
                this.theDebugInformation.WriteErrorEvent("The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as there was insufficient memory to read in all of the " +
                    System.IO.Path.GetFileName(this.thePacketCapture) +
                    " packet capture!!!");

                this.theDebugInformation.WriteInformationEvent("It is suggested that the analysis is run again with the 'Minimise Memory Usage' check-box checked");

                theResult = false;
            }

            return theResult;
        }

        //// Abstract methods - must be overridden with a concrete implementation by a derived class

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="theNetworkDataLinkType"></param>
        /// <param name="theTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <returns></returns>
        protected abstract bool ProcessGlobalHeader(System.IO.BinaryReader theBinaryReader, out uint theNetworkDataLinkType, out double theTimestampAccuracy);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="theNetworkDataLinkType"></param>
        /// <param name="theTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <param name="thePayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="theTimestamp">The timestamp read from the packet capture</param>
        /// <returns></returns>
        protected abstract bool ProcessPacketHeader(System.IO.BinaryReader theBinaryReader, uint theNetworkDataLinkType, double theTimestampAccuracy, out long thePayloadLength, out double theTimestamp);

        //// Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="theNetworkDataLinkType"></param>
        /// <param name="theTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <returns></returns>
        private bool ProcessPackets(System.IO.BinaryReader theBinaryReader, uint theNetworkDataLinkType, double theTimestampAccuracy)
        {
            bool theResult = true;

            ulong packetsProcessed = 0;

            // Read the start time to allow later calculation of the duration of the processing
            System.DateTime theStartTime = System.DateTime.Now;

            this.theDebugInformation.WriteInformationEvent("Started processing of the captured packets");

            if (this.theProgressWindowForm != null)
            {
                // Update the label and progress bar now the parsing of the packet capture has started
                this.theProgressWindowForm.ProgressBarLabel = "Performing Parsing Of Packet Capture";
                this.theProgressWindowForm.ProgressBar = 0;
                this.theProgressWindowForm.Refresh();
            }

            EthernetFrame.Processing theEthernetFrameProcessing = new EthernetFrame.Processing(this.theDebugInformation, theBinaryReader, this.performLatencyAnalysisProcessing, this.theLatencyAnalysisProcessing, this.performTimeAnalysisProcessing, this.theTimeAnalysisProcessing);

            // Attempt to process the packets in the packet capture
            try
            {
                // Store the length of the stream locally - the .NET framework does not cache it so each query requires an expensive read - this is OK so long as not editing the file at the same time as analysing it
                long theStreamLength = theBinaryReader.BaseStream.Length;

                // Declare an entity to be used for each payload length extracted from a packet header
                long thePayloadLength = 0;

                // Declare an entity to be used for each timestamp extracted from a packet header
                double theTimestamp = 0.0;

                //// Keep looping through the packet capture, processing each packet header and Ethernet frame in turn, while characters remain in the packet capture and there are no errors
                //// Cannot use the PeekChar method here as some characters in the packet capture may fall outside of the bounds of the character encoding - it is a binary format after all!
                //// Instead use the position of the binary reader within the stream, stopping once the length of stream has been reached

                while (theBinaryReader.BaseStream.Position < theStreamLength && theResult)
                {
                    ++packetsProcessed;

                    //// Restore the following lines if you want indication of progress through the packet capture or want to tie an error condition to a particular packet

                    // theDebugInformation.WriteInformationEvent
                    //     ("Started processing of captured packet #" +
                    //     PacketsProcessed.ToString());

                    // Check whether the end of the packet capture has been reached
                    if (theBinaryReader.BaseStream.Position < theStreamLength)
                    {
                        if (this.ProcessPacketHeader(theBinaryReader, theNetworkDataLinkType, theTimestampAccuracy, out thePayloadLength, out theTimestamp))
                        {
                            // Check whether the end of the packet capture has been reached
                            if (theBinaryReader.BaseStream.Position < theStreamLength)
                            {
                                switch (theNetworkDataLinkType)
                                {
                                    case (uint)CommonConstants.NetworkDataLinkType.NullLoopBack:
                                    case (uint)CommonConstants.NetworkDataLinkType.CiscoHDLC:
                                        {
                                            // Just read the bytes off from the packet capture so we can continue
                                            theBinaryReader.ReadBytes((int)thePayloadLength);
                                            break;
                                        }

                                    case (uint)CommonConstants.NetworkDataLinkType.Ethernet:
                                        {
                                            if (!theEthernetFrameProcessing.Process(packetsProcessed, thePayloadLength, theTimestamp))
                                            {
                                                theResult = false;

                                                this.theDebugInformation.WriteErrorEvent("Processing of the captured packet #" +
                                                    packetsProcessed.ToString() +
                                                    " failed during processing of packet header for Ethernet frame!!!");
                                            }

                                            break;
                                        }

                                    default:
                                        {
                                            theResult = false;

                                            this.theDebugInformation.WriteErrorEvent("Processing of the captured packet #" +
                                                packetsProcessed.ToString() +
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

                            this.theDebugInformation.WriteErrorEvent("Processing of the captured packet #" +
                                packetsProcessed.ToString() +
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
                        this.theProgressWindowForm.ProgressBar = (int)((theBinaryReader.BaseStream.Position * 100) / theStreamLength);
                    }
                }
            }
            catch (System.IO.EndOfStreamException e)
            {
                //// If the end of the stream is reached while reading the packet capture, ignore the error as there is no more processing to conduct and we don't want to lose the data we have already processed

                this.theDebugInformation.WriteErrorEvent("The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " has been caught and ignored!");

                theResult = true;
            }

            if (theResult)
            {
                //// Compute the duration between the start and the end times

                System.DateTime theEndTime = System.DateTime.Now;

                System.TimeSpan theDuration = theEndTime - theStartTime;

                this.theDebugInformation.WriteInformationEvent("Finished processing of " +
                    packetsProcessed.ToString() +
                    " captured packets in " +
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
