// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.PacketCapture.SnifferPackageCapture
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

            if (theBinaryReader == null)
            {
                throw new System.ArgumentNullException("theBinaryReader");
            }

            // Provide a default value for the output parameter for the network datalink type
            thePacketCaptureNetworkDataLinkType =
                (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Invalid;

            // Provide a default value to the output parameter for the timestamp accuracy
            thePacketCaptureTimestampAccuracy = 0.0;

            // Just read off the data for the Sniffer packet capture global header from the packet capture so we can move on
            // Some of the data will be stored for use below
            ulong theMagicNumberHigh = theBinaryReader.ReadUInt64();
            ulong theMagicNumberLow = theBinaryReader.ReadUInt64();
            byte theMagicNumberTerminator = theBinaryReader.ReadByte();
            ushort theRecordType = theBinaryReader.ReadUInt16();
            theBinaryReader.ReadUInt32(); // Record length
            short theVersionMajor = theBinaryReader.ReadInt16();
            short theVersionMinor = theBinaryReader.ReadInt16();
            theBinaryReader.ReadInt16(); // Time
            theBinaryReader.ReadInt16(); // Date
            sbyte theType = theBinaryReader.ReadSByte();
            byte theNetworkEncapsulationType = theBinaryReader.ReadByte();
            sbyte theFormatVersion = theBinaryReader.ReadSByte();
            byte theTimestampUnits = theBinaryReader.ReadByte();
            theBinaryReader.ReadSByte(); // Compression version
            theBinaryReader.ReadSByte(); // Compression level
            theBinaryReader.ReadInt32(); // Reserved

            // Validate fields from the Sniffer packet capture global header
            theResult = this.ValidateGlobalHeader(
                theMagicNumberHigh,
                theMagicNumberLow,
                theMagicNumberTerminator,
                theRecordType,
                theVersionMajor,
                theVersionMinor,
                theType,
                theNetworkEncapsulationType,
                theFormatVersion);

            if (theResult)
            {
                // Set up the output parameter for the network data link type
                thePacketCaptureNetworkDataLinkType =
                    theNetworkEncapsulationType;

                // Derive the output parameter for the timestamp accuracy (actually a timestamp slice for a Sniffer packet capture) from the timestamp units in the Sniffer packet capture global header
                theResult = this.CalculateTimestampAccuracy(
                    theTimestampUnits,
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

            if (theBinaryReader == null)
            {
                throw new System.ArgumentNullException("theBinaryReader");
            }

            // Provide a default value to the output parameter for the length of the Sniffer packet payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Read off and store the Sniffer packet record header type from the packet capture for use below
            ushort theRecordType = theBinaryReader.ReadUInt16();

            // Just read off the Sniffer packet record header record length so we can move on
            theBinaryReader.ReadUInt32();

            theResult = this.ValidateRecordHeader(theRecordType);

            if (theResult)
            {
                switch (theRecordType)
                {
                    case (ushort)Constants.RecordHeaderSnifferRecordType.Type2RecordType:
                        {
                            //// We have got a Sniffer type 2 data record

                            // Increment the number for the packet read from the packet capture for a Sniffer type 2 data record
                            ++thePacketNumber;

                            // Just read off the data for the Sniffer type 2 data record from the packet capture so we can move on
                            // Some of the data will be stored for use below
                            ushort theTimestampLow = theBinaryReader.ReadUInt16();
                            ushort theTimestampMiddle = theBinaryReader.ReadUInt16();
                            byte theTimestampHigh = theBinaryReader.ReadByte();
                            theBinaryReader.ReadByte(); // Time Days
                            short theSize = theBinaryReader.ReadInt16();
                            theBinaryReader.ReadByte(); // Frame error status bits
                            theBinaryReader.ReadByte(); // Flags
                            theBinaryReader.ReadInt16(); // True size
                            theBinaryReader.ReadInt16(); // Reserved

                            // Set up the output parameter for the length of the Sniffer packet payload
                            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
                            thePacketPayloadLength = theSize - 12;

                            // Set up the output parameter for the timestamp based on the supplied timestamp slice and the timestamp from the Sniffer type 2 data record
                            // This is the number of seconds passed on this particular day
                            // To match up with a timestamp displayed since epoch would have to get the value of the Date field from the Sniffer packet capture global header
                            thePacketTimestamp =
                                (thePacketCaptureTimestampAccuracy * (theTimestampHigh * 4294967296)) +
                                (thePacketCaptureTimestampAccuracy * (theTimestampMiddle * 65536)) +
                                (thePacketCaptureTimestampAccuracy * theTimestampLow);

                            break;
                        }

                    case (ushort)Constants.RecordHeaderSnifferRecordType.EndOfFileRecordType:
                        {
                            //// We have got a Sniffer end of file record

                            //// No further reading required for the Sniffer end of file record as it only consists of the Sniffer packet capture record header!

                            //// Do not increment the number for the packet read from the packet capture for a Sniffer end of file record as it will not show in Wireshark and so would confuse comparisons with other formats

                            break;
                        }

                    default:
                        {
                            //// We have got an Sniffer packet capture containing an unknown record type

                            // Increment the number for the packet read from the packet capture for an unknown Sniffer packet capture record
                            ++thePacketNumber;

                            //// Processing of Sniffer packet captures with record types not enumerated above is obviously not currently supported!

                            this.TheDebugInformation.WriteErrorEvent(
                                "The Sniffer packet capture contains an unexpected record type of " +
                                theRecordType.ToString(System.Globalization.CultureInfo.CurrentCulture) +
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
        /// <param name="theMagicNumberHigh">The high bytes from the magic number in the Sniffer packet capture global header</param>
        /// <param name="theMagicNumberLow">The low bytes from the magic number in the Sniffer packet capture global header</param>
        /// <param name="theMagicNumberTerminator">The terminating byte from the magic number in the Sniffer packet capture global header</param>
        /// <param name="theRecordType">The type of version records in Sniffer packet capture</param>
        /// <param name="theVersionMajor">The major version number in the Sniffer packet capture global header</param>
        /// <param name="theVersionMinor">The minor version number in the Sniffer packet capture global header</param>
        /// <param name="theType">The type of records in the Sniffer packet capture</param>
        /// <param name="theNetworkEncapsulationType">The type of network encapsulation in the Sniffer packet capture</param>
        /// <param name="theFormatVersion">The format version in the Sniffer packet capture global header</param>
        /// <returns>Boolean flag that indicates whether the Sniffer packet capture global header is valid</returns>
        private bool ValidateGlobalHeader(ulong theMagicNumberHigh, ulong theMagicNumberLow, byte theMagicNumberTerminator, ushort theRecordType, short theVersionMajor, short theVersionMinor, sbyte theType, byte theNetworkEncapsulationType, sbyte theFormatVersion)
        {
            bool theResult = true;

            //// Validate fields from the Sniffer packet capture global header

            if (theMagicNumberHigh != Constants.ExpectedMagicNumberHigh)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected high bytes for the magic number, is " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:X}", theMagicNumberHigh.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " not " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:X}", Constants.ExpectedMagicNumberHigh.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    "!!!");

                theResult = false;
            }

            if (theMagicNumberLow != Constants.ExpectedMagicNumberLow)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected low bytes for the magic number, is " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:X}", theMagicNumberLow.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " not " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:X}", Constants.ExpectedMagicNumberLow.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    "!!!");

                theResult = false;
            }

            if (theMagicNumberTerminator != Constants.ExpectedMagicNumberTerminator)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected magic number terminating character, is " +
                    theMagicNumberTerminator.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.ExpectedMagicNumberTerminator.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            if (theRecordType != (ushort)Constants.RecordHeaderSnifferRecordType.VersionRecordType)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected record type, is " +
                    theRecordType.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.RecordHeaderSnifferRecordType.VersionRecordType.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theVersionMajor != Constants.ExpectedVersionMajor)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected major version number, is " +
                    theVersionMajor.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.ExpectedVersionMajor.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            if (theVersionMinor != Constants.ExpectedVersionMinor)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected minor version number, is " +
                    theVersionMinor.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.ExpectedVersionMinor.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            if (theType != Constants.ExpectedType)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected record type, is " +
                    theType.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.ExpectedType.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            switch (theNetworkEncapsulationType)
            {
                case (byte)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopback:
                case (byte)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet:
                case (byte)PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC:
                    {
                        break;
                    }

                default:
                    {
                        this.TheDebugInformation.WriteErrorEvent(
                            "The Sniffer packet capture global header does not contain the expected network encapsulation type, is " +
                            theNetworkEncapsulationType.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                            " not " +
                            PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopback.ToString() +
                            " or " +
                            PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet.ToString() +
                            " or " +
                            PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC.ToString() +
                            "!!!");

                        theResult = false;

                        break;
                    }
            }

            if (theFormatVersion != Constants.ExpectedFormatVersion)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The Sniffer packet capture global header does not contain the expected format version, is " +
                    theFormatVersion.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.ExpectedFormatVersion.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }

        /// <summary>
        /// Validates the Sniffer packet record header
        /// </summary>
        /// <param name="theHeaderRecordType">The record type in the Sniffer packet record header</param>
        /// <returns>Boolean flag that indicates whether the Sniffer packet record header is valid</returns>
        private bool ValidateRecordHeader(ushort theHeaderRecordType)
        {
            bool theResult = true;

            //// Validate fields from the Sniffer packet record header

            switch (theHeaderRecordType)
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
                            theHeaderRecordType.ToString(System.Globalization.CultureInfo.CurrentCulture) +
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
                            thePacketCaptureTimestampUnits.ToString(System.Globalization.CultureInfo.CurrentCulture) +
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
