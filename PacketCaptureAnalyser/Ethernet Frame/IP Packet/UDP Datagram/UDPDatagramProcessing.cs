﻿//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace.IPPacketNamespace.UDPDatagramNamespace
{
    class UDPDatagramProcessing
    {
        private System.IO.BinaryReader TheBinaryReader;

        private UDPDatagramStructures.UDPDatagramHeaderStructure TheHeader;

        private bool PerformLatencyAnalysisProcessing;
        private AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing;

        private bool PerformTimeAnalysisProcessing;
        private AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing;

        public UDPDatagramProcessing(System.IO.BinaryReader TheBinaryReader, bool PerformLatencyAnalysisProcessing, AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing, bool PerformTimeAnalysisProcessing, AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing)
        {
            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the UDP datagram header
            TheHeader = new UDPDatagramStructures.UDPDatagramHeaderStructure();

            this.PerformLatencyAnalysisProcessing = PerformLatencyAnalysisProcessing;
            this.TheLatencyAnalysisProcessing = TheLatencyAnalysisProcessing;

            this.PerformTimeAnalysisProcessing = PerformTimeAnalysisProcessing;
            this.TheTimeAnalysisProcessing = TheTimeAnalysisProcessing;
        }

        public bool Process(ulong ThePacketNumber, double TheTimestamp, ushort TheLength)
        {
            bool TheResult = true;

            ushort ThePayloadLength = 0;

            ushort TheSourcePort = 0;
            ushort TheDestinationPort = 0;

            //Process the UDP datagram header
            TheResult = ProcessHeader(TheLength, out ThePayloadLength, out TheSourcePort, out TheDestinationPort);

            if (TheResult)
            {
                //Process the payload of the UDP datagram, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the UDP datagram header
                TheResult = ProcessPayload(ThePacketNumber, TheTimestamp, ThePayloadLength, TheSourcePort, TheDestinationPort);
            }

            return TheResult;
        }

        private bool ProcessHeader(ushort TheLength, out ushort ThePayloadLength, out ushort TheSourcePort, out ushort TheDestinationPort)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the length of the payload of the UDP datagram
            ThePayloadLength = 0;

            //Provide default values for the output parameters for source port and destination port
            TheSourcePort = 0;
            TheDestinationPort = 0;

            //Read the values for the UDP datagram header from the packet capture
            TheHeader.SourcePort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.DestinationPort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.Length = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.Checksum = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());

            //Validate the UDP datagram header
            TheResult = ValidateHeader(TheHeader, TheLength, TheHeader.Length);

            if (TheResult)
            {
                //Set up the output parameter for the length of the payload of the UDP datagram, which is the total length of the UDP datagram read from the UDP datagram header minus the length of the UDP datagram header
                ThePayloadLength = (ushort)(TheLength - UDPDatagramConstants.UDPDatagramHeaderLength);

                //Set up the output parameters for source port and destination port using the value read from the UDP datagram header
                TheSourcePort = TheHeader.SourcePort;
                TheDestinationPort = TheHeader.DestinationPort;
            }

            return TheResult;
        }

        private bool ProcessPayload(ulong ThePacketNumber, double TheTimestamp, ushort ThePayloadLength, ushort TheSourcePort, ushort TheDestinationPort)
        {
            bool TheResult = true;

            //Only process this UDP datagram if the payload has a non-zero payload length i.e. it actually includes data (unlikely to not include data, but retain check for consistency with TCP packet processing)
            if (ThePayloadLength > 0)
            {
                switch (TheSourcePort)
                {
                    //Put extra cases and code here to identify and process specific messages within the UDP datagram

                    default:
                        {
                            //Just read off the remaining bytes of the UDP datagram from the packet capture so we can move on
                            //The remaining length is the supplied length of the UDP datagram payload
                            TheBinaryReader.ReadBytes(ThePayloadLength);
                            break;
                        }
                }
            }

            return TheResult;
        }

        private bool ValidateHeader(UDPDatagramStructures.UDPDatagramHeaderStructure TheHeader, ushort TheLength, ushort TheHeaderLength)
        {
            bool TheResult = true;

            //The length in the UDP datagram header includes both the header itself and the payload so should equal the length of the UDP datagram payload in the IP packet
            if (TheHeader.Length != TheLength)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The UDP datagram header indicates a total length " +
                    TheHeader.Length.ToString() +
                    " which is not equal to the length of the UDP datagram within the IP packet of " +
                    TheLength.ToString() +
                    " !!!"
                    );

                TheResult = false;
            }

            //The length in the UDP datagram header includes both the header itself and the payload so the minimum length is that of just the header
            if (TheHeaderLength < UDPDatagramConstants.UDPDatagramHeaderLength)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The UDP datagram contains an unexpected header length, is " +
                    TheHeaderLength.ToString() +
                    " not " +
                    UDPDatagramConstants.UDPDatagramHeaderLength.ToString() +
                    " or above"
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}