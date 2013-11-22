//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureProcessingNamespace
{
    class PCAPNGPackageCaptureConstants
    {
        //
        //PCAP Next Generation packet capture section header block - 24 bytes
        //

        //Type
        public const System.UInt32 PCAPNGPackageCaptureSectionHeaderBlockExpectedBlockType = 0x0a0d0d0a;

        //Length

        public const ushort PCAPNGPackageCaptureSectionHeaderBlockLength = 24;

        //Magic number

        public const System.UInt32 PCAPNGPackageCaptureLittleEndianByteOrderMagic = 0x1a2b3c4d;
        public const System.UInt32 PCAPNGPackageCaptureBigEndianByteOrderMagic = 0x4d3c2b1a;

        //Version numbers

        public const System.UInt16 PCAPNGPackageCaptureExpectedMajorVersion = 1;
        public const System.UInt16 PCAPNGPackageCaptureExpectedMinorVersion = 0;

        //
        //PCAP Next Generation packet capture interface description block - 16 bytes
        //

        //Type

        public const System.UInt32 PCAPNGPackageCaptureInterfaceDescriptionBlockExpectedBlockType = 0x00000001;

        //Length

        public const ushort PCAPNGPackageCaptureInterfaceDescriptionBlockLength = 16;

        //
        //PCAP Next Generation packet capture enhanced packet block - 8 bytes
        //

        //Type

        public const System.UInt32 PCAPNGPackageCaptureEnhancedPacketBlockExpectedBlockType = 0x00000006;

        //Length

        public const ushort PCAPNGPackageCaptureEnhancedPacketBlockLength = 8;
    }
}