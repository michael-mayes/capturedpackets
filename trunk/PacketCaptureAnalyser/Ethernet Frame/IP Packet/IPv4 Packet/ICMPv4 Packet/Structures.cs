// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.IPPacket.IPv4Packet.ICMPv4Packet
{
    /// <summary>
    /// This class provides structures for use by the ICMP v4 packet processing
    /// </summary>
    public static class Structures
    {
        /// <summary>
        /// ICMP v4 packet header
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
        public struct HeaderStructure
        {
            /// <summary>
            /// Type of the ICMP v4 packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public byte Type;

            /// <summary>
            /// Sub-type of the ICMP v4 packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(1)]
            public byte Code;

            /// <summary>
            /// Checksum for the ICMP v4 packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort Checksum;
        }
    }
}