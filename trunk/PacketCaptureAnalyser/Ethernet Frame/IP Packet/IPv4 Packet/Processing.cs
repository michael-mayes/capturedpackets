//$Id$

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv4Packet
{
    class Processing
    {
        private Analysis.DebugInformation TheDebugInformation;

        private System.IO.BinaryReader TheBinaryReader;

        private Structures.HeaderStructure TheHeader;

        private ICMPv4Packet.Processing TheICMPv4PacketProcessing;
        private IGMPv2Packet.Processing TheIGMPv2PacketProcessing;
        private TCPPacket.Processing TheTCPPacketProcessing;
        private UDPDatagram.Processing TheUDPDatagramProcessing;

        public Processing(Analysis.DebugInformation TheDebugInformation, System.IO.BinaryReader TheBinaryReader, bool PerformLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing TheLatencyAnalysisProcessing, bool PerformTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing TheTimeAnalysisProcessing)
        {
            this.TheDebugInformation = TheDebugInformation;

            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the IPv4 packet header
            TheHeader = new Structures.HeaderStructure();

            //Create instances of the processing classes for each protocol
            TheICMPv4PacketProcessing = new ICMPv4Packet.Processing(TheBinaryReader);
            TheIGMPv2PacketProcessing = new IGMPv2Packet.Processing(TheBinaryReader);
            TheTCPPacketProcessing = new TCPPacket.Processing(TheDebugInformation, TheBinaryReader, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing);
            TheUDPDatagramProcessing = new UDPDatagram.Processing(TheDebugInformation, TheBinaryReader, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing);
        }

        public bool Process(long ThePayloadLength, ulong ThePacketNumber, double TheTimestamp)
        {
            bool TheResult = true;

            ushort TheHeaderLength;
            ushort ThePacketPayloadLength;
            byte TheProtocol;

            //Process the IPv4 packet header
            TheResult = ProcessHeader(ThePayloadLength, out TheHeaderLength, out ThePacketPayloadLength, out TheProtocol);

            if (TheResult)
            {
                //Process the payload of the IPv4 packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the IPv4 packet header
                TheResult = ProcessPayload(ThePacketNumber, TheTimestamp, ThePacketPayloadLength, TheProtocol);
            }

            return TheResult;
        }

        private bool ProcessHeader(long ThePayloadLength, out ushort TheHeaderLength, out ushort ThePacketPayloadLength, out byte TheProtocol)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the length of the IPv4 packet header
            TheHeaderLength = 0;

            //Provide a default value for the output parameter for the length of the IPv4 packet payload
            ThePacketPayloadLength = 0;

            //Provide a default value for the output parameter for the protocol for the IPv4 packet
            TheProtocol = 0;

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
                ThePacketPayloadLength = (ushort)(TheHeader.TotalLength - TheHeaderLength);

                //Set up the output parameter for the protocol for the IPv4 packet
                TheProtocol = TheHeader.Protocol;

                if (TheHeaderLength > Constants.HeaderMinimumLength)
                {
                    //The IPv4 packet contains a header length which is greater than the minimum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    //Just read off these remaining Options bytes of the IPv4 packet header from the packet capture so we can move on
                    TheBinaryReader.ReadBytes(TheHeaderLength - Constants.HeaderMinimumLength);
                }
            }

            return TheResult;
        }

        private bool ProcessPayload(ulong ThePacketNumber, double TheTimestamp, ushort ThePayloadLength, byte TheProtocol)
        {
            bool TheResult = true;

            //Process the IPv4 packet based on the value indicated for the protocol in the the IPv4 packet header
            switch (TheProtocol)
            {
                case (byte)Constants.Protocol.ICMPv4:
                    {
                        //We've got an IPv4 packet containing an ICMPv4 packet so process it
                        TheResult = TheICMPv4PacketProcessing.Process(ThePayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.IGMP:
                    {
                        //We've got an IPv4 packet containing an IGMPv2 packet so process it
                        TheResult = TheIGMPv2PacketProcessing.Process(ThePayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.TCP:
                    {
                        //We've got an IPv4 packet containing an TCP packet so process it
                        TheResult = TheTCPPacketProcessing.Process(ThePacketNumber, TheTimestamp, ThePayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.UDP:
                    {
                        //We've got an IPv4 packet containing an UDP datagram so process it
                        TheResult = TheUDPDatagramProcessing.Process(ThePacketNumber, TheTimestamp, ThePayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.EIGRP:
                    {
                        //We've got an IPv4 packet containing a Cisco EIGRP packet

                        //Processing of IPv4 packets containing a Cisco EIGRP packet is not currently supported!

                        //Just record the event and fall through as later processing will read off the remaining payload so we can move on
                        TheDebugInformation.WriteInformationEvent
                            (
                            "The IPv4 packet contains a Cisco EIGRP packet which is not currently supported!"
                            );

                        break;
                    }

                default:
                    {
                        //We've got an IPv4 packet containing an unknown protocol

                        //Processing of packets with network data link types not enumerated above are obviously not currently supported!

                        TheDebugInformation.WriteErrorEvent
                            (
                            "The IPv4 packet contains an unexpected protocol of " +
                            string.Format("{0:X}", TheProtocol) +
                            "!!!"
                            );

                        TheResult = false;

                        break;
                    }
            }

            return TheResult;
        }

        private bool ValidateHeader(Structures.HeaderStructure TheHeader, long ThePayloadLength, ushort TheHeaderVersion, ushort TheHeaderLength)
        {
            bool TheResult = true;

            //Validate the version in the IPv4 packet header
            if (TheHeader.TotalLength > ThePayloadLength)
            {
                //We've got an IPv4 packet containing an length that is higher than the payload in the Ethernet frame which is invalid

                TheDebugInformation.WriteErrorEvent
                    (
                    "The IPv4 packet indicates a total length of " +
                    TheHeader.TotalLength.ToString() +
                    " bytes that is greater than the length of the payload of " +
                    ThePayloadLength.ToString() +
                    " bytes in the Ethernet frame!!!"
                    );

                TheResult = false;
            }

            //Validate the version in the IPv4 packet header
            if (TheHeaderVersion != Constants.HeaderVersion)
            {
                //We've got an IPv4 packet header containing an unknown version

                TheDebugInformation.WriteErrorEvent
                    (
                    "The IPv4 packet header contains an unexpected version of " +
                    TheHeaderVersion.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            //Validate the length of the IPv4 packet header
            if (TheHeaderLength > Constants.HeaderMaximumLength ||
                TheHeaderLength < Constants.HeaderMinimumLength)
            {
                //We've got an IPv4 packet header containing an out of range header length

                TheDebugInformation.WriteErrorEvent
                    (
                    "The IPv4 packet header contains a header length " +
                    TheHeaderLength.ToString() +
                    " which is outside the range " +
                    Constants.HeaderMinimumLength.ToString() +
                    " to " +
                    Constants.HeaderMaximumLength.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}
