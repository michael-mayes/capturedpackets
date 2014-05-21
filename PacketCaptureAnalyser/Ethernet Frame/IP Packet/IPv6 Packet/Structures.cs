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
    /// <summary>
    /// This class provides structures for use by the IP v6 packet processing
    /// </summary>
    public class Structures
    {
        /// <summary>
        /// IP v6 packet header - 40 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
        public struct HeaderStructure
        {
            /// <summary>
            /// IP packet version and traffic class
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public byte VersionAndTrafficClass;

            /// <summary>
            /// Traffic class and flow label
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(1)]
            public byte TrafficClassAndFlowLabel;

            /// <summary>
            /// Traffic class and flow label
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort FlowLabel;

            /// <summary>
            /// Total length of the IP v6 packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public ushort PayloadLength;

            /// <summary>
            /// The next header within the IP v6 packet - indicates the protocol for the payload (ICMP, IGMP, TCP, UDP etc)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(6)]
            public byte NextHeader;

            /// <summary>
            /// Hop limit - the maximum number of routers that the IP v6 packet is allowed to travel through before being discarded
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(7)]
            public byte HopLimit;

            /// <summary>
            /// The high bytes of the source IP address (eight bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public long SourceAddressHigh;

            /// <summary>
            /// The low bytes of the source IP address (eight bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public long SourceAddressLow;

            /// <summary>
            /// The high bytes of the destination IP address (eight bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(24)]
            public long DestinationAddressHigh;

            /// <summary>
            /// The low bytes of the destination IP address (eight bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(32)]
            public long DestinationAddressLow;
        }
    }
}