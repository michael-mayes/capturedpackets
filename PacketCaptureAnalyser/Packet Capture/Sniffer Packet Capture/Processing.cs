//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.SnifferPackageCapture
{
    class Processing : CommonProcessing
    {
        //
        //Concrete methods - override abstract methods on the base class
        //

        public override bool ProcessGlobalHeader(System.IO.BinaryReader TheBinaryReader, out System.UInt32 TheNetworkDataLinkType, out double TheTimestampAccuracy)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the network datalink type
            TheNetworkDataLinkType = (uint)PacketCapture.CommonConstants.CommonNetworkDataLinkType.Invalid;

            //Provide a default value to the output parameter for the timestamp accuracy
            TheTimestampAccuracy = 0.0;

            //Create the single instance of the Sniffer packet capture global header
            Structures.SnifferPackageCaptureGlobalHeaderStructure TheGlobalHeader =
                new Structures.SnifferPackageCaptureGlobalHeaderStructure();

            //Populate the Sniffer packet capture global header from the packet capture
            TheGlobalHeader.MagicNumberHigh = TheBinaryReader.ReadUInt64();
            TheGlobalHeader.MagicNumberLow = TheBinaryReader.ReadUInt64();
            TheGlobalHeader.MagicNumberTerminator = TheBinaryReader.ReadByte();
            TheGlobalHeader.RecordType = TheBinaryReader.ReadUInt16();
            TheGlobalHeader.RecordLength = TheBinaryReader.ReadUInt32();
            TheGlobalHeader.VersionMajor = TheBinaryReader.ReadInt16();
            TheGlobalHeader.VersionMinor = TheBinaryReader.ReadInt16();
            TheGlobalHeader.Time = TheBinaryReader.ReadInt16();
            TheGlobalHeader.Date = TheBinaryReader.ReadInt16();
            TheGlobalHeader.Type = TheBinaryReader.ReadSByte();
            TheGlobalHeader.NetworkEncapsulationType = TheBinaryReader.ReadByte();
            TheGlobalHeader.FormatVersion = TheBinaryReader.ReadSByte();
            TheGlobalHeader.TimestampUnits = TheBinaryReader.ReadByte();
            TheGlobalHeader.CompressionVersion = TheBinaryReader.ReadSByte();
            TheGlobalHeader.CompressionLevel = TheBinaryReader.ReadSByte();
            TheGlobalHeader.Reserved = TheBinaryReader.ReadInt32();

            //Validate fields from the Sniffer packet capture global header
            TheResult = ValidateGlobalHeader(TheGlobalHeader);

            if (TheResult)
            {
                //Set up the output parameter for the network data link type
                TheNetworkDataLinkType = TheGlobalHeader.NetworkEncapsulationType;

                //Derive the output parameter for the timestamp accuracy (actually a timestamp slice for a Sniffer packet capture) from the timestamp units in the Sniffer packet capture global header
                TheResult = CalculateTimestampAccuracy(TheGlobalHeader.TimestampUnits, out TheTimestampAccuracy);
            }

            return TheResult;
        }

        public override bool ProcessPacketHeader(System.IO.BinaryReader TheBinaryReader, System.UInt32 TheNetworkDataLinkType, double TheTimestampAccuracy, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP packet capture packet payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //Create an instance of the Sniffer packet capture record header
            Structures.SnifferPackageCaptureRecordHeaderStructure TheRecordHeader =
                new Structures.SnifferPackageCaptureRecordHeaderStructure();

            //Populate the Sniffer packet capture record header from the packet capture
            TheRecordHeader.RecordType = TheBinaryReader.ReadUInt16();
            TheRecordHeader.RecordLength = TheBinaryReader.ReadUInt32();

            TheResult = ValidateRecordHeader(TheRecordHeader);

            if (TheResult)
            {
                switch (TheRecordHeader.RecordType)
                {
                    case (ushort)Constants.SnifferPackageCaptureRecordHeaderSnifferRecordType.Type2RecordType:
                        {
                            //Create an instance of the Sniffer packet capture Sniffer type 2 data record
                            Structures.SnifferPackageCaptureSnifferType2RecordStructure TheType2Record =
                                new Structures.SnifferPackageCaptureSnifferType2RecordStructure();

                            //Populate the Sniffer packet capture Sniffer type 2 data record from the packet capture
                            TheType2Record.TimestampLow = TheBinaryReader.ReadUInt16();
                            TheType2Record.TimestampMiddle = TheBinaryReader.ReadUInt16();
                            TheType2Record.TimestampHigh = TheBinaryReader.ReadByte();
                            TheType2Record.TimeDays = TheBinaryReader.ReadByte();
                            TheType2Record.Size = TheBinaryReader.ReadInt16();
                            TheType2Record.FrameErrorStatusBits = TheBinaryReader.ReadByte();
                            TheType2Record.Flags = TheBinaryReader.ReadByte();
                            TheType2Record.TrueSize = TheBinaryReader.ReadInt16();
                            TheType2Record.Reserved = TheBinaryReader.ReadInt16();

                            //Set up the output parameter for the length of the Sniffer packet payload
                            //Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
                            ThePayloadLength = TheType2Record.Size - 12;

                            //Set up the output parameter for the timestamp based on the supplied timestamp slice and the timestamp from the Sniffer type 2 data record
                            //This is the number of seconds passed on this particular day
                            //To match up with a timestamp displayed since epoch would have to get the value of the Date field from the Sniffer packet capture global header
                            TheTimestamp =
                                ((TheTimestampAccuracy * TheType2Record.TimestampHigh * 4294967296) +
                                (TheTimestampAccuracy * TheType2Record.TimestampMiddle * 65536) +
                                (TheTimestampAccuracy * TheType2Record.TimestampLow));

                            break;
                        }

                    case (ushort)Constants.SnifferPackageCaptureRecordHeaderSnifferRecordType.EndOfFileRecordType:
                        {
                            //No further reading required for the Sniffer packet capture Sniffer end of file data record as it only consists of the Sniffer packet capture record header!
                            break;
                        }

                    default:
                        {
                            //We've got an Sniffer packet capture containing an unknown record type

                            //Processing of Sniffer packet captures with record types not enumerated above are obviously not currently supported!

                            System.Diagnostics.Trace.WriteLine
                                (
                                "The Sniffer packet capture contains an unexpected record type of " +
                                TheRecordHeader.RecordType.ToString()
                                );

                            TheResult = false;

                            break;
                        }
                }
            }

            return TheResult;
        }

        //
        //Private methods - provide methods specific to Sniffer packet captures, not required to derive from the abstract base class
        //

        private bool ValidateGlobalHeader(Structures.SnifferPackageCaptureGlobalHeaderStructure TheGlobalHeader)
        {
            bool TheResult = true;

            //Validate fields from the Sniffer packet capture global header

            if (TheGlobalHeader.MagicNumberHigh != Constants.SnifferPackageCaptureExpectedMagicNumberHigh)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected high bytes for the magic number, is " +
                    string.Format("{0:X}", TheGlobalHeader.MagicNumberHigh.ToString()) +
                    " not " +
                    string.Format("{0:X}", Constants.SnifferPackageCaptureExpectedMagicNumberHigh.ToString())
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.MagicNumberLow != Constants.SnifferPackageCaptureExpectedMagicNumberLow)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected low bytes for the magic number, is " +
                    string.Format("{0:X}", TheGlobalHeader.MagicNumberLow.ToString()) +
                    " not " +
                    string.Format("{0:X}", Constants.SnifferPackageCaptureExpectedMagicNumberLow.ToString())
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.MagicNumberTerminator != Constants.SnifferPackageCaptureExpectedMagicNumberTerminator)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected magic number terminating character, is " +
                    TheGlobalHeader.MagicNumberTerminator.ToString() +
                    " not " +
                    Constants.SnifferPackageCaptureExpectedMagicNumberTerminator.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.RecordType != (ushort)Constants.SnifferPackageCaptureRecordHeaderSnifferRecordType.VersionRecordType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected record type, is " +
                    TheGlobalHeader.RecordType.ToString() +
                    " not " +
                    Constants.SnifferPackageCaptureRecordHeaderSnifferRecordType.VersionRecordType.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.VersionMajor != Constants.SnifferPackageCaptureExpectedVersionMajor)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected major version number, is " +
                    TheGlobalHeader.VersionMajor.ToString() +
                    " not " +
                    Constants.SnifferPackageCaptureExpectedVersionMajor.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.VersionMinor != Constants.SnifferPackageCaptureExpectedVersionMinor)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected minor version number, is " +
                    TheGlobalHeader.VersionMinor.ToString() +
                    " not " +
                    Constants.SnifferPackageCaptureExpectedVersionMinor.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.Type != Constants.SnifferPackageCaptureExpectedType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected record type, is " +
                    TheGlobalHeader.Type.ToString() +
                    " not " +
                    Constants.SnifferPackageCaptureExpectedType.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.NetworkEncapsulationType != (uint)PacketCapture.CommonConstants.CommonNetworkDataLinkType.NullLoopBack &&
                TheGlobalHeader.NetworkEncapsulationType != (uint)PacketCapture.CommonConstants.CommonNetworkDataLinkType.Ethernet)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected network encapsulation type, is " +
                    TheGlobalHeader.NetworkEncapsulationType.ToString() +
                    " not " +
                    PacketCapture.CommonConstants.CommonNetworkDataLinkType.NullLoopBack.ToString() +
                    " or " +
                    PacketCapture.CommonConstants.CommonNetworkDataLinkType.Ethernet.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.FormatVersion != Constants.SnifferPackageCaptureExpectedFormatVersion)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected format version, is " +
                    TheGlobalHeader.FormatVersion.ToString() +
                    " not " +
                    Constants.SnifferPackageCaptureExpectedFormatVersion.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }

        private bool ValidateRecordHeader(Structures.SnifferPackageCaptureRecordHeaderStructure TheRecordHeader)
        {
            bool TheResult = true;

            //Validate fields from the Sniffer packet capture record header
            if (TheRecordHeader.RecordType != (ushort)Constants.SnifferPackageCaptureRecordHeaderSnifferRecordType.Type2RecordType &&
                TheRecordHeader.RecordType != (ushort)Constants.SnifferPackageCaptureRecordHeaderSnifferRecordType.EndOfFileRecordType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture record header does not contain the expected type, is " +
                    TheRecordHeader.RecordType.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }

        private bool CalculateTimestampAccuracy(System.Byte TheTimestampUnits, out double TheTimestampAccuracy)
        {
            bool TheResult = true;

            switch (TheTimestampUnits)
            {
                //15.0 microseconds
                case 0:
                    {
                        TheTimestampAccuracy = 0.000015;
                        break;
                    }

                //0.838096 microseconds
                case 1:
                    {
                        TheTimestampAccuracy = 0.000000838096;
                        break;
                    }

                //15.0 microseconds
                case 2:
                    {
                        TheTimestampAccuracy = 0.000015;
                        break;
                    }

                //0.5 microseconds
                case 3:
                    {
                        TheTimestampAccuracy = 0.0000005;
                        break;
                    }

                //2.0 microseconds
                case 4:
                    {
                        TheTimestampAccuracy = 0.000002;
                        break;
                    }

                //0.08 microseconds
                case 5:
                    {
                        TheTimestampAccuracy = 0.00000008;
                        break;
                    }

                //0.1 microseconds
                case 6:
                    {
                        TheTimestampAccuracy = 0.0000001;
                        break;
                    }

                default:
                    {
                        System.Diagnostics.Trace.WriteLine
                            (
                            "The Sniffer packet capture contains an unexpected timestamp unit " +
                            TheTimestampUnits.ToString() +
                            "!!!"
                            );

                        TheTimestampAccuracy = 0.0;

                        TheResult = false;

                        break;
                    }
            }

            return TheResult;
        }
    }
}