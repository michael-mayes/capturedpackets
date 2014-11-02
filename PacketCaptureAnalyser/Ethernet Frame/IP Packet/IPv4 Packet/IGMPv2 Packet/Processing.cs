// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv4Packet.IGMPv2Packet
{
    /// <summary>
    /// This class provides the IGMP v2 packet processing
    /// </summary>
    public class Processing
    {
        /// <summary>
        /// The object that provides for binary reading from the packet capture
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// The reusable instance of the IGMP v2 packet
        /// </summary>
        private Structures.PacketStructure theIGMPv2Packet;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        public Processing(System.IO.BinaryReader theBinaryReader)
        {
            this.theBinaryReader = theBinaryReader;

            // Create an instance of the IGMP v2 packet
            this.theIGMPv2Packet = new Structures.PacketStructure();
        }

        /// <summary>
        /// Processes an IGMP v2 packet
        /// </summary>
        /// <param name="theIPv4PacketPayloadLength">The length of the payload of the IP v4 packet</param>
        public void ProcessIGMPv2Packet(ushort theIPv4PacketPayloadLength)
        {
            // There is no separate header for the IGMP v2 packet

            // Processes the payload of the IGMP v2 packet
            this.ProcessIGMPv2PacketPayload();
        }

        /// <summary>
        /// Processes the payload of the IGMP v2 packet
        /// </summary>
        private void ProcessIGMPv2PacketPayload()
        {
            // Just read off the bytes for the IGMP v2 packet from the packet capture so we can move on
            this.theIGMPv2Packet.Type = this.theBinaryReader.ReadByte();
            this.theIGMPv2Packet.MaxResponseTime = this.theBinaryReader.ReadByte();
            this.theIGMPv2Packet.Checksum = this.theBinaryReader.ReadUInt16();
            this.theIGMPv2Packet.GroupAddress = this.theBinaryReader.ReadUInt32();
        }
    }
}