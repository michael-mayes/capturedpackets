// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.IPPacket.IPv6Packet
{
    /// <summary>
    /// This class provides the IP v6 packet processing
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
        /// The reusable instance of the processing class for TCP packets
        /// </summary>
        private TCPPacket.Processing theTCPPacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for UDP datagrams
        /// </summary>
        private UDPDatagram.Processing theUDPDatagramProcessing;

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
        }

        /// <summary>
        /// Processes an IP v6 packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the IP v6 packet could be processed</returns>
        public bool ProcessIPv6Packet(long theEthernetFrameLength, ulong thePacketNumber, double thePacketTimestamp)
        {
            bool theResult = true;

            ushort theIPv6PacketPayloadLength;
            byte theIPv6PacketProtocol;

            // Process the IP v6 packet header
            theResult = this.ProcessIPv6PacketHeader(
                theEthernetFrameLength,
                out theIPv6PacketPayloadLength,
                out theIPv6PacketProtocol);

            if (theResult)
            {
                // Process the payload of the IP v6 packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the IP v6 packet header
                theResult = this.ProcessIPv6PacketPayload(
                    thePacketNumber,
                    thePacketTimestamp,
                    theIPv6PacketPayloadLength,
                    theIPv6PacketProtocol);
            }

            return theResult;
        }

        /// <summary>
        /// Processes an IP v6 packet header
        /// </summary>
        /// <param name="theEthernetFrameLength">The length read from the packet capture for the Ethernet frame</param>
        /// <param name="theIPv6PacketPayloadLength">The payload length of the IP v6 packet</param>
        /// <param name="theIPv6PacketProtocol">The protocol for the IP v6 packet</param>
        /// <returns>Boolean flag that indicates whether the IP v6 packet header could be processed</returns>
        private bool ProcessIPv6PacketHeader(long theEthernetFrameLength, out ushort theIPv6PacketPayloadLength, out byte theIPv6PacketProtocol)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the length of the IP v6 packet payload
            theIPv6PacketPayloadLength = 0;

            // Provide a default value for the output parameter for the protocol for the IP v6 packet
            theIPv6PacketProtocol = 0;

            // Read the values for the IP v6 packet header from the packet capture

            // Store the IP packet version and traffic class for use below
            byte theVersionAndTrafficClass = this.theBinaryReader.ReadByte();

            // Just read off the bytes for the IPv6 packet header traffic class and flow label from the packet capture so we can move on
            this.theBinaryReader.ReadByte();
            this.theBinaryReader.ReadUInt16();

            // Set up the output parameter for the length of the payload of the IP v6 packet (e.g. a TCP packet), which is the total length of the IP v6 packet minus the length of the IP v6 packet header just calculated
            theIPv6PacketPayloadLength =
                (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());

            // Set up the output parameter for the protocol for the IP v6 packet
            theIPv6PacketProtocol = this.theBinaryReader.ReadByte();

            // Just read off the bytes for the IPv6 packet header hop limit from the packet capture so we can move on
            this.theBinaryReader.ReadByte();

            // Just read off the bytes for the IPv6 packet header hop limit from the packet capture so we can move on
            this.theBinaryReader.ReadInt64(); // High bytes
            this.theBinaryReader.ReadInt64(); // Low bytes

            // Just read off the bytes for the IPv6 packet destination from the packet capture so we can move on
            this.theBinaryReader.ReadInt64(); // High bytes
            this.theBinaryReader.ReadInt64(); // Low bytes

            // Determine the version of the IP v6 packet header
            // Need to first extract the version value from the combined IP packet version and traffic class field
            // We want the higher four bits from the combined IP packet version and traffic class field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            ushort theIPv6PacketHeaderVersion =
                (ushort)((theVersionAndTrafficClass & 0xF0) >> 4);

            // Validate the IP v6 packet header
            theResult = this.ValidateIPv6PacketHeader(
                theEthernetFrameLength,
                theIPv6PacketHeaderVersion,
                theIPv6PacketPayloadLength);

            return theResult;
        }

        /// <summary>
        /// Processes the payload of the IP v6 packet
        /// </summary>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <param name="theIPv6PacketPayloadLength">The length of the payload of the IP v6 packet</param>
        /// <param name="theIPv6PacketProtocol">the protocol for the IP v6 packet</param>
        /// <returns>Boolean flag that indicates whether the payload of the IP v6 packet could be processed</returns>
        private bool ProcessIPv6PacketPayload(ulong thePacketNumber, double thePacketTimestamp, ushort theIPv6PacketPayloadLength, byte theIPv6PacketProtocol)
        {
            bool theResult = true;

            // Process the IP v6 packet based on the value indicated for the protocol in the the IP v6 packet header
            switch (theIPv6PacketProtocol)
            {
                case (byte)Constants.Protocol.TCP:
                    {
                        // We have got an IP v6 packet containing an TCP packet so process it
                        theResult = this.theTCPPacketProcessing.ProcessTCPPacket(
                            thePacketNumber,
                            thePacketTimestamp,
                            theIPv6PacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.UDP:
                    {
                        // We have got an IP v6 packet containing an UDP datagram so process it
                        theResult = this.theUDPDatagramProcessing.ProcessUDPDatagram(
                            thePacketNumber,
                            thePacketTimestamp,
                            theIPv6PacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.ICMPv6:
                    {
                        // We have got an IP v6 packet containing an ICMP v6 packet

                        // Processing of IP v6 packets containing an ICMP v6 packet is not currently supported!

                        // Just record the event and fall through as later processing will read off the remaining payload so we can move on
                        this.theDebugInformation.WriteInformationEvent(
                            "The IP v6 packet contains an ICMP v6 packet, which is not currently supported!");

                        break;
                    }

                case (byte)Constants.Protocol.EIGRP:
                    {
                        // We have got an IP v6 packet containing a Cisco EIGRP packet

                        // Processing of IP v6 packets containing a Cisco EIGRP packet is not currently supported!

                        // Just record the event and fall through as later processing will read off the remaining payload so we can move on
                        this.theDebugInformation.WriteInformationEvent(
                            "The IP v6 packet contains a Cisco EIGRP packet which is not currently supported!");

                        break;
                    }

                default:
                    {
                        //// We have got an IP v6 packet containing an unknown protocol

                        //// Processing of packets with network data link types not enumerated above are obviously not currently supported!

                        this.theDebugInformation.WriteErrorEvent(
                            "The IP v6 packet contains an unexpected protocol of " +
                            string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:X}", theIPv6PacketProtocol) +
                            "!!!");

                        theResult = false;

                        break;
                    }
            }

            return theResult;
        }

        /// <summary>
        /// Validates the IP v6 packet header
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <param name="theIPv6PacketHeaderVersion">The version of the IP v6 packet header</param>
        /// <param name="theIPv6PacketPayloadLength">The payload length of the IP v6 packet</param>
        /// <returns>Boolean flag that indicates whether the IP v6 packet header is valid</returns>
        private bool ValidateIPv6PacketHeader(long theEthernetFrameLength, ushort theIPv6PacketHeaderVersion, ushort theIPv6PacketPayloadLength)
        {
            bool theResult = true;

            // Validate the payload length in the IP v6 packet header
            if ((theIPv6PacketPayloadLength + Constants.HeaderLength) > theEthernetFrameLength)
            {
                //// We have got an IP v6 packet containing a payload length that is higher than the payload in the Ethernet frame which is invalid

                this.theDebugInformation.WriteErrorEvent(
                    "The IP v6 packet indicates a total length of " +
                    (theIPv6PacketPayloadLength + Constants.HeaderLength).ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " bytes that is greater than the length of " +
                    theEthernetFrameLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " bytes in the Ethernet frame!!!");

                theResult = false;
            }

            // Validate the version in the IP v6 packet header
            if (theIPv6PacketHeaderVersion != Constants.HeaderVersion)
            {
                //// We have got an IP v6 packet header containing an unknown version

                this.theDebugInformation.WriteErrorEvent(
                    "The IP v6 packet header contains an unexpected version of " +
                    theIPv6PacketHeaderVersion.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
