// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame.IPPacket.IPv4Packet.EIGRPPacket
{
    /// <summary>
    /// This class provides the EIGRP packet processing
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
        /// Processes an EIGRP packet
        /// </summary>
        /// <param name="theIPv4PacketPayloadLength">The length of the payload of the IP v4 packet</param>
        public void ProcessEIGRPPacket(ushort theIPv4PacketPayloadLength)
        {
            // Process the EIGRP packet header
            this.ProcessEIGRPPacketHeader();

            // Process the payload of the EIGRP packet
            this.ProcessEIGRPPacketPayload(
                theIPv4PacketPayloadLength);
        }

        /// <summary>
        /// Processes an EIGRP packet header
        /// </summary>
        private void ProcessEIGRPPacketHeader()
        {
            // Just read off the bytes for the EIGRP packet header from the packet capture so we can move on
            this.theBinaryReader.ReadBytes(
                Constants.HeaderLength);
        }

        /// <summary>
        /// Processes the payload of the EIGRP packet
        /// </summary>
        /// <param name="theIPv4PacketPayloadLength">The length of the payload of the IP v4 packet</param>
        private void ProcessEIGRPPacketPayload(ushort theIPv4PacketPayloadLength)
        {
            // Just read off the remaining bytes of the EIGRP packet from the packet capture so we can move on
            // The remaining length is the supplied length of the payload of the IP v4 packet payload minus the length for the EIGRP packet header
            this.theBinaryReader.ReadBytes(
                theIPv4PacketPayloadLength - Constants.HeaderLength);
        }
    }
}