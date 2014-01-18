//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureProcessing
{
    class PCAPPackageCaptureConstants
    {
        //
        //PCAP packet capture global header - 24 bytes
        //

        //Length

        public const ushort PCAPPackageCaptureGlobalHeaderLength = 24;

        //Magic number

        public const System.UInt32 PCAPPackageCaptureLittleEndianMagicNumber = 0xa1b2c3d4;
        public const System.UInt32 PCAPPackageCaptureBigEndianMagicNumber = 0xd4c3b2a1;

        //Version numbers

        public const System.UInt16 PCAPPackageCaptureExpectedVersionMajor = 2;
        public const System.UInt16 PCAPPackageCaptureExpectedVersionMinor = 4;

        //
        //PCAP packet capture packet header - 16 bytes
        //

        //Length

        public const ushort PCAPPackageCapturePacketHeaderLength = 16;
    }
}