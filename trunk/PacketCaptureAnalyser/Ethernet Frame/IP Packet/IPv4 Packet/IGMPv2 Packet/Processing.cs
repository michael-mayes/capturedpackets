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
        /// 
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// 
        /// </summary>
        private Structures.PacketStructure thePacket;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        public Processing(System.IO.BinaryReader theBinaryReader)
        {
            this.theBinaryReader = theBinaryReader;

            // Create an instance of the IGMP v2 packet
            this.thePacket = new Structures.PacketStructure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thePacketPayloadLength"></param>
        /// <returns></returns>
        public bool Process(ushort thePacketPayloadLength)
        {
            bool theResult = true;

            // There is no separate header for the IGMP v2 packet

            // Just read off the bytes for the IGMP v2 packet from the packet capture so we can move on
            this.thePacket.Type = this.theBinaryReader.ReadByte();
            this.thePacket.MaxResponseTime = this.theBinaryReader.ReadByte();
            this.thePacket.Checksum = this.theBinaryReader.ReadUInt16();
            this.thePacket.GroupAddress = this.theBinaryReader.ReadUInt32();

            return theResult;
        }
    }
}