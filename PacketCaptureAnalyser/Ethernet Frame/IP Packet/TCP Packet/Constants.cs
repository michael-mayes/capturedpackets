//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/IP%20Packet/TCP%20Packet/Constants.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace EthernetFrame.IPPacket.TCPPacket
{
    class Constants
    {
        //
        //TCP packet header - 20 bytes
        //

        //Length

        public const ushort HeaderMinimumLength = 20;
        public const ushort HeaderMaximumLength = 60;

        //Port number

        public enum PortNumber : ushort
        {
            DummyValueMin = 0,
            DummyValueMax = 65535
        }

        //Flags

        public enum Flags : byte
        {
            CWR = 0,
            ECE,
            URG,
            ACK,
            PSH,
            RST,
            SYN,
            FIN
        }

        public bool IsFlagSet(System.Byte TheByte, Flags TheFlag)
        {
            int Shift = Flags.FIN - TheFlag;

            // Get a single bit in the proper position
            byte BitMask = (byte)(1 << Shift);

            // Mask out the appropriate bit
            byte MaskedByte = (byte)(TheByte & BitMask);

            // If masked != 0, then the masked out bit is 1
            // Otherwise, masked will be 0
            return (MaskedByte != 0);
        }
    }
}