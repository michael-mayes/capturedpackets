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

namespace PacketCaptureProcessingNamespace
{
    abstract class CommonPacketCaptureProcessing
    {
        //
        //Abstract methods - must be overridden with a concrete implementation by a derived class
        //

        public abstract bool ProcessGlobalHeader(System.IO.BinaryReader TheBinaryReader, out double TheTimestampAccuracy);

        public abstract bool ProcessPacketHeader(System.IO.BinaryReader TheBinaryReader, double TheTimestampAccuracy, out long ThePayloadLength, out double TheTimestamp);

        //
        //Concrete methods - cannot be overridden by a derived class
        //

        public bool Process(AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing, AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing, string ThePacketCapture)
        {
            bool TheResult = true;

            try
            {
                if (System.IO.File.Exists(ThePacketCapture))
                {
                    //Read the start time to allow later calculation of the duration of the processing
                    System.DateTime TheStartTime = System.DateTime.Now;

                    System.Diagnostics.Trace.WriteLine
                        (
                        "Starting read of all bytes from the " +
                        ThePacketCapture +
                        " packet capture"
                        );

                    //Read all the bytes from the packet capture into an array
                    byte[] TheBytes = System.IO.File.ReadAllBytes(ThePacketCapture);

                    //Compute the duration between the start and the end times

                    System.DateTime TheEndTime = System.DateTime.Now;

                    System.TimeSpan TheDuration = TheEndTime - TheStartTime;

                    System.Diagnostics.Trace.WriteLine
                        (
                        "Finished read of all bytes from the " +
                        ThePacketCapture +
                        " packet capture in " +
                        TheDuration.Seconds.ToString() +
                        " seconds"
                        );

                    //Create a memory stream to read the packet capture from the byte array
                    using (System.IO.MemoryStream TheMemoryStream = new System.IO.MemoryStream(TheBytes))
                    {
                        //Open a binary reader for the memory stream for the packet capture
                        using (System.IO.BinaryReader TheBinaryReader = new System.IO.BinaryReader(TheMemoryStream))
                        {
                            //Ensure that the position of the binary reader is set to the beginning of the memory stream
                            TheBinaryReader.BaseStream.Position = 0;

                            //Declare an entity to be used for the timestamp accuracy extracted from the packet capture global header
                            double TheTimestampAccuracy = 0.0;

                            //Only continue reading from the packet capture if the packet capture global header was read successfully
                            if (ProcessGlobalHeader(TheBinaryReader, out TheTimestampAccuracy))
                            {
                                TheResult = ProcessPackets(TheBinaryReader, TheLatencyAnalysisProcessing, TheTimeAnalysisProcessing, TheTimestampAccuracy);
                            }
                            else
                            {
                                TheResult = false;
                            }
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine
                        (
                        "The " +
                        ThePacketCapture +
                        " packet capture does not exist!!!"
                        );

                    TheResult = false;
                }
            }

            catch (System.IO.IOException e)
            {

                System.Diagnostics.Trace.WriteLine
                    (
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as access to the " +
                    ThePacketCapture +
                    " packet capture was denied because it is being used by another process!!!"
                    );

                TheResult = false;
            }

            catch (System.UnauthorizedAccessException e)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The exception " +
                    e.GetType().Name +
                    " with the following message: " +
                    e.Message +
                    " was raised as access to the " +
                    ThePacketCapture +
                    " packet capture was denied because this process was deemed as unauthorised by the OS!!!"
                    );

                TheResult = false;
            }

            return TheResult;
        }

        public bool ProcessPackets(System.IO.BinaryReader TheBinaryReader, AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing, AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing, double TheTimestampAccuracy)
        {
            bool TheResult = true;

            long PacketsProcessed = 0;

            //Read the start time to allow later calculation of the duration of the processing
            System.DateTime TheStartTime = System.DateTime.Now;

            System.Diagnostics.Trace.WriteLine
                (
                "Started processing of the captured packets"
                );

            //Attempt to process the packets in the packet capture
            try
            {
                EthernetFrameNamespace.EthernetFrameProcessing TheEthernetFrameProcessing = new EthernetFrameNamespace.EthernetFrameProcessing(TheBinaryReader, TheLatencyAnalysisProcessing, TheTimeAnalysisProcessing);

                //Store the length of the stream locally - the .NET framework does not cache it so each query requires an expensive read - this is OK so long as not editing the file at the same time as analysing it
                long TheStreamLength = TheBinaryReader.BaseStream.Length;

                //Declare an entity to be used for each payload length extracted from a packet header
                long ThePayloadLength = 0;

                //Declare an entity to be used for each timestamp extracted from a packet header
                double TheTimestamp = 0.0;

                //Keep looping through the packet capture, processing each packet header and Ethernet frame in turn, while characters remain in the packet capture and there are no errors
                //Cannot use the PeekChar method here as some characters in the packet capture may fall outside of the bounds of the character encoding - it is a binary format after all!
                //Instead use the position of the binary reader within the stream, stopping once the length of stream has been reached

                while (TheBinaryReader.BaseStream.Position < TheStreamLength)
                {
                    ++PacketsProcessed;

                    //
                    //Restore the following lines if you want indication of progress through the packet capture or want to tie an error condition to a particular packet
                    //

                    //System.Diagnostics.Trace.WriteLine
                        //(
                        //"Started processing of captured packet #" +
                        //PacketsProcessed.ToString()
                        //);

                    //Check whether the end of the packet capture has been reached
                    if (TheBinaryReader.BaseStream.Position < TheStreamLength)
                    {
                        if (ProcessPacketHeader(TheBinaryReader, TheTimestampAccuracy, out ThePayloadLength, out TheTimestamp))
                        {
                            //Check whether the end of the packet capture has been reached
                            if (TheBinaryReader.BaseStream.Position < TheStreamLength)
                            {
                                if (TheEthernetFrameProcessing.Process(ThePayloadLength, TheTimestamp))
                                {
                                    //Start the next iteration of the loop
                                    continue;
                                }
                                else
                                {
                                    TheResult = false;

                                    System.Diagnostics.Trace.WriteLine
                                        (
                                        "Processing of the captured packet #" +
                                        PacketsProcessed.ToString() +
                                        " failed during processing of packet header!!!"
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

                        }
                        else
                        {
                            TheResult = false;

                            System.Diagnostics.Trace.WriteLine
                                (
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
                }
            }

            // If the end of the stream is reached while reading the packet capture, ignore the error as there is no more processing to conduct and we don't want to lose the data we have already processed
            catch (System.IO.EndOfStreamException e)
            {
                System.Diagnostics.Trace.WriteLine
                    (
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
                    "Finished processing of " +
                    PacketsProcessed.ToString() +
                    " captured packets in " +
                    TheDuration.TotalSeconds.ToString() +
                    " seconds"
                    );
            }

            return TheResult;
        }
    }
}