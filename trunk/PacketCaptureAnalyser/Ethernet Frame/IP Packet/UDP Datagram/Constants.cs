//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/IP%20Packet/UDP%20Datagram/Constants.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace EthernetFrame.IPPacket.UDPDatagram
{
    class Constants
    {
        //
        //UDP datagram header - 8 bytes
        //

        //Length

        public const ushort HeaderLength = 8;

        //Port number

        public enum PortNumber : ushort
        {
            DummyValueMin = 0,
            DummyValueMax = 65535
        }
    }
}