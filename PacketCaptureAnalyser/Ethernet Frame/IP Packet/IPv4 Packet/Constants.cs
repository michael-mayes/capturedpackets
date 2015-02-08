// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.IPPacket.IPv4Packet
{
    /// <summary>
    /// This class provides constants for use by the IP v4 packet processing
    /// </summary>
    public static class Constants
    {
        //// IP v4 packet header

        /// <summary>
        /// IP v4 packet header version
        /// </summary>
        public const ushort HeaderVersion = 4;

        /// <summary>
        /// IP v4 packet header minimum length
        /// </summary>
        public const ushort HeaderMinimumLength = 20;

        /// <summary>
        /// IP v4 packet header maximum length
        /// </summary>
        public const ushort HeaderMaximumLength = 60;

        /// <summary>
        /// Protocol for payload of the IP v4 packet
        /// </summary>
        public enum Protocol : byte
        {
            /// <summary>
            /// Default value
            /// </summary>
            None = 0,

            /// <summary>
            /// ICMP v4 - Internet Control Message Protocol version 4 for IP v4 (RFC 792)
            /// </summary>
            ICMPv4 = 0x01,

            /// <summary>
            /// IGMP - Internet Group Management Protocol (RFC 1112)
            /// </summary>
            IGMP = 0x02,

            /// <summary>
            /// TCP - Transmission Control Protocol (RFC 793)
            /// </summary>
            TCP = 0x06,

            /// <summary>
            /// UDP - User Datagram Protocol (RFC 768)
            /// </summary>
            UDP = 0x11,

            /// <summary>
            /// EIGRP - Cisco Enhanced Interior Gateway Routing Protocol (Proprietary)
            /// </summary>
            EIGRP = 0x58
        }
    }
}