//$Id$
//$URL$

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv4Packet.IGMPv2Packet
{
    class Structures
    {
        //
        //IGMPv2 packet - 8 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.PacketLength)]

        public struct PacketStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.Byte Type; //Type of IGMPv2 packet

            [System.Runtime.InteropServices.FieldOffset(1)]
            public System.Byte MaxResponseTime; //Maximum time allowed for response to the IGMPv2 packet - only non-zero for queries

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 Checksum; //Checksum for the IGMPv2 packet

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 GroupAddress; //The multicast address being queried by the IGMPv2 packet
        }
    }
}