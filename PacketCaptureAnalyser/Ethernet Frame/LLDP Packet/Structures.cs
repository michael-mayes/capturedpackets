//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.LLDPPacket
{
    class Structures
    {
        //
        //LLDP packet - 46 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.PacketLength)]

        public struct PacketStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt64 UnusedField1;

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt64 UnusedField2;

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.UInt64 UnusedField3;

            [System.Runtime.InteropServices.FieldOffset(24)]
            public System.UInt64 UnusedField4;

            [System.Runtime.InteropServices.FieldOffset(32)]
            public System.UInt64 UnusedField5;

            [System.Runtime.InteropServices.FieldOffset(40)]
            public System.UInt32 UnusedField6;

            [System.Runtime.InteropServices.FieldOffset(44)]
            public System.UInt16 UnusedField7;
        }
    }
}