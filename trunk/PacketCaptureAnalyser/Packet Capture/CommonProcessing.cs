//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture
{
    abstract class CommonProcessing
    {
        //
        //Abstract methods - must be overridden with a concrete implementation by a derived class
        //

        public abstract bool ProcessGlobalHeader(System.IO.BinaryReader TheBinaryReader, out System.UInt32 TheNetworkDataLinkType, out double TheTimestampAccuracy);

        public abstract bool ProcessPacketHeader(System.IO.BinaryReader TheBinaryReader, System.UInt32 TheNetworkDataLinkType, double TheTimestampAccuracy, out long ThePayloadLength, out double TheTimestamp);

        //
        //Concrete methods - cannot be overridden by a derived class
        //

        public bool Process(PacketCaptureAnalyser.ProgressWindowForm TheProgressWindowForm, bool PerformLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing TheLatencyAnalysisProcessing, bool PerformTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing TheTimeAnalysisProcessing, string ThePacketCapture, bool MinimiseMemoryUsage)
        {
            bool TheResult = true;

            try
            {
                TheProgressWindowForm.ProgressBar.Value = 55;

                if (System.IO.File.Exists(ThePacketCapture))
                {
                    TheProgressWindowForm.ProgressBar.Value = 60;

                    //Read the start time to allow later calculation of the duration of the processing
                    System.DateTime TheStartTime = System.DateTime.Now;

                    System.Diagnostics.Trace.WriteLine
                        (
                        "Info:  " +
                        "Starting read of all bytes from the " +
                        System.IO.Path.GetFileName(ThePacketCapture) +
                        " packet capture"
                        );

                    TheProgressWindowForm.ProgressBar.Value = 65;

                    System.IO.FileStream TheFileStream = null;

                    byte[] TheBytes = null;

                    //If there is a need to minimise memory usage then sacrifice the significant processing
                    //speed improvements that come from the up front read of the whole file into an array
                    if (MinimiseMemoryUsage)
                    {
                        //Open a file stream for the packet capture for reading
                        TheFileStream = System.IO.File.OpenRead(ThePacketCapture);
                    }
                    else
                    {
                        //Read all the bytes from the packet capture into an array
                        TheBytes = System.IO.File.ReadAllBytes(ThePacketCapture);
                    }

                    TheProgressWindowForm.ProgressBar.Value = 70;

                    //Compute the duration between the start and the end times

                    System.DateTime TheEndTime = System.DateTime.Now;

                    System.TimeSpan TheDuration = TheEndTime - TheStartTime;

                    System.Diagnostics.Trace.WriteLine
                        (
                        "Info:  " +
                        "Finished read of all bytes from the " +
                        System.IO.Path.GetFileName(ThePacketCapture) +
                        " packet capture in " +
                        TheDuration.Seconds.ToString() +
                        " seconds"
                        );

                    TheProgressWindowForm.ProgressBar.Value = 75;

                    //If there is a need to minimise memory usage then open the binary reader directly on the file stream, otherwise
                    //open a memory stream to the array containing the up front read of the file and then open the binary reader on that
                    if (MinimiseMemoryUsage)
                    {
                        TheProgressWindowForm.ProgressBar.Value = 80;

                        //Open a binary reader for the file stream for the packet capture
                        using (System.IO.BinaryReader TheBinaryReader =
                            new System.IO.BinaryReader(TheFileStream))
                        {
                            TheProgressWindowForm.ProgressBar.Value = 90;

                            //Ensure that the position of the binary reader is set to the beginning of the memory stream
                            TheBinaryReader.BaseStream.Position = 0;

                            //Declare an entity to be used for the network data link type extracted from the packet capture global header
                            System.UInt32 TheNetworkDataLinkType = 0;

                            //Declare an entity to be used for the timestamp accuracy extracted from the packet capture global header
                            double TheTimestampAccuracy = 0.0;

                            TheProgressWindowForm.ProgressBar.Value = 95;

                            //Only continue reading from the packet capture if the packet capture global header was read successfully
                            if (ProcessGlobalHeader(TheBinaryReader, out TheNetworkDataLinkType, out TheTimestampAccuracy))
                            {
                                TheProgressWindowForm.ProgressBar.Value = 100;

                                TheResult = ProcessPackets(TheBinaryReader, TheProgressWindowForm, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing, TheNetworkDataLinkType, TheTimestampAccuracy);
                            }
                            else
                            {
                                TheResult = false;
                            }
                        }
                    }
                    else
                    {
                        //Create a memory stream to read the packet capture from the byte array
                        using (System.IO.MemoryStream TheMemoryStream =
                            new System.IO.MemoryStream(TheBytes))
                        {
                            TheProgressWindowForm.ProgressBar.Value = 80;

                            //Open a binary reader for the memory stream for the packet capture
                            using (System.IO.BinaryReader TheBinaryReader =
                                new System.IO.BinaryReader(TheMemoryStream))
                            {
                                TheProgressWindowForm.ProgressBar.Value = 90;

                                //Ensure that the position of the binary reader is set to the beginning of the memory stream
                                TheBinaryReader.BaseStream.Position = 0;

                                //Declare an entity to be used for the network data link type extracted from the packet capture global header
                                System.UInt32 TheNetworkDataLinkType = 0;

                                //Declare an entity to be used for the timestamp accuracy extracted from the packet capture global header
                                double TheTimestampAccuracy = 0.0;

                                TheProgressWindowForm.ProgressBar.Value = 95;

                                //Only continue reading from the packet capture if the packet capture global header was read successfully
                                if (ProcessGlobalHeader(TheBinaryReader, out TheNetworkDataLinkType, out TheTimestampAccuracy))
                                {
                                    TheProgressWindowForm.ProgressBar.Value = 100;

                                    TheResult = ProcessPackets(TheBinaryReader, TheProgressWindowForm, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing, TheNetworkDataLinkType, TheTimestampAccuracy);
                                }
                                else
                                {
                                    TheResult = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine
                        (
                        "Error: " +
                        "The " +
                        System.IO.Path.GetFileName(ThePacketCapture) +
                        " packet capture does not exist!!!"
                        );

                    TheResult = false;
                }
            }

            catch (System.IO.IOException e)
            {

                System.Diagnostics.Trace.WriteLine
                    (
                    "Error: " +
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as access to the " +
                    System.IO.Path.GetFileName(ThePacketCapture) +
                    " packet capture was denied because it is being used by another process or because of insufficient resources!!!"
                    );

                TheResult = false;
            }

            catch (System.UnauthorizedAccessException e)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "Error: " +
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as access to the " +
                    System.IO.Path.GetFileName(ThePacketCapture) +
                    " packet capture was denied because this process was deemed as unauthorised by the OS!!!"
                    );

                TheResult = false;
            }

            catch (System.OutOfMemoryException e)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "Error: " +
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as there was insufficient memory to read in all of the " +
                    System.IO.Path.GetFileName(ThePacketCapture) +
                    " packet capture!!!"
                    );

                System.Diagnostics.Trace.WriteLine
                    (
                    "Info:  " +
                    "It is suggested that the analysis is run again with the 'Minimise Memory Usage' check-box checked"
                    );

                TheResult = false;
            }

            return TheResult;
        }

        private bool ProcessPackets(System.IO.BinaryReader TheBinaryReader, PacketCaptureAnalyser.ProgressWindowForm TheProgressWindowForm, bool PerformLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing TheLatencyAnalysisProcessing, bool PerformTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing TheTimeAnalysisProcessing, System.UInt32 TheNetworkDataLinkType, double TheTimestampAccuracy)
        {
            bool TheResult = true;

            ulong PacketsProcessed = 0;

            //Read the start time to allow later calculation of the duration of the processing
            System.DateTime TheStartTime = System.DateTime.Now;

            System.Diagnostics.Trace.WriteLine
                (
                "Info:  " +
                "Started processing of the captured packets"
                );

            if (TheProgressWindowForm != null)
            {
                //Update the label and progress bar now the parsing of the packet capture has started
                TheProgressWindowForm.ProgressBarLabel.Text = "Performing Parsing Of Packet Capture";
                TheProgressWindowForm.ProgressBar.Value = 0;
                TheProgressWindowForm.Refresh();
            }

            EthernetFrame.Processing TheEthernetFrameProcessing = new EthernetFrame.Processing(TheBinaryReader, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing);

            //Attempt to process the packets in the packet capture
            try
            {
                //Store the length of the stream locally - the .NET framework does not cache it so each query requires an expensive read - this is OK so long as not editing the file at the same time as analysing it
                long TheStreamLength = TheBinaryReader.BaseStream.Length;

                //Declare an entity to be used for each payload length extracted from a packet header
                long ThePayloadLength = 0;

                //Declare an entity to be used for each timestamp extracted from a packet header
                double TheTimestamp = 0.0;

                //Keep looping through the packet capture, processing each packet header and Ethernet frame in turn, while characters remain in the packet capture and there are no errors
                //Cannot use the PeekChar method here as some characters in the packet capture may fall outside of the bounds of the character encoding - it is a binary format after all!
                //Instead use the position of the binary reader within the stream, stopping once the length of stream has been reached

                while (TheBinaryReader.BaseStream.Position < TheStreamLength && TheResult)
                {
                    ++PacketsProcessed;

                    //
                    //Restore the following lines if you want indication of progress through the packet capture or want to tie an error condition to a particular packet
                    //

                    //System.Diagnostics.Trace.WriteLine
                    //    (
                    //    "Info:  " +
                    //    "Started processing of captured packet #" +
                    //    PacketsProcessed.ToString()
                    //    );

                    //Check whether the end of the packet capture has been reached
                    if (TheBinaryReader.BaseStream.Position < TheStreamLength)
                    {
                        if (ProcessPacketHeader(TheBinaryReader, TheNetworkDataLinkType, TheTimestampAccuracy, out ThePayloadLength, out TheTimestamp))
                        {
                            //Check whether the end of the packet capture has been reached
                            if (TheBinaryReader.BaseStream.Position < TheStreamLength)
                            {
                                switch (TheNetworkDataLinkType)
                                {
                                    case (uint)CommonConstants.NetworkDataLinkType.NullLoopBack:
                                    case (uint)CommonConstants.NetworkDataLinkType.CiscoHDLC:
                                        {
                                            //Just read the bytes off from the packet capture so we can continue
                                            TheBinaryReader.ReadBytes((int)ThePayloadLength);
                                            break;
                                        }

                                    case (uint)CommonConstants.NetworkDataLinkType.Ethernet:
                                        {
                                            if (!TheEthernetFrameProcessing.Process(PacketsProcessed, ThePayloadLength, TheTimestamp))
                                            {
                                                TheResult = false;

                                                System.Diagnostics.Trace.WriteLine
                                                    (
                                                    "Error: " +
                                                    "Processing of the captured packet #" +
                                                    PacketsProcessed.ToString() +
                                                    " failed during processing of packet header for Ethernet frame!!!"
                                                    );
                                            }

                                            break;
                                        }

                                    default:
                                        {
                                            TheResult = false;

                                            System.Diagnostics.Trace.WriteLine
                                                (
                                                "Error: " +
                                                "Processing of the captured packet #" +
                                                PacketsProcessed.ToString() +
                                                " failed during processing of packet header with unknown datalink type!!!"
                                                );

                                            break;
                                        }
                                }
                            }
                            else
                            {
                                //Stop looping as have reached the end of the packet capture
                                break;
                            }
                        }
                        else
                        {
                            TheResult = false;

                            System.Diagnostics.Trace.WriteLine
                                (
                                "Error: " +
                                "Processing of the captured packet #" +
                                PacketsProcessed.ToString() +
                                " failed during processing of Ethernet frame!!!"
                                );

                            //Stop looping as there has been an error!!!
                            break;
                        }
                    }
                    else
                    {
                        //Stop looping as have reached the end of the packet capture
                        break;
                    }

                    if (TheProgressWindowForm != null)
                    {
                        //Update the progress bar to reflect the completion of this iteration of the loop
                        TheProgressWindowForm.ProgressBar.Value = (int)((TheBinaryReader.BaseStream.Position * 100) / TheStreamLength);
                    }
                }
            }

            // If the end of the stream is reached while reading the packet capture, ignore the error as there is no more processing to conduct and we don't want to lose the data we have already processed
            catch (System.IO.EndOfStreamException e)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "Error: " +
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " has been caught and ignored!"
                    );

                TheResult = true;
            }

            if (TheResult)
            {
                //Compute the duration between the start and the end times

                System.DateTime TheEndTime = System.DateTime.Now;

                System.TimeSpan TheDuration = TheEndTime - TheStartTime;

                System.Diagnostics.Trace.WriteLine
                    (
                    "Info:  " +
                    "Finished processing of " +
                    PacketsProcessed.ToString() +
                    " captured packets in " +
                    TheDuration.TotalSeconds.ToString() +
                    " seconds"
                    );
            }

            if (TheProgressWindowForm != null)
            {
                //Update the label and progress bar now the parsing of the packet capture has completed
                TheProgressWindowForm.ProgressBarLabel.Text = "Completed Parsing Of Packet Capture";
                TheProgressWindowForm.ProgressBar.Value = 100;
                TheProgressWindowForm.Refresh();
            }

            return TheResult;
        }
    }
}
