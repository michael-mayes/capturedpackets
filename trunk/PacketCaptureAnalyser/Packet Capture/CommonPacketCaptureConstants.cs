//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureProcessingNamespace
{
    class CommonPackageCaptureConstants
    {
        //Network data link type/Network encapsulation type

        public const System.UInt32 CommonPackageCaptureNullLoopBackNetworkDataLinkType = 0; //Null/Loopback
        public const System.UInt32 CommonPackageCaptureEthernetNetworkDataLinkType = 1; //Ethernet
        public const System.UInt32 CommonPackageCaptureCiscoHDLCNetworkDataLinkType = 104; //Cisco HDLC (Proprietary)
        public const System.UInt32 CommonPackageCaptureInvalidNetworkDataLinkType = System.UInt32.MaxValue; //Invalid
    }
}