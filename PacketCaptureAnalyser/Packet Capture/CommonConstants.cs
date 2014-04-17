//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Packet%20Capture/CommonConstants.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace PacketCapture
{
    class CommonConstants
    {
        //Network data link type/Network encapsulation type

        public enum NetworkDataLinkType : ushort
        {
            NullLoopBack = 0, //Null/Loopback
            Ethernet = 1, //Ethernet
            CiscoHDLC = 104, //Cisco HDLC (Proprietary)
            Invalid = ushort.MaxValue //Invalid
        }
    }
}