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
    class IPv6PacketProcessing
    {
        private System.IO.BinaryReader TheBinaryReader;
        private AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing;
        private AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing;

        public IPv6PacketProcessing(System.IO.BinaryReader TheBinaryReader, AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing, AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing)
        {
            this.TheBinaryReader = TheBinaryReader;
            this.TheLatencyAnalysisProcessing = TheLatencyAnalysisProcessing;
            this.TheTimeAnalysisProcessing = TheTimeAnalysisProcessing;
        }

        public bool Process(long ThePayloadLength, ulong ThePacketNumber, double TheTimestamp)
        {
            bool TheResult = true;

            ushort ThePacketPayloadLength;
            byte TheProtocol;

            //Process the IPv6 packet header
            TheResult = ProcessHeader(ThePayloadLength, out ThePacketPayloadLength, out TheProtocol);

            if (TheResult)
            {
                //Process the payload of the IPv6 packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the IPv6 packet header
                TheResult = ProcessPayload(ThePacketNumber, TheTimestamp, ThePacketPayloadLength, TheProtocol);
            }

            return TheResult;
        }

        private bool ProcessHeader(long ThePayloadLength, out ushort ThePacketPayloadLength, out byte TheProtocol)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the length of the IPv6 packet payload
            ThePacketPayloadLength = 0;

            //Provide a default value for the output parameter for the protocol for the IPv6 packet
            TheProtocol = 0;

            //Create an instance of the IPv6 packet header
            IPv6PacketStructures.IPv6PacketHeaderStructure TheHeader = new IPv6PacketStructures.IPv6PacketHeaderStructure();

            //Read the values for the IPv6 packet header from the packet capture
            TheHeader.VersionAndTrafficClass = TheBinaryReader.ReadByte();
            TheHeader.TrafficClassAndFlowLabel = TheBinaryReader.ReadByte();
            TheHeader.FlowLabel = TheBinaryReader.ReadUInt16();
            TheHeader.PayloadLength = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.NextHeader = TheBinaryReader.ReadByte();
            TheHeader.HopLimit = TheBinaryReader.ReadByte();
            TheHeader.SourceAddressHigh = TheBinaryReader.ReadInt64();
            TheHeader.SourceAddressLow = TheBinaryReader.ReadInt64();
            TheHeader.DestinationAddressHigh = TheBinaryReader.ReadInt64();
            TheHeader.DestinationAddressLow = TheBinaryReader.ReadInt64();

            //Determine the version of the IPv6 packet header
            //Need to first extract the version value from the combined IP packet version and traffic class field
            //We want the higher four bits from the combined IP packet version and traffic class field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            ushort TheHeaderVersion = (ushort)(((TheHeader.VersionAndTrafficClass & 0xF0) >> 4));

            //Validate the IPv6 packet header
            TheResult = ValidateHeader(TheHeader, ThePayloadLength, TheHeaderVersion);

            if (TheResult)
            {
                //Set up the output parameter for the length of the payload of the IPv6 packet (e.g. a TCP packet), which is the total length of the IPv6 packet minus the length of the IPv6 packet header just calculated
                ThePacketPayloadLength = TheHeader.PayloadLength;

                //Set up the output parameter for the protocol for the IPv6 packet
                TheProtocol = TheHeader.NextHeader;
            }

            return TheResult;
        }

        private bool ProcessPayload(ulong ThePacketNumber, double TheTimestamp, ushort ThePayloadLength, byte TheProtocol)
        {
            bool TheResult = true;

            //Process the IPv6 packet based on the value indicated for the protocol in the the IPv6 packet header
            switch (TheProtocol)
            {
                case (byte)IPv6PacketConstants.IPv6PacketProtocol.IGMP:
                    {
                        System.Diagnostics.Trace.WriteLine
                            (
                            "The IPv6 packet contains an IGMP packet, which is not currently supported!"
                            );

                        //Just read off the bytes for the IGMP packet from the packet capture so we can move on
                        TheBinaryReader.ReadBytes(ThePayloadLength);

                        break;
                    }

                case (byte)IPv6PacketConstants.IPv6PacketProtocol.TCP:
                    {
                        TCPPacketNamespace.TCPPacketProcessing TheTCPPacketProcessing = new TCPPacketNamespace.TCPPacketProcessing(TheBinaryReader, TheLatencyAnalysisProcessing, TheTimeAnalysisProcessing);

                        //We've got an IPv6 packet containing an TCP packet so process it
                        TheResult = TheTCPPacketProcessing.Process(ThePacketNumber, TheTimestamp, ThePayloadLength);

                        break;
                    }

                case (byte)IPv6PacketConstants.IPv6PacketProtocol.UDP:
                    {
                        UDPDatagramNamespace.UDPDatagramProcessing TheUDPDatagramProcessing = new UDPDatagramNamespace.UDPDatagramProcessing(TheBinaryReader, TheLatencyAnalysisProcessing, TheTimeAnalysisProcessing);

                        //We've got an IPv6 packet containing an UDP datagram so process it
                        TheResult = TheUDPDatagramProcessing.Process(ThePacketNumber, TheTimestamp, ThePayloadLength);

                        break;
                    }

                case (byte)IPv6PacketConstants.IPv6PacketProtocol.ICMPv6:
                    {
                        System.Diagnostics.Trace.WriteLine
                            (
                            "The IPv6 packet contains an ICMPv6 packet, which is not currently supported!"
                            );

                        //Just read off the bytes for the ICMPv6 packet from the packet capture so we can move on
                        TheBinaryReader.ReadBytes(ThePayloadLength);

                        break;
                    }

                default:
                    {
                        //We've got an IPv6 packet containing an unknown protocol

                        //Processing of packets with network data link types not enumerated above are obviously not currently supported!

                        System.Diagnostics.Trace.WriteLine
                            (
                            "The IPv6 packet contains an unexpected protocol of " +
                            string.Format("{0:X}", TheProtocol)
                            );

                        TheResult = false;

                        break;
                    }
            }

            return TheResult;
        }

        private bool ValidateHeader(IPv6PacketStructures.IPv6PacketHeaderStructure TheHeader, long ThePayloadLength, ushort TheHeaderVersion)
        {
            bool TheResult = true;

            //Validate the version in the IPv4 packet header
            if ((TheHeader.PayloadLength + IPv6PacketConstants.IPv6PacketHeaderLength) != ThePayloadLength)
            {
                //We've got an IPv6 packet containing an length that is higher than the payload in the Ethernet frame which is invalid

                System.Diagnostics.Trace.WriteLine
                    (
                    "The IPv6 packet indicates a total length of " +
                    (TheHeader.PayloadLength + IPv6PacketConstants.IPv6PacketHeaderLength).ToString() +
                    " bytes that does not match the length of the payload of " +
                    ThePayloadLength.ToString() +
                    " bytes in the Ethernet frame!!!"
                    );

                TheResult = false;
            }

            //Validate the version in the IPv6 packet header
            if (TheHeaderVersion != IPv6PacketConstants.IPv6PacketHeaderVersion)
            {
                //We've got an IPv6 packet header containing an unknown version

                System.Diagnostics.Trace.WriteLine
                    (
                    "The IPv6 packet header contains an unexpected version of " +
                    TheHeaderVersion.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}