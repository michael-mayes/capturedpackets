// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv6Packet
{
    /// <summary>
    /// This class provides the IP v6 packet processing
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

            // Create an instance of the IP v6 packet header
            this.theHeader = new Structures.HeaderStructure();

            // Create instances of the processing classes for each protocol
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

            ushort thePacketPayloadLength;
            byte theProtocol;

            // Process the IP v6 packet header
            theResult = this.ProcessHeader(thePayloadLength, out thePacketPayloadLength, out theProtocol);

            if (theResult)
            {
                // Process the payload of the IP v6 packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the IP v6 packet header
                theResult = this.ProcessPayload(thePacketNumber, theTimestamp, thePacketPayloadLength, theProtocol);
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thePayloadLength">The payload length of the Ethernet frame read from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the IP v6 packet</param>
        /// <param name="theProtocol"></param>
        /// <returns></returns>
        private bool ProcessHeader(long thePayloadLength, out ushort thePacketPayloadLength, out byte theProtocol)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the length of the IP v6 packet payload
            thePacketPayloadLength = 0;

            // Provide a default value for the output parameter for the protocol for the IP v6 packet
            theProtocol = 0;

            // Read the values for the IP v6 packet header from the packet capture
            this.theHeader.VersionAndTrafficClass = this.theBinaryReader.ReadByte();
            this.theHeader.TrafficClassAndFlowLabel = this.theBinaryReader.ReadByte();
            this.theHeader.FlowLabel = this.theBinaryReader.ReadUInt16();
            this.theHeader.PayloadLength = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theHeader.NextHeader = this.theBinaryReader.ReadByte();
            this.theHeader.HopLimit = this.theBinaryReader.ReadByte();
            this.theHeader.SourceAddressHigh = this.theBinaryReader.ReadInt64();
            this.theHeader.SourceAddressLow = this.theBinaryReader.ReadInt64();
            this.theHeader.DestinationAddressHigh = this.theBinaryReader.ReadInt64();
            this.theHeader.DestinationAddressLow = this.theBinaryReader.ReadInt64();

            // Determine the version of the IP v6 packet header
            // Need to first extract the version value from the combined IP packet version and traffic class field
            // We want the higher four bits from the combined IP packet version and traffic class field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            ushort theHeaderVersion = (ushort)((this.theHeader.VersionAndTrafficClass & 0xF0) >> 4);

            // Validate the IP v6 packet header
            theResult = this.ValidateHeader(this.theHeader, thePayloadLength, theHeaderVersion);

            if (theResult)
            {
                // Set up the output parameter for the length of the payload of the IP v6 packet (e.g. a TCP packet), which is the total length of the IP v6 packet minus the length of the IP v6 packet header just calculated
                thePacketPayloadLength = this.theHeader.PayloadLength;

                // Set up the output parameter for the protocol for the IP v6 packet
                theProtocol = this.theHeader.NextHeader;
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

            // Process the IP v6 packet based on the value indicated for the protocol in the the IP v6 packet header
            switch (theProtocol)
            {
                case (byte)Constants.Protocol.TCP:
                    {
                        // We have got an IP v6 packet containing an TCP packet so process it
                        theResult = this.theTCPPacketProcessing.Process(thePacketNumber, theTimestamp, thePacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.UDP:
                    {
                        // We have got an IP v6 packet containing an UDP datagram so process it
                        theResult = this.theUDPDatagramProcessing.Process(thePacketNumber, theTimestamp, thePacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.ICMPv6:
                    {
                        //// We have got an IP v6 packet containing an ICMP v6 packet

                        //// Processing of IP v6 packets containing an ICMP v6 packet is not currently supported!

                        this.theDebugInformation.WriteInformationEvent("The IP v6 packet contains an ICMP v6 packet, which is not currently supported!");

                        // Just read off the bytes for the ICMP v6 packet from the packet capture so we can move on
                        this.theBinaryReader.ReadBytes(thePacketPayloadLength);

                        break;
                    }

                case (byte)Constants.Protocol.EIGRP:
                    {
                        // We have got an IP v6 packet containing a Cisco EIGRP packet

                        // Processing of IP v6 packets containing a Cisco EIGRP packet is not currently supported!

                        // Just record the event and fall through as later processing will read off the remaining payload so we can move on
                        this.theDebugInformation.WriteInformationEvent("The IP v6 packet contains a Cisco EIGRP packet which is not currently supported!");

                        break;
                    }

                default:
                    {
                        //// We have got an IP v6 packet containing an unknown protocol

                        //// Processing of packets with network data link types not enumerated above are obviously not currently supported!

                        this.theDebugInformation.WriteErrorEvent("The IP v6 packet contains an unexpected protocol of " +
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
        /// <returns></returns>
        private bool ValidateHeader(Structures.HeaderStructure theHeader, long thePayloadLength, ushort theHeaderVersion)
        {
            bool theResult = true;

            // Validate the version in the IP v6 packet header
            if ((this.theHeader.PayloadLength + Constants.HeaderLength) > thePayloadLength)
            {
                //// We have got an IP v6 packet containing an length that is higher than the payload in the Ethernet frame which is invalid

                this.theDebugInformation.WriteErrorEvent("The IP v6 packet indicates a total length of " +
                    (this.theHeader.PayloadLength + Constants.HeaderLength).ToString() +
                    " bytes that is greater than the length of the payload of " +
                    thePayloadLength.ToString() +
                    " bytes in the Ethernet frame!!!");

                theResult = false;
            }

            // Validate the version in the IP v6 packet header
            if (theHeaderVersion != Constants.HeaderVersion)
            {
                //// We have got an IP v6 packet header containing an unknown version

                this.theDebugInformation.WriteErrorEvent("The IP v6 packet header contains an unexpected version of " +
                    theHeaderVersion.ToString() +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
