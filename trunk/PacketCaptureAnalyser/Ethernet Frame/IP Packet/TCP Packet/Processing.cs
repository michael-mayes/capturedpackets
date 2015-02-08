// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.IPPacket.TCPPacket
{
    /// <summary>
    /// This class provides the TCP packet processing
    /// </summary>
    public class Processing
    {
        /// <summary>
        /// The object that provides for the logging of debug information
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// The object that provides for binary reading from the packet capture
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// The reusable instance of the TCP packet header
        /// </summary>
        private Structures.HeaderStructure theTCPPacketHeader;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="performLatencyAnalysisProcessing">Boolean flag that indicates whether to perform latency analysis processing for data read from the packet capture</param>
        /// <param name="theLatencyAnalysisProcessing">The object that provides the latency analysis processing for data read from the packet capture</param>
        /// <param name="performBurstAnalysisProcessing">Boolean flag that indicates whether to perform burst analysis processing for data read from the packet capture</param>
        /// <param name="theBurstAnalysisProcessing">The object that provides the burst analysis processing for data read from the packet capture</param>
        /// <param name="performTimeAnalysisProcessing">Boolean flag that indicates whether to perform time analysis processing for data read from the packet capture</param>
        /// <param name="theTimeAnalysisProcessing">The object that provides the time analysis processing for data read from the packet capture</param>
        /// <param name="useAlternativeSequenceNumber">Boolean flag that indicates whether to use the alternative sequence number in the data read from the packet capture, required for legacy recordings</param>
        public Processing(Analysis.DebugInformation theDebugInformation, System.IO.BinaryReader theBinaryReader, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performBurstAnalysisProcessing, Analysis.BurstAnalysis.Processing theBurstAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing, bool useAlternativeSequenceNumber)
        {
            this.theDebugInformation = theDebugInformation;

            this.theBinaryReader = theBinaryReader;

            // Create an instance of the TCP packet header
            this.theTCPPacketHeader = new Structures.HeaderStructure();
        }

        /// <summary>
        /// Processes a TCP packet
        /// </summary>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <param name="theIPPacketPayloadLength">The length of the payload of the IP v4/v6 packet</param>
        /// <returns>Boolean flag that indicates whether the TCP packet could be processed</returns>
        public bool ProcessTCPPacket(ulong thePacketNumber, double thePacketTimestamp, ushort theIPPacketPayloadLength)
        {
            bool theResult = true;

            ushort theTCPPacketPayloadLength = 0;

            ushort theTCPPacketSourcePort = 0;
            ushort theTCPPacketDestinationPort = 0;

            // Process the TCP packet header
            theResult = this.ProcessTCPPacketHeader(
                theIPPacketPayloadLength,
                out theTCPPacketPayloadLength,
                out theTCPPacketSourcePort,
                out theTCPPacketDestinationPort);

            if (theResult)
            {
                // Process the payload of the TCP packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the TCP packet header
                theResult = this.ProcessTCPPacketPayload(
                    thePacketNumber,
                    thePacketTimestamp,
                    theTCPPacketPayloadLength,
                    theTCPPacketSourcePort,
                    theTCPPacketDestinationPort);
            }

            return theResult;
        }

        /// <summary>
        /// Processes a TCP packet header
        /// </summary>
        /// <param name="theIPPacketPayloadLength">The length of the payload of the IP v4/v6 packet</param>
        /// <param name="theTCPPacketPayloadLength">The length of the payload of the TCP packet</param>
        /// <param name="theTCPPacketSourcePort">The source port for the TCP packet</param>
        /// <param name="theTCPPacketDestinationPort">The destination port for the TCP packet</param>
        /// <returns>Boolean flag that indicates whether the TCP packet header could be processed</returns>
        private bool ProcessTCPPacketHeader(ushort theIPPacketPayloadLength, out ushort theTCPPacketPayloadLength, out ushort theTCPPacketSourcePort, out ushort theTCPPacketDestinationPort)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the length of the payload of the TCP packet
            theTCPPacketPayloadLength = 0;

            // Provide default values for the output parameters for source port and destination port
            theTCPPacketSourcePort = 0;
            theTCPPacketDestinationPort = 0;

            // Read the values for the TCP packet header from the packet capture
            this.theTCPPacketHeader.SourcePort = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theTCPPacketHeader.DestinationPort = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theTCPPacketHeader.SequenceNumber = this.theBinaryReader.ReadUInt32();
            this.theTCPPacketHeader.AcknowledgmentNumber = this.theBinaryReader.ReadUInt32();
            this.theTCPPacketHeader.DataOffsetAndReservedAndNSFlag = this.theBinaryReader.ReadByte();
            this.theTCPPacketHeader.Flags = this.theBinaryReader.ReadByte();
            this.theTCPPacketHeader.WindowSize = this.theBinaryReader.ReadUInt16();
            this.theTCPPacketHeader.Checksum = this.theBinaryReader.ReadUInt16();
            this.theTCPPacketHeader.UrgentPointer = this.theBinaryReader.ReadUInt16();

            // Determine the length of the TCP packet header
            // Need to first extract the length value from the combined TCP packet header length, reserved fields and NS flag field
            // We want the higher four bits from the combined TCP packet header length, reserved fields and NS flag field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            // The extracted length value is the length of the TCP packet header in 32-bit words so multiply by four to get the actual length in bytes of the TCP packet header
            ushort theTCPPacketHeaderLength =
                (ushort)(((this.theTCPPacketHeader.DataOffsetAndReservedAndNSFlag & 0xF0) >> 4) * 4);

            // Validate the TCP packet header
            theResult = this.ValidateTCPPacketHeader(
                theTCPPacketHeaderLength);

            if (theResult)
            {
                // Set up the output parameter for the length of the payload of the TCP packet, which is the total length of the TCP packet minus the length of the TCP packet header just calculated
                theTCPPacketPayloadLength =
                    (ushort)(theIPPacketPayloadLength - theTCPPacketHeaderLength);

                // Set up the output parameters for source port and destination port using the value read from the TCP packet header
                theTCPPacketSourcePort = this.theTCPPacketHeader.SourcePort;
                theTCPPacketDestinationPort = this.theTCPPacketHeader.DestinationPort;

                if (theTCPPacketHeaderLength > Constants.HeaderMinimumLength)
                {
                    // The TCP packet contains a header length which is greater than the minimum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    // Just read off these remaining Options bytes of the TCP packet header from the packet capture so we can move on
                    this.theBinaryReader.ReadBytes(
                        theTCPPacketHeaderLength - Constants.HeaderMinimumLength);
                }
            }
            else
            {
                theTCPPacketPayloadLength = 0;
            }

            return theResult;
        }

        /// <summary>
        /// Processes the payload of the TCP packet
        /// </summary>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <param name="theTCPPacketPayloadLength">The length of the payload of the TCP packet</param>
        /// <param name="theTCPPacketSourcePort">The source port for the TCP packet</param>
        /// <param name="theTCPPacketDestinationPort">The destination port for the TCP packet</param>
        /// <returns>Boolean flag that indicates whether the payload of the TCP packet could be processed</returns>
        private bool ProcessTCPPacketPayload(ulong thePacketNumber, double thePacketTimestamp, ushort theTCPPacketPayloadLength, ushort theTCPPacketSourcePort, ushort theTCPPacketDestinationPort)
        {
            bool theResult = true;

            // Only process this TCP packet if the payload has a non-zero payload length i.e. it actually includes data so is not part of the three-way handshake or a plain acknowledgement
            if (theTCPPacketPayloadLength > 0)
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
                    this.theBinaryReader.ReadBytes(
                        theTCPPacketPayloadLength);
                }
            }

            return theResult;
        }

        /// <summary>
        /// Validates the TCP packet header
        /// </summary>
        /// <param name="theTCPPacketHeaderLength">The length of the TCP packet header</param>
        /// <returns>Boolean flag that indicates whether the TCP packet header is valid</returns>
        private bool ValidateTCPPacketHeader(ushort theTCPPacketHeaderLength)
        {
            bool theResult = true;

            if (theTCPPacketHeaderLength > Constants.HeaderMaximumLength ||
                theTCPPacketHeaderLength < Constants.HeaderMinimumLength)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The TCP packet contains a header length " +
                    theTCPPacketHeaderLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " which is outside the range " +
                    Constants.HeaderMinimumLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " to " +
                    Constants.HeaderMaximumLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
