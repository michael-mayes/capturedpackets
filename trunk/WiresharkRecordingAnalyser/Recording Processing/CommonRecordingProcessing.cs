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

namespace RecordingProcessingNamespace
{
    abstract class CommonRecordingProcessing
    {
        //
        //Abstract methods - must be overriden with a concrete implementation by a derived class
        //

        public abstract bool ProcessRecordingGlobalHeader(System.IO.BinaryReader TheRecordingBinaryReader);

        public abstract bool ProcessRecordingPacketHeader(System.IO.BinaryReader TheRecordingBinaryReader);

        //
        //Concrete methods - cannot be overriden by a derived class
        //

        public bool ProcessRecording(string TheRecordingPath)
        {
            bool TheResult = true;

            if (System.IO.File.Exists(TheRecordingPath))
            {
                //Open a stream for the recording for reading
                System.IO.FileStream TheRecording = System.IO.File.OpenRead(TheRecordingPath);

                //Open a binary reader for the stream for the recording
                System.IO.BinaryReader TheRecordingBinaryReader = new System.IO.BinaryReader(TheRecording);

                //Set position of the binary reader to the beginning of the stream
                TheRecordingBinaryReader.BaseStream.Position = 0;

                //Read the recording global header
                //Only continue reading from the recording if the recording global header was read successfully
                if (ProcessRecordingGlobalHeader(TheRecordingBinaryReader))
                {
                    //Process all packets int the recording
                    TheResult = ProcessRecordingPackets(TheRecordingBinaryReader);
                }
                else
                {
                    TheResult = false;
                }

                //Close both the binary reader and the underlying stream for the recording
                TheRecordingBinaryReader.Close();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("The recording " + TheRecordingPath + " does not exist!!!");

                TheResult = false;
            }

            return TheResult;
        }

        public bool ProcessRecordingPackets(System.IO.BinaryReader TheRecordingBinaryReader)
        {
            bool TheResult = true;

            try
            {
                int RecordingPacketNumber = 0;

                EthernetFrameNamespace.EthernetFrameProcessing TheEthernetFrameProcessing = new EthernetFrameNamespace.EthernetFrameProcessing();

                //Keep looping through the recording, processing each recording packet header and Ethernet frame in turn, while characters remain in the recording and there are no errors
                //Cannot use the PeekChar method here as some characters in the file may fall outside of the range of UTF-8 character encoding - it is a binary file after all!
                //Instead use the position of the binary reader within the stream, stopping once the length of stream has been reached
                while (TheRecordingBinaryReader.BaseStream.Position < TheRecordingBinaryReader.BaseStream.Length)
                {
                    System.Diagnostics.Debug.WriteLine("Starting processing of recording packet #{0}", ++RecordingPacketNumber);

                    //Check whether the end of the recording has been reached
                    if (TheRecordingBinaryReader.BaseStream.Position < TheRecordingBinaryReader.BaseStream.Length)
                    {
                        //Process the recording packet header
                        if (ProcessRecordingPacketHeader(TheRecordingBinaryReader))
                        {
                            //Check whether the end of the recording has been reached
                            if (TheRecordingBinaryReader.BaseStream.Position < TheRecordingBinaryReader.BaseStream.Length)
                            {
                                //Process the Ethernet frame
                                if (TheEthernetFrameProcessing.ProcessEthernetFrame(TheRecordingBinaryReader))
                                {
                                    //Start the next iteration of the loop i.e. perform the validation for the while loop condition
                                    continue;
                                }
                                else
                                {
                                    TheResult = false;

                                    System.Diagnostics.Debug.WriteLine("Processing of recording packet #{0} failed during processing of packet header!!!", RecordingPacketNumber);

                                    //Stop looping as there has been an error!!!
                                    break;
                                }
                            }
                            else
                            {
                                //Stop looping as have reached the end of the recording
                                break;
                            }

                        }
                        else
                        {
                            TheResult = false;

                            System.Diagnostics.Debug.WriteLine("Processing of recording packet #{0} failed during processing of Ethernet frame!!!", RecordingPacketNumber);

                            //Stop looping as there has been an error!!!
                            break;
                        }
                    }
                    else
                    {
                        //Stop looping as have reached the end of the recording
                        break;
                    }
                }
            }

            // If the end of the stream is reached while reading the recording, ignore the error as there is no more processing to conduct and we don't want to lose the data we have already processed
            catch (System.IO.EndOfStreamException e)
            {
                System.Diagnostics.Debug.WriteLine("The exception " + e.GetType().Name + " has been caught and ignored!");

                TheResult = true;
            }

            return TheResult;
        }
    }
}