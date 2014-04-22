// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv4Packet
{
    class Constants
    {
        //// IP v4 packet header - 20 bytes

        /// <summary>
        /// Version
        /// </summary>
        public const ushort HeaderVersion = 4;

        /// <summary>
        /// Length
        /// </summary>
        public const ushort HeaderMinimumLength = 20;
        public const ushort HeaderMaximumLength = 60;

        /// <summary>
        /// Protocol
        /// </summary>
        public enum Protocol : byte
        {
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