//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureProcessingNamespace
{
    class PCAPNGPackageCaptureStructures
    {
        //
        //PCAP Next Generation packet capture section header block - 8 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureSectionHeaderBlockLength)]

        public struct PCAPNGPackageCaptureSectionHeaderBlockStructure
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
        }

        //
        //PCAP Next Generation packet capture block header - 8 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureBlockHeaderLength)]

        public struct PCAPNGPackageCaptureBlockHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 BlockType; //Block type

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 BlockTotalLength; //Block total length
        }
    }
}