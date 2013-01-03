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
        public bool ProcessTCPPacket(System.IO.BinaryReader ThePackageCaptureBinaryReader, int TheTCPPacketLength)
        {
            bool TheResult = true;

            int TheTCPPacketPayloadLength = 0;
            int TheTCPPacketSourcePort = 0;
            int TheTCPPacketDestinationPort = 0;

            //Process the TCP packet header
            TheResult = ProcessTCPPacketHeader(ThePackageCaptureBinaryReader, TheTCPPacketLength, out TheTCPPacketPayloadLength, out TheTCPPacketSourcePort, out TheTCPPacketDestinationPort);

            if (TheResult)
            {
                //Process the payload of the TCP packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the TCP packet header
                TheResult = ProcessTCPPacketPayload(ThePackageCaptureBinaryReader, TheTCPPacketPayloadLength, TheTCPPacketSourcePort, TheTCPPacketDestinationPort);
            }

            return TheResult;
        }

        private bool ProcessTCPPacketHeader(System.IO.BinaryReader ThePackageCaptureBinaryReader, int TheTCPPacketLength, out int TheTCPPacketPayloadLength, out int TheTCPPacketSourcePort, out int TheTCPPacketDestinationPort)
        {
            bool TheResult = true;

            //Create an instance of the TCP packet header
            TCPPacketStructures.TCPPacketHeaderStructure TheTCPPacketHeader = new TCPPacketStructures.TCPPacketHeaderStructure();

            //Read the values for the TCP packet header from the packet capture
            TheTCPPacketHeader.SourcePort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheTCPPacketHeader.DestinationPort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheTCPPacketHeader.SequenceNumber = ThePackageCaptureBinaryReader.ReadUInt32();
            TheTCPPacketHeader.AcknowledgmentNumber = ThePackageCaptureBinaryReader.ReadUInt32();
            TheTCPPacketHeader.DataOffsetAndReservedAndNSFlag = ThePackageCaptureBinaryReader.ReadByte();
            TheTCPPacketHeader.Flags = ThePackageCaptureBinaryReader.ReadByte();
            TheTCPPacketHeader.WindowSize = ThePackageCaptureBinaryReader.ReadUInt16();
            TheTCPPacketHeader.Checksum = ThePackageCaptureBinaryReader.ReadUInt16();
            TheTCPPacketHeader.UrgentPointer = ThePackageCaptureBinaryReader.ReadUInt16();

            //Set up the output parameters for source port and destination port using the value read from the TCP packet header
            TheTCPPacketSourcePort = TheTCPPacketHeader.SourcePort;
            TheTCPPacketDestinationPort = TheTCPPacketHeader.DestinationPort;

            //Determine the length of the TCP packet header
            //Need to first extract the length value from the combined TCP packet header length, reserved fields and NS flag field
            //We want the higher four bits from the combined TCP packet header length, reserved fields and NS flag field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            //The extracted length value is the length of the TCP packet header in 32-bit words so multiply by four to get the actual length in bytes of the TCP packet header
            int TheTCPPacketHeaderLength = (((TheTCPPacketHeader.DataOffsetAndReservedAndNSFlag & 0xF0) >> 4) * 4);

            //Set up the output parameter for the length of the payload of the TCP packet, which is the total length of the TCP packet minus the length of the TCP packet header just calculated
            TheTCPPacketPayloadLength = (TheTCPPacketLength - TheTCPPacketHeaderLength);

            //Validate length of the TCP packet header
            if (TheTCPPacketHeaderLength > TCPPacketConstants.TCPPacketHeaderMaximumLength ||
                TheTCPPacketHeaderLength < TCPPacketConstants.TCPPacketHeaderMinimumLength)
            {
                System.Diagnostics.Debug.WriteLine("The TCP packet contains a header length {0} which is outside the range {1} to {2}", TheTCPPacketHeaderLength, TCPPacketConstants.TCPPacketHeaderMinimumLength, TCPPacketConstants.TCPPacketHeaderMaximumLength);

                TheResult = false;
            }
            else
            {
                if (TheTCPPacketHeaderLength > TCPPacketConstants.TCPPacketHeaderMinimumLength &&
                    TheTCPPacketHeaderLength <= TCPPacketConstants.TCPPacketHeaderMaximumLength)
                {
                    //The TCP packet contains a header length which is greater than the minimum and less than or equal to the maximum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    //Just read off these remaining Options bytes of the TCP packet header from the packet capture so we can move on
                    for (int i = TCPPacketConstants.TCPPacketHeaderMinimumLength; i < TheTCPPacketHeaderLength; ++i)
                    {
                        ThePackageCaptureBinaryReader.ReadByte();
                    }
                }
            }

            return TheResult;
        }

        private bool ProcessTCPPacketPayload(System.IO.BinaryReader ThePackageCaptureBinaryReader, int TheTCPPacketPayloadLength, int TheTCPPacketSourcePort, int TheTCPPacketDestinationPort)
        {
            bool TheResult = true;

            //Just read off the remaining bytes of the TCP packet from the packet capture so we can move on
            //The remaining length is the total supplied length of the TCP packet minus the length for the TCP packet header
            for (int i = 0; i < TheTCPPacketPayloadLength; ++i)
            {
                ThePackageCaptureBinaryReader.ReadByte();
            }

            return TheResult;

        }
    }
}