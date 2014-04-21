// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.ARPPacket
{
    class Processing
    {
        private Analysis.DebugInformation theDebugInformation;

        private System.IO.BinaryReader theBinaryReader;

        private Structures.PacketStructure thePacket;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation"></param>
        /// <param name="theBinaryReader"></param>
        public Processing(Analysis.DebugInformation theDebugInformation, System.IO.BinaryReader theBinaryReader)
        {
            this.theDebugInformation = theDebugInformation;

            this.theBinaryReader = theBinaryReader;

            // Create an instance of the ARP packet
            this.thePacket = new Structures.PacketStructure();
        }

        public bool Process(long thePayloadLength)
        {
            bool theResult = true;

            if (thePayloadLength < Constants.PacketLength)
            {
                this.theDebugInformation.WriteErrorEvent("The payload length of the Ethernet frame is lower than the length of the ARP packet!!!");

                return false;
            }

            // There is no separate header for the ARP packet

            // Just read off the bytes for the ARP packet from the packet capture so we can move on
            this.thePacket.HardwareType = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.thePacket.ProtocolType = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.thePacket.HardwareAddressLength = this.theBinaryReader.ReadByte();
            this.thePacket.ProtocolAddressLength = this.theBinaryReader.ReadByte();
            this.thePacket.Operation = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.thePacket.SenderHardwareAddressHigh = this.theBinaryReader.ReadUInt32();
            this.thePacket.SenderHardwareAddressLow = this.theBinaryReader.ReadUInt16();
            this.thePacket.SenderProtocolAddress = (uint)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt32());
            this.thePacket.TargetHardwareAddressHigh = this.theBinaryReader.ReadUInt32();
            this.thePacket.TargetHardwareAddressLow = this.theBinaryReader.ReadUInt16();
            this.thePacket.TargetProtocolAddress = (uint)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt32());

            return theResult;
        }
    }
}
