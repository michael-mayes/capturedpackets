//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/IP%20Packet/IPv4%20Packet/ICMPv4%20Packet/Processing.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace EthernetFrame.IPPacket.IPv4Packet.ICMPv4Packet
{
    class Processing
    {
        private System.IO.BinaryReader TheBinaryReader;

        private Structures.HeaderStructure TheHeader;

        public Processing(System.IO.BinaryReader TheBinaryReader)
        {
            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the ICMPv4 packet header
            TheHeader = new Structures.HeaderStructure();
        }

        public bool Process(ushort TheICMPv4PacketLength)
        {
            bool TheResult = true;

            //Process the ICMPv4 packet header
            TheResult = ProcessHeader();

            //Just read off the remaining bytes of the ICMPv4 packet from the packet capture so we can move on
            //The remaining length is the supplied length of the ICMPv4 packet minus the length for the ICMPv4 packet header
            TheBinaryReader.ReadBytes(TheICMPv4PacketLength - Constants.HeaderLength);

            return TheResult;
        }

        private bool ProcessHeader()
        {
            bool TheResult = true;

            //Read the values for the ICMPv4 packet header from the packet capture
            TheHeader.Type = TheBinaryReader.ReadByte();
            TheHeader.Code = TheBinaryReader.ReadByte();
            TheHeader.Checksum = TheBinaryReader.ReadUInt16();

            return TheResult;
        }
    }
}