//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$Id$
//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/Loopback%20Packet/Constants.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace EthernetFrame.LoopbackPacket
{
    class Constants
    {
        //
        //Loopback packet header - 6 bytes
        //

        //Length

        public const ushort HeaderLength = 6;

        //
        //Loopback packet payload
        //

        //Length

        public const ushort PayloadLength = 40;
    }
}