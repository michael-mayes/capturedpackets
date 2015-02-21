// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.PacketCapture.PCAPPackageCapture
{
    /// <summary>
    /// This class provides the PCAP packet capture processing
    /// </summary>
    public class Processing : CommonProcessing
    {
        /// <summary>
        /// Boolean flag that indicates whether the PCAP packet capture global header indicates little endian data in the PCAP packet payload
        /// </summary>
        private bool isTheGlobalHeaderLittleEndian = true;

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
        /// Processes the PCAP packet capture global header
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP packet capture global header could be processed</returns>
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

            // Set up the output parameter for the timestamp accuracy - not used for PCAP packet captures so default to zero
            thePacketCaptureTimestampAccuracy = 0.0;

            // Read the magic number of the PCAP packet capture global header from the packet capture
            uint theMagicNumber = theBinaryReader.ReadUInt32();

            // The endianism of the remainder of the values in the PCAP packet capture global header will be corrected to little endian if the magic number indicates big endian representation
            if (theMagicNumber == Constants.LittleEndianMagicNumber)
            {
                this.TheDebugInformation.WriteInformationEvent(
                    "The PCAP packet capture contains the little endian magic number");

                this.isTheGlobalHeaderLittleEndian = true;
            }
            else if (theMagicNumber == Constants.BigEndianMagicNumber)
            {
                this.TheDebugInformation.WriteInformationEvent(
                    "The PCAP packet capture contains the big endian magic number");

                this.isTheGlobalHeaderLittleEndian = false;
            }
            else
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP packet capture contains an unknown endian magic number");

                // Assume little endian
                this.isTheGlobalHeaderLittleEndian = true;
            }

            ushort theVersionMajor = 0;
            ushort theVersionMinor = 0;
            uint theNetworkDataLinkType = 0;

            // Just read off the remainder of the PCAP packet capture global header from the packet capture so we can move on
            // Some of the data will be stored for use below
            if (this.isTheGlobalHeaderLittleEndian)
            {
                theVersionMajor = theBinaryReader.ReadUInt16();
                theVersionMinor = theBinaryReader.ReadUInt16();
                theBinaryReader.ReadInt32(); // This time zone
                theBinaryReader.ReadUInt32(); // Significant figures
                theBinaryReader.ReadUInt32(); // Snapshot length
                theNetworkDataLinkType = theBinaryReader.ReadUInt32();
            }
            else
            {
                theVersionMajor = (ushort)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt16());
                theVersionMinor = (ushort)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt16());
                theBinaryReader.ReadInt32(); // This time zone
                theBinaryReader.ReadUInt32(); // Significant figures
                theBinaryReader.ReadUInt32(); // Snapshot length
                theNetworkDataLinkType = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
            }

            // Validate fields from the PCAP packet capture global header
            theResult = this.ValidateGlobalHeader(
                theMagicNumber,
                theVersionMajor,
                theVersionMinor,
                theNetworkDataLinkType);

            if (theResult)
            {
                // Set up the output parameter for the network data link type
                thePacketCaptureNetworkDataLinkType =
                    theNetworkDataLinkType;
            }

            return theResult;
        }

        /// <summary>
        /// Processes the PCAP packet header
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP packet header could be processed</returns>
        protected override bool ProcessPacketHeader(System.IO.BinaryReader theBinaryReader, uint thePacketCaptureNetworkDataLinkType, double thePacketCaptureTimestampAccuracy, ref ulong thePacketNumber, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            if (theBinaryReader == null)
            {
                throw new System.ArgumentNullException("theBinaryReader");
            }

            // Provide a default value to the output parameter for the length of the PCAP packet payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Always increment the number for the packet read from the packet capture for a PCAP packet
            ++thePacketNumber;

            uint theTimestampSeconds = 0;
            uint theTimestampMicroseconds = 0;
            uint theSavedLength = 0;

            // Just read off the remainder of the PCAP packet header from the packet capture so we can move on
            // Some of the data will be stored for use below
            if (this.isTheGlobalHeaderLittleEndian)
            {
                theTimestampSeconds = theBinaryReader.ReadUInt32();
                theTimestampMicroseconds = theBinaryReader.ReadUInt32();
                theSavedLength = theBinaryReader.ReadUInt32();
                theBinaryReader.ReadUInt32(); // Actual length
            }
            else
            {
                theTimestampSeconds = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
                theTimestampMicroseconds = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
                theSavedLength = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
                theBinaryReader.ReadUInt32(); // Actual length
            }

            // No need to validate fields from the PCAP packet header
            if (theResult)
            {
                // Set up the output parameter for the length of the PCAP packet capture packet payload
                switch (thePacketCaptureNetworkDataLinkType)
                {
                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet:
                        {
                            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
                            thePacketPayloadLength = theSavedLength - 12;

                            break;
                        }

                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopback:
                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC:
                    default:
                        {
                            thePacketPayloadLength = theSavedLength;
                            break;
                        }
                }

                // Set up the output parameter for the timestamp based on the timestamp values in seconds and microseconds
                thePacketTimestamp =
                    (double)theTimestampSeconds +
                    ((double)theTimestampMicroseconds / 1000000.0);
            }

            return theResult;
        }

        //// Private methods - provide methods specific to PCAP packet captures, not required to derive from the abstract base class

        /// <summary>
        /// Validates the PCAP packet capture global header
        /// </summary>
        /// <param name="theMagicNumber">The magic number in the PCAP packet capture global header</param>
        /// <param name="theVersionMajor">The major version number in the PCAP packet capture global header</param>
        /// <param name="theVersionMinor">The minor version number in the PCAP packet capture global header</param>
        /// <param name="theNetworkDataLinkType">The type of network encapsulation in the PCAP packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP packet capture global header is valid</returns>
        private bool ValidateGlobalHeader(uint theMagicNumber, ushort theVersionMajor, ushort theVersionMinor, uint theNetworkDataLinkType)
        {
            bool theResult = true;

            //// Validate fields from the PCAP packet capture global header

            if (theMagicNumber != Constants.LittleEndianMagicNumber &&
                theMagicNumber != Constants.BigEndianMagicNumber)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP packet capture global header does not contain the expected magic number, is " +
                    theMagicNumber.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.LittleEndianMagicNumber.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " or " +
                    Constants.BigEndianMagicNumber.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            if (theVersionMajor != Constants.ExpectedVersionMajor)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP packet capture global header does not contain the expected major version number, is " +
                    theVersionMajor.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.ExpectedVersionMajor.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            if (theVersionMinor != Constants.ExpectedVersionMinor)
            {
                this.TheDebugInformation.WriteErrorEvent(
                    "The PCAP packet capture global header does not contain the expected minor version number, is " +
                    theVersionMinor.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    " not " +
                    Constants.ExpectedVersionMinor.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                    "!!!");

                theResult = false;
            }

            switch (theNetworkDataLinkType)
            {
                case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopback:
                case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet:
                case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC:
                    {
                        break;
                    }

                default:
                    {
                        this.TheDebugInformation.WriteErrorEvent(
                            "The PCAP packet capture global header does not contain the expected network data link type, is " +
                            theNetworkDataLinkType.ToString(System.Globalization.CultureInfo.CurrentCulture) +
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

            return theResult;
        }
    }
}
