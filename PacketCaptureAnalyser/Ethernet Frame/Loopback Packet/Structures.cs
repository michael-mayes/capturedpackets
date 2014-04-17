//$Header: https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/Loopback%20Packet/Structures.cs 212 2014-04-17 18:01:00Z michaelmayes $

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//SVN revision information:
//File:    : $URL$
//Revision : $Revision$
//Date     : $Date$
//Author   : $Author$

namespace EthernetFrame.LoopbackPacket
{
    class Structures
    {
        //
        //Loopback packet header - 6 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]

        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt16 SkipCount;

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 Function;

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt16 ReceiptNumber;
        }
    }
}