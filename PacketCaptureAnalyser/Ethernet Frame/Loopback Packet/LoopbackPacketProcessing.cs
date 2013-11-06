﻿//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace.LoopbackPacketNamespace
{
    class LoopbackPacketProcessing
    {
        private System.IO.BinaryReader TheBinaryReader;

        private LoopbackPacketStructures.LoopbackPacketHeaderStructure TheHeader;

        public LoopbackPacketProcessing(System.IO.BinaryReader TheBinaryReader)
        {
            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the Loopback packet header
            TheHeader = new LoopbackPacketStructures.LoopbackPacketHeaderStructure();
        }

        public bool Process(long ThePayloadLength)
        {
            bool TheResult = true;

            if (ThePayloadLength < (LoopbackPacketConstants.LoopbackPacketHeaderLength + LoopbackPacketConstants.LoopbackPacketPayloadLength))
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The payload length of the Ethernet frame is lower than the length of the Loopback packet!!!"
                    );

                return false;
            }

            //Process the Loopback packet header
            TheResult = ProcessHeader();

            //Just read off the remaining bytes of the Loopback packet from the packet capture so we can move on
            //The remaining length is the length for the Loopback packet payload
            TheBinaryReader.ReadBytes(LoopbackPacketConstants.LoopbackPacketPayloadLength);

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