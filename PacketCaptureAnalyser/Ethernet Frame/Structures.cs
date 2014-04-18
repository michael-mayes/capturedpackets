//$Id$
//$URL$

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame
{
    class Structures
    {
        //
        //Ethernet frame header - 14 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]

        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt32 DestinationMACAddressHigh; //The high bytes of the media address of the intended receiver (four bytes)

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt16 DestinationMACAddressLow; //The low bytes of the media address of the intended receiver (two bytes)

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.UInt32 SourceMACAddressHigh; //The high bytes of the media address of the sender (four bytes)

            [System.Runtime.InteropServices.FieldOffset(10)]
            public System.UInt16 SourceMACAddressLow; //The low bytes of the media address of the sender (two bytes)

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt16 EtherType;
        }
    }
}