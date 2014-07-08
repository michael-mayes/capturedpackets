// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv4Packet.ICMPv4Packet
{
    /// <summary>
    /// This class provides the ICMP v4 packet processing
    /// </summary>
    public class Processing
    {
        /// <summary>
        /// The object that provides for binary reading from the packet capture
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// The reusable instance of the ICMP v4 packet header
        /// </summary>
        private Structures.HeaderStructure theICMPv4PacketHeader;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        public Processing(System.IO.BinaryReader theBinaryReader)
        {
            this.theBinaryReader = theBinaryReader;

            // Create an instance of the ICMP v4 packet header
            this.theICMPv4PacketHeader = new Structures.HeaderStructure();
        }

        /// <summary>
        /// Processes an ICMP v4 packet
        /// </summary>
        /// <param name="theIPv4PacketPayloadLength">The length of the payload of the IP v4 packet</param>
        public void ProcessICMPv4Packet(ushort theIPv4PacketPayloadLength)
        {
            // Process the ICMP v4 packet header
            this.ProcessICMPv4PacketHeader();

            // Just read off the remaining bytes of the ICMP v4 packet from the packet capture so we can move on
            // The remaining length is the supplied length of the payload of the IP v4 packet payload minus the length for the ICMP v4 packet header
            this.theBinaryReader.ReadBytes(theIPv4PacketPayloadLength - Constants.HeaderLength);
        }

        /// <summary>
        /// Processes an ICMP v4 packet header
        /// </summary>
        private void ProcessICMPv4PacketHeader()
        {
            // Read the values for the ICMP v4 packet header from the packet capture
            this.theICMPv4PacketHeader.Type = this.theBinaryReader.ReadByte();
            this.theICMPv4PacketHeader.Code = this.theBinaryReader.ReadByte();
            this.theICMPv4PacketHeader.Checksum = this.theBinaryReader.ReadUInt16();
        }
    }
}