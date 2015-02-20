// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame.IPPacket.IPv6Packet.ICMPv6Packet
{
    /// <summary>
    /// This class provides the ICMP v6 packet processing
    /// </summary>
    public class Processing
    {
        /// <summary>
        /// The object that provides for binary reading from the packet capture
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        public Processing(System.IO.BinaryReader theBinaryReader)
        {
            this.theBinaryReader = theBinaryReader;
        }

        /// <summary>
        /// Processes an ICMP v6 packet
        /// </summary>
        /// <param name="theIPv6PacketPayloadLength">The length of the payload of the IP v6 packet</param>
        public void ProcessICMPv6Packet(ushort theIPv6PacketPayloadLength)
        {
            // Process the ICMP v6 packet header
            this.ProcessICMPv6PacketHeader();

            // Process the payload of the ICMP v6 packet
            this.ProcessICMPv6PacketPayload(
                theIPv6PacketPayloadLength);
        }

        /// <summary>
        /// Processes an ICMP v6 packet header
        /// </summary>
        private void ProcessICMPv6PacketHeader()
        {
            // Just read off the bytes for the ICMP v6 packet header from the packet capture so we can move on
            this.theBinaryReader.ReadBytes(
                Constants.HeaderLength);
        }

        /// <summary>
        /// Processes the payload of the ICMP v6 packet
        /// </summary>
        /// <param name="theIPv6PacketPayloadLength">The length of the payload of the IP v6 packet</param>
        private void ProcessICMPv6PacketPayload(ushort theIPv6PacketPayloadLength)
        {
            // Just read off the remaining bytes of the ICMP v6 packet from the packet capture so we can move on
            // The remaining length is the supplied length of the payload of the IP v6 packet payload minus the length for the ICMP v6 packet header
            this.theBinaryReader.ReadBytes(
                theIPv6PacketPayloadLength - Constants.HeaderLength);
        }
    }
}