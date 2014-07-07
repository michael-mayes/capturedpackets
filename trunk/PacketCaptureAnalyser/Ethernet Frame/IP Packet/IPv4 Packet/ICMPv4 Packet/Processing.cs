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
        /// 
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// 
        /// </summary>
        private Structures.HeaderStructure theHeader;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        public Processing(System.IO.BinaryReader theBinaryReader)
        {
            this.theBinaryReader = theBinaryReader;

            // Create an instance of the ICMP v4 packet header
            this.theHeader = new Structures.HeaderStructure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thePacketPayloadLength"></param>
        /// <returns></returns>
        public bool Process(ushort thePacketPayloadLength)
        {
            bool theResult = true;

            // Process the ICMP v4 packet header
            theResult = this.ProcessHeader();

            // Just read off the remaining bytes of the ICMP v4 packet from the packet capture so we can move on
            // The remaining length is the supplied length of the packet payload minus the length for the ICMP v4 packet header
            this.theBinaryReader.ReadBytes(thePacketPayloadLength - Constants.HeaderLength);

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ProcessHeader()
        {
            bool theResult = true;

            // Read the values for the ICMP v4 packet header from the packet capture
            this.theHeader.Type = this.theBinaryReader.ReadByte();
            this.theHeader.Code = this.theBinaryReader.ReadByte();
            this.theHeader.Checksum = this.theBinaryReader.ReadUInt16();

            return theResult;
        }
    }
}