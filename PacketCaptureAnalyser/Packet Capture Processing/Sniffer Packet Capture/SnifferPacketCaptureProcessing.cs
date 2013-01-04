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

        public override bool ProcessPacketCaptureGlobalHeader(System.IO.BinaryReader TheBinaryReader)
        {
            bool TheResult = true;

            //Create the single instance of the Sniffer packet capture global header
            SnifferPackageCaptureStructures.SnifferPackageCaptureGlobalHeaderStructure TheSnifferPackageCaptureGlobalHeader = new SnifferPackageCaptureStructures.SnifferPackageCaptureGlobalHeaderStructure();

            //Populate the Sniffer packet capture global header from the packet capture
            TheSnifferPackageCaptureGlobalHeader.MagicNumberHigh = (System.UInt64)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt64());
            TheSnifferPackageCaptureGlobalHeader.MagicNumberLow = (System.UInt64)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt64());
            TheSnifferPackageCaptureGlobalHeader.MagicNumberTerminator = TheBinaryReader.ReadByte();
            TheSnifferPackageCaptureGlobalHeader.RecordType = TheBinaryReader.ReadInt16();
            TheSnifferPackageCaptureGlobalHeader.RecordLength = TheBinaryReader.ReadInt32();
            TheSnifferPackageCaptureGlobalHeader.VersionMajor = TheBinaryReader.ReadInt16();
            TheSnifferPackageCaptureGlobalHeader.VersionMinor = TheBinaryReader.ReadInt16();
            TheSnifferPackageCaptureGlobalHeader.Time = TheBinaryReader.ReadInt16();
            TheSnifferPackageCaptureGlobalHeader.Date = TheBinaryReader.ReadInt16();
            TheSnifferPackageCaptureGlobalHeader.Type = TheBinaryReader.ReadSByte();
            TheSnifferPackageCaptureGlobalHeader.NetworkEncapsulationType = TheBinaryReader.ReadByte();
            TheSnifferPackageCaptureGlobalHeader.FormatVersion = TheBinaryReader.ReadSByte();
            TheSnifferPackageCaptureGlobalHeader.TimestampUnits = TheBinaryReader.ReadByte();
            TheSnifferPackageCaptureGlobalHeader.CompressionVersion = TheBinaryReader.ReadSByte();
            TheSnifferPackageCaptureGlobalHeader.CompressionLevel = TheBinaryReader.ReadSByte();
            TheSnifferPackageCaptureGlobalHeader.Reserved = TheBinaryReader.ReadInt32();

            //Validate fields from the Sniffer packet capture global header
            TheResult = ValidateSnifferPackageCaptureGlobalHeader(TheSnifferPackageCaptureGlobalHeader);

            return TheResult;
        }

        public override bool ProcessPacketCapturePacketHeader(System.IO.BinaryReader TheBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the Sniffer packet capture record header
            SnifferPackageCaptureStructures.SnifferPackageCaptureRecordHeaderStructure TheSnifferPackageCaptureRecordHeader = new SnifferPackageCaptureStructures.SnifferPackageCaptureRecordHeaderStructure();

            //Populate the Sniffer packet capture record header from the packet capture
            TheSnifferPackageCaptureRecordHeader.RecordType = TheBinaryReader.ReadUInt16();
            TheSnifferPackageCaptureRecordHeader.RecordLength = TheBinaryReader.ReadUInt32();

            TheResult = ValidateSnifferPackageCaptureRecordHeader(TheSnifferPackageCaptureRecordHeader);

            if (TheResult)
            {
                switch (TheSnifferPackageCaptureRecordHeader.RecordType)
                {
                    case SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderSnifferType2RecordType:
                        {
                            //Create an instance of the Sniffer packet capture Sniffer type 2 data record
                            SnifferPackageCaptureStructures.SnifferPackageCaptureSnifferType2RecordStructure TheSnifferPackageCaptureSnifferType2Record = new SnifferPackageCaptureStructures.SnifferPackageCaptureSnifferType2RecordStructure();

                            //Populate the Sniffer packet capture Sniffer type 2 data record from the packet capture
                            TheSnifferPackageCaptureSnifferType2Record.TimestampLow = TheBinaryReader.ReadUInt16();
                            TheSnifferPackageCaptureSnifferType2Record.TimestampMiddle = TheBinaryReader.ReadUInt16();
                            TheSnifferPackageCaptureSnifferType2Record.TimestampHigh = TheBinaryReader.ReadByte();
                            TheSnifferPackageCaptureSnifferType2Record.TimeDays = TheBinaryReader.ReadByte();
                            TheSnifferPackageCaptureSnifferType2Record.Size = TheBinaryReader.ReadInt16();
                            TheSnifferPackageCaptureSnifferType2Record.FrameErrorStatusBits = TheBinaryReader.ReadByte();
                            TheSnifferPackageCaptureSnifferType2Record.Flags = TheBinaryReader.ReadByte();
                            TheSnifferPackageCaptureSnifferType2Record.TrueSize = TheBinaryReader.ReadInt16();
                            TheSnifferPackageCaptureSnifferType2Record.Reserved = TheBinaryReader.ReadInt16();
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

                            System.Diagnostics.Debug.WriteLine("The Sniffer packet capture contains an unexpected record type of {0}", TheSnifferPackageCaptureRecordHeader.RecordType);

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

        private bool ValidateSnifferPackageCaptureGlobalHeader(SnifferPackageCaptureStructures.SnifferPackageCaptureGlobalHeaderStructure TheSnifferPackageCaptureGlobalHeader)
        {
            bool TheResult = true;

            //Validate fields from the Sniffer packet capture global header

            if (TheSnifferPackageCaptureGlobalHeader.MagicNumberHigh != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberHigh)
            {
                System.Diagnostics.Debug.WriteLine("The Sniffer packet capture does not contain the expected high bytes for the magic number, is {0:X} not {1:X}", TheSnifferPackageCaptureGlobalHeader.MagicNumberHigh, SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberHigh);

                TheResult = false;
            }

            if (TheSnifferPackageCaptureGlobalHeader.MagicNumberLow != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberLow)
            {
                System.Diagnostics.Debug.WriteLine("The Sniffer packet capture does not contain the expected low bytes for the magic number, is {0:X} not {1:X}", TheSnifferPackageCaptureGlobalHeader.MagicNumberLow, SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberLow);

                TheResult = false;
            }

            if (TheSnifferPackageCaptureGlobalHeader.MagicNumberTerminator != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberTerminator)
            {
                System.Diagnostics.Debug.WriteLine("The Sniffer packet capture does not contain the expected magic number terminating character, is {0} not {1}", TheSnifferPackageCaptureGlobalHeader.MagicNumberTerminator, SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedMagicNumberTerminator);

                TheResult = false;
            }

            if (TheSnifferPackageCaptureGlobalHeader.VersionMajor != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedVersionMajor)
            {
                System.Diagnostics.Debug.WriteLine("The Sniffer packet capture does not contain the expected major version number, is {0} not {1}", TheSnifferPackageCaptureGlobalHeader.VersionMajor, SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedVersionMajor);

                TheResult = false;
            }

            if (TheSnifferPackageCaptureGlobalHeader.VersionMinor != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedVersionMinor)
            {
                System.Diagnostics.Debug.WriteLine("The Sniffer packet capture does not contain the expected minor version number, is {0} not {1}", TheSnifferPackageCaptureGlobalHeader.VersionMinor, SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedVersionMinor);

                TheResult = false;
            }

            if (TheSnifferPackageCaptureGlobalHeader.Type != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedType)
            {
                System.Diagnostics.Debug.WriteLine("The Sniffer packet capture does not contain the expected record type, is {0} not {1}", TheSnifferPackageCaptureGlobalHeader.Type, SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedType);

                TheResult = false;
            }

            if (TheSnifferPackageCaptureGlobalHeader.NetworkEncapsulationType != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedNetworkEncapsulationType)
            {
                System.Diagnostics.Debug.WriteLine("The Sniffer packet capture does not contain the expected network encapsulation type, is {0} not {1}", TheSnifferPackageCaptureGlobalHeader.NetworkEncapsulationType, SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedNetworkEncapsulationType);

                TheResult = false;
            }

            if (TheSnifferPackageCaptureGlobalHeader.FormatVersion != SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedFormatVersion)
            {
                System.Diagnostics.Debug.WriteLine("The Sniffer packet capture does not contain the expected format version, is {0} not {1}", TheSnifferPackageCaptureGlobalHeader.FormatVersion, SnifferPackageCaptureConstants.SnifferPackageCaptureExpectedFormatVersion);

                TheResult = false;
            }

            return TheResult;
        }

        private bool ValidateSnifferPackageCaptureRecordHeader(SnifferPackageCaptureStructures.SnifferPackageCaptureRecordHeaderStructure TheSnifferPackageCaptureRecordHeader)
        {
            bool TheResult = true;

            //Validate fields from the Sniffer packet capture record header
            if (TheSnifferPackageCaptureRecordHeader.RecordType != SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderSnifferType2RecordType &&
                TheSnifferPackageCaptureRecordHeader.RecordType != SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderSnifferEndOfFileRecordType)
            {
                System.Diagnostics.Debug.WriteLine("The Sniffer packet capture does not contain the expected record type, is {0}", TheSnifferPackageCaptureRecordHeader.RecordType);

                TheResult = false;
            }

            return TheResult;
        }
    }
}