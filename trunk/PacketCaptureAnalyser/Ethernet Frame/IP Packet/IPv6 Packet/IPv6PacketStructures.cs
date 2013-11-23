﻿//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace.IPPacketNamespace
{
    class IPv6PacketStructures
    {
        //
        //IPv6 packet header - 40 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = IPv6PacketConstants.IPv6PacketHeaderLength)]

        public struct IPv6PacketHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.Byte VersionAndTrafficClass; //IP packet version and traffic class

            [System.Runtime.InteropServices.FieldOffset(1)]
            public System.Byte TrafficClassAndFlowLabel; //Traffic class and flow label

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 FlowLabel; //Traffic class and flow label

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt16 PayloadLength; //Total length of the IPv6 packet

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.Byte NextHeader; //The next header within the IPv6 packet - indicates the protocol for the payload (ICMP, IGMP, TCP, UDP etc)

            [System.Runtime.InteropServices.FieldOffset(7)]
            public System.Byte HopLimit; //Hop limit - the maximum number of routers that the IPv6 packet is allowed to travel through before being discarded

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.Int64 SourceAddressHigh; //The high bytes of the source IP address (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.Int64 SourceAddressLow; //The low bytes of the source IP address (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(24)]
            public System.Int64 DestinationAddressHigh; //The high bytes of the destination IP address (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(32)]
            public System.Int64 DestinationAddressLow; //The low bytes of the destination IP address (eight bytes)
        }
    }
}