// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.TCPPacket
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

            // Create an instance of the TCP packet header
            this.theHeader = new Structures.HeaderStructure();
        }

        public bool Process(ulong thePacketNumber, double theTimestamp, ushort theLength)
        {
            bool theResult = true;

            ushort thePayloadLength = 0;

            ushort theSourcePort = 0;
            ushort theDestinationPort = 0;

            // Process the TCP packet header
            theResult = this.ProcessHeader(theLength, out thePayloadLength, out theSourcePort, out theDestinationPort);

            if (theResult)
            {
                // Process the payload of the TCP packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the TCP packet header
                theResult = this.ProcessPayload(thePacketNumber, theTimestamp, thePayloadLength, theSourcePort, theDestinationPort);
            }

            return theResult;
        }

        private bool ProcessHeader(ushort theLength, out ushort thePayloadLength, out ushort theSourcePort, out ushort theDestinationPort)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the length of the payload of the TCP packet
            thePayloadLength = 0;

            // Provide default values for the output parameters for source port and destination port
            theSourcePort = 0;
            theDestinationPort = 0;

            // Read the values for the TCP packet header from the packet capture
            this.theHeader.SourcePort = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theHeader.DestinationPort = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theHeader.SequenceNumber = this.theBinaryReader.ReadUInt32();
            this.theHeader.AcknowledgmentNumber = this.theBinaryReader.ReadUInt32();
            this.theHeader.DataOffsetAndReservedAndNSFlag = this.theBinaryReader.ReadByte();
            this.theHeader.Flags = this.theBinaryReader.ReadByte();
            this.theHeader.WindowSize = this.theBinaryReader.ReadUInt16();
            this.theHeader.Checksum = this.theBinaryReader.ReadUInt16();
            this.theHeader.UrgentPointer = this.theBinaryReader.ReadUInt16();

            // Determine the length of the TCP packet header
            // Need to first extract the length value from the combined TCP packet header length, reserved fields and NS flag field
            // We want the higher four bits from the combined TCP packet header length, reserved fields and NS flag field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            // The extracted length value is the length of the TCP packet header in 32-bit words so multiply by four to get the actual length in bytes of the TCP packet header
            ushort theHeaderLength = (ushort)(((this.theHeader.DataOffsetAndReservedAndNSFlag & 0xF0) >> 4) * 4);

            // Validate the TCP packet header
            theResult = this.ValidateHeader(this.theHeader, theHeaderLength);

            if (theResult)
            {
                // Set up the output parameter for the length of the payload of the TCP packet, which is the total length of the TCP packet minus the length of the TCP packet header just calculated
                thePayloadLength = (ushort)(theLength - theHeaderLength);

                // Set up the output parameters for source port and destination port using the value read from the TCP packet header
                theSourcePort = this.theHeader.SourcePort;
                theDestinationPort = this.theHeader.DestinationPort;

                if (theHeaderLength > Constants.HeaderMinimumLength)
                {
                    // The TCP packet contains a header length which is greater than the minimum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    // Just read off these remaining Options bytes of the TCP packet header from the packet capture so we can move on
                    this.theBinaryReader.ReadBytes(theHeaderLength - Constants.HeaderMinimumLength);
                }
            }
            else
            {
                thePayloadLength = 0;
            }

            return theResult;
        }

        private bool ProcessPayload(ulong thePacketNumber, double theTimestamp, ushort thePayloadLength, ushort theSourcePort, ushort theDestinationPort)
        {
            bool theResult = true;

            // Only process this TCP packet if the payload has a non-zero payload length i.e. it actually includes data so is not part of the three-way handshake or a plain acknowledgement
            if (thePayloadLength > 0)
            {
                // Change this logic statement to allow identification and processing of specific messages within the TCP packet
                if (false)
                {
                    // Put code here to identify and process specific messages within the TCP packet
                }
                else
                {
                    // Just read off the remaining bytes of the TCP packet from the packet capture so we can move on
                    // The remaining length is the supplied length of the TCP packet payload
                    this.theBinaryReader.ReadBytes(thePayloadLength);
                }
            }

            return theResult;
        }

        private bool ValidateHeader(Structures.HeaderStructure theHeader, ushort theHeaderLength)
        {
            bool theResult = true;

            if (theHeaderLength > Constants.HeaderMaximumLength ||
                theHeaderLength < Constants.HeaderMinimumLength)
            {
                this.theDebugInformation.WriteErrorEvent("The TCP packet contains a header length " +
                    theHeaderLength.ToString() +
                    " which is outside the range " +
                    Constants.HeaderMinimumLength.ToString() +
                    " to " +
                    Constants.HeaderMaximumLength.ToString() +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
