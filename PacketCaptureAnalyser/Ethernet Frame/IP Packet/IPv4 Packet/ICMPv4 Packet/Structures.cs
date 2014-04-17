//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$Id$
//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/IP%20Packet/IPv4%20Packet/ICMPv4%20Packet/Structures.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace EthernetFrame.IPPacket.IPv4Packet.ICMPv4Packet
{
    class Structures
    {
        //
        //ICMPv4 packet header - 4 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]

        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.Byte Type; //Type of ICMPv4 packet

            [System.Runtime.InteropServices.FieldOffset(1)]
            public System.Byte Code; //Sub-type of ICMPv4 packet

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 Checksum; //Checksum for the ICMPv4 packet
        }
    }
}