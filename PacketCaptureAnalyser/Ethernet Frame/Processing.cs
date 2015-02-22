// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame
{
    /// <summary>
    /// This class provides the Ethernet frame processing
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
        /// The length of the Ethernet frame
        /// </summary>
        private long theEthernetFrameLength;

        /// <summary>
        /// The type of the Ethernet frame
        /// </summary>
        private ushort theEthernetFrameType;

        /// <summary>
        /// The reusable instance of the processing class for ARP packets
        /// </summary>
        private ARPPacket.Processing theARPPacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for DEC DNA Remote Console packets
        /// </summary>
        private DECDNARemoteConsolePacket.Processing theDECDNARemoteConsolePacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for IP v4 packets
        /// </summary>
        private IPPacket.IPv4Packet.Processing theIPv4PacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for IP v6 packets
        /// </summary>
        private IPPacket.IPv6Packet.Processing theIPv6PacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for LLDP packets
        /// </summary>
        private LLDPPacket.Processing theLLDPPacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for Loopback packets
        /// </summary>
        private LoopbackPacket.Processing theLoopbackPacketProcessing;

        /// <summary>
        /// The reusable instance of the processing class for RARP packets
        /// </summary>
        private RARPPacket.Processing theRARPPacketProcessing;

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

            //// Create instances of the processing classes for each supported type for an Ethernet frame

            this.theARPPacketProcessing =
                new ARPPacket.Processing(
                    theDebugInformation,
                    theBinaryReader);

            this.theDECDNARemoteConsolePacketProcessing =
                new DECDNARemoteConsolePacket.Processing(
                    theDebugInformation,
                    theBinaryReader);

            this.theIPv4PacketProcessing =
                new IPPacket.IPv4Packet.Processing(
                    theDebugInformation,
                    theBinaryReader,
                    performLatencyAnalysisProcessing,
                    theLatencyAnalysisProcessing,
                    performBurstAnalysisProcessing,
                    theBurstAnalysisProcessing,
                    performTimeAnalysisProcessing,
                    theTimeAnalysisProcessing,
                    useAlternativeSequenceNumber);

            this.theIPv6PacketProcessing =
                new IPPacket.IPv6Packet.Processing(
                    theDebugInformation,
                    theBinaryReader,
                    performLatencyAnalysisProcessing,
                    theLatencyAnalysisProcessing,
                    performBurstAnalysisProcessing,
                    theBurstAnalysisProcessing,
                    performTimeAnalysisProcessing,
                    theTimeAnalysisProcessing,
                    useAlternativeSequenceNumber);

            this.theLLDPPacketProcessing =
                new LLDPPacket.Processing(
                    theDebugInformation,
                    theBinaryReader);

            this.theLoopbackPacketProcessing =
                new LoopbackPacket.Processing(
                    theDebugInformation,
                    theBinaryReader);

            this.theRARPPacketProcessing =
                new RARPPacket.Processing(
                    theDebugInformation,
                    theBinaryReader);
        }

        /// <summary>
        /// Process an Ethernet frame
        /// </summary>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        public void ProcessEthernetFrame(ulong thePacketNumber, long thePacketPayloadLength, double thePacketTimestamp)
        {
            // Only process the payload of the packet as an Ethernet frame if it has a positive length
            if (thePacketPayloadLength > 0)
            {
                // Store the length of the payload of the packet as the total length of the Ethernet frame for use in further processing
                this.theEthernetFrameLength =
                    thePacketPayloadLength;

                // Process the Ethernet frame header
                this.ProcessEthernetFrameHeader();

                // Read the type for the Ethernet frame from the packet capture and process it
                this.DetermineEthernetFrameType(
                    thePacketNumber);

                this.ProcessEthernetFramePayload(
                    thePacketNumber,
                    thePacketTimestamp);
            }
        }

        /// <summary>
        /// Processes the Ethernet frame header
        /// </summary>
        private void ProcessEthernetFrameHeader()
        {
            // Read the Destination MAC Address for the Ethernet frame from the packet capture
            this.theBinaryReader.ReadUInt32(); // High bytes
            this.theBinaryReader.ReadUInt16(); // Low bytes

            // Read the Source MAC Address for the Ethernet frame from the packet capture
            this.theBinaryReader.ReadUInt32(); // High bytes
            this.theBinaryReader.ReadUInt16(); // Low bytes

            // Read off and store the type of the Ethernet frame from the packet capture for use in further processing
            this.theEthernetFrameType =
                (ushort)System.Net.IPAddress.NetworkToHostOrder(
                this.theBinaryReader.ReadInt16());

            // Reduce the length of the Ethernet frame to reflect that the two bytes for the type of the Ethernet frame would have been included
            this.theEthernetFrameLength -= 2;
        }

        /// <summary>
        /// Determines the type of the Ethernet frame
        /// </summary>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        private void DetermineEthernetFrameType(ulong thePacketNumber)
        {
            switch (this.theEthernetFrameType)
            {
                case (ushort)Constants.HeaderEthernetFrameType.ARP:
                case (ushort)Constants.HeaderEthernetFrameType.DECDNARemoteConsole:
                case (ushort)Constants.HeaderEthernetFrameType.IPv4:
                case (ushort)Constants.HeaderEthernetFrameType.IPv6:
                case (ushort)Constants.HeaderEthernetFrameType.LLDP:
                case (ushort)Constants.HeaderEthernetFrameType.Loopback:
                case (ushort)Constants.HeaderEthernetFrameType.RARP:
                    {
                        break;
                    }

                case (ushort)Constants.HeaderEthernetFrameType.VLANTagged:
                    {
                        // We have got an Ethernet frame with a VLAN tag (IEEE 802.1Q) so must advance and re-read the type

                        // The "type" we've just read will actually be the IEEE 802.1Q Tag Protocol Identifier

                        // First just read off the IEEE 802.1Q Tag Control Identifier so we can move on
                        this.theBinaryReader.ReadUInt16();

                        // Then re-read the type of the Ethernet frame, this time obtaining the real value (so long as there is only one VLAN tag of course!)
                        this.theEthernetFrameType =
                            (ushort)System.Net.IPAddress.NetworkToHostOrder(
                            this.theBinaryReader.ReadInt16());

                        // Reduce the length of the Ethernet frame to reflect that the VLAN tag of four bytes would have been included
                        this.theEthernetFrameLength -= 4;

                        break;
                    }

                default:
                    {
                        // We have got an Ethernet frame containing an unrecognised type

                        // Check against the minimum value for the type of the Ethernet frame - lower values indicate length of an IEEE 802.3 Ethernet frame
                        if (this.theEthernetFrameType < (ushort)Constants.HeaderEthernetFrameType.MinimumValue)
                        {
                            // This Ethernet frame has a value for "type" lower than the minimum
                            // This is an IEEE 802.3 Ethernet frame rather than an Ethernet II frame
                            // This value is the length of the IEEE 802.3 Ethernet frame
                        }
                        else
                        {
                            // The type for this Ethernet frame has an unknown value
                            this.theDebugInformation.WriteInformationEvent(
                                "The Ethernet frame in packet number " +
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                                " contains an unexpected type of 0x" +
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:X}", this.theEthernetFrameType) +
                                "! - Attempt to recover and continue processing");
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Processes the payload of the Ethernet frame
        /// </summary>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp read from the packet capture</param>
        private void ProcessEthernetFramePayload(ulong thePacketNumber, double thePacketTimestamp)
        {
            // Record the position in the stream for the packet capture so we can later determine how far has been progressed
            long theStartingStreamPosition =
                this.theBinaryReader.BaseStream.Position;

            bool thePacketProcessingResult = true;

            // Check the value of the type of this Ethernet frame
            switch (this.theEthernetFrameType)
            {
                case (ushort)Constants.HeaderEthernetFrameType.ARP:
                    {
                        // We have got an Ethernet frame containing an ARP packet so process it
                        thePacketProcessingResult =
                            this.theARPPacketProcessing.ProcessARPPacket(
                            this.theEthernetFrameLength);

                        break;
                    }

                case (ushort)Constants.HeaderEthernetFrameType.DECDNARemoteConsole:
                    {
                        // We have got an Ethernet frame containing an ARP packet so process it
                        thePacketProcessingResult =
                            this.theDECDNARemoteConsolePacketProcessing.ProcessDECDNARemoteConsolePacket();

                        break;
                    }

                case (ushort)Constants.HeaderEthernetFrameType.IPv4:
                    {
                        // We have got an Ethernet frame containing an IP v4 packet so process it
                        thePacketProcessingResult =
                            this.theIPv4PacketProcessing.ProcessIPv4Packet(
                            this.theEthernetFrameLength,
                            thePacketNumber,
                            thePacketTimestamp);

                        break;
                    }

                case (ushort)Constants.HeaderEthernetFrameType.IPv6:
                    {
                        // We have got an Ethernet frame containing an IP v6 packet so process it
                        thePacketProcessingResult =
                            this.theIPv6PacketProcessing.ProcessIPv6Packet(
                            this.theEthernetFrameLength,
                            thePacketNumber,
                            thePacketTimestamp);

                        break;
                    }

                case (ushort)Constants.HeaderEthernetFrameType.LLDP:
                    {
                        // We have got an Ethernet frame containing an LLDP packet so process it
                        thePacketProcessingResult =
                            this.theLLDPPacketProcessing.ProcessLLDPPacket(
                            this.theEthernetFrameLength);

                        break;
                    }

                case (ushort)Constants.HeaderEthernetFrameType.Loopback:
                    {
                        // We have got an Ethernet frame containing an Configuration Test Protocol (Loopback) packet so process it
                        thePacketProcessingResult =
                            this.theLoopbackPacketProcessing.ProcessLoopbackPacket(
                            this.theEthernetFrameLength);

                        break;
                    }

                case (ushort)Constants.HeaderEthernetFrameType.RARP:
                    {
                        // We have got an Ethernet frame containing a RARP packet so process it
                        thePacketProcessingResult =
                            this.theRARPPacketProcessing.ProcessRARPPacket(
                            this.theEthernetFrameLength);

                        break;
                    }

                case (ushort)Constants.HeaderEthernetFrameType.VLANTagged:
                    {
                        //// We have got an Ethernet frame containing a second VLAN tag!

                        this.theDebugInformation.WriteInformationEvent(
                            "The Ethernet frame in packet number " +
                            string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                            " contains a second VLAN tag!" +
                            " - Attempt to recover and continue processing");

                        this.DetermineEthernetFrameType(
                            thePacketNumber);

                        // Now that the second VLAN tag has been dealt with rerecord the position in the stream for the packet capture so we can later determine how far has been progressed
                        theStartingStreamPosition =
                            this.theBinaryReader.BaseStream.Position;

                        // Call this method again recursively to process the Ethernet frame stripped of the second VLAN tag
                        this.ProcessEthernetFramePayload(
                            thePacketNumber,
                            thePacketTimestamp);

                        break;
                    }

                default:
                    {
                        // We have got an Ethernet frame containing an unrecognised type

                        // This is not strictly an error as it may be that the type of an Ethernet frame is just not supported yet
                        // Debug information line will have been previously output to indicate this instance so just continue
                        thePacketProcessingResult = true;

                        // Check against the minimum value for type of an Ethernet frame - lower values indicate length of the Ethernet frame
                        if (this.theEthernetFrameType < (ushort)Constants.HeaderEthernetFrameType.MinimumValue)
                        {
                            // This Ethernet frame has a value for "type" lower than the minimum
                            // This is an IEEE 802.3 Ethernet frame rather than an Ethernet II frame
                            // This value is the length of the IEEE 802.3 Ethernet frame

                            // Not going to process IEEE 802.3 Ethernet frames currently as they do not include any data of interest
                            // Just read off the bytes for the IEEE 802.3 Ethernet frame from the packet capture so we can move on
                            this.theBinaryReader.ReadBytes(
                                this.theEthernetFrameType);
                        }
                        else
                        {
                            // Processing of Ethernet frames with types not enumerated above are obviously not currently recognised or supported!
                            // Just fall through to the processing below that will read off the payload so we can move on
                        }

                        break;
                    }
            }

            if (!thePacketProcessingResult)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Processing of the payload of the Ethernet frame in packet number " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " encountered an error!" +
                    " - Attempt to recover and continue processing");
            }

            // Calculate how far has been progressed through the stream by the actions above to read from the packet capture
            long theStreamPositionDifference =
                this.theBinaryReader.BaseStream.Position - theStartingStreamPosition;

            //// Check whether the Ethernet frame payload has extra trailer bytes
            //// These would typically not be exposed in the packet capture by the recorder, but sometimes are for whatever reason!

            //// This processing would also read off the payload of the Ethernet frame in the event of an unrecognised, unknown or unsupported type

            if (this.theEthernetFrameLength != theStreamPositionDifference)
            {
                if (this.theEthernetFrameLength > theStreamPositionDifference)
                {
                    // Trim the extra trailer bytes (or bytes for an unrecognised, unknown or unsupported message)
                    this.theBinaryReader.ReadBytes(
                        (int)(this.theEthernetFrameLength -
                        theStreamPositionDifference));
                }
                else
                {
                    //// This is a strange error condition

                    // Back up the stream position to where it "should be"
                    this.theBinaryReader.BaseStream.Position =
                        this.theBinaryReader.BaseStream.Position -
                        (theStreamPositionDifference - this.theEthernetFrameLength);

                    // Warn about the error condition having arisen
                    this.theDebugInformation.WriteInformationEvent(
                        "The length " +
                        this.theEthernetFrameLength.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                        " of the Ethernet frame in packet number " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " does not match the progression " +
                        theStreamPositionDifference.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                        " through the stream!" +
                        " - Attempt to recover and continue processing");
                }
            }
        }
    }
}
