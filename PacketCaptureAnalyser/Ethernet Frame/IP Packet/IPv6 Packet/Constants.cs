// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame.IPPacket.IPv6Packet
{
    /// <summary>
    /// This class provides constants for use by the IP v6 packet processing
    /// </summary>
    public static class Constants
    {
        //// IP v6 packet header

        /// <summary>
        /// IP v6 packet header version
        /// </summary>
        public const ushort HeaderVersion = 6;

        /// <summary>
        /// IP v6 packet header length
        /// </summary>
        public const ushort HeaderLength = 40;

        /// <summary>
        /// Protocol for payload of the IP v6 packet
        /// </summary>
        public enum Protocol : byte
        {
            /// <summary>
            /// Hop-by-hop options for IP v6
            /// </summary>
            HopByHopOptions = 0x00,

            /// <summary>
            /// TCP - Transmission Control Protocol (RFC 793)
            /// </summary>
            TCP = 0x06,

            /// <summary>
            /// UDP - User Datagram Protocol (RFC 768)
            /// </summary>
            UDP = 0x11,

            /// <summary>
            /// ICMP v6 - Internet Control Message Protocol version 6 for IP v6 (RFC 4443, RFC 4884)
            /// </summary>
            ICMPv6 = 0x3A,

            /// <summary>
            /// Destination options for IP v6
            /// </summary>
            DestinationOptions = 0x3C,

            /// <summary>
            /// EIGRP - Cisco Enhanced Interior Gateway Routing Protocol (Proprietary)
            /// </summary>
            EIGRP = 0x58
        }
    }
}