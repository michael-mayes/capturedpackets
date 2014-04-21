// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.PCAPNGPackageCapture
{
    class Constants
    {
        //// PCAP Next Generation packet capture section header block

        // Magic number
        public const uint LittleEndianByteOrderMagic = 0x1a2b3c4d;
        public const uint BigEndianByteOrderMagic = 0x4d3c2b1a;

        // Version numbers
        public const ushort ExpectedMajorVersion = 1;
        public const ushort ExpectedMinorVersion = 0;

        /// <summary>
        /// PCAP Next Generation packet capture block type
        /// </summary>
        public enum BlockType : uint
        {
            /// <summary>
            /// PCAP Next Generation packet capture section header
            /// </summary>
            SectionHeaderBlock = 0x0a0d0d0a,

            /// <summary>
            /// PCAP Next Generation packet capture interface description block
            /// </summary>
            InterfaceDescriptionBlock = 0x00000001,

            /// <summary>
            /// PCAP Next Generation packet capture packet block
            /// </summary>
            PacketBlock = 0x00000002,

            /// <summary>
            /// PCAP Next Generation packet capture simple packet block
            /// </summary>
            SimplePacketBlock = 0x00000003,

            /// <summary>
            /// PCAP Next Generation packet capture enhanced packet block
            /// </summary>
            EnhancedPacketBlock = 0x00000006,

            /// <summary>
            /// PCAP Next Generation packet capture interface statistics block
            /// </summary>
            InterfaceStatisticsBlock = 0x00000005
        }

        /// <summary>
        /// PCAP Next Generation packet capture block total length
        /// </summary>
        public enum BlockTotalLength : uint
        {
            /// <summary>
            /// PCAP Next Generation packet capture section header block length
            /// </summary>
            SectionHeaderBlock = 24,

            /// <summary>
            /// PCAP Next Generation packet capture interface description block length
            /// </summary>
            InterfaceDescriptionBlock = 16,

            /// <summary>
            /// PCAP Next Generation packet capture packet block length
            /// </summary>
            PacketBlock = 28,

            /// <summary>
            /// PCAP Next Generation packet capture simple packet block length
            /// </summary>
            SimplePacketBlock = 12,

            /// <summary>
            /// PCAP Next Generation packet capture enhanced packet block length
            /// </summary>
            EnhancedPacketBlock = 28,

            /// <summary>
            /// PCAP Next Generation packet capture interface statistics block length
            /// </summary>
            InterfaceStatisticsBlock = 20
        }
    }
}