//$Header: https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Packet%20Capture/CommonConstants.cs 212 2014-04-17 18:01:00Z michaelmayes $

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//SVN revision information:
//File:    : $URL$
//Revision : $Revision$
//Date     : $Date$
//Author   : $Author$

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