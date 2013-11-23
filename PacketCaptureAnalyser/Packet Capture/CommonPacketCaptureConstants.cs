//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureProcessingNamespace
{
    class CommonPackageCaptureConstants
    {
        //Network data link type/Network encapsulation type

        public enum CommonPackageCaptureNetworkDataLinkType : ushort
        {
            NullLoopBack = 0, //Null/Loopback
            Ethernet = 1, //Ethernet
            CiscoHDLC = 104, //Cisco HDLC (Proprietary)
            Invalid = ushort.MaxValue //Invalid
        }
    }
}