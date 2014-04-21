// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.TCPPacket
{
    class Structures
    {
        /// <summary>
        /// TCP packet header - 20 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderMinimumLength)]
        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ushort SourcePort; // Source port number

            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort DestinationPort; // Destination port number

            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint SequenceNumber; // Sequence number

            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint AcknowledgmentNumber; // Acknowledgment number

            [System.Runtime.InteropServices.FieldOffset(12)]
            public byte DataOffsetAndReservedAndNSFlag; // TCP packet header length, reserved fields and NS flag

            [System.Runtime.InteropServices.FieldOffset(13)]
            public byte Flags; // Other flags (including ACK, PSH, RST, SYN and FIN)

            [System.Runtime.InteropServices.FieldOffset(14)]
            public ushort WindowSize; // Window size

            [System.Runtime.InteropServices.FieldOffset(16)]
            public ushort Checksum; // Checksum - includes TCP packer header and TCP packet data payload as well as an IPv4 "pseudo header"

            [System.Runtime.InteropServices.FieldOffset(18)]
            public ushort UrgentPointer; // Urgent pointer

            // There may be a options field of 0 – 40 bytes at the end of the structure dependent on the value of the TCP packet header length field
        }
    }
}