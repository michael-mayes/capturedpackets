//$Id$
//$URL$

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.PCAPPackageCapture
{
    class Processing : CommonProcessing
    {
        private bool IsTheGlobalHeaderLittleEndian = true;

        //
        //Concrete methods - override abstract methods on the base class
        //

        public override bool ProcessGlobalHeader(Analysis.DebugInformation TheDebugInformation, System.IO.BinaryReader TheBinaryReader, out System.UInt32 TheNetworkDataLinkType, out double TheTimestampAccuracy)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the network datalink type
            TheNetworkDataLinkType = (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Invalid;

            //Set up the output parameter for the timestamp accuracy - not used for PCAP packet captures so default to zero
            TheTimestampAccuracy = 0.0;

            //Create the single instance of the PCAP packet capture global header
            Structures.GlobalHeaderStructure TheGlobalHeader =
                new Structures.GlobalHeaderStructure();

            //Read the magic number of the PCAP packet capture global header from the packet capture
            TheGlobalHeader.MagicNumber = TheBinaryReader.ReadUInt32();

            //The endianism of the remainder of the values in the PCAP packet capture global header will be corrected to little endian if the magic number indicates big endian representation
            if (TheGlobalHeader.MagicNumber == Constants.LittleEndianMagicNumber)
            {
                TheDebugInformation.WriteInformationEvent
                    (
                    "The PCAP packet capture contains the little endian magic number"
                    );

                IsTheGlobalHeaderLittleEndian = true;
            }
            else if (TheGlobalHeader.MagicNumber == Constants.BigEndianMagicNumber)
            {
                TheDebugInformation.WriteInformationEvent
                    (
                    "The PCAP packet capture contains the big endian magic number"
                    );

                IsTheGlobalHeaderLittleEndian = false;
            }

            //Read the remainder of the PCAP packet capture global header from the packet capture
            if (IsTheGlobalHeaderLittleEndian)
            {
                TheGlobalHeader.VersionMajor = TheBinaryReader.ReadUInt16();
                TheGlobalHeader.VersionMinor = TheBinaryReader.ReadUInt16();
                TheGlobalHeader.ThisTimeZone = TheBinaryReader.ReadInt32();
                TheGlobalHeader.SignificantFigures = TheBinaryReader.ReadUInt32();
                TheGlobalHeader.SnapshotLength = TheBinaryReader.ReadUInt32();
                TheGlobalHeader.NetworkDataLinkType = TheBinaryReader.ReadUInt32();
            }
            else
            {
                TheGlobalHeader.VersionMajor = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
                TheGlobalHeader.VersionMinor = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
                TheGlobalHeader.ThisTimeZone = System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
                TheGlobalHeader.SignificantFigures = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
                TheGlobalHeader.SnapshotLength = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
                TheGlobalHeader.NetworkDataLinkType = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
            }

            //Validate fields from the PCAP packet capture global header
            TheResult = ValidateGlobalHeader(TheDebugInformation, TheGlobalHeader);

            if (TheResult)
            {
                //Set up the output parameter for the network data link type
                TheNetworkDataLinkType = TheGlobalHeader.NetworkDataLinkType;
            }

            return TheResult;
        }

        public override bool ProcessPacketHeader(Analysis.DebugInformation TheDebugInformation, System.IO.BinaryReader TheBinaryReader, System.UInt32 TheNetworkDataLinkType, double TheTimestampAccuracy, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP packet capture packet payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //Create an instance of the PCAP packet capture packet header
            Structures.HeaderStructure ThePacketHeader =
                new Structures.HeaderStructure();

            //Populate the PCAP packet capture packet header from the packet capture
            if (IsTheGlobalHeaderLittleEndian)
            {
                ThePacketHeader.TimestampSeconds = TheBinaryReader.ReadUInt32();
                ThePacketHeader.TimestampMicroseconds = TheBinaryReader.ReadUInt32();
                ThePacketHeader.SavedLength = TheBinaryReader.ReadUInt32();
                ThePacketHeader.ActualLength = TheBinaryReader.ReadUInt32();
            }
            else
            {
                ThePacketHeader.TimestampSeconds = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
                ThePacketHeader.TimestampMicroseconds = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
                ThePacketHeader.SavedLength = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
                ThePacketHeader.ActualLength = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
            }

            //No need to validate fields from the PCAP packet capture packet header
            if (TheResult)
            {
                //Set up the output parameter for the length of the PCAP packet capture packet payload

                switch (TheNetworkDataLinkType)
                {
                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet:
                        {
                            //Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
                            ThePayloadLength = ThePacketHeader.SavedLength - 12;

                            break;
                        }

                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack:
                    case (uint)PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC:
                    default:
                        {
                            ThePayloadLength = ThePacketHeader.SavedLength;
                            break;
                        }
                }

                //Set up the output parameter for the timestamp based on the timestamp values in seconds and microseconds
                TheTimestamp = (double)ThePacketHeader.TimestampSeconds + ((double)(ThePacketHeader.TimestampMicroseconds) / 1000000.0);
            }

            return TheResult;
        }

        //
        //Private methods - provide methods specific to PCAP packet captures, not required to derive from the abstract base class
        //

        private bool ValidateGlobalHeader(Analysis.DebugInformation TheDebugInformation, Structures.GlobalHeaderStructure TheGlobalHeader)
        {
            bool TheResult = true;

            //Validate fields from the PCAP packet capture global header

            if (TheGlobalHeader.MagicNumber != Constants.LittleEndianMagicNumber &&
                TheGlobalHeader.MagicNumber != Constants.BigEndianMagicNumber)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The PCAP packet capture global header does not contain the expected magic number, is " +
                    TheGlobalHeader.MagicNumber.ToString() +
                    " not " +
                    Constants.LittleEndianMagicNumber.ToString() +
                    " or " +
                    Constants.BigEndianMagicNumber.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.VersionMajor != Constants.ExpectedVersionMajor)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The PCAP packet capture global header does not contain the expected major version number, is " +
                    TheGlobalHeader.VersionMajor.ToString() +
                    " not " +
                    Constants.ExpectedVersionMajor.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.VersionMinor != Constants.ExpectedVersionMinor)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The PCAP packet capture global header does not contain the expected minor version number, is " +
                    TheGlobalHeader.VersionMinor.ToString() +
                    " not " +
                    Constants.ExpectedVersionMinor.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.NetworkDataLinkType != (uint)PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack &&
                TheGlobalHeader.NetworkDataLinkType != (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet &&
                TheGlobalHeader.NetworkDataLinkType != (uint)PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The PCAP packet capture global header does not contain the expected network data link type, is " +
                    TheGlobalHeader.NetworkDataLinkType.ToString() +
                    " not " +
                    PacketCapture.CommonConstants.NetworkDataLinkType.NullLoopBack.ToString() +
                    " or " +
                    PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet.ToString() +
                    " or " +
                    PacketCapture.CommonConstants.NetworkDataLinkType.CiscoHDLC.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}
