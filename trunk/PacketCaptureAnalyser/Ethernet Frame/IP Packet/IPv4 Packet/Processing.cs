// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv4Packet
{
    /// <summary>
    /// This class provides the IP v4 packet processing
    /// </summary>
    public class Processing
    {
        /// <summary>
        /// 
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// 
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// 
        /// </summary>
        private Structures.HeaderStructure theHeader;

        /// <summary>
        /// 
        /// </summary>
        private ICMPv4Packet.Processing theICMPv4PacketProcessing;

        /// <summary>
        /// 
        /// </summary>
        private IGMPv2Packet.Processing theIGMPv2PacketProcessing;

        /// <summary>
        /// 
        /// </summary>
        private TCPPacket.Processing theTCPPacketProcessing;

        /// <summary>
        /// 
        /// </summary>
        private UDPDatagram.Processing theUDPDatagramProcessing;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="performLatencyAnalysisProcessing">The flag that indicates whether to perform latency analysis processing for data read from the packet capture</param>
        /// <param name="theLatencyAnalysisProcessing">The object that provides the latency analysis processing for data read from the packet capture</param>
        /// <param name="performTimeAnalysisProcessing">The flag that indicates whether to perform time analysis processing for data read from the packet capture</param>
        /// <param name="theTimeAnalysisProcessing">The object that provides the time analysis processing for data read from the packet capture</param>
        public Processing(Analysis.DebugInformation theDebugInformation, System.IO.BinaryReader theBinaryReader, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing)
        {
            this.theDebugInformation = theDebugInformation;

            this.theBinaryReader = theBinaryReader;

            // Create an instance of the IP v4 packet header
            this.theHeader = new Structures.HeaderStructure();

            // Create instances of the processing classes for each protocol
            this.theICMPv4PacketProcessing = new ICMPv4Packet.Processing(theBinaryReader);
            this.theIGMPv2PacketProcessing = new IGMPv2Packet.Processing(theBinaryReader);
            this.theTCPPacketProcessing = new TCPPacket.Processing(theDebugInformation, theBinaryReader, performLatencyAnalysisProcessing, theLatencyAnalysisProcessing, performTimeAnalysisProcessing, theTimeAnalysisProcessing);
            this.theUDPDatagramProcessing = new UDPDatagram.Processing(theDebugInformation, theBinaryReader, performLatencyAnalysisProcessing, theLatencyAnalysisProcessing, performTimeAnalysisProcessing, theTimeAnalysisProcessing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thePayloadLength">The payload length of the Ethernet frame read from the packet capture</param>
        /// <param name="thePacketNumber"></param>
        /// <param name="theTimestamp">The timestamp read from the packet capture</param>
        /// <returns></returns>
        public bool Process(long thePayloadLength, ulong thePacketNumber, double theTimestamp)
        {
            bool theResult = true;

            ushort theHeaderLength;
            ushort thePacketPayloadLength;
            byte theProtocol;

            // Process the IP v4 packet header
            theResult = this.ProcessHeader(thePayloadLength, out theHeaderLength, out thePacketPayloadLength, out theProtocol);

            if (theResult)
            {
                // Process the payload of the IP v4 packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the IP v4 packet header
                theResult = this.ProcessPayload(thePacketNumber, theTimestamp, thePacketPayloadLength, theProtocol);
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thePayloadLength">The payload length of the Ethernet frame read from the packet capture</param>
        /// <param name="theHeaderLength"></param>
        /// <param name="thePacketPayloadLength">The payload length of the IP v6 packet</param>
        /// <param name="theProtocol"></param>
        /// <returns></returns>
        private bool ProcessHeader(long thePayloadLength, out ushort theHeaderLength, out ushort thePacketPayloadLength, out byte theProtocol)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the length of the IP v4 packet header
            theHeaderLength = 0;

            // Provide a default value for the output parameter for the length of the IP v4 packet payload
            thePacketPayloadLength = 0;

            // Provide a default value for the output parameter for the protocol for the IP v4 packet
            theProtocol = 0;

            // Read the values for the IP v4 packet header from the packet capture
            this.theHeader.VersionAndHeaderLength = this.theBinaryReader.ReadByte();
            this.theHeader.TypeOfService = this.theBinaryReader.ReadByte();
            this.theHeader.TotalLength = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theHeader.Identifier = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theHeader.FlagsAndOffset = this.theBinaryReader.ReadUInt16();
            this.theHeader.TimeToLive = this.theBinaryReader.ReadByte();
            this.theHeader.Protocol = this.theBinaryReader.ReadByte();
            this.theHeader.HeaderChecksum = this.theBinaryReader.ReadUInt16();
            this.theHeader.SourceAddress = this.theBinaryReader.ReadInt32();
            this.theHeader.DestinationAddress = this.theBinaryReader.ReadInt32();

            // Determine the version of the IP v4 packet header
            // Need to first extract the version value from the combined IP version/IP header length field
            // We want the higher four bits from the combined IP version/IP header length field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            ushort theHeaderVersion = (ushort)((this.theHeader.VersionAndHeaderLength & 0xF0) >> 4);

            // Determine the length of the IP v4 packet header
            // Need to first extract the length value from the combined IP version/IP header length field
            // We want the lower four bits from the combined IP version/IP header length field (as it's in a big endian representation) so do a bitwise OR with 0xF (i.e. 00001111 in binary)
            // The extracted length value is the length of the IP v4 packet header in 32-bit words so multiply by four to get the actual length in bytes of the IP v4 packet header
            theHeaderLength = (ushort)((this.theHeader.VersionAndHeaderLength & 0xF) * 4);

            // Validate the IP v4 packet header
            theResult = this.ValidateHeader(this.theHeader, thePayloadLength, theHeaderVersion, theHeaderLength);

            if (theResult)
            {
                // Set up the output parameter for the length of the payload of the IP v4 packet (e.g. a TCP packet), which is the total length of the IP v4 packet minus the length of the IP v4 packet header just calculated
                thePacketPayloadLength = (ushort)(this.theHeader.TotalLength - theHeaderLength);

                // Set up the output parameter for the protocol for the IP v4 packet
                theProtocol = this.theHeader.Protocol;

                if (theHeaderLength > Constants.HeaderMinimumLength)
                {
                    // The IP v4 packet contains a header length which is greater than the minimum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    // Just read off these remaining Options bytes of the IP v4 packet header from the packet capture so we can move on
                    this.theBinaryReader.ReadBytes(theHeaderLength - Constants.HeaderMinimumLength);
                }
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thePacketNumber"></param>
        /// <param name="theTimestamp">The timestamp read from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the IP v6 packet</param>
        /// <param name="theProtocol"></param>
        /// <returns></returns>
        private bool ProcessPayload(ulong thePacketNumber, double theTimestamp, ushort thePacketPayloadLength, byte theProtocol)
        {
            bool theResult = true;

            // Process the IP v4 packet based on the value indicated for the protocol in the the IP v4 packet header
            switch (theProtocol)
            {
                case (byte)Constants.Protocol.ICMPv4:
                    {
                        // We have got an IP v4 packet containing an ICMP v4 packet so process it
                        theResult = this.theICMPv4PacketProcessing.Process(thePacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.IGMP:
                    {
                        // We have got an IP v4 packet containing an IGMP v2 packet so process it
                        theResult = this.theIGMPv2PacketProcessing.Process(thePacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.TCP:
                    {
                        // We have got an IP v4 packet containing an TCP packet so process it
                        theResult = this.theTCPPacketProcessing.Process(thePacketNumber, theTimestamp, thePacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.UDP:
                    {
                        // We have got an IP v4 packet containing an UDP datagram so process it
                        theResult = this.theUDPDatagramProcessing.Process(thePacketNumber, theTimestamp, thePacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.EIGRP:
                    {
                        // We have got an IP v4 packet containing a Cisco EIGRP packet

                        // Processing of IP v4 packets containing a Cisco EIGRP packet is not currently supported!

                        // Just record the event and fall through as later processing will read off the remaining payload so we can move on
                        this.theDebugInformation.WriteInformationEvent("The IP v4 packet contains a Cisco EIGRP packet which is not currently supported!");

                        break;
                    }

                default:
                    {
                        //// We have got an IP v4 packet containing an unknown protocol

                        //// Processing of packets with network data link types not enumerated above are obviously not currently supported!

                        this.theDebugInformation.WriteErrorEvent("The IP v4 packet contains an unexpected protocol of " +
                            string.Format("{0:X}", theProtocol) +
                            "!!!");

                        theResult = false;

                        break;
                    }
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theHeader"></param>
        /// <param name="thePayloadLength">The payload length of the Ethernet frame read from the packet capture</param>
        /// <param name="theHeaderVersion"></param>
        /// <param name="theHeaderLength"></param>
        /// <returns></returns>
        private bool ValidateHeader(Structures.HeaderStructure theHeader, long thePayloadLength, ushort theHeaderVersion, ushort theHeaderLength)
        {
            bool theResult = true;

            // Validate the version in the IP v4 packet header
            if (this.theHeader.TotalLength > thePayloadLength)
            {
                //// We have got an IP v4 packet containing an length that is higher than the payload in the Ethernet frame which is invalid

                this.theDebugInformation.WriteErrorEvent("The IP v4 packet indicates a total length of " +
                    this.theHeader.TotalLength.ToString() +
                    " bytes that is greater than the length of the payload of " +
                    thePayloadLength.ToString() +
                    " bytes in the Ethernet frame!!!");

                theResult = false;
            }

            // Validate the version in the IP v4 packet header
            if (theHeaderVersion != Constants.HeaderVersion)
            {
                //// We have got an IP v4 packet header containing an unknown version

                this.theDebugInformation.WriteErrorEvent("The IP v4 packet header contains an unexpected version of " +
                    theHeaderVersion.ToString() +
                    "!!!");

                theResult = false;
            }

            // Validate the length of the IP v4 packet header
            if (theHeaderLength > Constants.HeaderMaximumLength ||
                theHeaderLength < Constants.HeaderMinimumLength)
            {
                //// We have got an IP v4 packet header containing an out of range header length

                this.theDebugInformation.WriteErrorEvent("The IP v4 packet header contains a header length " +
                    theHeaderLength.ToString() +
                    " which is outside the range " +
                    Constants.HeaderMinimumLength.ToString() +
                    " to " +
                    Constants.HeaderMaximumLength.ToString() +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
