// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.UDPDatagram
{
    class Processing
    {
        private Analysis.DebugInformation theDebugInformation;

        private System.IO.BinaryReader theBinaryReader;

        private Structures.HeaderStructure theHeader;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation"></param>
        /// <param name="theBinaryReader"></param>
        /// <param name="performLatencyAnalysisProcessing"></param>
        /// <param name="theLatencyAnalysisProcessing"></param>
        /// <param name="performTimeAnalysisProcessing"></param>
        /// <param name="theTimeAnalysisProcessing"></param>
        public Processing(Analysis.DebugInformation theDebugInformation, System.IO.BinaryReader theBinaryReader, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing)
        {
            this.theDebugInformation = theDebugInformation;

            this.theBinaryReader = theBinaryReader;

            // Create an instance of the UDP datagram header
            this.theHeader = new Structures.HeaderStructure();
        }

        public bool Process(ulong thePacketNumber, double theTimestamp, ushort theLength)
        {
            bool theResult = true;

            ushort thePayloadLength = 0;

            ushort theSourcePort = 0;
            ushort theDestinationPort = 0;

            // Process the UDP datagram header
            theResult = this.ProcessHeader(theLength, out thePayloadLength, out theSourcePort, out theDestinationPort);

            if (theResult)
            {
                // Process the payload of the UDP datagram, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the UDP datagram header
                theResult = this.ProcessPayload(thePacketNumber, theTimestamp, thePayloadLength, theSourcePort, theDestinationPort);
            }

            return theResult;
        }

        private bool ProcessHeader(ushort theLength, out ushort thePayloadLength, out ushort theSourcePort, out ushort theDestinationPort)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the length of the payload of the UDP datagram
            thePayloadLength = 0;

            // Provide default values for the output parameters for source port and destination port
            theSourcePort = 0;
            theDestinationPort = 0;

            // Read the values for the UDP datagram header from the packet capture
            this.theHeader.SourcePort = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theHeader.DestinationPort = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theHeader.Length = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theHeader.Checksum = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());

            // Validate the UDP datagram header
            theResult = this.ValidateHeader(this.theHeader, theLength, this.theHeader.Length);

            if (theResult)
            {
                // Set up the output parameter for the length of the payload of the UDP datagram, which is the total length of the UDP datagram read from the UDP datagram header minus the length of the UDP datagram header
                thePayloadLength = (ushort)(theLength - Constants.HeaderLength);

                // Set up the output parameters for source port and destination port using the value read from the UDP datagram header
                theSourcePort = this.theHeader.SourcePort;
                theDestinationPort = this.theHeader.DestinationPort;
            }

            return theResult;
        }

        private bool ProcessPayload(ulong thePacketNumber, double theTimestamp, ushort thePayloadLength, ushort theSourcePort, ushort theDestinationPort)
        {
            bool theResult = true;

            // Only process this UDP datagram if the payload has a non-zero payload length i.e. it actually includes data (unlikely to not include data, but retain check for consistency with TCP packet processing)
            if (thePayloadLength > 0)
            {
                switch (theSourcePort)
                {
                    //// Put extra cases and code here to identify and process specific messages within the UDP datagram

                    default:
                        {
                            // Just read off the remaining bytes of the UDP datagram from the packet capture so we can move on
                            // The remaining length is the supplied length of the UDP datagram payload
                            this.theBinaryReader.ReadBytes(thePayloadLength);
                            break;
                        }
                }
            }

            return theResult;
        }

        private bool ValidateHeader(Structures.HeaderStructure theHeader, ushort theLength, ushort theHeaderLength)
        {
            bool theResult = true;

            // The length in the UDP datagram header includes both the header itself and the payload so should equal the length of the UDP datagram payload in the IP packet
            if (this.theHeader.Length != theLength)
            {
                this.theDebugInformation.WriteErrorEvent("The UDP datagram header indicates a total length " +
                    this.theHeader.Length.ToString() +
                    " which is not equal to the length of the UDP datagram within the IP packet of " +
                    theLength.ToString() +
                    " !!!");

                theResult = false;
            }

            // The length in the UDP datagram header includes both the header itself and the payload so the minimum length is that of just the header
            if (theHeaderLength < Constants.HeaderLength)
            {
                this.theDebugInformation.WriteErrorEvent("The UDP datagram contains an unexpected header length, is " +
                    theHeaderLength.ToString() +
                    " not " +
                    Constants.HeaderLength.ToString() +
                    " or above!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
