// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.LLDPPacket
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

            // Create an instance of the LLDP packet
            this.thePacket = new Structures.PacketStructure();
        }

        public bool Process(long thePayloadLength)
        {
            bool theResult = true;

            if (thePayloadLength < Constants.PacketLength)
            {
                this.theDebugInformation.WriteErrorEvent("The payload length of the Ethernet frame is lower than the length of the LLDP packet!!!");

                return false;
            }

            // There is no separate header for the LLDP packet

            // Just read off the bytes for the LLDP packet from the packet capture so we can move on
            this.thePacket.UnusedField1 = this.theBinaryReader.ReadUInt64();
            this.thePacket.UnusedField2 = this.theBinaryReader.ReadUInt64();
            this.thePacket.UnusedField3 = this.theBinaryReader.ReadUInt64();
            this.thePacket.UnusedField4 = this.theBinaryReader.ReadUInt64();
            this.thePacket.UnusedField5 = this.theBinaryReader.ReadUInt64();
            this.thePacket.UnusedField6 = this.theBinaryReader.ReadUInt32();
            this.thePacket.UnusedField7 = this.theBinaryReader.ReadUInt16();

            return theResult;
        }
    }
}
