//$Header$

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.ARPPacket
{
    class Processing
    {
        private Analysis.DebugInformation TheDebugInformation;

        private System.IO.BinaryReader TheBinaryReader;

        private Structures.PacketStructure ThePacket;

        public Processing(Analysis.DebugInformation TheDebugInformation, System.IO.BinaryReader TheBinaryReader)
        {
            this.TheDebugInformation = TheDebugInformation;

            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the ARP packet
            ThePacket = new Structures.PacketStructure();
        }

        public bool Process(long ThePayloadLength)
        {
            bool TheResult = true;

            if (ThePayloadLength < Constants.PacketLength)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The payload length of the Ethernet frame is lower than the length of the ARP packet!!!"
                    );

                return false;
            }

            //There is no separate header for the ARP packet

            //Just read off the bytes for the ARP packet from the packet capture so we can move on
            ThePacket.HardwareType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            ThePacket.ProtocolType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            ThePacket.HardwareAddressLength = TheBinaryReader.ReadByte();
            ThePacket.ProtocolAddressLength = TheBinaryReader.ReadByte();
            ThePacket.Operation = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            ThePacket.SenderHardwareAddressHigh = TheBinaryReader.ReadUInt32();
            ThePacket.SenderHardwareAddressLow = TheBinaryReader.ReadUInt16();
            ThePacket.SenderProtocolAddress = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
            ThePacket.TargetHardwareAddressHigh = TheBinaryReader.ReadUInt32();
            ThePacket.TargetHardwareAddressLow = TheBinaryReader.ReadUInt16();
            ThePacket.TargetProtocolAddress = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());

            return TheResult;
        }
    }
}
