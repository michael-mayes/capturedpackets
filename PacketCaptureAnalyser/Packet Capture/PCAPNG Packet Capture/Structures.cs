// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.PacketCapture.PCAPNGPackageCapture
{
    /// <summary>
    /// This class provides structures for use by the PCAP Next Generation packet capture processing
    /// </summary>
    public static class Structures
    {
        /// <summary>
        /// PCAP Next Generation section header block
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.SectionHeaderBlock)]
        public struct SectionHeaderBlockStructure
        {
            /// <summary>
            /// Block type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint BlockType;

            /// <summary>
            /// Block total length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint BlockTotalLength;

            /// <summary>
            /// Byte-Order Magic (BOM)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint ByteOrderMagic;

            /// <summary>
            /// Major version number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public ushort MajorVersion;

            /// <summary>
            /// Minor version number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(14)]
            public ushort MinorVersion;

            /// <summary>
            /// Section Length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public ulong SectionLength;
        }

        /// <summary>
        /// PCAP Next Generation interface description block
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.InterfaceDescriptionBlock)]
        public struct InterfaceDescriptionBlockStructure
        {
            /// <summary>
            /// Block type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint BlockType;

            /// <summary>
            /// Block total length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint BlockTotalLength;

            /// <summary>
            /// Reserved field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public ushort Reserved;

            /// <summary>
            /// Network data link type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(10)]
            public ushort LinkType;

            /// <summary>
            /// Snap length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public uint SnapLen;
        }

        /// <summary>
        /// PCAP Next Generation packet block
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.PacketBlock)]
        public struct PacketBlockStructure
        {
            /// <summary>
            /// Block type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint BlockType;

            /// <summary>
            /// Block total length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint BlockTotalLength;

            /// <summary>
            /// Interface Id
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public ushort InterfaceId;

            /// <summary>
            /// Drops count
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(10)]
            public ushort DropsCount;

            /// <summary>
            /// High bytes of the timestamp
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public uint TimestampHigh;

            /// <summary>
            /// Low bytes of the timestamp
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public uint TimestampLow;

            /// <summary>
            /// Captured length for the packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(20)]
            public uint CapturedLength;

            /// <summary>
            /// Actual length of the packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(24)]
            public uint PacketLength;
        }

        /// <summary>
        /// PCAP Next Generation simple packet block
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.SimplePacketBlock)]
        public struct SimplePacketBlockStructure
        {
            /// <summary>
            /// Block type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint BlockType;

            /// <summary>
            /// Block total length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint BlockTotalLength;

            /// <summary>
            /// Actual length of the packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint PacketLength;
        }

        /// <summary>
        /// PCAP Next Generation enhanced packet block
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.EnhancedPacketBlock)]
        public struct EnhancedPacketBlockStructure
        {
            /// <summary>
            /// Block type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint BlockType;

            /// <summary>
            /// Block total length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint BlockTotalLength;

            /// <summary>
            /// Interface Id
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint InterfaceId;

            /// <summary>
            /// High bytes of the timestamp
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public uint TimestampHigh;

            /// <summary>
            /// Low bytes of the timestamp
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public uint TimestampLow;

            /// <summary>
            /// Captured length for the packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(20)]
            public uint CapturedLength;

            /// <summary>
            /// Actual length of the packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(24)]
            public uint PacketLength;
        }

        /// <summary>
        /// PCAP Next Generation name resolution block
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.NameResolutionBlock)]
        public struct NameResolutionBlockStructure
        {
            /// <summary>
            /// Block type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint BlockType;

            /// <summary>
            /// Block total length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint BlockTotalLength;

            /// <summary>
            /// Record type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public ushort RecordType;

            /// <summary>
            /// Record length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(10)]
            public ushort RecordLength;
        }

        /// <summary>
        /// PCAP Next Generation interface statistics block
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.InterfaceStatisticsBlock)]
        public struct InterfaceStatisticsBlockStructure
        {
            /// <summary>
            /// Block type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint BlockType;

            /// <summary>
            /// Block total length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint BlockTotalLength;

            /// <summary>
            /// Interface Id
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint InterfaceId;

            /// <summary>
            /// High bytes of the timestamp
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public uint TimestampHigh;

            /// <summary>
            /// Low bytes of the timestamp
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public uint TimestampLow;
        }
    }
}