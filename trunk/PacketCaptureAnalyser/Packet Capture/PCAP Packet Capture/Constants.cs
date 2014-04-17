//$Header: https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Packet%20Capture/PCAP%20Packet%20Capture/Constants.cs 212 2014-04-17 18:01:00Z michaelmayes $

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//SVN revision information:
//File:    : $URL$
//Revision : $Revision$
//Date     : $Date$
//Author   : $Author$

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