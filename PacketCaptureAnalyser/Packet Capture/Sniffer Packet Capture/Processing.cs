// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.SnifferPackageCapture
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
        /// <param name="theProgressWindowForm"></param>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="performLatencyAnalysisProcessing">The flag that indicates whether to perform latency analysis processing for data read from the packet capture</param>
        /// <param name="theLatencyAnalysisProcessing">The object that provides the latency analysis processing for data read from the packet capture</param>
        /// <param name="performTimeAnalysisProcessing">The flag that indicates whether to perform time analysis processing for data read from the packet capture</param>
        /// <param name="theTimeAnalysisProcessing">The object that provides the time analysis processing for data read from the packet capture</param>
        /// <param name="thePacketCapture"></param>
        /// <param name="minimiseMemoryUsage"></param>
        public Processing(PacketCaptureAnalyser.ProgressWindowForm theProgressWindowForm, Analysis.DebugInformation theDebugInformation, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing, string thePacketCapture, bool minimiseMemoryUsage) :
            base(
            theProgressWindowForm,
            theDebugInformation,
            performLatencyAnalysisProcessing,
            theLatencyAnalysisProcessing,
            performTimeAnalysisProcessing,
            theTimeAnalysisProcessing,
            thePacketCapture,
            minimiseMemoryUsage)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="theNetworkDataLinkType"></param>
        /// <param name="theTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <returns></returns>
        protected override bool ProcessGlobalHeader(System.IO.BinaryReader theBinaryReader, out uint theNetworkDataLinkType, out double theTimestampAccuracy)
        {
            bool theResult = true;

            // Provide a default value for the output parameter for the network datalink type
            theNetworkDataLinkType = (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Invalid;

            // Provide a default value to the output parameter for the timestamp accuracy
            theTimestampAccuracy = 0.0;

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
            theResult = this.ValidateGlobalHeader(theGlobalHeader);

            if (theResult)
            {
                // Set up the output parameter for the network data link type
                theNetworkDataLinkType = theGlobalHeader.NetworkEncapsulationType;

                // Derive the output parameter for the timestamp accuracy (actually a timestamp slice for a Sniffer packet capture) from the timestamp units in the Sniffer packet capture global header
                theResult = this.CalculateTimestampAccuracy(theGlobalHeader.TimestampUnits, out theTimestampAccuracy);
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="theNetworkDataLinkType"></param>
        /// <param name="theTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <param name="thePayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="theTimestamp">The timestamp read from the packet capture</param>
        /// <returns></returns>
        protected override bool ProcessPacketHeader(System.IO.BinaryReader theBinaryReader, uint theNetworkDataLinkType, double theTimestampAccuracy, out long thePayloadLength, out double theTimestamp)
        {
            bool theResult = true;

            // Provide a default value to the output parameter for the length of the PCAP packet capture packet payload
            thePayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            theTimestamp = 0.0;

            // Create an instance of the Sniffer packet capture record header
            Structures.RecordHeaderStructure theRecordHeader =
                new Structures.RecordHeaderStructure();

            // Populate the Sniffer packet capture record header from the packet capture
            theRecordHeader.RecordType = theBinaryReader.ReadUInt16();
            theRecordHeader.RecordLength = theBinaryReader.ReadUInt32();

            theResult = this.ValidateRecordHeader(theRecordHeader);

            if (theResult)
            {
                switch (theRecordHeader.RecordType)
                {
                    case (ushort)Constants.RecordHeaderSnifferRecordType.Type2RecordType:
                        {
                            // Create an instance of the Sniffer packet capture Sniffer type 2 data record
                            Structures.SnifferType2RecordStructure theType2Record =
                                new Structures.SnifferType2RecordStructure();

                            // Populate the Sniffer packet capture Sniffer type 2 data record from the packet capture
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
                            thePayloadLength = theType2Record.Size - 12;

                            // Set up the output parameter for the timestamp based on the supplied timestamp slice and the timestamp from the Sniffer type 2 data record
                            // This is the number of seconds passed on this particular day
                            // To match up with a timestamp displayed since epoch would have to get the value of the Date field from the Sniffer packet capture global header
                            theTimestamp =
                                (theTimestampAccuracy * (theType2Record.TimestampHigh * 4294967296)) +
                                (theTimestampAccuracy * (theType2Record.TimestampMiddle * 65536)) +
                                (theTimestampAccuracy * theType2Record.TimestampLow);

                            break;
                        }

                    case (ushort)Constants.RecordHeaderSnifferRecordType.EndOfFileRecordType:
                        {
                            // No further reading required for the Sniffer packet capture Sniffer end of file data record as it only consists of the Sniffer packet capture record header!
                            break;
                        }

                    default:
                        {
                            //// We have got an Sniffer packet capture containing an unknown record type

                            //// Processing of Sniffer packet captures with record types not enumerated above are obviously not currently supported!

                            theDebugInformation.WriteErrorEvent("The Sniffer packet capture contains an unexpected record type of " +
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
        /// 
        /// </summary>
        /// <param name="theGlobalHeader"></param>
        /// <returns></returns>
        private bool ValidateGlobalHeader(Structures.GlobalHeaderStructure theGlobalHeader)
        {
            bool theResult = true;

            //// Validate fields from the Sniffer packet capture global header

            if (theGlobalHeader.MagicNumberHigh != Constants.ExpectedMagicNumberHigh)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture global header does not contain the expected high bytes for the magic number, is " +
                    string.Format("{0:X}", theGlobalHeader.MagicNumberHigh.ToString()) +
                    " not " +
                    string.Format("{0:X}", Constants.ExpectedMagicNumberHigh.ToString()) +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.MagicNumberLow != Constants.ExpectedMagicNumberLow)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture global header does not contain the expected low bytes for the magic number, is " +
                    string.Format("{0:X}", theGlobalHeader.MagicNumberLow.ToString()) +
                    " not " +
                    string.Format("{0:X}", Constants.ExpectedMagicNumberLow.ToString()) +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.MagicNumberTerminator != Constants.ExpectedMagicNumberTerminator)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture global header does not contain the expected magic number terminating character, is " +
                    theGlobalHeader.MagicNumberTerminator.ToString() +
                    " not " +
                    Constants.ExpectedMagicNumberTerminator.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.RecordType != (ushort)Constants.RecordHeaderSnifferRecordType.VersionRecordType)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture global header does not contain the expected record type, is " +
                    theGlobalHeader.RecordType.ToString() +
                    " not " +
                    Constants.RecordHeaderSnifferRecordType.VersionRecordType.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.VersionMajor != Constants.ExpectedVersionMajor)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture global header does not contain the expected major version number, is " +
                    theGlobalHeader.VersionMajor.ToString() +
                    " not " +
                    Constants.ExpectedVersionMajor.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.VersionMinor != Constants.ExpectedVersionMinor)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture global header does not contain the expected minor version number, is " +
                    theGlobalHeader.VersionMinor.ToString() +
                    " not " +
                    Constants.ExpectedVersionMinor.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.Type != Constants.ExpectedType)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture global header does not contain the expected record type, is " +
                    theGlobalHeader.Type.ToString() +
                    " not " +
                    Constants.ExpectedType.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.NetworkEncapsulationType != (uint)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack &&
                theGlobalHeader.NetworkEncapsulationType != (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture global header does not contain the expected network encapsulation type, is " +
                    theGlobalHeader.NetworkEncapsulationType.ToString() +
                    " not " +
                    PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack.ToString() +
                    " or " +
                    PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.FormatVersion != Constants.ExpectedFormatVersion)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture global header does not contain the expected format version, is " +
                    theGlobalHeader.FormatVersion.ToString() +
                    " not " +
                    Constants.ExpectedFormatVersion.ToString() +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theRecordHeader"></param>
        /// <returns></returns>
        private bool ValidateRecordHeader(Structures.RecordHeaderStructure theRecordHeader)
        {
            bool theResult = true;

            // Validate fields from the Sniffer packet capture record header
            if (theRecordHeader.RecordType != (ushort)Constants.RecordHeaderSnifferRecordType.Type2RecordType &&
                theRecordHeader.RecordType != (ushort)Constants.RecordHeaderSnifferRecordType.EndOfFileRecordType)
            {
                theDebugInformation.WriteErrorEvent("The Sniffer packet capture record header does not contain the expected type, is " +
                    theRecordHeader.RecordType.ToString() +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theTimestampUnits"></param>
        /// <param name="theTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <returns></returns>
        private bool CalculateTimestampAccuracy(byte theTimestampUnits, out double theTimestampAccuracy)
        {
            bool theResult = true;

            switch (theTimestampUnits)
            {
                // 15.0 microseconds
                case 0:
                    {
                        theTimestampAccuracy = 0.000015;
                        break;
                    }

                // 0.838096 microseconds
                case 1:
                    {
                        theTimestampAccuracy = 0.000000838096;
                        break;
                    }

                // 15.0 microseconds
                case 2:
                    {
                        theTimestampAccuracy = 0.000015;
                        break;
                    }

                // 0.5 microseconds
                case 3:
                    {
                        theTimestampAccuracy = 0.0000005;
                        break;
                    }

                // 2.0 microseconds
                case 4:
                    {
                        theTimestampAccuracy = 0.000002;
                        break;
                    }

                // 0.08 microseconds
                case 5:
                    {
                        theTimestampAccuracy = 0.00000008;
                        break;
                    }

                // 0.1 microseconds
                case 6:
                    {
                        theTimestampAccuracy = 0.0000001;
                        break;
                    }

                default:
                    {
                        theDebugInformation.WriteErrorEvent("The Sniffer packet capture contains an unexpected timestamp unit " +
                            theTimestampUnits.ToString() +
                            "!!!");

                        theTimestampAccuracy = 0.0;

                        theResult = false;

                        break;
                    }
            }

            return theResult;
        }
    }
}
