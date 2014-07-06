// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.TCPPacket
{
    /// <summary>
    /// This class provides structures for use by the TCP packet processing
    /// </summary>
    public class Structures
    {
        /// <summary>
        /// TCP packet header
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderMinimumLength)]
        public struct HeaderStructure
        {
            /// <summary>
            /// Source port number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ushort SourcePort;

            /// <summary>
            /// Destination port number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort DestinationPort;

            /// <summary>
            /// Sequence number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint SequenceNumber;

            /// <summary>
            /// Acknowledgment number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint AcknowledgmentNumber;

            /// <summary>
            /// TCP packet header length, reserved fields and NS flag
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public byte DataOffsetAndReservedAndNSFlag;

            /// <summary>
            /// Other flags (including ACK, PSH, RST, SYN and FIN)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(13)]
            public byte Flags;

            /// <summary>
            /// Window size
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(14)]
            public ushort WindowSize;

            /// <summary>
            /// Checksum - Includes TCP packer header and TCP packet data payload as well as an IP v4 "pseudo header"
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public ushort Checksum;

            /// <summary>
            /// Urgent pointer
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(18)]
            public ushort UrgentPointer;

            //// There may be a options field of 0 – 40 bytes at the end of the structure dependent on the value of the TCP packet header length field
        }
    }
}