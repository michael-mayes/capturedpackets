// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.PacketCapture.SnifferPackageCapture
{
    /// <summary>
    /// This class provides the Sniffer packet capture processing
    /// </summary>
    public class Processing : CommonProcessing
    {
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
        /// Processes the Sniffer packet capture global header
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the Sniffer packet capture global header could be processed</returns>
        protected override bool ProcessPacketCaptureGlobalHeader(System.IO.BinaryReader theBinaryReader, out uint thePacketCaptureNetworkDataLinkType, out double thePacketCaptureTimestampAccuracy)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the network datalink type
            thePacketCaptureNetworkDataLinkType =
                (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Invalid;

            // Provide a default value to the output parameter for the timestamp accuracy
            thePacketCaptureTimestampAccuracy = 0.0;

            // Create the single instance of the Sniffer packet capture global header
            Structures.GlobalHeaderStructure theGlobalHeader =
                new Structures.GlobalHeaderStructure();

            // Populate the Sniffer packet capture global header from the packet capture
            theGlobalHeader.MagicNumberHigh = theBinaryReader.ReadUInt64();
            theGlobalHeader.MagicNumberLow = theBinaryReader.ReadUInt64();
            theGlobalHeader.MagicNumberTerminator = theBinaryReader.ReadByte();
            theGlobalHeader.RecordType = theBinaryReader.ReadUInt16();
            theGlobalHeader.RecordLength = theBinaryReader.ReadUInt32();
            theGlobalHeader.VersionMajor = theBinaryReader.ReadInt16();
            theGlobalHeader.VersionMinor = theBinaryReader.ReadInt16();
            theGlobalHeader.Time = theBinaryReader.ReadInt16();
            theGlobalHeader.Date = theBinaryReader.ReadInt16();
            theGlobalHeader.Type = theBinaryReader.ReadSByte();
            theGlobalHeader.NetworkEncapsulationType = theBinaryReader.ReadByte();
            theGlobalHeader.FormatVersion = theBinaryReader.ReadSByte();
            theGlobalHeader.TimestampUnits = theBinaryReader.ReadByte();
            theGlobalHeader.CompressionVersion = theBinaryReader.ReadSByte();
            theGlobalHeader.CompressionLevel = theBinaryReader.ReadSByte();
            theGlobalHeader.Reserved = theBinaryReader.ReadInt32();

            // Validate fields from the Sniffer packet capture global header
            theResult = this.ValidateGlobalHeader(
                theGlobalHeader);

            if (theResult)
            {
                // Set up the output parameter for the network data link type
                thePacketCaptureNetworkDataLinkType =
                    theGlobalHeader.NetworkEncapsulationType;

                // Derive the output parameter for the timestamp accuracy (actually a timestamp slice for a Sniffer packet capture) from the timestamp units in the Sniffer packet capture global header
                theResult = this.CalculateTimestampAccuracy(
                    theGlobalHeader.TimestampUnits,
                    out thePacketCaptureTimestampAccuracy);
            }

            return theResult;
        }

        /// <summary>
        /// Processes the Sniffer packet record header
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the Sniffer packet record header could be processed</returns>
        protected override bool ProcessPacketHeader(System.IO.BinaryReader theBinaryReader, uint thePacketCaptureNetworkDataLinkType, double thePacketCaptureTimestampAccuracy, ref ulong thePacketNumber, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            // Provide a default value to the output parameter for the length of the Sniffer packet payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Always increment the number for the packet read from the packet capture for a Sniffer packet
            ++thePacketNumber;

            // Create an instance of the Sniffer packet record header
            Structures.RecordHeaderStructure theRecordHeader =
                new Structures.RecordHeaderStructure();

            // Populate the Sniffer packet record header from the packet capture
            theRecordHeader.RecordType = theBinaryReader.ReadUInt16();
            theRecordHeader.RecordLength = theBinaryReader.ReadUInt32();

            theResult = this.ValidateRecordHeader(theRecordHeader);

            if (theResult)
            {
                switch (theRecordHeader.RecordType)
                {
                    case (ushort)Constants.RecordHeaderSnifferRecordType.Type2RecordType:
                        {
                            // Create an instance of the Sniffer type 2 data record
                            Structures.SnifferType2RecordStructure theType2Record =
                                new Structures.SnifferType2RecordStructure();

                            // Populate the Sniffer type 2 data record from the packet capture
                            theType2Record.TimestampLow = theBinaryReader.ReadUInt16();
                            theType2Record.TimestampMiddle = theBinaryReader.ReadUInt16();
                            theType2Record.TimestampHigh = theBinaryReader.ReadByte();
                            theType2Record.TimeDays = theBinaryReader.ReadByte();
                            theType2Record.Size = theBinaryReader.ReadInt16();
                            theType2Record.FrameErrorStatusBits = theBinaryReader.ReadByte();
                            theType2Record.Flags = theBinaryReader.ReadByte();
                            theType2Record.TrueSize = theBinaryReader.ReadInt16();
                            theType2Record.Reserved = theBinaryReader.ReadInt16();

                            // Set up the output parameter for the length of the Sniffer packet payload
                            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
                            thePacketPayloadLength = theType2Record.Size - 12;

                            // Set up the output parameter for the timestamp based on the supplied timestamp slice and the timestamp from the Sniffer type 2 data record
                            // This is the number of seconds passed on this particular day
                            // To match up with a timestamp displayed since epoch would have to get the value of the Date field from the Sniffer packet capture global header
                            thePacketTimestamp =
                                (thePacketCaptureTimestampAccuracy * (theType2Record.TimestampHigh * 4294967296)) +
                                (thePacketCaptureTimestampAccuracy * (theType2Record.TimestampMiddle * 65536)) +
                                (thePacketCaptureTimestampAccuracy * theType2Record.TimestampLow);

                            break;
                        }

                    case (ushort)Constants.RecordHeaderSnifferRecordType.EndOfFileRecordType:
                        {
                            // No further reading required for the Sniffer end of file data record as it only consists of the Sniffer packet capture record header!
                            break;
                        }

                    default:
                        {
                            //// We have got an Sniffer packet capture containing an unknown record type

                            //// Processing of Sniffer packet captures with record types not enumerated above are obviously not currently supported!

                            this.TheDebugInformation.WriteErrorEvent(
                                "The Sniffer packet capture contains an unexpected record type of " +
                                theRecordHeader.RecordType.ToString() +
                                "!!!");

                            theResult = false;

                            break;
                        }
                }
            }

            return theResult;
        }

        //// Private methods - provide methods specific to Sniffer packet captures, not required to derive from the abstract base class

        /// <summary>
        /// Validates the Sniffer packet capture global header
        /// </summary>
        /// <param name="theGlobalHeader">The Sniffer packet capture global header</param>
        /// <returns>Boolean flag that indicates whether the Sniffer packet capture global header is valid</returns>
        private bool ValidateGlobalHeader(Structures.GlobalHeaderStructure theGlobalHeader)
        {
            bool theResult = true;

            //// Validate fields from the Sniffer packet capture global header

            if (theGlobalHeader.MagicNumberHigh != Constants.ExpectedMagicNumberHigh)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected high bytes for the magic number, is " +
                    string.Format("{0:X}", theGlobalHeader.MagicNumberHigh.ToString()) +
                    " not " +
                    string.Format("{0:X}", Constants.ExpectedMagicNumberHigh.ToString()) +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.MagicNumberLow != Constants.ExpectedMagicNumberLow)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected low bytes for the magic number, is " +
                    string.Format("{0:X}", theGlobalHeader.MagicNumberLow.ToString()) +
                    " not " +
                    string.Format("{0:X}", Constants.ExpectedMagicNumberLow.ToString()) +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.MagicNumberTerminator != Constants.ExpectedMagicNumberTerminator)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected magic number terminating character, is " +
                    theGlobalHeader.MagicNumberTerminator.ToString() +
                    " not " +
                    Constants.ExpectedMagicNumberTerminator.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.RecordType != (ushort)Constants.RecordHeaderSnifferRecordType.VersionRecordType)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected record type, is " +
                    theGlobalHeader.RecordType.ToString() +
                    " not " +
                    Constants.RecordHeaderSnifferRecordType.VersionRecordType.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.VersionMajor != Constants.ExpectedVersionMajor)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected major version number, is " +
                    theGlobalHeader.VersionMajor.ToString() +
                    " not " +
                    Constants.ExpectedVersionMajor.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.VersionMinor != Constants.ExpectedVersionMinor)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected minor version number, is " +
                    theGlobalHeader.VersionMinor.ToString() +
                    " not " +
                    Constants.ExpectedVersionMinor.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.Type != Constants.ExpectedType)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected record type, is " +
                    theGlobalHeader.Type.ToString() +
                    " not " +
                    Constants.ExpectedType.ToString() +
                    "!!!");

                theResult = false;
            }

            switch (theGlobalHeader.NetworkEncapsulationType)
            {
                case (byte)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack:
                case (byte)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet:
                case (byte)PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC:
                    {
                        break;
                    }

                default:
                    {
                        this.TheDebugInformation.WriteErrorEvent(
                            "The Sniffer packet capture global header does not contain the expected network encapsulation type, is " +
                            theGlobalHeader.NetworkEncapsulationType.ToString() +
                            " not " +
                            PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack.ToString() +
                            " or " +
                            PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet.ToString() +
                            " or " +
                            PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC.ToString() +
                            "!!!");

                        theResult = false;

                        break;
                    }
            }

            if (theGlobalHeader.FormatVersion != Constants.ExpectedFormatVersion)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected format version, is " +
                    theGlobalHeader.FormatVersion.ToString() +
                    " not " +
                    Constants.ExpectedFormatVersion.ToString() +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }

        /// <summary>
        /// Validates the Sniffer packet record header
        /// </summary>
        /// <param name="theRecordHeader">The Sniffer packet record header</param>
        /// <returns>Boolean flag that indicates whether the Sniffer packet record header is valid</returns>
        private bool ValidateRecordHeader(Structures.RecordHeaderStructure theRecordHeader)
        {
            bool theResult = true;

            //// Validate fields from the Sniffer packet record header

            switch (theRecordHeader.RecordType)
            {
                case (ushort)Constants.RecordHeaderSnifferRecordType.Type2RecordType:
                case (ushort)Constants.RecordHeaderSnifferRecordType.EndOfFileRecordType:
                    {
                        break;
                    }

                default:
                    {
                        this.TheDebugInformation.WriteErrorEvent(
                            "The Sniffer packet capture record header does not contain the expected type, is " +
                            theRecordHeader.RecordType.ToString() +
                            "!!!");

                        theResult = false;

                        break;
                    }
            }

            return theResult;
        }

        /// <summary>
        /// Calculates the accuracy of the timestamp for the packet capture based on the timestamp units read from the packet capture
        /// </summary>
        /// <param name="thePacketCaptureTimestampUnits">The timestamp units read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp for the packet capture</param>
        /// <returns>Boolean flag that indicates whether the accuracy of the timestamp for the packet capture could be calculated</returns>
        private bool CalculateTimestampAccuracy(byte thePacketCaptureTimestampUnits, out double thePacketCaptureTimestampAccuracy)
        {
            bool theResult = true;

            switch (thePacketCaptureTimestampUnits)
            {
                // 15.0 microseconds
                case 0:
                    {
                        thePacketCaptureTimestampAccuracy = 0.000015;
                        break;
                    }

                // 0.838096 microseconds
                case 1:
                    {
                        thePacketCaptureTimestampAccuracy = 0.000000838096;
                        break;
                    }

                // 15.0 microseconds
                case 2:
                    {
                        thePacketCaptureTimestampAccuracy = 0.000015;
                        break;
                    }

                // 0.5 microseconds
                case 3:
                    {
                        thePacketCaptureTimestampAccuracy = 0.0000005;
                        break;
                    }

                // 2.0 microseconds
                case 4:
                    {
                        thePacketCaptureTimestampAccuracy = 0.000002;
                        break;
                    }

                // 0.08 microseconds
                case 5:
                    {
                        thePacketCaptureTimestampAccuracy = 0.00000008;
                        break;
                    }

                // 0.1 microseconds
                case 6:
                    {
                        thePacketCaptureTimestampAccuracy = 0.0000001;
                        break;
                    }

                default:
                    {
                        this.TheDebugInformation.WriteErrorEvent(
                            "The Sniffer packet capture contains an unexpected timestamp unit " +
                            thePacketCaptureTimestampUnits.ToString() +
                            "!!!");

                        thePacketCaptureTimestampAccuracy = 0.0;

                        theResult = false;

                        break;
                    }
            }

            return theResult;
        }
    }
}
