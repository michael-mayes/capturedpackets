//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$Id$
//$Revision$
//$Date$
//$Author$

namespace EthernetFrame.IPPacket.IPv4Packet
{
    class Structures
    {
        //
        //IPv4 packet header - 20 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderMinimumLength)]

        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.Byte VersionAndHeaderLength; //IP version and IPv4 packet header length

            [System.Runtime.InteropServices.FieldOffset(1)]
            public System.Byte TypeOfService; //Type of IP service

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 TotalLength; //Total length of the IPv4 packet

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt16 Identifier; //IP unique identifier

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.UInt16 FlagsAndOffset; //IP header flags and fragment offset

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.Byte TimeToLive; //IP Time To Live

            [System.Runtime.InteropServices.FieldOffset(9)]
            public System.Byte Protocol; //IPv4 packet protocol (ICMP, IGMP, TCP, UDP etc)

            [System.Runtime.InteropServices.FieldOffset(10)]
            public System.UInt16 HeaderChecksum; //IPv4 packet header checksum - includes IPv4 packet header only

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.Int32 SourceAddress; //Source IP address

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.Int32 DestinationAddress; //Destination IP address

            //There may be a options field of 0 – 40 bytes at the end of the structure dependent on the value of the IPv4 packet header length field
        }
    }
}