// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame.IPPacket.IPv4Packet
{
    /// <summary>
    /// This class provides the IP v4 packet processing
    /// </summary>
    public class Processing
    {
        /// <summary>
        /// The object that provides for the logging of debug information
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// The object that provides for binary reading from the packet capture
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// The reusable instance of the processing class for ICMP v4 packets
        /// </summary>
        private ICMPv4Packet.Processing theICMPv4PacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for IGMP v2 packets
        /// </summary>
        private IGMPv2Packet.Processing theIGMPv2PacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for TCP packets
        /// </summary>
        private TCPPacket.Processing theTCPPacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for UDP datagrams
        /// </summary>
        private UDPDatagram.Processing theUDPDatagramProcessing;

        /// <summary>
        /// The reusable instance of the processing class for EIGRP packets
        /// </summary>
        private EIGRPPacket.Processing theEIGRPPacketProcessing;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="performLatencyAnalysisProcessing">Boolean flag that indicates whether to perform latency analysis processing for data read from the packet capture</param>
        /// <param name="theLatencyAnalysisProcessing">The object that provides the latency analysis processing for data read from the packet capture</param>
        /// <param name="performBurstAnalysisProcessing">Boolean flag that indicates whether to perform burst analysis processing for data read from the packet capture</param>
        /// <param name="theBurstAnalysisProcessing">The object that provides the burst analysis processing for data read from the packet capture</param>
        /// <param name="performTimeAnalysisProcessing">Boolean flag that indicates whether to perform time analysis processing for data read from the packet capture</param>
        /// <param name="theTimeAnalysisProcessing">The object that provides the time analysis processing for data read from the packet capture</param>
        /// <param name="useAlternativeSequenceNumber">Boolean flag that indicates whether to use the alternative sequence number in the data read from the packet capture, required for legacy recordings</param>
        public Processing(Analysis.DebugInformation theDebugInformation, System.IO.BinaryReader theBinaryReader, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performBurstAnalysisProcessing, Analysis.BurstAnalysis.Processing theBurstAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing, bool useAlternativeSequenceNumber)
        {
            this.theDebugInformation = theDebugInformation;

            this.theBinaryReader = theBinaryReader;

            //// Create instances of the processing classes for each protocol

            this.theICMPv4PacketProcessing = new ICMPv4Packet.Processing(theBinaryReader);

            this.theIGMPv2PacketProcessing = new IGMPv2Packet.Processing(theBinaryReader);

            this.theTCPPacketProcessing =
                new TCPPacket.Processing(
                    theDebugInformation,
                    theBinaryReader,
                    performLatencyAnalysisProcessing,
                    theLatencyAnalysisProcessing,
                    performBurstAnalysisProcessing,
                    theBurstAnalysisProcessing,
                    performTimeAnalysisProcessing,
                    theTimeAnalysisProcessing,
                    useAlternativeSequenceNumber);

            this.theUDPDatagramProcessing =
                new UDPDatagram.Processing(
                    theDebugInformation,
                    theBinaryReader,
                    performLatencyAnalysisProcessing,
                    theLatencyAnalysisProcessing,
                    performBurstAnalysisProcessing,
                    theBurstAnalysisProcessing,
                    performTimeAnalysisProcessing,
                    theTimeAnalysisProcessing,
                    useAlternativeSequenceNumber);

            this.theEIGRPPacketProcessing = new EIGRPPacket.Processing(theBinaryReader);
        }

        /// <summary>
        /// Processes an IP v4 packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the IP v4 packet could be processed</returns>
        public bool ProcessIPv4Packet(long theEthernetFrameLength, ulong thePacketNumber, double thePacketTimestamp)
        {
            bool theResult = true;

            ushort theIPv4HeaderLength;
            ushort theIPv4PacketPayloadLength;
            byte theIPv4PacketProtocol;

            // Process the IP v4 packet header
            theResult = this.ProcessIPv4PacketHeader(
                theEthernetFrameLength,
                out theIPv4HeaderLength,
                out theIPv4PacketPayloadLength,
                out theIPv4PacketProtocol);

            if (theResult)
            {
                // Process the payload of the IP v4 packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the IP v4 packet header
                theResult = this.ProcessIPv4PacketPayload(
                    thePacketNumber,
                    thePacketTimestamp,
                    theIPv4PacketPayloadLength,
                    theIPv4PacketProtocol);
            }

            return theResult;
        }

        /// <summary>
        /// Processes an IP v4 packet header
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <param name="theIPv4PacketHeaderLength">The length of the header for the IP v4 packet</param>
        /// <param name="theIPv4PacketPayloadLength">The length of the payload of the IP v4 packet</param>
        /// <param name="theIPv4PacketProtocol">The protocol for the IP v4 packet</param>
        /// <returns>Boolean flag that indicates whether the IP v4 packet header could be processed</returns>
        private bool ProcessIPv4PacketHeader(long theEthernetFrameLength, out ushort theIPv4PacketHeaderLength, out ushort theIPv4PacketPayloadLength, out byte theIPv4PacketProtocol)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the length of the IP v4 packet header
            theIPv4PacketHeaderLength = 0;

            // Provide a default value for the output parameter for the length of the IP v4 packet payload
            theIPv4PacketPayloadLength = 0;

            // Provide a default value for the output parameter for the protocol for the IP v4 packet
            theIPv4PacketProtocol = 0;

            // Read the values for the IP v4 packet header from the packet capture

            // Store the IP packet version and IP v4 packet header length for use below
            byte theVersionAndHeaderLength = this.theBinaryReader.ReadByte();

            // Just read off the bytes for the IP v4 packet header type of service from the packet capture so we can move on
            this.theBinaryReader.ReadByte();

            // Store the total length of the IP v4 packet for use below
            ushort theIPv4PacketTotalLength = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());

            // Just read off the bytes for the IP v4 packet header identifier from the packet capture so we can move on
            this.theBinaryReader.ReadInt16();

            // Just read off the bytes for the IP v4 packet header flags and offset from the packet capture so we can move on
            this.theBinaryReader.ReadUInt16();

            // Just read off the bytes for the IP v4 packet header time to live from the packet capture so we can move on
            this.theBinaryReader.ReadByte();

            // Set up the output parameter for the protocol for the IP v4 packet
            theIPv4PacketProtocol = this.theBinaryReader.ReadByte();

            // Just read off the bytes for the IP v4 packet header checksum from the packet capture so we can move on
            this.theBinaryReader.ReadUInt16();

            // Just read off the bytes for the IP v4 packet header source address from the packet capture so we can move on
            this.theBinaryReader.ReadInt32();

            // Just read off the bytes for the IP v4 packet header destination address from the packet capture so we can move on
            this.theBinaryReader.ReadInt32();

            // Determine the version of the IP v4 packet header
            // Need to first extract the version value from the combined IP version/IP header length field
            // We want the higher four bits from the combined IP version/IP header length field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            ushort theIPv4PacketHeaderVersion =
                (ushort)((theVersionAndHeaderLength & 0xF0) >> 4);

            // Determine the length of the IP v4 packet header
            // Need to first extract the length value from the combined IP version/IP header length field
            // We want the lower four bits from the combined IP version/IP header length field (as it's in a big endian representation) so do a bitwise OR with 0xF (i.e. 00001111 in binary)
            // The extracted length value is the length of the IP v4 packet header in 32-bit words so multiply by four to get the actual length in bytes of the IP v4 packet header
            theIPv4PacketHeaderLength =
                (ushort)((theVersionAndHeaderLength & 0xF) * 4);

            // Validate the IP v4 packet header
            theResult = this.ValidateIPv4PacketHeader(
                theEthernetFrameLength,
                theIPv4PacketHeaderVersion,
                theIPv4PacketHeaderLength,
                theIPv4PacketTotalLength);

            if (theResult)
            {
                // Set up the output parameter for the length of the payload of the IP v4 packet (e.g. a TCP packet), which is the total length of the IP v4 packet minus the length of the IP v4 packet header just calculated
                theIPv4PacketPayloadLength =
                    (ushort)(theIPv4PacketTotalLength - theIPv4PacketHeaderLength);

                if (theIPv4PacketHeaderLength > Constants.HeaderMinimumLength)
                {
                    // The IP v4 packet contains a header length which is greater than the minimum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    // Just read off these remaining Options bytes of the IP v4 packet header from the packet capture so we can move on
                    this.theBinaryReader.ReadBytes(
                        theIPv4PacketHeaderLength -
                        Constants.HeaderMinimumLength);
                }
            }

            return theResult;
        }

        /// <summary>
        /// Processes the payload of the IP v4 packet
        /// </summary>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <param name="theIPv4PacketPayloadLength">The length of the payload of the IP v4 packet</param>
        /// <param name="theIPv4Protocol">The protocol for the IP v4 packet</param>
        /// <returns>Boolean flag that indicates whether the payload of the IP v4 packet could be processed</returns>
        private bool ProcessIPv4PacketPayload(ulong thePacketNumber, double thePacketTimestamp, ushort theIPv4PacketPayloadLength, byte theIPv4Protocol)
        {
            bool theResult = true;

            // Process the IP v4 packet based on the value indicated for the protocol in the the IP v4 packet header
            switch (theIPv4Protocol)
            {
                case (byte)Constants.Protocol.ICMP:
                    {
                        // We have got an IP v4 packet containing an ICMP v4 packet so process it
                        this.theICMPv4PacketProcessing.ProcessICMPv4Packet(
                            theIPv4PacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.IGMP:
                    {
                        // We have got an IP v4 packet containing an IGMP v2 packet so process it
                        this.theIGMPv2PacketProcessing.ProcessIGMPv2Packet();

                        break;
                    }

                case (byte)Constants.Protocol.TCP:
                    {
                        // We have got an IP v4 packet containing an TCP packet so process it
                        theResult = this.theTCPPacketProcessing.ProcessTCPPacket(
                            thePacketNumber,
                            thePacketTimestamp,
                            theIPv4PacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.UDP:
                    {
                        // We have got an IP v4 packet containing an UDP datagram so process it
                        theResult = this.theUDPDatagramProcessing.ProcessUDPDatagram(
                            thePacketNumber,
                            thePacketTimestamp,
                            theIPv4PacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.EIGRP:
                    {
                        // We have got an IP v4 packet containing a Cisco EIGRP packet so process it
                        this.theEIGRPPacketProcessing.ProcessEIGRPPacket(
                            theIPv4PacketPayloadLength);

                        break;
                    }

                default:
                    {
                        //// We have got an IP v4 packet containing an unknown protocol

                        //// Processing of packets with network data link types not enumerated above are obviously not currently supported!

                        this.theDebugInformation.WriteErrorEvent(
                            "The IP v4 packet contains an unexpected protocol of " +
                            string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:X}", theIPv4Protocol) +
                            "!!!");

                        theResult = false;

                        break;
                    }
            }

            return theResult;
        }

        /// <summary>
        /// Validates the IP v4 packet header
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <param name="theIPv4HeaderVersion">The version of the IP v4 packet header</param>
        /// <param name="theIPv4HeaderLength">The length of the IP v4 packet header</param>
        /// <param name="theIPv4PacketTotalLength">The total length of the IP v4 packet</param>
        /// <returns>Boolean flag that indicates whether the IP v4 packet header is valid</returns>
        private bool ValidateIPv4PacketHeader(long theEthernetFrameLength, ushort theIPv4HeaderVersion, ushort theIPv4HeaderLength, ushort theIPv4PacketTotalLength)
        {
            bool theResult = true;

            // Validate the version in the IP v4 packet header
            if (theIPv4PacketTotalLength > theEthernetFrameLength)
            {
                //// We have got an IP v4 packet containing a total length that is higher than the payload in the Ethernet frame which is invalid

                this.theDebugInformation.WriteErrorEvent(
                    "The IP v4 packet indicates a total length of " +
                    theIPv4PacketTotalLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " bytes that is greater than the length of " +
                    theEthernetFrameLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " bytes in the Ethernet frame!!!");

                theResult = false;
            }

            // Validate the version in the IP v4 packet header
            if (theIPv4HeaderVersion != Constants.HeaderVersion)
            {
                //// We have got an IP v4 packet header containing an unknown version

                this.theDebugInformation.WriteErrorEvent(
                    "The IP v4 packet header contains an unexpected version of " +
                    theIPv4HeaderVersion.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            // Validate the length of the IP v4 packet header
            if (theIPv4HeaderLength > Constants.HeaderMaximumLength ||
                theIPv4HeaderLength < Constants.HeaderMinimumLength)
            {
                //// We have got an IP v4 packet header containing an out of range header length

                this.theDebugInformation.WriteErrorEvent(
                    "The IP v4 packet header contains a header length " +
                    theIPv4HeaderLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " which is outside the range " +
                    Constants.HeaderMinimumLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " to " +
                    Constants.HeaderMaximumLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
