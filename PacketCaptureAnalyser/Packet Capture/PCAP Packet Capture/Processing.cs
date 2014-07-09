// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.PCAPPackageCapture
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
        /// <param name="performTimeAnalysisProcessing">Boolean flag that indicates whether to perform time analysis processing for data read from the packet capture</param>
        /// <param name="theTimeAnalysisProcessing">The object that provides the time analysis processing for data read from the packet capture</param>
        /// <param name="theSelectedPacketCapturePath">The path of the selected packet capture</param>
        /// <param name="minimizeMemoryUsage">Boolean flag that indicates whether to perform reading from the packet capture using a method that will minimize memory usage, possibly at the expense of increased processing time</param>
        public Processing(PacketCaptureAnalyser.ProgressWindowForm theProgressWindowForm, Analysis.DebugInformation theDebugInformation, bool performLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing theLatencyAnalysisProcessing, bool performTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing theTimeAnalysisProcessing, string theSelectedPacketCapturePath, bool minimizeMemoryUsage) :
            base(
            theProgressWindowForm,
            theDebugInformation,
            performLatencyAnalysisProcessing,
            theLatencyAnalysisProcessing,
            performTimeAnalysisProcessing,
            theTimeAnalysisProcessing,
            theSelectedPacketCapturePath,
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

            // Provide a default value for the output parameter for the network datalink type
            thePacketCaptureNetworkDataLinkType = (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Invalid;

            // Set up the output parameter for the timestamp accuracy - not used for PCAP packet captures so default to zero
            thePacketCaptureTimestampAccuracy = 0.0;

            // Create the single instance of the PCAP packet capture global header
            Structures.GlobalHeaderStructure theGlobalHeader =
                new Structures.GlobalHeaderStructure();

            // Read the magic number of the PCAP packet capture global header from the packet capture
            theGlobalHeader.MagicNumber = theBinaryReader.ReadUInt32();

            // The endianism of the remainder of the values in the PCAP packet capture global header will be corrected to little endian if the magic number indicates big endian representation
            if (theGlobalHeader.MagicNumber == Constants.LittleEndianMagicNumber)
            {
                this.TheDebugInformation.WriteInformationEvent("The PCAP packet capture contains the little endian magic number");

                this.isTheGlobalHeaderLittleEndian = true;
            }
            else if (theGlobalHeader.MagicNumber == Constants.BigEndianMagicNumber)
            {
                this.TheDebugInformation.WriteInformationEvent("The PCAP packet capture contains the big endian magic number");

                this.isTheGlobalHeaderLittleEndian = false;
            }

            // Read the remainder of the PCAP packet capture global header from the packet capture
            if (this.isTheGlobalHeaderLittleEndian)
            {
                theGlobalHeader.VersionMajor = theBinaryReader.ReadUInt16();
                theGlobalHeader.VersionMinor = theBinaryReader.ReadUInt16();
                theGlobalHeader.ThisTimeZone = theBinaryReader.ReadInt32();
                theGlobalHeader.SignificantFigures = theBinaryReader.ReadUInt32();
                theGlobalHeader.SnapshotLength = theBinaryReader.ReadUInt32();
                theGlobalHeader.NetworkDataLinkType = theBinaryReader.ReadUInt32();
            }
            else
            {
                theGlobalHeader.VersionMajor = (ushort)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt16());
                theGlobalHeader.VersionMinor = (ushort)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt16());
                theGlobalHeader.ThisTimeZone = System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
                theGlobalHeader.SignificantFigures = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
                theGlobalHeader.SnapshotLength = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
                theGlobalHeader.NetworkDataLinkType = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
            }

            // Validate fields from the PCAP packet capture global header
            theResult = this.ValidateGlobalHeader(theGlobalHeader);

            if (theResult)
            {
                // Set up the output parameter for the network data link type
                thePacketCaptureNetworkDataLinkType = theGlobalHeader.NetworkDataLinkType;
            }

            return theResult;
        }

        /// <summary>
        /// Processes the PCAP packet header
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="thePacketCaptureNetworkDataLinkType">The network data link type read from the packet capture</param>
        /// <param name="thePacketCaptureTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <param name="thePacketPayloadLength">The payload length of the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <returns>Boolean flag that indicates whether the PCAP packet header could be processed</returns>
        protected override bool ProcessPacketHeader(System.IO.BinaryReader theBinaryReader, uint thePacketCaptureNetworkDataLinkType, double thePacketCaptureTimestampAccuracy, out long thePacketPayloadLength, out double thePacketTimestamp)
        {
            bool theResult = true;

            // Provide a default value to the output parameter for the length of the PCAP packet payload
            thePacketPayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            thePacketTimestamp = 0.0;

            // Create an instance of the PCAP packet header
            Structures.HeaderStructure thePacketHeader =
                new Structures.HeaderStructure();

            // Populate the PCAP packet header from the packet capture
            if (this.isTheGlobalHeaderLittleEndian)
            {
                thePacketHeader.TimestampSeconds = theBinaryReader.ReadUInt32();
                thePacketHeader.TimestampMicroseconds = theBinaryReader.ReadUInt32();
                thePacketHeader.SavedLength = theBinaryReader.ReadUInt32();
                thePacketHeader.ActualLength = theBinaryReader.ReadUInt32();
            }
            else
            {
                thePacketHeader.TimestampSeconds = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
                thePacketHeader.TimestampMicroseconds = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
                thePacketHeader.SavedLength = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
                thePacketHeader.ActualLength = (uint)System.Net.IPAddress.NetworkToHostOrder(theBinaryReader.ReadInt32());
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
                            thePacketPayloadLength = thePacketHeader.SavedLength - 12;

                            break;
                        }

                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack:
                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC:
                    default:
                        {
                            thePacketPayloadLength = thePacketHeader.SavedLength;
                            break;
                        }
                }

                // Set up the output parameter for the timestamp based on the timestamp values in seconds and microseconds
                thePacketTimestamp = (double)thePacketHeader.TimestampSeconds + ((double)thePacketHeader.TimestampMicroseconds / 1000000.0);
            }

            return theResult;
        }

        //// Private methods - provide methods specific to PCAP packet captures, not required to derive from the abstract base class

        /// <summary>
        /// Validates the PCAP packet capture global header
        /// </summary>
        /// <param name="theGlobalHeader">The PCAP packet capture global header</param>
        /// <returns>Boolean flag that indicates whether the PCAP packet capture global header is valid</returns>
        private bool ValidateGlobalHeader(Structures.GlobalHeaderStructure theGlobalHeader)
        {
            bool theResult = true;

            //// Validate fields from the PCAP packet capture global header

            if (theGlobalHeader.MagicNumber != Constants.LittleEndianMagicNumber &&
                theGlobalHeader.MagicNumber != Constants.BigEndianMagicNumber)
            {
                this.TheDebugInformation.WriteErrorEvent("The PCAP packet capture global header does not contain the expected magic number, is " +
                    theGlobalHeader.MagicNumber.ToString() +
                    " not " +
                    Constants.LittleEndianMagicNumber.ToString() +
                    " or " +
                    Constants.BigEndianMagicNumber.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.VersionMajor != Constants.ExpectedVersionMajor)
            {
                this.TheDebugInformation.WriteErrorEvent("The PCAP packet capture global header does not contain the expected major version number, is " +
                    theGlobalHeader.VersionMajor.ToString() +
                    " not " +
                    Constants.ExpectedVersionMajor.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.VersionMinor != Constants.ExpectedVersionMinor)
            {
                this.TheDebugInformation.WriteErrorEvent("The PCAP packet capture global header does not contain the expected minor version number, is " +
                    theGlobalHeader.VersionMinor.ToString() +
                    " not " +
                    Constants.ExpectedVersionMinor.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.NetworkDataLinkType != (uint)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack &&
                theGlobalHeader.NetworkDataLinkType != (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet &&
                theGlobalHeader.NetworkDataLinkType != (uint)PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC)
            {
                this.TheDebugInformation.WriteErrorEvent("The PCAP packet capture global header does not contain the expected network data link type, is " +
                    theGlobalHeader.NetworkDataLinkType.ToString() +
                    " not " +
                    PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack.ToString() +
                    " or " +
                    PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet.ToString() +
                    " or " +
                    PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC.ToString() +
                    "!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
