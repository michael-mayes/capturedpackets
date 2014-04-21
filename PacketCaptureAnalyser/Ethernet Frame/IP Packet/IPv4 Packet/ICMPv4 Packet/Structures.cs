// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv4Packet.ICMPv4Packet
{
    class Structures
    {
        /// <summary>
        /// ICMPv4 packet header - 4 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public byte Type; // Type of ICMPv4 packet

            [System.Runtime.InteropServices.FieldOffset(1)]
            public byte Code; // Sub-type of ICMPv4 packet

            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort Checksum; // Checksum for the ICMPv4 packet
        }
    }
}