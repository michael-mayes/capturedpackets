﻿//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

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