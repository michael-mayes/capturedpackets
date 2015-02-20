// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.IPPacket.IPv4Packet.IGMPv2Packet
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
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        public Processing(System.IO.BinaryReader theBinaryReader)
        {
            this.theBinaryReader = theBinaryReader;
        }

        /// <summary>
        /// Processes an IGMP v2 packet
        /// </summary>
        public void ProcessIGMPv2Packet()
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
            this.theBinaryReader.ReadBytes(
			    Constants.PacketLength);
        }
    }
}