//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace.LoopbackPacketNamespace
{
    class LoopbackPacketStructures
    {
        //
        //Loopback packet header - 6 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = LoopbackPacketConstants.LoopbackPacketHeaderLength)]

        public struct LoopbackPacketHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt16 SkipCount;

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 Function;

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt16 ReceiptNumber;
        }
    }
}