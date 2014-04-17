//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$Id$
//$URL$
//$Revision$
//$Date$
//$Author$

namespace EthernetFrame.IPPacket.UDPDatagram
{
    class Constants
    {
        //
        //UDP datagram header - 8 bytes
        //

        //Length

        public const ushort HeaderLength = 8;

        //Port number

        public enum PortNumber : ushort
        {
            DummyValueMin = 0,
            DummyValueMax = 65535
        }
    }
}