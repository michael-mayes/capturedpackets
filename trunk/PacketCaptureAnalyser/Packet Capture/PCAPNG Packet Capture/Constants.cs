//$Header: https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Packet%20Capture/PCAPNG%20Packet%20Capture/Constants.cs 212 2014-04-17 18:01:00Z michaelmayes $

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//SVN revision information:
//File:    : $URL$
//Revision : $Revision$
//Date     : $Date$
//Author   : $Author$

namespace PacketCapture.PCAPNGPackageCapture
{
    class Constants
    {
        //
        //PCAP Next Generation packet capture block type
        //

        public enum BlockType : uint
        {
            SectionHeaderBlock = 0x0a0d0d0a,
            InterfaceDescriptionBlock = 0x00000001,
            PacketBlock = 0x00000002,
            SimplePacketBlock = 0x00000003,
            EnhancedPacketBlock = 0x00000006,
            InterfaceStatisticsBlock = 0x00000005
        }

        //
        //PCAP Next Generation packet capture block total length
        //

        public enum BlockTotalLength : uint
        {
            SectionHeaderBlock = 24,
            InterfaceDescriptionBlock = 16,
            PacketBlock = 28,
            SimplePacketBlock = 12,
            EnhancedPacketBlock = 28,
            InterfaceStatisticsBlock = 20
        }

        //
        //PCAP Next Generation packet capture section header block
        //

        //Magic number

        public const System.UInt32 LittleEndianByteOrderMagic = 0x1a2b3c4d;
        public const System.UInt32 BigEndianByteOrderMagic = 0x4d3c2b1a;

        //Version numbers

        public const System.UInt16 ExpectedMajorVersion = 1;
        public const System.UInt16 ExpectedMinorVersion = 0;
    }
}