// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.PCAPNGPackageCapture
{
    /// <summary>
    /// This class provides the PCAP Next Generation packet capture processing
    /// </summary>
    public class Processing : CommonProcessing
    {
        /// <summary>
        /// Boolean flag that indicates whether the PCAP Next Generation packet capture section header block indicates little endian data in the PCAP Next Generation packet payload
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
        public Processing(PacketCaptureAnalyser.ProgressWindowForm theProgressWindowForm, Analysis.DebugInformation theDebugInformation, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performBurstAnalysisProcessing, Analysis.BurstAnalysis.Processing theBurstAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing, string theSelectedPacketCapturePath, bool useAlternativeSequenceNumber, bool minimizeMemoryUsage) :
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

            // Provide a default value for the output parameter for the network datalink type
            thePacketCaptureNetworkDataLinkType =
                (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Invalid;

            // Set up the output parameter for the timestamp accuracy - not used for PCAP Next Generation packet captures so default to zero
            thePacketCaptureTimestampAccuracy = 0.0;

            // Create the single instance of the PCAP Next Generation packet capture section header block
            Structures.SectionHeaderBlockStructure theSectionHeaderBlock =
                new Structures.SectionHeaderBlockStructure();

            // Read the block type for the PCAP Next Generation packet capture section header block
            theSectionHeaderBlock.BlockType =
                theBinaryReader.ReadUInt32();

            // Read the block total length for the PCAP Next Generation packet capture section header block, adjusting it up to the next even four byte boundary
            theSectionHeaderBlock.BlockTotalLength =
                this.AdjustBlockTotalLength(theBinaryReader.ReadUInt32());

            // Read the magic number of the PCAP Next Generation packet capture section header block from the packet capture
            theSectionHeaderBlock.ByteOrderMagic =
                theBinaryReader.ReadUInt32();

            // The endianism of the remainder of the values in the PCAP Next Generation packet capture section header block will be corrected to little endian if the magic number indicates big endian representation
            if (theSectionHeaderBlock.ByteOrderMagic == Constants.LittleEndianByteOrderMagic)
            {
                this.TheDebugInformation.WriteInformationEvent(
                    "The PCAP Next Generation packet capture contains the little endian byte-order magic");

                this.isTheSectionHeaderBlockLittleEndian = true;
            }
            else if (theSectionHeaderBlock.ByteOrderMagic == Constants.BigEndianByteOrderMagic)
            {
                this.TheDebugInformation.WriteInformationEvent(
                    "The PCAP Next Generation packet capture contains the big endian byte-order magic");

                this.isTheSectionHeaderBlockLittleEndian = false;
            }

            // Read the remainder of the PCAP Next Generation packet capture section header block from the packet capture
            if (this.isTheSectionHeaderBlockLittleEndian)
            {
                theSectionHeaderBlock.MajorVersion = theBinaryReader.ReadUInt16();
                theSectionHeaderBlock.MinorVersion = theBinaryReader.ReadUInt16();
                theSectionHeaderBlock.SectionLength = theBinaryReader.ReadUInt64();
            }
            else
            {
                theSectionHeaderBlock.MajorVersion = (ushort)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt16());
                theSectionHeaderBlock.MinorVersion = (ushort)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt16());
                theSectionHeaderBlock.SectionLength = (ulong)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt64());
            }

            // Just read the bytes off the remaining bytes from the section header block so we can continue
            theBinaryReader.ReadBytes(
                (int)(theSectionHeaderBlock.BlockTotalLength -
                (uint)Constants.BlockTotalLength.SectionHeaderBlock));

            // Validate fields from the PCAP Next Generation packet capture section header block
            theResult = this.ValidateSectionHeaderBlock(
                theSectionHeaderBlock);

            if (theResult)
            {
                // Set up the output parameter for the network data link type to the default value as it is not supplied by the section header block
                thePacketCaptureNetworkDataLinkType =
                    (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet;
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

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Peek a view at the block type for the PCAP Next Generation block
            uint theBlockType = theBinaryReader.ReadUInt32();

            // Wind back the binary stream by four to hide that we have just peeked a view at the block type
            theBinaryReader.BaseStream.Position -= 4;

            switch (theBlockType)
            {
                case (uint)Constants.BlockType.InterfaceDescriptionBlock:
                    {
                        //// We have got a PCAP Next Generation interface description block

                        //// Do not increment the number for the packet read from the packet capture for a PCAP Next Generation interface description block as it will not show in Wireshark and so would confuse comparisons with other formats

                        theResult = this.ProcessInterfaceDescriptionBlock(
                            theBinaryReader,
                            out thePacketPayloadLength);

                        break;
                    }

                case (uint)Constants.BlockType.PacketBlock:
                    {
                        //// We have got a PCAP Next Generation packet block

                        // Increment the number for the packet read from the packet capture for a PCAP Next Generation packet block
                        ++thePacketNumber;

                        theResult = this.ProcessPacketBlock(
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

                        theResult = this.ProcessSimplePacketBlock(
                            theBinaryReader,
                            out thePacketPayloadLength);

                        break;
                    }

                case (uint)Constants.BlockType.EnhancedPacketBlock:
                    {
                        //// We have got a PCAP Next Generation enhanced packet block

                        // Increment the number for the packet read from the packet capture for a PCAP Next Generation enhanced packet block
                        ++thePacketNumber;

                        theResult = this.ProcessEnhancedPacketBlock(
                            theBinaryReader,
                            out thePacketPayloadLength,
                            out thePacketTimestamp);

                        break;
                    }

                case (uint)Constants.BlockType.InterfaceStatisticsBlock:
                    {
                        //// We have got a PCAP Next Generation interface statistics block

                        //// Do not increment the number for the packet read from the packet capture for a PCAP Next Generation interface statistics block as it will not show in Wireshark and so would confuse comparisons with other formats

                        theResult = this.ProcessInterfaceStatisticsBlock(
                            theBinaryReader,
                            out thePacketPayloadLength,
                            out thePacketTimestamp);

                        break;
                    }

                default:
                    {
                        // Increment the number for the packet read from the packet capture for an unknow PCAP Next Generation block
                        ++thePacketNumber;

                        // We have got an PCAP Next Generation packet capture packet containing an unknown Block Type
                        this.TheDebugInformation.WriteErrorEvent(
                            "The PCAP Next Generation packet capture block contains an unexpected Block Type of 0x" +
                            string.Format("{0:X}", theBlockType) +
                            "!!!");

                        theResult = false;

                        break;
                    }
            }

            return theResult;
        }

        //// Private methods - provide methods specific to PCAP Next Generation packet captures, not required to derive from the abstract base class

        /// <summary>
        /// Validates the PCAP Next Generation packet capture section header block
        /// </summary>
        /// <param name="theSectionHeaderBlock">The PCAP Next Generation packet capture section header block</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation packet capture section header block is valid</returns>
        private bool ValidateSectionHeaderBlock(Structures.SectionHeaderBlockStructure theSectionHeaderBlock)
        {
            bool theResult = true;

            //// Validate fields from the PCAP Next Generation packet capture section header block

            if (theSectionHeaderBlock.BlockType != (uint)Constants.BlockType.SectionHeaderBlock)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP Next Generation packet capture section header block does not contain the expected block type, is " +
                    theSectionHeaderBlock.BlockType.ToString() +
                    " not " +
                    Constants.BlockType.SectionHeaderBlock.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theSectionHeaderBlock.ByteOrderMagic != Constants.LittleEndianByteOrderMagic &&
                theSectionHeaderBlock.ByteOrderMagic != Constants.BigEndianByteOrderMagic)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP Next Generation packet capture section header block does not contain the expected magic number, is " +
                    theSectionHeaderBlock.ByteOrderMagic.ToString() +
                    " not " +
                    Constants.LittleEndianByteOrderMagic.ToString() +
                    " or " +
                    Constants.BigEndianByteOrderMagic.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theSectionHeaderBlock.MajorVersion != Constants.ExpectedMajorVersion)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP Next Generation packet capture section header block does not contain the expected major version number, is " +
                    theSectionHeaderBlock.MajorVersion.ToString() +
                    " not " +
                    Constants.ExpectedMajorVersion.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theSectionHeaderBlock.MinorVersion != Constants.ExpectedMinorVersion)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP Next Generation packet capture section header block does not contain the expected minor version number, is " +
                    theSectionHeaderBlock.MinorVersion.ToString() +
                    " not " +
                    Constants.ExpectedMinorVersion.ToString() +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }

        /// <summary>
        /// Process a PCAP Next Generation interface description block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation interface description block could be processed</returns>
        private bool ProcessInterfaceDescriptionBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation interface description block

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Create the single instance of the PCAP Next Generation interface description block
            Structures.InterfaceDescriptionBlockStructure theInterfaceDescriptionBlock =
                new Structures.InterfaceDescriptionBlockStructure();

            // Read the block type for the PCAP Next Generation interface description block
            theInterfaceDescriptionBlock.BlockType = theBinaryReader.ReadUInt32();

            // Read the block total length for the PCAP Next Generation interface description block, adjusting it up to the next even four byte boundary
            theInterfaceDescriptionBlock.BlockTotalLength =
                this.AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Read the reserved field for the PCAP Next Generation interface description block
            theInterfaceDescriptionBlock.Reserved = theBinaryReader.ReadUInt16();

            // Read the network data link type for the PCAP Next Generation interface description block
            theInterfaceDescriptionBlock.LinkType = theBinaryReader.ReadUInt16();

            // Read the snap length for the PCAP Next Generation interface description block
            theInterfaceDescriptionBlock.SnapLen = theBinaryReader.ReadUInt32();

            // The PCAP Next Generation interface description block does not contain an Ethernet frame
            // Just read the bytes off the remaining bytes from the PCAP Next Generation interface description block so we can continue
            theBinaryReader.ReadBytes(
                (int)(theInterfaceDescriptionBlock.BlockTotalLength -
                (uint)Constants.BlockTotalLength.InterfaceDescriptionBlock));

            // The PCAP Next Generation interface description block does not contain an Ethernet frame
            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Set this output parameter to a value of zero to prevent any attempt to process an Ethernet frame from the PCAP Next Generation packet capture packet payload
            thePacketPayloadLength = 0;

            return theResult;
        }

        /// <summary>
        /// Processes a PCAP Next Generation packet block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation packet block could be processed</returns>
        private bool ProcessPacketBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation packet block

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Create an instance of the PCAP Next Generation packet block
            Structures.PacketBlockStructure thePacketBlock =
                new Structures.PacketBlockStructure();

            // Read the block type for the PCAP Next Generation packet block
            thePacketBlock.BlockType = theBinaryReader.ReadUInt32();

            // Read the block total length for the PCAP Next Generation packet block, adjusting it up to the next even four byte boundary
            thePacketBlock.BlockTotalLength =
                this.AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Read the interface Id for the PCAP Next Generation packet block
            thePacketBlock.InterfaceId = theBinaryReader.ReadUInt16();

            // Read the drops count for the PCAP Next Generation packet block
            thePacketBlock.DropsCount = theBinaryReader.ReadUInt16();

            // Read the high bytes of the timestamp for the PCAP Next Generation packet block
            thePacketBlock.TimestampHigh = theBinaryReader.ReadUInt32();

            // Read the low bytes of the timestamp for the PCAP Next Generation packet block
            thePacketBlock.TimestampLow = theBinaryReader.ReadUInt32();

            // Read the capture length for packet within the PCAP Next Generation packet block
            thePacketBlock.CapturedLength = theBinaryReader.ReadUInt32();

            // Read the actual length for the packet within the PCAP Next Generation packet block
            thePacketBlock.PacketLength = theBinaryReader.ReadUInt32();

            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            thePacketPayloadLength =
                thePacketBlock.BlockTotalLength -
                (uint)Constants.BlockTotalLength.PacketBlock -
                12;

            // Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            thePacketTimestamp =
                ((thePacketBlock.TimestampHigh * 4294967296) +
                thePacketBlock.TimestampLow) / 1000000.0;

            return theResult;
        }

        /// <summary>
        /// Processes a PCAP Next Generation simple packet block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation simple packet block could be processed</returns>
        private bool ProcessSimplePacketBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation simple packet block

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Create an instance of the PCAP Next Generation simple packet block
            Structures.SimplePacketBlockStructure theSimplePacketBlock =
                new Structures.SimplePacketBlockStructure();

            // Read the block type for the PCAP Next Generation simple packet block
            theSimplePacketBlock.BlockType = theBinaryReader.ReadUInt32();

            // Read the block total length for the PCAP Next Generation simple packet block, adjusting it up to the next even four byte boundary
            theSimplePacketBlock.BlockTotalLength =
                this.AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Read the actual length for the packet within the PCAP Next Generation simple packet block
            theSimplePacketBlock.PacketLength = theBinaryReader.ReadUInt32();

            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            thePacketPayloadLength =
                theSimplePacketBlock.BlockTotalLength -
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
        private bool ProcessEnhancedPacketBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation enhanced packet block

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Create an instance of the PCAP Next Generation enhanced packet block
            Structures.EnhancedPacketBlockStructure theEnhancedPacketBlock =
                new Structures.EnhancedPacketBlockStructure();

            // Read the block type for the PCAP Next Generation enhanced packet block
            theEnhancedPacketBlock.BlockType = theBinaryReader.ReadUInt32();

            // Read the block total length for the PCAP Next Generation enhanced packet block, adjusting it up to the next even four byte boundary
            theEnhancedPacketBlock.BlockTotalLength =
                this.AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Read the interface Id for the PCAP Next Generation enhanced packet block
            theEnhancedPacketBlock.InterfaceId = theBinaryReader.ReadUInt32();

            // Read the high bytes of the timestamp for the PCAP Next Generation enhanced packet block
            theEnhancedPacketBlock.TimestampHigh = theBinaryReader.ReadUInt32();

            // Read the low bytes of the timestamp for the PCAP Next Generation enhanced packet block
            theEnhancedPacketBlock.TimestampLow = theBinaryReader.ReadUInt32();

            // Read the capture length for packet within the PCAP Next Generation enhanced packet block
            theEnhancedPacketBlock.CapturedLength = theBinaryReader.ReadUInt32();

            // Read the actual length for the packet within the PCAP Next Generation enhanced packet block
            theEnhancedPacketBlock.PacketLength = theBinaryReader.ReadUInt32();

            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            thePacketPayloadLength =
                theEnhancedPacketBlock.BlockTotalLength -
                (uint)Constants.BlockTotalLength.EnhancedPacketBlock -
                12;

            // Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            thePacketTimestamp =
                ((theEnhancedPacketBlock.TimestampHigh * 4294967296) +
                theEnhancedPacketBlock.TimestampLow) / 1000000.0;

            return theResult;
        }

        /// <summary>
        /// Process a PCAP Next Generation interface statistics block
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP Next Generation interface statistics block could be processed</returns>
        private bool ProcessInterfaceStatisticsBlock(System.IO.BinaryReader theBinaryReader, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            // We have got a PCAP Next Generation interface statistics block

            // Provide a default value to the output parameter for the length of the PCAP Next Generation block payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Create an instance of the PCAP Next Generation interface statistics block
            Structures.InterfaceStatisticsBlockStructure theInterfaceStatisticsBlock =
                new Structures.InterfaceStatisticsBlockStructure();

            // Read the block type for the PCAP Next Generation interface statistics block
            theInterfaceStatisticsBlock.BlockType = theBinaryReader.ReadUInt32();

            // Read the block total length for the PCAP Next Generation interface statistics block, adjusting it up to the next even four byte boundary
            theInterfaceStatisticsBlock.BlockTotalLength =
                this.AdjustBlockTotalLength(
                theBinaryReader.ReadUInt32());

            // Read the interface Id for the PCAP Next Generation interface statistics block
            theInterfaceStatisticsBlock.InterfaceId = theBinaryReader.ReadUInt32();

            // Read the high bytes of the timestamp for the PCAP Next Generation interface statistics block
            theInterfaceStatisticsBlock.TimestampHigh = theBinaryReader.ReadUInt32();

            // Read the low bytes of the timestamp for the PCAP Next Generation interface statistics block
            theInterfaceStatisticsBlock.TimestampLow = theBinaryReader.ReadUInt32();

            // The PCAP Next Generation interface statistics block does not contain an Ethernet frame
            // Just read the bytes off the remaining bytes from the PCAP Next Generation interface statistics block so we can continue
            theBinaryReader.ReadBytes(
                (int)(theInterfaceStatisticsBlock.BlockTotalLength -
                (uint)Constants.BlockTotalLength.InterfaceStatisticsBlock));

            // The PCAP Next Generation interface statistics block does not contain an Ethernet frame
            // Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            // Set this output parameter to a value of zero to prevent any attempt to process an Ethernet frame from the PCAP Next Generation packet capture packet payload
            thePacketPayloadLength = 0;

            // Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            thePacketTimestamp =
                ((theInterfaceStatisticsBlock.TimestampHigh * 4294967296) +
                theInterfaceStatisticsBlock.TimestampLow) / 1000000.0;

            return theResult;
        }

        /// <summary>
        /// Adjust the supplied block total length up to the next even four byte boundary
        /// </summary>
        /// <param name="blockTotalLength">The current block total length</param>
        /// <returns>The adjusted block total length</returns>
        private uint AdjustBlockTotalLength(uint blockTotalLength)
        {
            uint theAdjustedBlockTotalLength = blockTotalLength;

            //// Adjust the supplied block total length up to the next even four byte boundary

            //// A non-obvious characteristic of the current block total length field is that the block total length may not actually
            //// indicate the length of the actual data block (the payload). If the number of payload octets is not evenly divisable
            //// by four then the one or more padding bytes will have been added after the payload and before the block trailer to
            //// pad the length of the payload to a 32 bit boundry. To seek forwards or backwards to the next or previous block
            //// one will need to add from 0 to 3 to the reported block total length to detremine where the next block will start

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
    }
}
