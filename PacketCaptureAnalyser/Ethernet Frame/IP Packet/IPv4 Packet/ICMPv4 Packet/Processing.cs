//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace.IPPacketNamespace.IPv4PacketNamespace.ICMPv4PacketNamespace
{
    class Processing
    {
        private System.IO.BinaryReader TheBinaryReader;

        private Structures.ICMPv4PacketHeaderStructure TheHeader;

        public Processing(System.IO.BinaryReader TheBinaryReader)
        {
            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the ICMPv4 packet header
            TheHeader = new Structures.ICMPv4PacketHeaderStructure();
        }

        public bool Process(ushort TheICMPv4PacketLength)
        {
            bool TheResult = true;

            //Process the ICMPv4 packet header
            TheResult = ProcessHeader();

            //Just read off the remaining bytes of the ICMPv4 packet from the packet capture so we can move on
            //The remaining length is the supplied length of the ICMPv4 packet minus the length for the ICMPv4 packet header
            TheBinaryReader.ReadBytes(TheICMPv4PacketLength - Constants.ICMPv4PacketHeaderLength);

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