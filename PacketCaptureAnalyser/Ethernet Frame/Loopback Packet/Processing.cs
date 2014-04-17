//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/Loopback%20Packet/Processing.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace EthernetFrame.LoopbackPacket
{
    class Processing
    {
        private Analysis.DebugInformation TheDebugInformation;

        private System.IO.BinaryReader TheBinaryReader;

        private Structures.HeaderStructure TheHeader;

        public Processing(Analysis.DebugInformation TheDebugInformation, System.IO.BinaryReader TheBinaryReader)
        {
            this.TheDebugInformation = TheDebugInformation;

            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the Loopback packet header
            TheHeader = new Structures.HeaderStructure();
        }

        public bool Process(long ThePayloadLength)
        {
            bool TheResult = true;

            if (ThePayloadLength < (Constants.HeaderLength + Constants.PayloadLength))
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The payload length of the Ethernet frame is lower than the length of the Loopback packet!!!"
                    );

                return false;
            }

            //Process the Loopback packet header
            TheResult = ProcessHeader();

            //Just read off the remaining bytes of the Loopback packet from the packet capture so we can move on
            //The remaining length is the length for the Loopback packet payload
            TheBinaryReader.ReadBytes(Constants.PayloadLength);

            return TheResult;
        }

        private bool ProcessHeader()
        {
            bool TheResult = true;

            //Read the values for the Loopback packet header from the packet capture
            TheHeader.SkipCount = TheBinaryReader.ReadUInt16();
            TheHeader.Function = TheBinaryReader.ReadUInt16();
            TheHeader.ReceiptNumber = TheBinaryReader.ReadUInt16();

            return TheResult;
        }
    }
}
