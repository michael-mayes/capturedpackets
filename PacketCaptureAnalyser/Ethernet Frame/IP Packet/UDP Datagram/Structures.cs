//$Header: https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/IP%20Packet/UDP%20Datagram/Structures.cs 212 2014-04-17 18:01:00Z michaelmayes $

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//SVN revision information:
//File:    : $URL$
//Revision : $Revision$
//Date     : $Date$
//Author   : $Author$

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