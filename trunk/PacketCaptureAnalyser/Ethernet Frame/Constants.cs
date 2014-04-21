// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame
{
    class Constants
    {
        //// Ethernet frame header - 14 bytes

        /// <summary>
        /// Length
        /// </summary>
        public const ushort HeaderLength = 14;

        /// <summary>
        /// Ether Type - provided in little endian representation
        /// </summary>
        public enum HeaderEtherType : ushort
        {
            /// <summary>
            /// Minimum value for Ether Type - lower values indicate length for an IEEE 802.3 Ethernet frame
            /// </summary>
            MinimumValue = 0x0600,

            /// <summary>
            /// Ethernet frame containing an ARP packet
            /// </summary>
            ARP = 0x0806,

            /// <summary>
            /// Ethernet frame containing an IPv4 packet
            /// </summary>
            IPv4 = 0x0800,

            /// <summary>
            /// Ethernet frame containing an IPv6 packet
            /// </summary>
            IPv6 = 0x86DD,

            /// <summary>
            /// Ethernet frame containing an LLDP packet (IEEE 802.1AB)
            /// </summary>
            LLDP = 0x88CC,

            /// <summary>
            /// Configuration Test Protocol (Loopback)
            /// </summary>
            Loopback = 0x9000,

            /// <summary>
            /// Ethernet frame with a VLAN tag (IEEE 802.1Q)
            /// </summary>
            VLANTagged = 0x8100
        }
    }
}