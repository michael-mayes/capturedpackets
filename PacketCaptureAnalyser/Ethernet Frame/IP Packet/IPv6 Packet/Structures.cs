// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv6Packet
{
    class Structures
    {
        /// <summary>
        /// IPv6 packet header - 40 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public byte VersionAndTrafficClass; // IP packet version and traffic class

            [System.Runtime.InteropServices.FieldOffset(1)]
            public byte TrafficClassAndFlowLabel; // Traffic class and flow label

            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort FlowLabel; // Traffic class and flow label

            [System.Runtime.InteropServices.FieldOffset(4)]
            public ushort PayloadLength; // Total length of the IPv6 packet

            [System.Runtime.InteropServices.FieldOffset(6)]
            public byte NextHeader; // The next header within the IPv6 packet - indicates the protocol for the payload (ICMP, IGMP, TCP, UDP etc)

            [System.Runtime.InteropServices.FieldOffset(7)]
            public byte HopLimit; // Hop limit - the maximum number of routers that the IPv6 packet is allowed to travel through before being discarded

            [System.Runtime.InteropServices.FieldOffset(8)]
            public long SourceAddressHigh; // The high bytes of the source IP address (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(16)]
            public long SourceAddressLow; // The low bytes of the source IP address (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(24)]
            public long DestinationAddressHigh; // The high bytes of the destination IP address (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(32)]
            public long DestinationAddressLow; // The low bytes of the destination IP address (eight bytes)
        }
    }
}