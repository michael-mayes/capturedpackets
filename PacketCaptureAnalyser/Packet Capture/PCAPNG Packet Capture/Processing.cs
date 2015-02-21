// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.PacketCapture.PCAPNGPackageCapture
{
    /// <summary>
    /// This class provides the PCAP Next Generation packet capture processing
    /// </summary>
    public class Processing : CommonProcessing
    {
        /// <summary>
        /// Boolean flag that indicates whether the PCAP Next Generation section header block indicates little endian data in the PCAP Next Generation packet payload
        /// </summary>
        private bool isTheSectionHeaderBlockLittleEndian = true;

        //// Concrete methods - override abstract methods on the base class

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theProgressWindowForm">The instance of the progress window form to use for reporting progress of the processing</param>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="performLatencyAnalysisProcessing">Boolean flag that indicates whether to perform latency analysis processing for data read from the packet capture</param>
        /// <param name="theLatencyAnalysisProcessing">The object that provides the latency analysis processing for data read from the packet capture</param>
        /// <param name="performBurstAnalysisProcessing">Boolean flag that indicates whether to perform burst analysis processing for data read from the packet capture</param>
        /// <param name="theBurstAnalysisProcessing">The object that provides the burst analysis processing for data read from the packet capture</param>
        /// <param name="performTimeAnalysisProcessing">Boolean flag that indicates whether to perform time analysis processing for data read from the packet capture</param>
        /// <param name="theTimeAnalysisProcessing">The object that provides the time analysis processing for data read from the packet capture</param>
        /// <param name="theSelectedPacketCapturePath">The path of the selected packet capture</param>
        /// <param name="useAlternativeSequenceNumber">Boolean flag that indicates whether to use the alternative sequence number in the data read from the packet capture, required for legacy recordings</param>
        /// <param name="minimizeMemoryUsage">Boolean flag that indicates whether to perform reading from the packet capture using a method that will minimize memory usage, possibly at the expense of increased processing time</param>
        public Processing(ProgressWindowForm theProgressWindowForm, Analysis.DebugInformation theDebugInformation, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performBurstAnalysisProcessing, Analysis.BurstAnalysis.Processing theBurstAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing, string theSelectedPacketCapturePath, bool useAlternativeSequenceNumber, bool minimizeMemoryUsage) :
            base(
            theProgressWindowForm,
            theDebugInformation,
            performLatencyAnalysisProcessing,
            theLatencyAnalysisProcessing,
            performBurstAnalysisProcessing,
            theBurstAnalysisProcessing,
            performTimeAnalysisProcessing,
            theTimeAnalysisProcessing,
            theSelectedPacketCapturePath,
            useAlternativeSequenceNumber,
            minimizeMemoryUsage)
        {
        }

        /// <summary>
        /// Processes the PCAP Next Generation packet capture global header
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation packet capture global header could be processed</returns>
        protected override bool ProcessPacketCaptureGlobalHeader(System.IO.BinaryReader theBinaryReader, out uint thePacketCaptureNetworkDataLinkType, out double thePacketCaptureTimestampAccuracy)
        {
            bool theResult = true;

            if (theBinaryReader == null)
            {
                throw new System.ArgumentNullException("theBinaryReader");
            }

            // Provide a default value for the output parameter for the network datalink type
            thePacketCaptureNetworkDataLinkType =
                (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Invalid;

            // Set up the output parameter for the timestamp accuracy - not used for PCAP Next Generation packet captures so default to zero
            thePacketCaptureTimestampAccuracy = 0.0;

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            long thePacketPayloadLength = 0;

            // Peek a view at the block type for the PCAP Next Generation block
            uint theBlockType = theBinaryReader.ReadUInt32();

            // Wind back the binary stream by four to hide that we have just peeked a view at the block type
            theBinaryReader.BaseStream.Position -= 4;

            // It is expected that the PCAP Next Generation packet capture starts with a PCAP Next Generation section header block
            if (theBlockType != (uint)Constants.BlockType.SectionHeaderBlock)
            {
                // We have got an PCAP Next Generation packet capture that does not start with a section header block
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP Next Generation packet capture does not start with a section header block!!!");

                theResult = false;
            }

            if (theResult)
            {
                // Process the PCAP Next Generation section header block
                theResult = this.ProcessSectionHeaderBlock(
                    theBinaryReader,
                    out thePacketPayloadLength);

                if (theResult)
                {
                    // Set up the output parameter for the network data link type to the default value as it is not supplied by the section header block
                    thePacketCaptureNetworkDataLinkType =
                        (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet;
                }
            }

            return theResult;
        }

        /// <summary>
        /// Process the PCAP Next Generation block header
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation block header could be processed</returns>
        protected override bool ProcessPacketHeader(System.IO.BinaryReader theBinaryReader, uint thePacketCaptureNetworkDataLinkType, double thePacketCaptureTimestampAccuracy, ref ulong thePacketNumber, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            if (theBinaryReader == null)
            {
                throw new System.ArgumentNullException("theBinaryReader");
            }

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Peek a view at the block type for the PCAP Next Generation block
            uint theBlockType = theBinaryReader.ReadUInt32();

            // Wind back the binary stream by four to hide that we have just peeked a view at the block type
            theBinaryReader.BaseStream.Position -= 4;

            // Check the block type for this PCAP Next Generation block
            switch (theBlockType)
            {
                case (uint)Constants.BlockType.SectionHeaderBlock:
                    {
                        //// We have got a PCAP Next Generation section header block

                        //// Do not increment the number for the packet read from the packet capture for a PCAP Next Generation section header block as it will not show in Wireshark and so would confuse comparisons with other formats

                        // We have got a second PCAP Next Generation section header block which is currently not supported
                        this.TheDebugInformation.WriteErrorEvent(
                            "The PCAP Next Generation packet capture contains a second section header block, which is not currently supported!!!");

                        theResult = false;

                        break;
                    }

                case (uint)Constants.BlockType.InterfaceDescriptionBlock:
                    {
                        //// We have got a PCAP Next Generation interface description block

                        //// Do not increment the number for the packet read from the packet capture for a PCAP Next Generation interface description block as it will not show in Wireshark and so would confuse comparisons with other formats

                        // Process the PCAP Next Generation interface description block
                        theResult = ProcessInterfaceDescriptionBlock(
                            theBinaryReader,
                            out thePacketPayloadLength);

                        break;
                    }

                case (uint)Constants.BlockType.PacketBlock:
                    {
                        //// We have got a PCAP Next Generation packet block

                        // Increment the number for the packet read from the packet capture for a PCAP Next Generation packet block
                        ++thePacketNumber;

                        // Process the PCAP Next Generation packet block
                        theResult = ProcessPacketBlock(
                            theBinaryReader,
                            out thePacketPayloadLength,
                            out thePacketTimestamp);

                        break;
                    }

                case (uint)Constants.BlockType.SimplePacketBlock:
                    {
                        //// We have got a PCAP Next Generation simple packet block

                        // Increment the number for the packet read from the packet capture for a PCAP Next Generation simple packet block
                        ++thePacketNumber;

                        // Process the PCAP Next Generation simple packet block
                        theResult = ProcessSimplePacketBlock(
                            theBinaryReader,
                            out thePacketPayloadLength);

                        break;
                    }

                case (uint)Constants.BlockType.EnhancedPacketBlock:
                    {
                        //// We have got a PCAP Next Generation enhanced packet block

                        // Increment the number for the packet read from the packet capture for a PCAP Next Generation enhanced packet block
                        ++thePacketNumber;

                        // Process the PCAP Next Generation enhanced packet block
                        theResult = ProcessEnhancedPacketBlock(
                            theBinaryReader,
                            out thePacketPayloadLength,
                            out thePacketTimestamp);

                        break;
                    }

                case (uint)Constants.BlockType.NameResolutionBlock:
                    {
                        //// We have got a PCAP Next Generation name resolution block

                        //// Do not increment the number for the packet read from the packet capture for a PCAP Next Generation name resolution block as it will not show in Wireshark and so would confuse comparisons with other formats

                        // Process the PCAP Next Generation name resolution block
                        theResult = ProcessNameResolutionBlock(
                            theBinaryReader,
                            out thePacketPayloadLength);

                        break;
                    }

                case (uint)Constants.BlockType.InterfaceStatisticsBlock:
                    {
                        //// We have got a PCAP Next Generation interface statistics block

                        //// Do not increment the number for the packet read from the packet capture for a PCAP Next Generation interface statistics block as it will not show in Wireshark and so would confuse comparisons with other formats

                        // Process the PCAP Next Generation interface statistics block
                        theResult = ProcessInterfaceStatisticsBlock(
                            theBinaryReader,
                            out thePacketPayloadLength,
                            out thePacketTimestamp);

                        break;
                    }

                default:
                    {
                        //// We have got a PCAP Next Generation packet capture block containing an unknown Block Type

                        // Increment the number for the packet read from the packet capture for an unknown PCAP Next Generation block
                        ++thePacketNumber;

                        //// Processing of PCAP Next Generation packet capture blocks with Block Types not enumerated above is obviously not currently supported!

                        this.TheDebugInformation.WriteErrorEvent(
                            "The PCAP Next Generation packet capture block contains an unexpected Block Type of 0x" +
                            string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:X}", theBlockType) +
                            "!!!");

                        theResult = false;

                        break;
                    }
            }

            return theResult;
        }

        //// Private methods - provide methods specific to PCAP Next Generation packet captures, not required to derive from the abstract base class

        /// <summary>
        /// Adjust the supplied block total length up to the next even four byte boundary
        /// </summary>
        /// <param name="blockTotalLength">The current block total length</param>
        /// <returns>The adjusted block total length</returns>
        private static uint AdjustBlockTotalLength(uint blockTotalLength)
        {
            uint theAdjustedBlockTotalLength = blockTotalLength;

            //// Adjust the supplied block total length up to the next even four byte boundary

            //// A non-obvious characteristic of the current block total length field is that the block total length may not actually
            //// indicate the length of the actual data block (the payload). If the number of payload octets is not evenly divisible
            //// by four then the one or more padding bytes will have been added after the payload and before the block trailer to
            //// pad the length of the payload to a four byte boundry. To seek forwards or backwards to the next or previous block
            //// one will need to add from 0 to 3 to the reported block total length to determine where the next block will start

            switch (theAdjustedBlockTotalLength % 4)
            {
                case 0:
                    {
                        break;
                    }

                case 1:
                    {
                        theAdjustedBlockTotalLength += 3;
                        break;
                    }

                case 2:
                    {
                        theAdjustedBlockTotalLength += 2;
                        break;
                    }

                case 3:
                    {
                        theAdjustedBlockTotalLength += 1;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            return theAdjustedBlockTotalLength;
        }

        /// <summary>
        /// Process a PCAP Next Generation interface description block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation interface description block could be processed</returns>
        private static bool ProcessInterfaceDescriptionBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation interface description block

            // The PCAP Next Generation interface description block does not contain an Ethernet frame
            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Set this output parameter to a value of zero to prevent any attempt to process an Ethernet frame from the PCAP Next Generation packet capture packet payload
            thePacketPayloadLength = 0;

            // Read the block type for the PCAP Next Generation interface description block so we can move on
            theBinaryReader.ReadUInt32();

            // Read off and store the block total length for the PCAP Next Generation interface description block, adjusting it up to the next even four byte boundary, for use below
            uint theBlockTotalLength =
                AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Just read off the reserved field for the PCAP Next Generation interface description block so we can move on
            theBinaryReader.ReadUInt16();

            // Just read off the network data link type for the PCAP Next Generation interface description block so we can move on
            theBinaryReader.ReadUInt16();

            // Just read off the snap length for the PCAP Next Generation interface description block so we can move on
            theBinaryReader.ReadUInt32();

            // The PCAP Next Generation interface description block does not contain an Ethernet frame
            // Just read the bytes off the remaining bytes from the PCAP Next Generation interface description block so we can continue
            theBinaryReader.ReadBytes(
                (int)(theBlockTotalLength -
                (uint)Constants.BlockTotalLength.InterfaceDescriptionBlock));

            return theResult;
        }

        /// <summary>
        /// Processes a PCAP Next Generation packet block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation packet block could be processed</returns>
        private static bool ProcessPacketBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation packet block

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Just read off the block type for the PCAP Next Generation packet block so we can move on
            theBinaryReader.ReadUInt32();

            // Read off and store the block total length for the PCAP Next Generation packet block, adjusting it up to the next even four byte boundary, for use below
            uint theBlockTotalLength =
                AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Just read off the interface Id for the PCAP Next Generation packet block so we can move on
            theBinaryReader.ReadUInt16();

            // Just read off the drops count for the PCAP Next Generation packet block so we can move on
            theBinaryReader.ReadUInt16();

            // Read off and store the high bytes of the timestamp for the PCAP Next Generation packet block for use below
            uint theTimestampHigh = theBinaryReader.ReadUInt32();

            // Read off and store the low bytes of the timestamp for the PCAP Next Generation packet block for use below
            uint theTimestampLow = theBinaryReader.ReadUInt32();

            // Just read off the capture length for packet within the PCAP Next Generation packet block so we can move on
            theBinaryReader.ReadUInt32();

            // Just read off the actual length for the packet within the PCAP Next Generation packet block so we can move on
            theBinaryReader.ReadUInt32();

            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            thePacketPayloadLength =
                theBlockTotalLength -
                (uint)Constants.BlockTotalLength.PacketBlock -
                12;

            // Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            thePacketTimestamp =
                ((theTimestampHigh * 4294967296) +
                theTimestampLow) / 1000000.0;

            return theResult;
        }

        /// <summary>
        /// Processes a PCAP Next Generation simple packet block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation simple packet block could be processed</returns>
        private static bool ProcessSimplePacketBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation simple packet block

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Just read off the block type for the PCAP Next Generation simple packet block so we can move on
            theBinaryReader.ReadUInt32();

            // Read off and store the block total length for the PCAP Next Generation simple packet block, adjusting it up to the next even four byte boundary, for use below
            uint theBlockTotalLength =
                AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Just read off the actual length for the packet within the PCAP Next Generation simple packet block so we can move on
            theBinaryReader.ReadUInt32();

            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            thePacketPayloadLength =
                theBlockTotalLength -
                (uint)Constants.BlockTotalLength.SimplePacketBlock -
                12;

            return theResult;
        }

        /// <summary>
        /// Processes a PCAP Next Generation enhanced packet block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation enhanced packet block could be processed</returns>
        private static bool ProcessEnhancedPacketBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation enhanced packet block

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Just read off the block type for the PCAP Next Generation enhanced packet block so we can move on
            theBinaryReader.ReadUInt32();

            // Read the block total length for the PCAP Next Generation enhanced packet block, adjusting it up to the next even four byte boundary
            uint theBlockTotalLength =
                AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Just read off the interface Id for the PCAP Next Generation enhanced packet block so we can move on
            theBinaryReader.ReadUInt32();

            // Read off and store the high bytes of the timestamp for the PCAP Next Generation enhanced packet block for use below
            uint theTimestampHigh = theBinaryReader.ReadUInt32();

            // Read off and store the low bytes of the timestamp for the PCAP Next Generation enhanced packet block for use below
            uint theTimestampLow = theBinaryReader.ReadUInt32();

            // Just read off the capture length for packet within the PCAP Next Generation enhanced packet block so we can move on
            theBinaryReader.ReadUInt32();

            // Read the actual length for the packet within the PCAP Next Generation enhanced packet block so we can move on
            theBinaryReader.ReadUInt32();

            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            thePacketPayloadLength =
                theBlockTotalLength -
                (uint)Constants.BlockTotalLength.EnhancedPacketBlock -
                12;

            // Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            thePacketTimestamp =
                ((theTimestampHigh * 4294967296) +
                theTimestampLow) / 1000000.0;

            return theResult;
        }

        /// <summary>
        /// Process a PCAP Next Generation name resolution block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation name resolution block could be processed</returns>
        private static bool ProcessNameResolutionBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation name resolution block

            // The PCAP Next Generation name resolution block does not contain an Ethernet frame
            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Set this output parameter to a value of zero to prevent any attempt to process an Ethernet frame from the PCAP Next Generation packet capture packet payload
            thePacketPayloadLength = 0;

            // Just read off the block type for the PCAP Next Generation name resolution block so we can move on
            theBinaryReader.ReadUInt32();

            // Read off and store the block total length for the PCAP Next Generation name resolution block, adjusting it up to the next even four byte boundary, for use below
            uint theBlockTotalLength =
                AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Just read off the record type for the PCAP Next Generation name resolution block so we can move on
            theBinaryReader.ReadUInt16();

            // Just read off the record length for the PCAP Next Generation name resolution block so we can move on
            theBinaryReader.ReadUInt16();

            // The PCAP Next Generation name resolution block does not contain an Ethernet frame
            // Just read the bytes off the remaining bytes from the PCAP Next Generation name resolution block so we can continue
            theBinaryReader.ReadBytes(
                (int)(theBlockTotalLength -
                (uint)Constants.BlockTotalLength.NameResolutionBlock));

            return theResult;
        }

        /// <summary>
        /// Process a PCAP Next Generation interface statistics block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation interface statistics block could be processed</returns>
        private static bool ProcessInterfaceStatisticsBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation interface statistics block

            // The PCAP Next Generation interface statistics block does not contain an Ethernet frame
            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Set this output parameter to a value of zero to prevent any attempt to process an Ethernet frame from the PCAP Next Generation packet capture packet payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Just read off the block type for the PCAP Next Generation interface statistics block so we can move on
            theBinaryReader.ReadUInt32();

            // Read off and store the block total length for the PCAP Next Generation interface statistics block, adjusting it up to the next even four byte boundary, for use below
            uint theBlockTotalLength =
                AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Just read off the interface Id for the PCAP Next Generation interface statistics block so move on
            theBinaryReader.ReadUInt32();

            // Read off and store the high bytes of the timestamp for the PCAP Next Generation interface statistics block for use below
            uint theTimestampHigh = theBinaryReader.ReadUInt32();

            // Read off and store the low bytes of the timestamp for the PCAP Next Generation interface statistics block for use below
            uint theTimestampLow = theBinaryReader.ReadUInt32();

            // The PCAP Next Generation interface statistics block does not contain an Ethernet frame
            // Just read the bytes off the remaining bytes from the PCAP Next Generation interface statistics block so we can continue
            theBinaryReader.ReadBytes(
                (int)(theBlockTotalLength -
                (uint)Constants.BlockTotalLength.InterfaceStatisticsBlock));

            // Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            thePacketTimestamp =
                ((theTimestampHigh * 4294967296) +
                theTimestampLow) / 1000000.0;

            return theResult;
        }

        /// <summary>
        /// Process a PCAP Next Generation section header block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation section header block could be processed</returns>
        private bool ProcessSectionHeaderBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation section header block

            // The PCAP Next Generation section header block does not contain an Ethernet frame
            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Set this output parameter to a value of zero to prevent any attempt to process an Ethernet frame from the PCAP Next Generation packet capture packet payload
            thePacketPayloadLength = 0;

            // Read off and store the block type for the PCAP Next Generation section header block for use below
            uint theBlockType =
                theBinaryReader.ReadUInt32();

            // Read off and store the block total length for the PCAP Next Generation section header block, adjusting it up to the next even four byte boundary, for use below
            uint theBlockTotalLength =
                AdjustBlockTotalLength(theBinaryReader.ReadUInt32());

            // Read off and store the magic number of the PCAP Next Generation section header block for use below
            uint theByteOrderMagic =
                theBinaryReader.ReadUInt32();

            // The endianism of the remainder of the values in the PCAP Next Generation section header block will be corrected to little endian if the magic number indicates big endian representation
            if (theByteOrderMagic == Constants.LittleEndianByteOrderMagic)
            {
                this.TheDebugInformation.WriteInformationEvent(
                    "The PCAP Next Generation packet capture contains the little endian byte-order magic");

                this.isTheSectionHeaderBlockLittleEndian = true;
            }
            else if (theByteOrderMagic == Constants.BigEndianByteOrderMagic)
            {
                this.TheDebugInformation.WriteInformationEvent(
                    "The PCAP Next Generation packet capture contains the big endian byte-order magic");

                this.isTheSectionHeaderBlockLittleEndian = false;
            }

            ushort theMajorVersion = 0;
            ushort theMinorVersion = 0;

            // Just read off the remainder of the PCAP Next Generation section header block from the packet capture so we can move on
            // Some of the data will be stored for use below
            if (this.isTheSectionHeaderBlockLittleEndian)
            {
                theMajorVersion = theBinaryReader.ReadUInt16();
                theMinorVersion = theBinaryReader.ReadUInt16();
                theBinaryReader.ReadUInt64(); // Section length
            }
            else
            {
                theMajorVersion = (ushort)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt16());
                theMinorVersion = (ushort)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt16());
                theBinaryReader.ReadInt64(); // Section length
            }

            // The PCAP Next Generation section header block does not contain an Ethernet frame
            // Just read the bytes off the remaining bytes from the PCAP Next Generation section header block so we can continue
            theBinaryReader.ReadBytes(
                (int)(theBlockTotalLength -
                (uint)Constants.BlockTotalLength.SectionHeaderBlock));

            // Validate fields from the PCAP Next Generation section header block
            theResult = this.ValidateSectionHeaderBlock(
                theBlockType,
                theByteOrderMagic,
                theMajorVersion,
                theMinorVersion);

            return theResult;
        }

        /// <summary>
        /// Validates the PCAP Next Generation section header block
        /// </summary>
        /// <param name="theBlockType">The block type in the PCAP Next Generation section header block</param>
        /// <param name="theByteOrderMagic">The magic number in the PCAP Next Generation section header block</param>
        /// <param name="theMajorVersion">The major version number in the PCAP Next Generation section header block</param>
        /// <param name="theMinorVersion">The minor version number in the PCAP Next Generation section header block</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation section header block is valid</returns>
        private bool ValidateSectionHeaderBlock(uint theBlockType, uint theByteOrderMagic, ushort theMajorVersion, ushort theMinorVersion)
        {
            bool theResult = true;

            //// Validate fields from the PCAP Next Generation section header block

            if (theBlockType != (uint)Constants.BlockType.SectionHeaderBlock)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP Next Generation section header block does not contain the expected block type, is " +
                    theBlockType.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.BlockType.SectionHeaderBlock.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theByteOrderMagic != Constants.LittleEndianByteOrderMagic &&
                theByteOrderMagic != Constants.BigEndianByteOrderMagic)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP Next Generation section header block does not contain the expected magic number, is " +
                    theByteOrderMagic.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.LittleEndianByteOrderMagic.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " or " +
                    Constants.BigEndianByteOrderMagic.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            if (theMajorVersion != Constants.ExpectedMajorVersion)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP Next Generation section header block does not contain the expected major version number, is " +
                    theMajorVersion.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.ExpectedMajorVersion.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            if (theMinorVersion != Constants.ExpectedMinorVersion)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP Next Generation section header block does not contain the expected minor version number, is " +
                    theMinorVersion.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.ExpectedMinorVersion.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
