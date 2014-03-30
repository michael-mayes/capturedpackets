//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv6Packet
{
    class Processing
    {
        private Analysis.DebugInformation TheDebugInformation;

        private System.IO.BinaryReader TheBinaryReader;

        private Structures.HeaderStructure TheHeader;

        private TCPPacket.Processing TheTCPPacketProcessing;
        private UDPDatagram.Processing TheUDPDatagramProcessing;

        public Processing(Analysis.DebugInformation TheDebugInformation, System.IO.BinaryReader TheBinaryReader, bool PerformLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing TheLatencyAnalysisProcessing, bool PerformTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing TheTimeAnalysisProcessing)
        {
            this.TheDebugInformation = TheDebugInformation;

            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the IPv6 packet header
            TheHeader = new Structures.HeaderStructure();

            //Create instances of the processing classes for each protocol
            TheTCPPacketProcessing = new TCPPacket.Processing(TheDebugInformation, TheBinaryReader, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing);
            TheUDPDatagramProcessing = new UDPDatagram.Processing(TheDebugInformation, TheBinaryReader, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing);
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
                case (byte)Constants.Protocol.TCP:
                    {
                        //We've got an IPv6 packet containing an TCP packet so process it
                        TheResult = TheTCPPacketProcessing.Process(ThePacketNumber, TheTimestamp, ThePayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.UDP:
                    {
                        //We've got an IPv6 packet containing an UDP datagram so process it
                        TheResult = TheUDPDatagramProcessing.Process(ThePacketNumber, TheTimestamp, ThePayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.ICMPv6:
                    {
                        //We've got an IPv6 packet containing an ICMPv6 packet

                        //Processing of IPv6 packets containing an ICMPv6 packet is not currently supported!

                        TheDebugInformation.WriteInformationEvent
                            (
                            "The IPv6 packet contains an ICMPv6 packet, which is not currently supported!"
                            );

                        //Just read off the bytes for the ICMPv6 packet from the packet capture so we can move on
                        TheBinaryReader.ReadBytes(ThePayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.EIGRP:
                    {
                        //We've got an IPv6 packet containing a Cisco EIGRP packet

                        //Processing of IPv6 packets containing a Cisco EIGRP packet is not currently supported!

                        //Just record the event and fall through as later processing will read off the remaining payload so we can move on
                        TheDebugInformation.WriteInformationEvent
                            (
                            "The IPv6 packet contains a Cisco EIGRP packet which is not currently supported!"
                            );

                        break;
                    }

                default:
                    {
                        //We've got an IPv6 packet containing an unknown protocol

                        //Processing of packets with network data link types not enumerated above are obviously not currently supported!

                        TheDebugInformation.WriteErrorEvent
                            (
                            "The IPv6 packet contains an unexpected protocol of " +
                            string.Format("{0:X}", TheProtocol) +
                            "!!!"
                            );

                        TheResult = false;

                        break;
                    }
            }

            return TheResult;
        }

        private bool ValidateHeader(Structures.HeaderStructure TheHeader, long ThePayloadLength, ushort TheHeaderVersion)
        {
            bool TheResult = true;

            //Validate the version in the IPv4 packet header
            if ((TheHeader.PayloadLength + Constants.HeaderLength) > ThePayloadLength)
            {
                //We've got an IPv6 packet containing an length that is higher than the payload in the Ethernet frame which is invalid

                TheDebugInformation.WriteErrorEvent
                    (
                    "The IPv6 packet indicates a total length of " +
                    (TheHeader.PayloadLength + Constants.HeaderLength).ToString() +
                    " bytes that is greater than the length of the payload of " +
                    ThePayloadLength.ToString() +
                    " bytes in the Ethernet frame!!!"
                    );

                TheResult = false;
            }

            //Validate the version in the IPv6 packet header
            if (TheHeaderVersion != Constants.HeaderVersion)
            {
                //We've got an IPv6 packet header containing an unknown version

                TheDebugInformation.WriteErrorEvent
                    (
                    "The IPv6 packet header contains an unexpected version of " +
                    TheHeaderVersion.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}
