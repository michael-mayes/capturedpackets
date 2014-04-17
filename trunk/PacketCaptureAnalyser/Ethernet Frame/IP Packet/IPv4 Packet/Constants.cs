//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/IP%20Packet/IPv4%20Packet/Constants.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace EthernetFrame.IPPacket.IPv4Packet
{
    class Constants
    {
        //
        //IPv4 packet header - 20 bytes
        //

        //Version

        public const ushort HeaderVersion = 4;

        //Length

        public const ushort HeaderMinimumLength = 20;
        public const ushort HeaderMaximumLength = 60;

        //Protocol

        public enum Protocol : byte
        {

            ICMPv4 = 0x01, //ICMPv4 - Internet Control Message Protocol version 4 for IPv4 (RFC 792)
            IGMP = 0x02, //IGMP - Internet Group Management Protocol (RFC 1112)
            TCP = 0x06, //TCP - Transmission Control Protocol (RFC 793)
            UDP = 0x11, //UDP - User Datagram Protocol (RFC 768)
            EIGRP = 0x58 //EIGRP - Cisco Enhanced Interior Gateway Routing Protocol (Proprietary)
        }
    }
}