// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.IPPacket.UDPDatagram
{
    /// <summary>
    /// This class provides structures for use by the UDP datagram processing
    /// </summary>
    public static class Structures
    {
        /// <summary>
        /// UDP datagram header
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
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
            /// UDP datagram length - Includes UDP datagram header and UDP datagram data payload
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public ushort Length;

            /// <summary>
            /// Checksum - Includes UDP datagram header and UDP datagram data payload as well as an IP v4 "pseudo header"
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(6)]
            public ushort Checksum;
        }
    }
}