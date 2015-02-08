// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.IPPacket.UDPDatagram
{
    /// <summary>
    /// This class provides the UDP datagram processing
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
        /// The reusable instance of the UDP datagram header
        /// </summary>
        private Structures.HeaderStructure theUDPDatagramHeader;

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

            // Create an instance of the UDP datagram header
            this.theUDPDatagramHeader = new Structures.HeaderStructure();
        }

        /// <summary>
        /// Processes a UDP datagram
        /// </summary>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <param name="theIPPacketPayloadLength">The length of the payload of the IP v4/v6 packet</param>
        /// <returns>Boolean flag that indicates whether the UDP datagram could be processed</returns>
        public bool ProcessUDPDatagram(ulong thePacketNumber, double thePacketTimestamp, ushort theIPPacketPayloadLength)
        {
            bool theResult = true;

            ushort theUDPDatagramPayloadLength = 0;

            ushort theUDPDatagramSourcePort = 0;
            ushort theUDPDatagramDestinationPort = 0;

            // Process the UDP datagram header
            theResult = this.ProcessUDPDatagramHeader(
                theIPPacketPayloadLength,
                out theUDPDatagramPayloadLength,
                out theUDPDatagramSourcePort,
                out theUDPDatagramDestinationPort);

            if (theResult)
            {
                // Process the payload of the UDP datagram, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the UDP datagram header
                theResult = this.ProcessUDPDatagramPayload(
                    thePacketNumber,
                    thePacketTimestamp,
                    theUDPDatagramPayloadLength,
                    theUDPDatagramSourcePort,
                    theUDPDatagramDestinationPort);
            }

            return theResult;
        }

        /// <summary>
        /// Processes a UDP datagram header
        /// </summary>
        /// <param name="theIPPacketPayloadLength">The length of the payload of the IP v4/v6 packet</param>
        /// <param name="theUDPDatagramPayloadLength">The length of the payload of the UDP datagram</param>
        /// <param name="theUDPDatagramSourcePort">The source port for the UDP datagrams</param>
        /// <param name="theUDPDatagramDestinationPort">The destination port for the UDP datagrams</param>
        /// <returns>Boolean flag that indicates whether the UDP datagram packet header could be processed</returns>
        private bool ProcessUDPDatagramHeader(ushort theIPPacketPayloadLength, out ushort theUDPDatagramPayloadLength, out ushort theUDPDatagramSourcePort, out ushort theUDPDatagramDestinationPort)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the length of the payload of the UDP datagram
            theUDPDatagramPayloadLength = 0;

            // Provide default values for the output parameters for source port and destination port
            theUDPDatagramSourcePort = 0;
            theUDPDatagramDestinationPort = 0;

            // Read the values for the UDP datagram header from the packet capture
            this.theUDPDatagramHeader.SourcePort = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theUDPDatagramHeader.DestinationPort = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theUDPDatagramHeader.Length = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theUDPDatagramHeader.Checksum = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());

            // Validate the UDP datagram header
            theResult = this.ValidateUDPDatagramHeader(
                theIPPacketPayloadLength);

            if (theResult)
            {
                // Set up the output parameter for the length of the payload of the UDP datagram, which is the total length of the UDP datagram read from the UDP datagram header minus the length of the UDP datagram header
                theUDPDatagramPayloadLength =
                    (ushort)(theIPPacketPayloadLength - Constants.HeaderLength);

                // Set up the output parameters for source port and destination port using the value read from the UDP datagram header
                theUDPDatagramSourcePort = this.theUDPDatagramHeader.SourcePort;
                theUDPDatagramDestinationPort = this.theUDPDatagramHeader.DestinationPort;
            }

            return theResult;
        }

        /// <summary>
        /// Processes the payload of the UDP datagram
        /// </summary>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <param name="theUDPDatagramPayloadLength">The length of the payload of the UDP datagram</param>
        /// <param name="theUDPDatagramSourcePort">The source port for the UDP datagrams</param>
        /// <param name="theUDPDatagramDestinationPort">The destination port for the UDP datagrams</param>
        /// <returns>Boolean flag that indicates whether the payload of the UDP datagram could be processed</returns>
        private bool ProcessUDPDatagramPayload(ulong thePacketNumber, double thePacketTimestamp, ushort theUDPDatagramPayloadLength, ushort theUDPDatagramSourcePort, ushort theUDPDatagramDestinationPort)
        {
            bool theResult = true;

            // Only process this UDP datagram if the payload has a non-zero payload length i.e. it actually includes data (unlikely to not include data, but retain check for consistency with TCP packet processing)
            if (theUDPDatagramPayloadLength > 0)
            {
                switch (theUDPDatagramSourcePort)
                {
                    //// Put extra cases and code here to identify and process specific messages within the UDP datagram

                    default:
                        {
                            // Just read off the remaining bytes of the UDP datagram from the packet capture so we can move on
                            // The remaining length is the supplied length of the UDP datagram payload
                            this.theBinaryReader.ReadBytes(
                                theUDPDatagramPayloadLength);

                            break;
                        }
                }
            }

            return theResult;
        }

        /// <summary>
        /// Validates the UDP datagram header
        /// </summary>
        /// <param name="theIPPacketPayloadLength">The length of the payload of the IP v4/v6 packet</param>
        /// <returns>Boolean flag that indicates whether the UDP datagram header is valid</returns>
        private bool ValidateUDPDatagramHeader(ushort theIPPacketPayloadLength)
        {
            bool theResult = true;

            // The length in the UDP datagram header includes both the header itself and the payload so should equal the length of the UDP datagram payload in the IP packet
            if (this.theUDPDatagramHeader.Length != theIPPacketPayloadLength)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The UDP datagram header indicates a total length " +
                    this.theUDPDatagramHeader.Length.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " which is not equal to the length of the UDP datagram within the IP packet of " +
                    theIPPacketPayloadLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " !!!");

                theResult = false;
            }

            // The length in the UDP datagram header includes both the header itself and the payload so the minimum length is that of just the header
            if (this.theUDPDatagramHeader.Length < Constants.HeaderLength)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The UDP datagram contains an unexpected header length, is " +
                    this.theUDPDatagramHeader.Length.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.HeaderLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " or above!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
