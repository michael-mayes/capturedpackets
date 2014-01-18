//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace.IPPacketNamespace.IPv4PacketNamespace
{
    class Processing
    {
        private System.IO.BinaryReader TheBinaryReader;

        private Structures.IPv4PacketHeaderStructure TheHeader;

        private bool PerformLatencyAnalysisProcessing;
        private AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing;

        private bool PerformTimeAnalysisProcessing;
        private AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing;

        private ICMPv4PacketNamespace.Processing TheICMPv4PacketProcessing;
        private IGMPv2PacketNamespace.Processing TheIGMPv2PacketProcessing;
        private TCPPacketNamespace.Processing TheTCPPacketProcessing;
        private UDPDatagramNamespace.Processing TheUDPDatagramProcessing;

        public Processing(System.IO.BinaryReader TheBinaryReader, bool PerformLatencyAnalysisProcessing, AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing, bool PerformTimeAnalysisProcessing, AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing)
        {
            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the IPv4 packet header
            TheHeader = new Structures.IPv4PacketHeaderStructure();

            this.PerformLatencyAnalysisProcessing = PerformLatencyAnalysisProcessing;
            this.TheLatencyAnalysisProcessing = TheLatencyAnalysisProcessing;

            this.PerformTimeAnalysisProcessing = PerformTimeAnalysisProcessing;
            this.TheTimeAnalysisProcessing = TheTimeAnalysisProcessing;

            TheICMPv4PacketProcessing = new ICMPv4PacketNamespace.Processing(TheBinaryReader);
            TheIGMPv2PacketProcessing = new IGMPv2PacketNamespace.Processing(TheBinaryReader);
            TheTCPPacketProcessing = new TCPPacketNamespace.Processing(TheBinaryReader, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing);
            TheUDPDatagramProcessing = new UDPDatagramNamespace.Processing(TheBinaryReader, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing);
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

                if (TheHeaderLength > Constants.IPv4PacketHeaderMinimumLength)
                {
                    //The IPv4 packet contains a header length which is greater than the minimum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    //Just read off these remaining Options bytes of the IPv4 packet header from the packet capture so we can move on
                    TheBinaryReader.ReadBytes(TheHeaderLength - Constants.IPv4PacketHeaderMinimumLength);
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
                case (byte)Constants.IPv4PacketProtocol.ICMPv4:
                    {
                        //We've got an IPv4 packet containing an ICMPv4 packet so process it
                        TheResult = TheICMPv4PacketProcessing.Process(ThePayloadLength);

                        break;
                    }

                case (byte)Constants.IPv4PacketProtocol.IGMP:
                    {
                        //We've got an IPv4 packet containing an IGMPv2 packet so process it
                        TheResult = TheIGMPv2PacketProcessing.Process(ThePayloadLength);

                        break;
                    }

                case (byte)Constants.IPv4PacketProtocol.TCP:
                    {
                        //We've got an IPv4 packet containing an TCP packet so process it
                        TheResult = TheTCPPacketProcessing.Process(ThePacketNumber, TheTimestamp, ThePayloadLength);

                        break;
                    }

                case (byte)Constants.IPv4PacketProtocol.UDP:
                    {
                        //We've got an IPv4 packet containing an UDP datagram so process it
                        TheResult = TheUDPDatagramProcessing.Process(ThePacketNumber, TheTimestamp, ThePayloadLength);

                        break;
                    }

                case (byte)Constants.IPv4PacketProtocol.EIGRP:
                    {
                        //We've got an IPv4 packet containing a Cisco EIGRP packet

                        //Processing of IPv4 packets containing a Cisco EIGRP packet is not currently supported!

                        //Just record the event and fall through as later processing will read off the remaining payload so we can move on
                        System.Diagnostics.Trace.WriteLine
                            (
                            "The IPv4 packet contains a Cisco EIGRP packet which is not currently supported!!!"
                            );

                        break;
                    }

                default:
                    {
                        //We've got an IPv4 packet containing an unknown protocol

                        //Processing of packets with network data link types not enumerated above are obviously not currently supported!

                        System.Diagnostics.Trace.WriteLine
                            (
                            "The IPv4 packet contains an unexpected protocol of " +
                            string.Format("{0:X}", TheProtocol)
                            );

                        TheResult = false;

                        break;
                    }
            }

            return TheResult;
        }

        private bool ValidateHeader(Structures.IPv4PacketHeaderStructure TheHeader, long ThePayloadLength, ushort TheHeaderVersion, ushort TheHeaderLength)
        {
            bool TheResult = true;

            //Validate the version in the IPv4 packet header
            if (TheHeader.TotalLength > ThePayloadLength)
            {
                //We've got an IPv4 packet containing an length that is higher than the payload in the Ethernet frame which is invalid

                System.Diagnostics.Trace.WriteLine
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
            if (TheHeaderVersion != Constants.IPv4PacketHeaderVersion)
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
            if (TheHeaderLength > Constants.IPv4PacketHeaderMaximumLength ||
                TheHeaderLength < Constants.IPv4PacketHeaderMinimumLength)
            {
                //We've got an IPv4 packet header containing an out of range header length

                System.Diagnostics.Trace.WriteLine
                    (
                    "The IPv4 packet header contains a header length " +
                    TheHeaderLength.ToString() +
                    " which is outside the range " +
                    Constants.IPv4PacketHeaderMinimumLength.ToString() +
                    " to " +
                    Constants.IPv4PacketHeaderMaximumLength.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}