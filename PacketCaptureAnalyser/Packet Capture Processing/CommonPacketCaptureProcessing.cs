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
        //Abstract methods - must be overriden with a concrete implementation by a derived class
        //

        public abstract bool ProcessGlobalHeader(System.IO.BinaryReader TheBinaryReader, out double TheTimestampAccuracy);

        public abstract bool ProcessPacketHeader(System.IO.BinaryReader TheBinaryReader, double TheTimestampAccuracy, out double TheTimestamp);

        //
        //Concrete methods - cannot be overriden by a derived class
        //

        public bool Process(string ThePacketCapture)
        {
            bool TheResult = true;

            if (System.IO.File.Exists(ThePacketCapture))
            {
                //Read the start time to allow later calculation of the duration of the processing
                System.DateTime TheStartTime = System.DateTime.Now;

                System.Diagnostics.Debug.WriteLine("Starting read of all bytes from " + ThePacketCapture + " packet capture");

                //Read all the bytes from the packet capture into an array
                byte[] TheBytes = System.IO.File.ReadAllBytes(ThePacketCapture);

                //Compute the duration between the start and the end times

                System.DateTime TheEndTime = System.DateTime.Now;

                System.TimeSpan TheDuration = TheEndTime - TheStartTime;

                System.Diagnostics.Debug.WriteLine("Finished read of all bytes from " + ThePacketCapture + " packet capture in {0} seconds", TheDuration.Seconds);

                //Create a memory stream to read the packet capture from the byte array
                System.IO.MemoryStream TheMemoryStream = new System.IO.MemoryStream(TheBytes);

                //Open a binary reader for the memory stream for the packet capture
                System.IO.BinaryReader TheBinaryReader = new System.IO.BinaryReader(TheMemoryStream);

                //Ensure that the position of the binary reader is set to the beginning of the memory stream
                TheBinaryReader.BaseStream.Position = 0;

                //Declare an entity to be used for the timestamp accuracy extracted from the packet capture global header
                double TheTimestampAccuracy = 0.0;

                //Only continue reading from the packet capture if the packet capture global header was read successfully
                if (ProcessGlobalHeader(TheBinaryReader, out TheTimestampAccuracy))
                {
                    TheResult = ProcessPackets(TheBinaryReader, TheTimestampAccuracy);
                }
                else
                {
                    TheResult = false;
                }

                //Close both the binary reader and the underlying memory stream
                TheBinaryReader.Close();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("The " + ThePacketCapture + " packet capture does not exist!!!");

                TheResult = false;
            }

            return TheResult;
        }

        public bool ProcessPackets(System.IO.BinaryReader TheBinaryReader, double TheTimestampAccuracy)
        {
            bool TheResult = true;

            int PacketsProcessed = 0;

            //Read the start time to allow later calculation of the duration of the processing
            System.DateTime TheStartTime = System.DateTime.Now;

            System.Diagnostics.Debug.WriteLine("Started processing of captured packets");

            //Attempt to process the packets in the packet capture
            try
            {
                EthernetFrameNamespace.EthernetFrameProcessing TheEthernetFrameProcessing = new EthernetFrameNamespace.EthernetFrameProcessing();

                //Store the length of the stream locally - the .NET framework does not cache it so each query requires an expensive read - this is OK so long as not editing the file at the same time as analysing it
                long TheStreamLength = TheBinaryReader.BaseStream.Length;

                //Declare an entity to be used for each timestamp extracted from a packet header
                double TheTimestamp = 0.0;

                //Keep looping through the packet capture, processing each packet header and Ethernet frame in turn, while characters remain in the packet capture and there are no errors
                //Cannot use the PeekChar method here as some characters in the file may fall outside of the bounds of the character encoding - it is a binary file after all!
                //Instead use the position of the binary reader within the stream, stopping once the length of stream has been reached

                while (TheBinaryReader.BaseStream.Position < TheStreamLength)
                {
                    ++PacketsProcessed;

                    //Check whether the end of the packet capture has been reached
                    if (TheBinaryReader.BaseStream.Position < TheStreamLength)
                    {
                        if (ProcessPacketHeader(TheBinaryReader, TheTimestampAccuracy, out TheTimestamp))
                        {
                            //Check whether the end of the packet capture has been reached
                            if (TheBinaryReader.BaseStream.Position < TheStreamLength)
                            {
                                if (TheEthernetFrameProcessing.Process(TheBinaryReader))
                                {
                                    //Start the next iteration of the loop
                                    continue;
                                }
                                else
                                {
                                    TheResult = false;

                                    System.Diagnostics.Debug.WriteLine("Processing of captured packet #{0} failed during processing of packet header!!!", PacketsProcessed);

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

                            System.Diagnostics.Debug.WriteLine("Processing of captured packet #{0} failed during processing of Ethernet frame!!!", PacketsProcessed);

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
                System.Diagnostics.Debug.WriteLine("The exception " + e.GetType().Name + " has been caught and ignored!");

                TheResult = true;
            }

            if (TheResult)
            {
                //Compute the duration between the start and the end times

                System.DateTime TheEndTime = System.DateTime.Now;

                System.TimeSpan TheDuration = TheEndTime - TheStartTime;

                System.Diagnostics.Debug.WriteLine("Finished processing of {0} captured packets in {1} seconds", PacketsProcessed, TheDuration.TotalSeconds);
            }

            return TheResult;
        }
    }
}