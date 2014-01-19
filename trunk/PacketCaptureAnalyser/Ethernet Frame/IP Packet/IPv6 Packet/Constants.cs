//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.IPv6Packet
{
    class Constants
    {
        //
        //IPv6 packet header - 40 bytes
        //

        //Version

        public const ushort HeaderVersion = 6;

        //Length

        public const ushort HeaderLength = 40;

        //Protocol

        public enum Protocol : byte
        {
            TCP = 0x06, //TCP - Transmission Control Protocol (RFC 793)
            UDP = 0x11, //UDP - User Datagram Protocol (RFC 768)
            ICMPv6 = 0x3A, //ICMPv6 - Internet Control Message Protocol version 6 for IPv6 (RFC 4443, RFC 4884)
            EIGRP = 0x58 //EIGRP - Cisco Enhanced Interior Gateway Routing Protocol (Proprietary)
        }
    }
}