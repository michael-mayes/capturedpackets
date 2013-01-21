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

namespace EthernetFrameNamespace.IPv4PacketNamespace.TCPPacketNamespace
{
    class TCPPacketProcessing
    {
        private System.IO.BinaryReader TheBinaryReader;
        private AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing;
        private AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing;

        public TCPPacketProcessing(System.IO.BinaryReader TheBinaryReader, AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing, AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing)
        {
            this.TheBinaryReader = TheBinaryReader;
            this.TheLatencyAnalysisProcessing = TheLatencyAnalysisProcessing;
            this.TheTimeAnalysisProcessing = TheTimeAnalysisProcessing;
        }

        public bool Process(double TheTimestamp, long ThePayloadLength, int TheTCPPacketLength)
        {
            bool TheResult = true;

            int TheTCPPacketPayloadLength = 0;
            int TheSourcePort = 0;
            int TheDestinationPort = 0;

            //Process the TCP packet header
            TheResult = ProcessHeader(TheTCPPacketLength, out TheTCPPacketPayloadLength, out TheSourcePort, out TheDestinationPort);

            if (TheResult)
            {
                //Process the payload of the TCP packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the TCP packet header
                TheResult = ProcessPayload(TheTimestamp, ThePayloadLength, TheTCPPacketPayloadLength, TheSourcePort, TheDestinationPort);
            }

            return TheResult;
        }

        private bool ProcessHeader(int TheLength, out int ThePayloadLength, out int TheSourcePort, out int TheDestinationPort)
        {
            bool TheResult = true;

            //Provide default values for the output parameters for source port and destination port
            TheSourcePort = 0;
            TheDestinationPort = 0;

            //Create an instance of the TCP packet header
            TCPPacketStructures.TCPPacketHeaderStructure TheHeader = new TCPPacketStructures.TCPPacketHeaderStructure();

            //Read the values for the TCP packet header from the packet capture
            TheHeader.SourcePort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.DestinationPort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.SequenceNumber = TheBinaryReader.ReadUInt32();
            TheHeader.AcknowledgmentNumber = TheBinaryReader.ReadUInt32();
            TheHeader.DataOffsetAndReservedAndNSFlag = TheBinaryReader.ReadByte();
            TheHeader.Flags = TheBinaryReader.ReadByte();
            TheHeader.WindowSize = TheBinaryReader.ReadUInt16();
            TheHeader.Checksum = TheBinaryReader.ReadUInt16();
            TheHeader.UrgentPointer = TheBinaryReader.ReadUInt16();

            //Determine the length of the TCP packet header
            //Need to first extract the length value from the combined TCP packet header length, reserved fields and NS flag field
            //We want the higher four bits from the combined TCP packet header length, reserved fields and NS flag field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            //The extracted length value is the length of the TCP packet header in 32-bit words so multiply by four to get the actual length in bytes of the TCP packet header
            int TheHeaderLength = (((TheHeader.DataOffsetAndReservedAndNSFlag & 0xF0) >> 4) * 4);

            //Validate the TCP packet header
            TheResult = ValidateHeader(TheHeader, TheHeaderLength);

            if (TheResult)
            {
                //Set up the output parameters for source port and destination port using the value read from the TCP packet header
                TheSourcePort = TheHeader.SourcePort;
                TheDestinationPort = TheHeader.DestinationPort;

                //Set up the output parameter for the length of the payload of the TCP packet, which is the total length of the TCP packet minus the length of the TCP packet header just calculated
                ThePayloadLength = (TheLength - TheHeaderLength);

                if (TheHeaderLength > TCPPacketConstants.TCPPacketHeaderMinimumLength &&
                    TheHeaderLength <= TCPPacketConstants.TCPPacketHeaderMaximumLength)
                {
                    //The TCP packet contains a header length which is greater than the minimum and less than or equal to the maximum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    //Just read off these remaining Options bytes of the TCP packet header from the packet capture so we can move on
                    for (int i = TCPPacketConstants.TCPPacketHeaderMinimumLength; i < TheHeaderLength; ++i)
                    {
                        TheBinaryReader.ReadByte();
                    }
                }
            }
            else
            {
                ThePayloadLength = 0;
            }

            return TheResult;
        }

        private bool ProcessPayload(double TheTimestamp, long ThePayloadLength, int TheTCPPacketPayloadLength, int TheSourcePort, int TheDestinationPort)
        {
            bool TheResult = true;

            //Only process this TCP packet if the payload has a non-zero payload length i.e. it actually includes data so is not part of the three-way handshake or a plain acknowledgement
            if (TheTCPPacketPayloadLength > 0)
            {
                //Change this logic statement to allow identification and processing of specific messages within the TCP packet
                if (false)
                {
                    //Put code here to identify and process specific messages within the TCP packet
                }
                else
                {
                    //Just read off the remaining bytes of the TCP packet from the packet capture so we can move on
                    //The remaining length is the supplied length of the TCP packet payload
                    TheBinaryReader.ReadBytes(TheTCPPacketPayloadLength);
                }
            }

            return TheResult;
        }

        private bool ValidateHeader(TCPPacketStructures.TCPPacketHeaderStructure TheHeader, int TheHeaderLength)
        {
            bool TheResult = true;

            if (TheHeaderLength > TCPPacketConstants.TCPPacketHeaderMaximumLength ||
                TheHeaderLength < TCPPacketConstants.TCPPacketHeaderMinimumLength)
            {
                System.Diagnostics.Debug.WriteLine("The TCP packet contains a header length {0} which is outside the range {1} to {2}", TheHeaderLength, TCPPacketConstants.TCPPacketHeaderMinimumLength, TCPPacketConstants.TCPPacketHeaderMaximumLength);

                TheResult = false;
            }

            return TheResult;
        }
    }
}