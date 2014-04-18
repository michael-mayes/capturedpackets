//$Id$

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.PCAPPackageCapture
{
    class Structures
    {
        //
        //PCAP packet capture global header - 24 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.GlobalHeaderLength)]

        public struct GlobalHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 MagicNumber; //Magic number

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt16 VersionMajor; //Major version number

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.UInt16 VersionMinor; //Minor version number

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.Int32 ThisTimeZone; //GMT to local correction

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt32 SignificantFigures; //Accuracy of timestamps

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.UInt32 SnapshotLength; //Max length of captured packets, in octets

            [System.Runtime.InteropServices.FieldOffset(20)]
            public System.UInt32 NetworkDataLinkType; //Network data link type
        }

        //
        //PCAP packet capture packet header - 16 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]

        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 TimestampSeconds; //Timestamp seconds

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 TimestampMicroseconds; //Timestamp microseconds

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt32 SavedLength; //Number of octets of packet saved in packet capture

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt32 ActualLength; //Actual length of packet
        }
    }
}