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

namespace EthernetFrameNamespace.IPPacketNamespace
{
    class IPv4PacketProcessing
    {
        private System.IO.BinaryReader TheBinaryReader;
        private AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing;
        private AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing;

        public IPv4PacketProcessing(System.IO.BinaryReader TheBinaryReader, AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing, AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing)
        {
            this.TheBinaryReader = TheBinaryReader;
            this.TheLatencyAnalysisProcessing = TheLatencyAnalysisProcessing;
            this.TheTimeAnalysisProcessing = TheTimeAnalysisProcessing;
        }

        public bool Process(long ThePayloadLength, ulong ThePacketNumber, double TheTimestamp)
        {
            bool TheResult = true;

            ushort TheIPv4PacketHeaderLength;
            ushort TheIPv4PacketPayloadLength;
            byte TheIPv4PacketProtocol;

            //Process the IPv4 packet header
            TheResult = ProcessHeader(ThePayloadLength, out TheIPv4PacketHeaderLength, out TheIPv4PacketPayloadLength, out TheIPv4PacketProtocol);

            if (TheResult)
            {
                //Process the payload of the IPv4 packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the IPv4 packet header
                TheResult = ProcessPayload(ThePacketNumber, TheTimestamp, TheIPv4PacketPayloadLength, TheIPv4PacketProtocol);
            }

            return TheResult;
        }

        private bool ProcessHeader(long ThePayloadLength, out ushort TheHeaderLength, out ushort TheIPv4PacketPayloadLength, out byte TheProtocol)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the length of the IPv4 packet header
            TheHeaderLength = 0;

            //Provide a default value for the output parameter for the length of the IPv4 packet payload
            TheIPv4PacketPayloadLength = 0;

            //Provide a default value for the output parameter for the protocol for the IPv4 packet
            TheProtocol = 0;

            //Create an instance of the IPv4 packet header
            IPv4PacketStructures.IPv4PacketHeaderStructure TheHeader = new IPv4PacketStructures.IPv4PacketHeaderStructure();

            //Read the values for the IPv4 packet header from the packet capture
            TheHeader.VersionAndHeaderLength = TheBinaryReader.ReadByte();
            TheHeader.TypeOfService = TheBinaryReader.ReadByte();
            TheHeader.TotalLength = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.Identifier = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.FlagsAndOffset = TheBinaryReader.ReadUInt16();
            TheHeader.TimeToLive = TheBinaryReader.ReadByte();
            TheHeader.Protocol = TheBinaryReader.ReadByte();
            TheHeader.HeaderChecksum = TheBinaryReader.ReadUInt16();
            TheHeader.SourceAddress = TheBinaryReader.ReadInt32();
            TheHeader.DestinationAddress = TheBinaryReader.ReadInt32();

            //Determine the version of the IPv4 packet header
            //Need to first extract the version value from the combined IP version/IP header length field
            //We want the higher four bits from the combined IP version/IP header length field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            ushort TheHeaderVersion = (ushort)(((TheHeader.VersionAndHeaderLength & 0xF0) >> 4));

            //Determine the length of the IPv4 packet header
            //Need to first extract the length value from the combined IP version/IP header length field
            //We want the lower four bits from the combined IP version/IP header length field (as it's in a big endian representation) so do a bitwise OR with 0xF (i.e. 00001111 in binary)
            //The extracted length value is the length of the IPv4 packet header in 32-bit words so multiply by four to get the actual length in bytes of the IPv4 packet header
            TheHeaderLength = (ushort)((TheHeader.VersionAndHeaderLength & 0xF) * 4);

            //Validate the IPv4 packet header
            TheResult = ValidateHeader(TheHeader, ThePayloadLength, TheHeaderVersion, TheHeaderLength);

            if (TheResult)
            {
                //Set up the output parameter for the length of the payload of the IPv4 packet (e.g. a TCP packet), which is the total length of the IPv4 packet minus the length of the IPv4 packet header just calculated
                TheIPv4PacketPayloadLength = (ushort)(TheHeader.TotalLength - TheHeaderLength);

                //Set up the output parameter for the protocol for the IPv4 packet
                TheProtocol = TheHeader.Protocol;

                if (TheHeaderLength > IPv4PacketConstants.IPv4PacketHeaderMinimumLength)
                {
                    //The IPv4 packet contains a header length which is greater than the minimum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    //Just read off these remaining Options bytes of the IPv4 packet header from the packet capture so we can move on
                    TheBinaryReader.ReadBytes(TheHeaderLength - IPv4PacketConstants.IPv4PacketHeaderMinimumLength);
                }
            }

            return TheResult;
        }

