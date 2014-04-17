//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Packet%20Capture/PCAP%20Packet%20Capture/Constants.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace PacketCapture.PCAPPackageCapture
{
    class Constants
    {
        //
        //PCAP packet capture global header - 24 bytes
        //

        //Length

        public const ushort GlobalHeaderLength = 24;

        //Magic number

        public const System.UInt32 LittleEndianMagicNumber = 0xa1b2c3d4;
        public const System.UInt32 BigEndianMagicNumber = 0xd4c3b2a1;

        //Version numbers

        public const System.UInt16 ExpectedVersionMajor = 2;
        public const System.UInt16 ExpectedVersionMinor = 4;

        //
        //PCAP packet capture packet header - 16 bytes
        //

        //Length

        public const ushort HeaderLength = 16;
    }
}