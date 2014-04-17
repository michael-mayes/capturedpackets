//$Header: https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/LLDP%20Packet/Processing.cs 212 2014-04-17 18:01:00Z michaelmayes $

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.LLDPPacket
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

            //Create an instance of the LLDP packet
            ThePacket = new Structures.PacketStructure();
        }

        public bool Process(long ThePayloadLength)
        {
            bool TheResult = true;

            if (ThePayloadLength < Constants.PacketLength)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The payload length of the Ethernet frame is lower than the length of the LLDP packet!!!"
                    );

                return false;
            }

            //There is no separate header for the LLDP packet

            //Just read off the bytes for the LLDP packet from the packet capture so we can move on
            ThePacket.UnusedField1 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField2 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField3 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField4 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField5 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField6 = TheBinaryReader.ReadUInt32();
            ThePacket.UnusedField7 = TheBinaryReader.ReadUInt16();

            return TheResult;
        }
    }
}
