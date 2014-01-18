//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace.LoopbackPacketNamespace
{
    class Constants
    {
        //
        //Loopback packet header - 6 bytes
        //

        //Length

        public const ushort LoopbackPacketHeaderLength = 6;

        //
        //Loopback packet payload
        //

        //Length

        public const ushort LoopbackPacketPayloadLength = 40;
    }
}