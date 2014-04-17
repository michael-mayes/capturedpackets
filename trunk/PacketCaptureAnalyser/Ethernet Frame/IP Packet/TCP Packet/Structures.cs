//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$Id$
//$Revision$
//$Date$
//$Author$

namespace EthernetFrame.IPPacket.TCPPacket
{
    class Structures
    {
        //
        //TCP packet header - 20 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderMinimumLength)]

        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt16 SourcePort; //Source port number

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 DestinationPort; //Destination port number

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 SequenceNumber; //Sequence number

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt32 AcknowledgmentNumber; //Acknowledgment number

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.Byte DataOffsetAndReservedAndNSFlag; //TCP packet header length, reserved fields and NS flag

            [System.Runtime.InteropServices.FieldOffset(13)]
            public System.Byte Flags; //Other flags (including ACK, PSH, RST, SYN and FIN)

            [System.Runtime.InteropServices.FieldOffset(14)]
            public System.UInt16 WindowSize; //Window size

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.UInt16 Checksum; //Checksum - includes TCP packer header and TCP packet data payload as well as an IPv4 "pseudo header"

            [System.Runtime.InteropServices.FieldOffset(18)]
            public System.UInt16 UrgentPointer; //Urgent pointer

            //There may be a options field of 0 – 40 bytes at the end of the structure dependent on the value of the TCP packet header length field
        }
    }
}