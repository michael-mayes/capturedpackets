//$Header$

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.PCAPNGPackageCapture
{
    class Structures
    {
        //
        //PCAP Next Generation packet capture section header block
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.SectionHeaderBlock)]

        public struct SectionHeaderBlockStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 BlockType; //Block type

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 BlockTotalLength; //Block total length

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt32 ByteOrderMagic; //Byte-Order Magic

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt16 MajorVersion; //Major version number

            [System.Runtime.InteropServices.FieldOffset(14)]
            public System.UInt16 MinorVersion; //Minor version number

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.UInt64 SectionLength; //Section Length
        }

        //
        //PCAP Next Generation packet capture interface description block
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.InterfaceDescriptionBlock)]

        public struct InterfaceDescriptionBlockStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 BlockType; //Block type

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 BlockTotalLength; //Block total length

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt16 Reserved; //Reserved field

            [System.Runtime.InteropServices.FieldOffset(10)]
            public System.UInt16 LinkType; //Network data link type

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt32 SnapLen; //Snap length
        }

        //
        //PCAP Next Generation packet capture enhanced packet block
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.PacketBlock)]

        public struct PacketBlockStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 BlockType; //Block type

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 BlockTotalLength; //Block total length

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt16 InterfaceId; //Interface Id

            [System.Runtime.InteropServices.FieldOffset(10)]
            public System.UInt16 DropsCount; //Drops Count

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt32 TimestampHigh; //High bytes of the timestamp

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.UInt32 TimestampLow; //Low bytes of the timestamp

            [System.Runtime.InteropServices.FieldOffset(20)]
            public System.UInt32 CapturedLength; //Captured length for the packet

            [System.Runtime.InteropServices.FieldOffset(24)]
            public System.UInt32 PacketLength; //Actual length of the packet
        }

        //
        //PCAP Next Generation packet capture simple packet block
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.SimplePacketBlock)]

        public struct SimplePacketBlockStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 BlockType; //Block type

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 BlockTotalLength; //Block total length

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt32 PacketLength; //Actual length of the packet
        }

        //
        //PCAP Next Generation packet capture enhanced packet block
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.EnhancedPacketBlock)]

        public struct EnhancedPacketBlockStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 BlockType; //Block type

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 BlockTotalLength; //Block total length

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt32 InterfaceId; //Interface Id

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt32 TimestampHigh; //High bytes of the timestamp

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.UInt32 TimestampLow; //Low bytes of the timestamp

            [System.Runtime.InteropServices.FieldOffset(20)]
            public System.UInt32 CapturedLength; //Captured length for the packet

            [System.Runtime.InteropServices.FieldOffset(24)]
            public System.UInt32 PacketLength; //Actual length of the packet
        }

        //
        //PCAP Next Generation packet capture interface statistics block
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = (int)Constants.BlockTotalLength.InterfaceStatisticsBlock)]

        public struct InterfaceStatisticsBlockStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 BlockType; //Block type

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 BlockTotalLength; //Block total length

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt32 InterfaceId; //Interface Id

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt32 TimestampHigh; //High bytes of the timestamp

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.UInt32 TimestampLow; //Low bytes of the timestamp
        }
    }
}