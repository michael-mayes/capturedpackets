//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/Loopback%20Packet/Structures.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

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