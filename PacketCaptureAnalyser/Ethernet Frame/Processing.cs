// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame
{
    /// <summary>
    /// This class provides the Ethernet frame processing
    /// </summary>
    public class Processing
    {
        /// <summary>
        /// 
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// 
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// 
        /// </summary>
        private Structures.HeaderStructure theHeader;

        /// <summary>
        /// 
        /// </summary>
        private ARPPacket.Processing theARPPacketProcessing;

        /// <summary>
        /// 
        /// </summary>
        private IPPacket.IPv4Packet.Processing theIPv4PacketProcessing;

        /// <summary>
        /// 
        /// </summary>
        private IPPacket.IPv6Packet.Processing theIPv6PacketProcessing;

        /// <summary>
        /// 
        /// </summary>
        private LLDPPacket.Processing theLLDPPacketProcessing;

        /// <summary>
        /// 
        /// </summary>
        private LoopbackPacket.Processing theLoopbackPacketProcessing;

        /// <summary>
        /// 
        /// </summary>
        private long thePayloadLength;

        /// <summary>
        /// 
        /// </summary>
        private ushort theEtherType;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="performLatencyAnalysisProcessing">The flag that indicates whether to perform latency analysis processing for data read from the packet capture</param>
        /// <param name="theLatencyAnalysisProcessing">The object that provides the latency analysis processing for data read from the packet capture</param>
        /// <param name="performTimeAnalysisProcessing">The flag that indicates whether to perform time analysis processing for data read from the packet capture</param>
        /// <param name="theTimeAnalysisProcessing">The object that provides the time analysis processing for data read from the packet capture</param>
        public Processing(Analysis.DebugInformation theDebugInformation, System.IO.BinaryReader theBinaryReader, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing)
        {
            this.theDebugInformation = theDebugInformation;

            this.theBinaryReader = theBinaryReader;

            // Create an instance of the Ethernet frame header
            this.theHeader = new Structures.HeaderStructure();

            // Create instances of the processing classes for each Ether Type
            this.theARPPacketProcessing = new ARPPacket.Processing(theDebugInformation, theBinaryReader);
            this.theIPv4PacketProcessing = new IPPacket.IPv4Packet.Processing(theDebugInformation, theBinaryReader, performLatencyAnalysisProcessing, theLatencyAnalysisProcessing, performTimeAnalysisProcessing, theTimeAnalysisProcessing);
            this.theIPv6PacketProcessing = new IPPacket.IPv6Packet.Processing(theDebugInformation, theBinaryReader, performLatencyAnalysisProcessing, theLatencyAnalysisProcessing, performTimeAnalysisProcessing, theTimeAnalysisProcessing);
            this.theLLDPPacketProcessing = new LLDPPacket.Processing(theDebugInformation, theBinaryReader);
            this.theLoopbackPacketProcessing = new LoopbackPacket.Processing(theDebugInformation, theBinaryReader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thePacketNumber"></param>
        /// <param name="thePayloadLength">The payload length of the Ethernet frame read from the packet capture</param>
        /// <param name="theTimestamp">The timestamp read from the packet capture</param>
        /// <returns></returns>
        public bool Process(ulong thePacketNumber, long thePayloadLength, double theTimestamp)
        {
            bool theResult = true;

            // Only process the payload as an Ethernet frame if it has a positive length
            if (thePayloadLength > 0)
            {
                // Store the length of the payload of the Ethernet frame for use in further processing
                this.thePayloadLength = thePayloadLength;

                // Process the Ethernet frame header
                theResult = this.ProcessHeader();

                if (theResult)
                {
                    // Read the Ether Type for the Ethernet frame from the packet capture and process it
                    theResult = this.ProcessEtherType(thePacketNumber);

                    if (theResult)
                    {
                        theResult = this.ProcessPayload(thePacketNumber, theTimestamp);
                    }
                }
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ProcessHeader()
        {
            bool theResult = true;

            // Read the Destination MAC Address for the Ethernet frame from the packet capture
            this.theHeader.DestinationMACAddressHigh = this.theBinaryReader.ReadUInt32();
            this.theHeader.DestinationMACAddressLow = this.theBinaryReader.ReadUInt16();

            // Read the Source MAC Address for the Ethernet frame from the packet capture
            this.theHeader.SourceMACAddressHigh = this.theBinaryReader.ReadUInt32();
            this.theHeader.SourceMACAddressLow = this.theBinaryReader.ReadUInt16();

            // Read the Ether Type for the Ethernet frame from the packet capture
            this.theHeader.EtherType = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());

            // Reduce the length of the payload of the Ethernet frame to reflect that the Ether Type of two bytes would have been included
            this.thePayloadLength -= 2;

            // Store the Ether Type for use in further processing
            this.theEtherType = this.theHeader.EtherType;

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thePacketNumber"></param>
        /// <returns></returns>
        private bool ProcessEtherType(ulong thePacketNumber)
        {
            bool theResult = true;

            switch (this.theEtherType)
            {
                case (ushort)Constants.HeaderEtherType.ARP:
                case (ushort)Constants.HeaderEtherType.IPv4:
                case (ushort)Constants.HeaderEtherType.IPv6:
                case (ushort)Constants.HeaderEtherType.LLDP:
                case (ushort)Constants.HeaderEtherType.Loopback:
                    {
                        break;
                    }

                case (ushort)Constants.HeaderEtherType.VLANTagged:
                    {
                        // We have got an Ethernet frame with a VLAN tag (IEEE 802.1Q) so must advance and re-read the Ether Type

                        // The "Ether Type" we've just read will actually be the IEEE 802.1Q Tag Protocol Identifier

                        // First just read off the IEEE 802.1Q Tag Control Identifier so we can move on
                        this.theBinaryReader.ReadUInt16();

                        // Then re-read the Ether Type, this time obtaining the real value (so long as there is only one VLAN tag of course!)
                        this.theEtherType = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());

                        // Reduce the length of the payload of the Ethernet frame to reflect that the VLAN tag of four bytes would have been included
                        this.thePayloadLength -= 4;

                        break;
                    }

                default:
                    {
                        // We have got an Ethernet frame containing an unrecognised Ether Type

                        // Check against the minimum value for Ether Type - lower values indicate length of an IEEE 802.3 Ethernet frame
                        if (this.theEtherType < (ushort)Constants.HeaderEtherType.MinimumValue)
                        {
                            // This Ethernet frame has a value for "Ether Type" lower than the minimum
                            // This is an IEEE 802.3 Ethernet frame rather than an Ethernet II frame
                            // This value is the length of the IEEE 802.3 Ethernet frame
                        }
                        else
                        {
                            // This Ethernet frame has an unknown value for Ether Type
                            this.theDebugInformation.WriteInformationEvent("The Ethernet frame in captured packet #" +
                                thePacketNumber.ToString() +
                                " contains an unexpected Ether Type of 0x" +
                                string.Format("{0:X}", this.theEtherType) +
                                "! - Attempt to recover and continue processing");
                        }

                        break;
                    }
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thePacketNumber"></param>
        /// <param name="theTimestamp">The timestamp read from the packet capture</param>
        /// <returns></returns>
        private bool ProcessPayload(ulong thePacketNumber, double theTimestamp)
        {
            bool theResult = true;
            bool thePacketProcessingResult = true;

            // Record the position in the stream for the packet capture so we can later determine how far has been progressed
            long theStartingStreamPosition = this.theBinaryReader.BaseStream.Position;

            // Check the value of the Ether Type for this Ethernet frame
            switch (this.theEtherType)
            {
                case (ushort)Constants.HeaderEtherType.ARP:
                    {
                        // We have got an Ethernet frame containing an ARP packet so process it
                        thePacketProcessingResult = this.theARPPacketProcessing.Process(this.thePayloadLength);

                        break;
                    }

                case (ushort)Constants.HeaderEtherType.IPv4:
                    {
                        // We have got an Ethernet frame containing an IP v4 packet so process it
                        thePacketProcessingResult = this.theIPv4PacketProcessing.Process(this.thePayloadLength, thePacketNumber, theTimestamp);

                        break;
                    }

                case (ushort)Constants.HeaderEtherType.IPv6:
                    {
                        // We have got an Ethernet frame containing an IP v6 packet so process it
                        thePacketProcessingResult = this.theIPv6PacketProcessing.Process(this.thePayloadLength, thePacketNumber, theTimestamp);

                        break;
                    }

                case (ushort)Constants.HeaderEtherType.LLDP:
                    {
                        // We have got an Ethernet frame containing an LLDP packet so process it
                        thePacketProcessingResult = this.theLLDPPacketProcessing.Process(this.thePayloadLength);

                        break;
                    }

                case (ushort)Constants.HeaderEtherType.Loopback:
                    {
                        // We have got an Ethernet frame containing an Configuration Test Protocol (Loopback) packet so process it
                        thePacketProcessingResult = this.theLoopbackPacketProcessing.Process(this.thePayloadLength);

                        break;
                    }

                case (ushort)Constants.HeaderEtherType.VLANTagged:
                    {
                        //// We have got an Ethernet frame containing a second VLAN tag!

                        this.theDebugInformation.WriteInformationEvent("The Ethernet frame in captured packet #" +
                            thePacketNumber.ToString() +
                            " contains a second VLAN tag!" +
                            " - Attempt to recover and continue processing");

                        thePacketProcessingResult = this.ProcessEtherType(thePacketNumber);

                        if (thePacketProcessingResult)
                        {
                            // Now that the second VLAN tag has been dealt with rerecord the position in the stream for the packet capture so we can later determine how far has been progressed
                            theStartingStreamPosition = this.theBinaryReader.BaseStream.Position;

                            // Call this method again recursively to process the Ethernet frame stripped of the second VLAN tag
                            thePacketProcessingResult = this.ProcessPayload(thePacketNumber, theTimestamp);
                        }

                        break;
                    }

                default:
                    {
                        // We have got an Ethernet frame containing an unrecognised Ether Type

                        // This is not strictly an error as it may be that the Ether Type is just not supported yet
                        // Debug information line will have been previously output to indicate this instance so just continue
                        thePacketProcessingResult = true;

                        // Check against the minimum value for Ether Type - lower values indicate length of the Ethernet frame
                        if (this.theEtherType < (ushort)Constants.HeaderEtherType.MinimumValue)
                        {
                            // This Ethernet frame has a value for "Ether Type" lower than the minimum
                            // This is an IEEE 802.3 Ethernet frame rather than an Ethernet II frame
                            // This value is the length of the IEEE 802.3 Ethernet frame

                            // Not going to process IEEE 802.3 Ethernet frames currently as they do not include any data of interest
                            // Just read off the bytes for the IEEE 802.3 Ethernet frame from the packet capture so we can move on
                            this.theBinaryReader.ReadBytes(this.theEtherType);
                        }
                        else
                        {
                            // Processing of Ethernet frames with Ether Types not enumerated above are obviously not currently recognised or supported!
                            // Just fall through to the processing below that will read off the payload so we can move on
                        }

                        break;
                    }
            }

            if (!thePacketProcessingResult)
            {
                this.theDebugInformation.WriteInformationEvent("Processing of the Ethernet frame in captured packet #" +
                    thePacketNumber.ToString() +
                    " encountered an error during processing of the payload!" +
                    " - Attempt to recover and continue processing");
            }

            // Calculate how far has been progressed through the stream by the actions above to read from the packet capture
            long theStreamPositionDifference = this.theBinaryReader.BaseStream.Position - theStartingStreamPosition;

            //// Check whether the Ethernet frame payload has extra trailer bytes
            //// These would typically not be exposed in the packet capture by the recorder, but sometimes are for whatever reason!

            //// This processing would also read off the payload of the Ethernet frame in the event of an unrecognised, unknown or unsupported Ether Type

            if (this.thePayloadLength != theStreamPositionDifference)
            {
                if (this.thePayloadLength > theStreamPositionDifference)
                {
                    // Trim the extra trailer bytes (or bytes for an unrecognised, unknown or unsupported message)
                    this.theBinaryReader.ReadBytes((int)(this.thePayloadLength - theStreamPositionDifference));
                }
                else
                {
                    //// This is a strange error condition

                    // Back up the stream position to where it "should be"
                    this.theBinaryReader.BaseStream.Position = this.theBinaryReader.BaseStream.Position - (theStreamPositionDifference - this.thePayloadLength);

                    // Warn about the error condition having arisen
                    this.theDebugInformation.WriteInformationEvent("The length " +
                        this.thePayloadLength.ToString() +
                        " of payload of Ethernet frame in captured packet #" +
                        thePacketNumber.ToString() +
                        " does not match the progression " +
                        theStreamPositionDifference.ToString() +
                        " through the stream!" +
                        " - Attempt to recover and continue processing");
                }
            }

            return theResult;
        }
    }
}
