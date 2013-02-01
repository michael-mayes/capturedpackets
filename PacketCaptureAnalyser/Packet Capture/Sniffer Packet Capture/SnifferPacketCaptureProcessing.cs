//This is free and unencumbered software released into the public domain.

//Anyone is free to copy, modify, publish, use, compile, sell, or
//distribute this software, either in source code form or as a compiled
//binary, for any purpose, commercial or non-commercial, and by any
//means.

//In jurisdictions that recognize copyright laws, the author or authors
//of this software dedicate any and all copyright interest in the
//software to the public domain. We make this dedication for the benefit
//of the public at large and to the detriment of our heirs and
//successors. We intend this dedication to be an overt act of
//relinquishment in perpetuity of all present and future rights to this
//software under copyright law.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.

//For more information, please refer to <http://unlicense.org/>

namespace PacketCaptureProcessingNamespace
{
    class SnifferPackageCaptureProcessing : CommonPacketCaptureProcessing
    {
        //
        //Concrete methods - override abstract methods on the base class
        //

        public override bool ProcessGlobalHeader(System.IO.BinaryReader TheBinaryReader, out System.UInt32 TheNetworkDataLinkType, out double TheTimestampAccuracy)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the network datalink type
            TheNetworkDataLinkType = CommonPackageCaptureConstants.CommonPackageCaptureInvalidNetworkDataLinkType;

            //Provide a default value to the output parameter for the timestamp accuracy
            TheTimestampAccuracy = 0.0;

            //Create the single instance of the Sniffer packet capture global header
            SnifferPackageCaptureStructures.SnifferPackageCaptureGlobalHeaderStructure TheGlobalHeader = new SnifferPackageCaptureStructures.SnifferPackageCaptureGlobalHeaderStructure();

            //Populate the Sniffer packet capture global header from the packet capture
            TheGlobalHeader.MagicNumberHigh = (System.UInt64)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt64());
            TheGlobalHeader.MagicNumberLow = (System.UInt64)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt64());
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
            SnifferPackageCaptureStructures.SnifferPackageCaptureRecordHeaderStructure TheRecordHeader = new SnifferPackageCaptureStructures.SnifferPackageCaptureRecordHeaderStructure();

            //Populate the Sniffer packet capture record header from the packet capture
            TheRecordHeader.RecordType = TheBinaryReader.ReadUInt16();
            TheRecordHeader.RecordLength = TheBinaryReader.ReadUInt32();

            TheResult = ValidateRecordHeader(TheRecordHeader);

            if (TheResult)
            {
                switch (TheRecordHeader.RecordType)
                {
                    case SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderSnifferType2RecordType:
                        {
                            //Create an instance of the Sniffer packet capture Sniffer type 2 data record
                            SnifferPackageCaptureStructures.SnifferPackageCaptureSnifferType2RecordStructure TheType2Record = new SnifferPackageCaptureStructures.SnifferPackageCaptureSnifferType2RecordStructure();

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

                    case SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderSnifferEndOfFileRecordType:
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

        private bool ValidateGlobalHeader(SnifferPackageCaptureStructures.SnifferPackageCaptureGlobalHeaderStructure TheGlobalHeader)
        {
            bool TheResult = true;

            //Validate fields from the Sniffer packet capture global header

            if (TheGlobalHeader.MagicNumberHigh != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberHigh)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected high bytes for the magic number, is " +
                    string.Format("{0:X}", TheGlobalHeader.MagicNumberHigh.ToString()) +
                    " not " +
                    string.Format("{0:X}", SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberHigh.ToString())
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.MagicNumberLow != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberLow)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected low bytes for the magic number, is " +
                    string.Format("{0:X}", TheGlobalHeader.MagicNumberLow.ToString()) +
                    " not " +
                    string.Format("{0:X}", SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberLow.ToString())
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.MagicNumberTerminator != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberTerminator)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected magic number terminating character, is " +
                    TheGlobalHeader.MagicNumberTerminator.ToString() +
                    " not " +
                    SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberTerminator.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.RecordType != SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderSnifferVersionRecordType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected record type, is " +
                    TheGlobalHeader.RecordType.ToString() +
                    " not " +
                    SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderSnifferVersionRecordType.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.VersionMajor != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedVersionMajor)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected major version number, is " +
                    TheGlobalHeader.VersionMajor.ToString() +
                    " not " +
                    SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedVersionMajor.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.VersionMinor != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedVersionMinor)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected minor version number, is " +
                    TheGlobalHeader.VersionMinor.ToString() +
                    " not " +
                    SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedVersionMinor.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.Type != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected record type, is " +
                    TheGlobalHeader.Type.ToString() +
                    " not " +
                    SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedType.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.NetworkEncapsulationType != CommonPackageCaptureConstants.CommonPackageCaptureNullLoopBackNetworkDataLinkType &&
                TheGlobalHeader.NetworkEncapsulationType != CommonPackageCaptureConstants.CommonPackageCaptureEthernetNetworkDataLinkType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected network encapsulation type, is " +
                    TheGlobalHeader.NetworkEncapsulationType.ToString() +
                    " not " +
                    CommonPackageCaptureConstants.CommonPackageCaptureNullLoopBackNetworkDataLinkType.ToString() +
                    " or " +
                    CommonPackageCaptureConstants.CommonPackageCaptureEthernetNetworkDataLinkType.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.FormatVersion != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedFormatVersion)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The Sniffer packet capture global header does not contain the expected format version, is " +
                    TheGlobalHeader.FormatVersion.ToString() +
                    " not " +
                    SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedFormatVersion.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }

        private bool ValidateRecordHeader(SnifferPackageCaptureStructures.SnifferPackageCaptureRecordHeaderStructure TheRecordHeader)
        {
            bool TheResult = true;

            //Validate fields from the Sniffer packet capture record header
            if (TheRecordHeader.RecordType != SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderSnifferType2RecordType &&
                TheRecordHeader.RecordType != SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderSnifferEndOfFileRecordType)
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