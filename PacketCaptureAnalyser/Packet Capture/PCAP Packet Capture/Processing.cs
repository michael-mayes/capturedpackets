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
        /// 
        /// </summary>
        private bool isTheGlobalHeaderLittleEndian = true;

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

            // Set up the output parameter for the timestamp accuracy - not used for PCAP packet captures so default to zero
            theTimestampAccuracy = 0.0;

            // Create the single instance of the PCAP packet capture global header
            Structures.GlobalHeaderStructure theGlobalHeader =
                new Structures.GlobalHeaderStructure();

            // Read the magic number of the PCAP packet capture global header from the packet capture
            theGlobalHeader.MagicNumber = theBinaryReader.ReadUInt32();

            // The endianism of the remainder of the values in the PCAP packet capture global header will be corrected to little endian if the magic number indicates big endian representation
            if (theGlobalHeader.MagicNumber == Constants.LittleEndianMagicNumber)
            {
                theDebugInformation.WriteInformationEvent("The PCAP packet capture contains the little endian magic number");

                this.isTheGlobalHeaderLittleEndian = true;
            }
            else if (theGlobalHeader.MagicNumber == Constants.BigEndianMagicNumber)
            {
                theDebugInformation.WriteInformationEvent("The PCAP packet capture contains the big endian magic number");

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
                theNetworkDataLinkType = theGlobalHeader.NetworkDataLinkType;
            }

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        /// <param name="theNetworkDataLinkType"></param>
        /// <param name="theTimestampAccuracy">The accuracy of the timestamp read from the packet capture</param>
        /// <param name="thePayloadLength"></param>
        /// <param name="theTimestamp">The timestamp read from the packet capture</param>
        /// <returns></returns>
        protected override bool ProcessPacketHeader(System.IO.BinaryReader theBinaryReader, uint theNetworkDataLinkType, double theTimestampAccuracy, out long thePayloadLength, out double theTimestamp)
        {
            bool theResult = true;

            // Provide a default value to the output parameter for the length of the PCAP packet capture packet payload
            thePayloadLength = 0;

            // Provide a default value to the output parameter for the timestamp
            theTimestamp = 0.0;

            // Create an instance of the PCAP packet capture packet header
            Structures.HeaderStructure thePacketHeader =
                new Structures.HeaderStructure();

            // Populate the PCAP packet capture packet header from the packet capture
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

            // No need to validate fields from the PCAP packet capture packet header
            if (theResult)
            {
                // Set up the output parameter for the length of the PCAP packet capture packet payload
                switch (theNetworkDataLinkType)
                {
                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet:
                        {
                            // Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
                            thePayloadLength = thePacketHeader.SavedLength - 12;

                            break;
                        }

                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack:
                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC:
                    default:
                        {
                            thePayloadLength = thePacketHeader.SavedLength;
                            break;
                        }
                }

                // Set up the output parameter for the timestamp based on the timestamp values in seconds and microseconds
                theTimestamp = (double)thePacketHeader.TimestampSeconds + ((double)thePacketHeader.TimestampMicroseconds / 1000000.0);
            }

            return theResult;
        }

        //// Private methods - provide methods specific to PCAP packet captures, not required to derive from the abstract base class

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theGlobalHeader"></param>
        /// <returns></returns>
        private bool ValidateGlobalHeader(Structures.GlobalHeaderStructure theGlobalHeader)
        {
            bool theResult = true;

            //// Validate fields from the PCAP packet capture global header

            if (theGlobalHeader.MagicNumber != Constants.LittleEndianMagicNumber &&
                theGlobalHeader.MagicNumber != Constants.BigEndianMagicNumber)
            {
                theDebugInformation.WriteErrorEvent("The PCAP packet capture global header does not contain the expected magic number, is " +
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
                theDebugInformation.WriteErrorEvent("The PCAP packet capture global header does not contain the expected major version number, is " +
                    theGlobalHeader.VersionMajor.ToString() +
                    " not " +
                    Constants.ExpectedVersionMajor.ToString() +
                    "!!!");

                theResult = false;
            }

            if (theGlobalHeader.VersionMinor != Constants.ExpectedVersionMinor)
            {
                theDebugInformation.WriteErrorEvent("The PCAP packet capture global header does not contain the expected minor version number, is " +
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
                theDebugInformation.WriteErrorEvent("The PCAP packet capture global header does not contain the expected network data link type, is " +
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
