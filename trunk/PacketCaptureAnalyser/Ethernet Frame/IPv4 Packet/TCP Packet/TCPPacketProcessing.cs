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

            //Process the TCP packet header
            TheResult = ProcessTCPPacketHeader(ThePackageCaptureBinaryReader);

            //Just read off the remaining bytes of the TCP packet from the packet capture so we can move on
            //The remaining length is the total supplied length of the ICMPv4 packet minus the length for the TCP packet header
            for (int i = 0; i < (TheTCPPacketLength - TCPPacketConstants.TCPPacketHeaderLength); ++i)
            {
                ThePackageCaptureBinaryReader.ReadByte();
            }

            return TheResult;
        }

        private bool ProcessTCPPacketHeader(System.IO.BinaryReader ThePackageCaptureBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the TCP packet header
            TCPPacketStructures.TCPPacketHeaderStructure TheTCPPacketHeader = new TCPPacketStructures.TCPPacketHeaderStructure();

            //Read the values for the TCP packet header from the packet capture
            TheTCPPacketHeader.SourcePort = ThePackageCaptureBinaryReader.ReadUInt16();
            TheTCPPacketHeader.DestinationPort = ThePackageCaptureBinaryReader.ReadUInt16();
            TheTCPPacketHeader.SequenceNumber = ThePackageCaptureBinaryReader.ReadUInt32();
            TheTCPPacketHeader.AcknowledgmentNumber = ThePackageCaptureBinaryReader.ReadUInt32();
            TheTCPPacketHeader.DataOffsetAndReservedAndNSFlag = ThePackageCaptureBinaryReader.ReadByte();
            TheTCPPacketHeader.Flags = ThePackageCaptureBinaryReader.ReadByte();
            TheTCPPacketHeader.WindowSize = ThePackageCaptureBinaryReader.ReadUInt16();
            TheTCPPacketHeader.Checksum = ThePackageCaptureBinaryReader.ReadUInt16();
            TheTCPPacketHeader.UrgentPointer = ThePackageCaptureBinaryReader.ReadUInt16();

            return TheResult;
        }
    }
}