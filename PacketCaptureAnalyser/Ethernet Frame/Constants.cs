// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame
{
    /// <summary>
    /// This class provides constants for use by the Ethernet frame processing
    /// </summary>
    public static class Constants
    {
        //// Ethernet frame header

        /// <summary>
        /// Ethernet frame header length
        /// </summary>
        public const ushort HeaderLength = 14;

        /// <summary>
        /// Type of an Ethernet frame - provided in little endian representation
        /// </summary>
        public enum HeaderEthernetFrameType
        {
            /// <summary>
            /// Default value
            /// </summary>
            None = 0,

            /// <summary>
            /// Minimum value for type of an Ethernet frame - lower values indicate length for an IEEE 802.3 Ethernet frame
            /// </summary>
            MinimumValue = 0x0600,

            /// <summary>
            /// Ethernet frame containing an ARP packet
            /// </summary>
            ARP = 0x0806,

            /// <summary>
            /// Ethernet frame containing an IP v4 packet
            /// </summary>
            IPv4 = 0x0800,

            /// <summary>
            /// Ethernet frame containing an IP v6 packet
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
            /// Ethernet frame containing a RARP packet
            /// </summary>
            RARP = 0x8035,

            /// <summary>
            /// Ethernet frame with a VLAN tag (IEEE 802.1Q)
            /// </summary>
            VLANTagged = 0x8100
        }
    }
}