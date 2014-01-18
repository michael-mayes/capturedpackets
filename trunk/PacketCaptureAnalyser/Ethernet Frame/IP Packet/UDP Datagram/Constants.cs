//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace.IPPacketNamespace.UDPDatagramNamespace
{
    class Constants
    {
        //
        //UDP datagram header - 8 bytes
        //

        //Length

        public const ushort UDPDatagramHeaderLength = 8;

        //Port number

        public enum UDPDatagramPortNumberEnumeration : ushort
        {
            DummyValueMin = 0,
            DummyValueMax = 65535
        }
    }
}