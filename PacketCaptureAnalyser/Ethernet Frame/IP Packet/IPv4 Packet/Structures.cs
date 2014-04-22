// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv4Packet
{
    class Structures
    {
        /// <summary>
        /// IP v4 packet header - 20 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderMinimumLength)]
        public struct HeaderStructure
        {
            /// <summary>
            /// IP version and IP v4 packet header length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public byte VersionAndHeaderLength;

            /// <summary>
            /// Type of IP service
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(1)]
            public byte TypeOfService;

            /// <summary>
            /// Total length of the IP v4 packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort TotalLength;

            /// <summary>
            /// IP unique identifier
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public ushort Identifier;

            /// <summary>
            /// IP header flags and fragment offset
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(6)]
            public ushort FlagsAndOffset;

            /// <summary>
            /// IP Time To Live
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public byte TimeToLive;

            /// <summary>
            /// IP v4 packet protocol (ICMP, IGMP, TCP, UDP etc)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(9)]
            public byte Protocol;

            /// <summary>
            /// IP v4 packet header checksum - includes IP v4 packet header only
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(10)]
            public ushort HeaderChecksum;

            /// <summary>
            /// Source IP address
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public int SourceAddress;

            /// <summary>
            /// Destination IP address
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public int DestinationAddress;

            //// There may be a options field of 0 – 40 bytes at the end of the structure dependent on the value of the IP v4 packet header length field
        }
    }
}