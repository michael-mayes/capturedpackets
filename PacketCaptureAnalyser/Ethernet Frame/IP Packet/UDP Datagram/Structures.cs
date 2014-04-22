// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.UDPDatagram
{
    class Structures
    {
        /// <summary>
        /// UDP datagram header - 8 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ushort SourcePort; // Source port number

            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort DestinationPort; // Destination port number

            [System.Runtime.InteropServices.FieldOffset(4)]
            public ushort Length; // UDP datagram length - includes UDP datagram header and UDP datagram data payload

            [System.Runtime.InteropServices.FieldOffset(6)]
            public ushort Checksum; // Checksum - includes UDP datagram header and UDP datagram data payload as well as an IP v4 "pseudo header"
        }
    }
}