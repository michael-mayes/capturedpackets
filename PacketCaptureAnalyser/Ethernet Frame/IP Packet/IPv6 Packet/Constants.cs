// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv6Packet
{
    class Constants
    {
        //// IPv6 packet header - 40 bytes

        // Version
        public const ushort HeaderVersion = 6;

        // Length
        public const ushort HeaderLength = 40;

        /// <summary>
        /// Protocol
        /// </summary>
        public enum Protocol : byte
        {
            /// <summary>
            /// TCP - Transmission Control Protocol (RFC 793)
            /// </summary>
            TCP = 0x06,

            /// <summary>
            /// UDP - User Datagram Protocol (RFC 768)
            /// </summary>
            UDP = 0x11,

            /// <summary>
            /// ICMPv6 - Internet Control Message Protocol version 6 for IPv6 (RFC 4443, RFC 4884)
            /// </summary>
            ICMPv6 = 0x3A,

            /// <summary>
            /// EIGRP - Cisco Enhanced Interior Gateway Routing Protocol (Proprietary)
            /// </summary>
            EIGRP = 0x58
        }
    }
}