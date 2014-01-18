//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace.IPPacketNamespace.UDPDatagramNamespace
{
    class Structures
    {
        //
        //UDP datagram header - 8 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.UDPDatagramHeaderLength)]

        public struct UDPDatagramHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt16 SourcePort; //Source port number

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 DestinationPort; //Destination port number

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt16 Length; //UDP datagram length - includes UDP datagram header and UDP datagram data payload

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.UInt16 Checksum; //Checksum - includes UDP datagram header and UDP datagram data payload as well as an IPv4 "pseudo header"
        }
    }
}