        private bool ProcessPayload(ulong ThePacketNumber, double TheTimestamp, ushort TheIPv4PacketPayloadLength, byte TheIPv4PacketProtocol)
        {
            bool TheResult = true;

            //Process the IPv4 packet based on the value indicated for the protocol in the the IPv4 packet header
            switch (TheIPv4PacketProtocol)
            {
                case (byte)IPv4PacketConstants.IPv4PacketProtocol.ICMPv4:
                    {
                        ICMPv4PacketNamespace.ICMPv4PacketProcessing TheICMPv4PacketProcessing = new ICMPv4PacketNamespace.ICMPv4PacketProcessing(TheBinaryReader);

                        //We've got an IPv4 packet containing an ICMPv4 packet so process it
                        TheResult = TheICMPv4PacketProcessing.Process(TheIPv4PacketPayloadLength);

                        break;
                    }

                case (byte)IPv4PacketConstants.IPv4PacketProtocol.IGMP:
                    {
                        IGMPv2PacketNamespace.IGMPv2PacketProcessing TheIGMPv2PacketProcessing = new IGMPv2PacketNamespace.IGMPv2PacketProcessing(TheBinaryReader);

                        //We've got an IPv4 packet containing an IGMPv2 packet so process it
                        TheResult = TheIGMPv2PacketProcessing.Process(TheIPv4PacketPayloadLength);

                        break;
                    }

                case (byte)IPv4PacketConstants.IPv4PacketProtocol.TCP:
                    {
                        TCPPacketNamespace.TCPPacketProcessing TheTCPPacketProcessing = new TCPPacketNamespace.TCPPacketProcessing(TheBinaryReader, TheLatencyAnalysisProcessing, TheTimeAnalysisProcessing);

                        //We've got an IPv4 packet containing an TCP packet so process it
                        TheResult = TheTCPPacketProcessing.Process(ThePacketNumber, TheTimestamp, TheIPv4PacketPayloadLength);

                        break;
                    }

                case (byte)IPv4PacketConstants.IPv4PacketProtocol.UDP:
                    {
                        UDPDatagramNamespace.UDPDatagramProcessing TheUDPDatagramProcessing = new UDPDatagramNamespace.UDPDatagramProcessing(TheBinaryReader, TheLatencyAnalysisProcessing, TheTimeAnalysisProcessing);

                        //We've got an IPv4 packet containing an UDP datagram so process it
                        TheResult = TheUDPDatagramProcessing.Process(ThePacketNumber, TheTimestamp, TheIPv4PacketPayloadLength);

                        break;
                    }

                default:
                    {
                        //We've got an IPv4 packet containing an unknown protocol

                        //Processing of packets with network data link types not enumerated above are obviously not currently supported!

                        System.Diagnostics.Trace.WriteLine
                            (
                            "The IPv4 packet contains an unexpected protocol of " +
                            string.Format("{0:X}", TheIPv4PacketProtocol)
                            );

                        TheResult = false;

                        break;
                    }
            }

            return TheResult;
        }

        private bool ValidateHeader(IPv4PacketStructures.IPv4PacketHeaderStructure TheHeader, long ThePayloadLength, ushort TheHeaderVersion, ushort TheHeaderLength)
        {
            bool TheResult = true;

            //Validate the version in the IPv4 packet header
            if (TheHeader.TotalLength != ThePayloadLength)
            {
                //We've got an IPv4 packet containing an length that is higher than the payload in the Ethernet frame which is invalid

                System.Diagnostics.Trace.WriteLine
                    (
                    "The IPv4 packet indicates a total length of " +
                    TheHeader.TotalLength.ToString() +
                    " bytes does not match the length of the payload of " +
                    ThePayloadLength.ToString() +
                    " bytes in the Ethernet frame!!!"
                    );

                TheResult = false;
            }

            //Validate the version in the IPv4 packet header
            if (TheHeaderVersion != IPv4PacketConstants.IPv4PacketHeaderVersion)
            {
                //We've got an IPv4 packet header containing an unknown version

                System.Diagnostics.Trace.WriteLine
                    (
                    "The IPv4 packet header contains an unexpected version of " +
                    TheHeaderVersion.ToString()
                    );

                TheResult = false;
            }

            //Validate the length of the IPv4 packet header
            if (TheHeaderLength > IPv4PacketConstants.IPv4PacketHeaderMaximumLength ||
                TheHeaderLength < IPv4PacketConstants.IPv4PacketHeaderMinimumLength)
            {
                //We've got an IPv4 packet header containing an out of range header length

                System.Diagnostics.Trace.WriteLine
                    (
                    "The IPv4 packet header contains a header length " +
                    TheHeaderLength.ToString() +
                    " which is outside the range " +
                    IPv4PacketConstants.IPv4PacketHeaderMinimumLength.ToString() +
                    " to " +
                    IPv4PacketConstants.IPv4PacketHeaderMaximumLength.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}