//$Id$
//$URL$

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.ARPPacket
{
    class Structures
    {
        //
        //ARP packet - 28 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.PacketLength)]

        public struct PacketStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt16 HardwareType; //The network protocol type (e.g. Ethernet is 1)

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 ProtocolType; //The internetwork protocol for which the ARP request is intended (e.g. IPv4 is 0x0800) - same possible values as for Ether Type in Ethernet frame header

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.Byte HardwareAddressLength; //Length (in octets) of a hardware address (e.g. Ethernet addresses size is 6)

            [System.Runtime.InteropServices.FieldOffset(5)]
            public System.Byte ProtocolAddressLength; //Length (in octets) of addresses used in the upper layer internetwork protocol (e.g. IPv4 address size is 4)

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.UInt16 Operation; //The operation that the sender is performing - 1 for request and 2 for reply

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt32 SenderHardwareAddressHigh; //The high bytes of the media address of the sender (four bytes)

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt16 SenderHardwareAddressLow; //The low bytes of the media address of the sender (two bytes)

            [System.Runtime.InteropServices.FieldOffset(14)]
            public System.UInt32 SenderProtocolAddress; //The internetwork address of the sender

            [System.Runtime.InteropServices.FieldOffset(18)]
            public System.UInt32 TargetHardwareAddressHigh; //The high bytes of the media address of the intended receiver (four bytes) - ignored in requests

            [System.Runtime.InteropServices.FieldOffset(22)]
            public System.UInt16 TargetHardwareAddressLow; //The low bytes of the media address of the intended receiver (two bytes) - ignored in requests

            [System.Runtime.InteropServices.FieldOffset(24)]
            public System.UInt32 TargetProtocolAddress; //The internetwork address of the intended receiver
        }
    }
}