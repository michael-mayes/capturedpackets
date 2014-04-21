// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv4Packet.IGMPv2Packet
{
    class Structures
    {
        /// <summary>
        /// IGMPv2 packet - 8 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.PacketLength)]
        public struct PacketStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public byte Type; // Type of IGMPv2 packet

            [System.Runtime.InteropServices.FieldOffset(1)]
            public byte MaxResponseTime; // Maximum time allowed for response to the IGMPv2 packet - only non-zero for queries

            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort Checksum; // Checksum for the IGMPv2 packet

            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint GroupAddress; // The multicast address being queried by the IGMPv2 packet
        }
    }
}