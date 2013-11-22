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
        public const System.UInt32 PCAPNGPackageCaptureSectionHeaderExpectedBlockType = 0x0A0D0D0A;

        //Length

        public const ushort PCAPNGPackageCaptureSectionHeaderBlockLength = 24;

        //Magic number

        public const System.UInt32 PCAPNGPackageCaptureLittleEndianByteOrderMagic = 0x1a2b3c4d;
        public const System.UInt32 PCAPNGPackageCaptureBigEndianByteOrderMagic = 0x4d3c2b1a;

        //Version numbers

        public const System.UInt16 PCAPNGPackageCaptureExpectedMajorVersion = 1;
        public const System.UInt16 PCAPNGPackageCaptureExpectedMinorVersion = 0;

        //
        //PCAP Next Generation packet capture block header - 8 bytes
        //

        //Length

        public const ushort PCAPNGPackageCaptureBlockHeaderLength = 8;
    }
}