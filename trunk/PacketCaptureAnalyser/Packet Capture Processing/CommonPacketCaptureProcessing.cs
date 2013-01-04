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

        public abstract bool ProcessPacketCaptureGlobalHeader(System.IO.BinaryReader ThePackageCaptureBinaryReader);

        public abstract bool ProcessPacketCapturePacketHeader(System.IO.BinaryReader ThePackageCaptureBinaryReader);

        //
        //Concrete methods - cannot be overriden by a derived class
        //

        public bool ProcessPacketCapture(string ThePacketCapture)
        {
            bool TheResult = true;

            if (System.IO.File.Exists(ThePacketCapture))
            {
                //Read the start time for reading all bytes from the packet capture
                System.DateTime TheReadAllBytesStartTime = System.DateTime.Now;

                System.Diagnostics.Debug.WriteLine("Starting read of all bytes from " + ThePacketCapture + " packet capture");

                //Read all the bytes from the packet capture into an array
                byte[] ThePackageCaptureBytes = System.IO.File.ReadAllBytes(ThePacketCapture);

                //Read the end time for reading all bytes from the packet capture
                System.DateTime TheReadAllBytesEndTime = System.DateTime.Now;

                //Compute the duration between the start and the end times
                System.TimeSpan TheReadAllBytesDuration = TheReadAllBytesEndTime - TheReadAllBytesStartTime;

                System.Diagnostics.Debug.WriteLine("Finished read of all bytes from " + ThePacketCapture + " packet capture in {0} seconds", TheReadAllBytesDuration.Seconds);

                //Create a memory stream to read the packet capture from the byte array
                System.IO.MemoryStream TheMemoryStream = new System.IO.MemoryStream(ThePackageCaptureBytes);

                //Open a binary reader for the memory stream for the packet capture
                System.IO.BinaryReader ThePackageCaptureBinaryReader = new System.IO.BinaryReader(TheMemoryStream);

                //Ensure that the position of the binary reader is set to the beginning of the memory stream
                ThePackageCaptureBinaryReader.BaseStream.Position = 0;

                //Read the packet capture global header
                //Only continue reading from the packet capture if the packet capture global header was read successfully
                if (ProcessPacketCaptureGlobalHeader(ThePackageCaptureBinaryReader))
                {
                    //Process all packets in the packet capture
                    TheResult = ProcessPacketCapturePackets(ThePackageCaptureBinaryReader);
                }
                else
                {
                    TheResult = false;
                }

                //Close both the binary reader and the underlying stream for the packet capture
                ThePackageCaptureBinaryReader.Close();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("The " + ThePacketCapture + " packet capture does not exist!!!");

                TheResult = false;
            }

            return TheResult;
        }

        public bool ProcessPacketCapturePackets(System.IO.BinaryReader ThePackageCaptureBinaryReader)
        {
            bool TheResult = true;

            int PacketCapturePacketNumber = 0;

            //Read the start time
            System.DateTime TheStartTime = System.DateTime.Now;

            //Attempt to process the packets in the packet capture

            System.Diagnostics.Debug.WriteLine("Started processing of captured packets");

            try
            {
                EthernetFrameNamespace.EthernetFrameProcessing TheEthernetFrameProcessing = new EthernetFrameNamespace.EthernetFrameProcessing();

                //Store the length of the stream locally - the .NET framework does not cache it so each query requires an expensive read - this is OK so long as not editing the file at the same time as analysing it
                long TheStreamLength = ThePackageCaptureBinaryReader.BaseStream.Length;

                //Keep looping through the packet capture, processing each packet capture packet header and Ethernet frame in turn, while characters remain in the packet capture and there are no errors
                //Cannot use the PeekChar method here as some characters in the file may fall outside of the range of UTF-8 character encoding - it is a binary file after all!
                //Instead use the position of the binary reader within the stream, stopping once the length of stream has been reached
                while (ThePackageCaptureBinaryReader.BaseStream.Position < TheStreamLength)
                {
                    //Increment the counter for the number captured packets processed
                    ++PacketCapturePacketNumber;

                    //Check whether the end of the packet capture has been reached
                    if (ThePackageCaptureBinaryReader.BaseStream.Position < TheStreamLength)
                    {
                        //Process the packet capture packet header
                        if (ProcessPacketCapturePacketHeader(ThePackageCaptureBinaryReader))
                        {
                            //Check whether the end of the packet capture has been reached
                            if (ThePackageCaptureBinaryReader.BaseStream.Position < TheStreamLength)
                            {
                                //Process the Ethernet frame
                                if (TheEthernetFrameProcessing.ProcessEthernetFrame(ThePackageCaptureBinaryReader))
                                {
                                    //Start the next iteration of the loop i.e. perform the validation for the while loop condition
                                    continue;
                                }
                                else
                                {
                                    TheResult = false;

                                    System.Diagnostics.Debug.WriteLine("Processing of captured packet #{0} failed during processing of packet header!!!", PacketCapturePacketNumber);

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

                            System.Diagnostics.Debug.WriteLine("Processing of captured packet #{0} failed during processing of Ethernet frame!!!", PacketCapturePacketNumber);

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
                //Read the end time
                System.DateTime TheEndTime = System.DateTime.Now;

                //Compute the duration between the start and the end times
                System.TimeSpan TheDuration = TheEndTime - TheStartTime;

                System.Diagnostics.Debug.WriteLine("Finished processing {0} captured packets in {1} seconds", PacketCapturePacketNumber, TheDuration.TotalSeconds);
            }

            return TheResult;
        }
    }
}