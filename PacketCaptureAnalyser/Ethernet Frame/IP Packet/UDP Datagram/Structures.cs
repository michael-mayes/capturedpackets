//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$Id$
//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/IP%20Packet/UDP%20Datagram/Structures.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace EthernetFrame.IPPacket.UDPDatagram
{
    class Structures
    {
        //
        //UDP datagram header - 8 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]

        public struct HeaderStructure
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