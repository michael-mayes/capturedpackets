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

namespace RecordingProcessingNamespace
{
    class ENCRecordingProcessing : CommonRecordingProcessing
    {
        //
        //Concrete methods - override abstract methods on the base class
        //

        public override bool ProcessRecordingGlobalHeader(System.IO.BinaryReader TheRecordingBinaryReader)
        {
            bool TheResult = true;

            //Create the single instance of the ENC recording global header
            ENCRecordingStructures.ENCRecordingGlobalHeaderStructure TheENCRecordingGlobalHeader = new ENCRecordingStructures.ENCRecordingGlobalHeaderStructure();

            //Populate the ENC recording global header from the recording
            TheENCRecordingGlobalHeader.MagicNumberHigh = (System.UInt64)System.Net.IPAddress.NetworkToHostOrder(TheRecordingBinaryReader.ReadInt64());
            TheENCRecordingGlobalHeader.MagicNumberLow = (System.UInt64)System.Net.IPAddress.NetworkToHostOrder(TheRecordingBinaryReader.ReadInt64());
            TheENCRecordingGlobalHeader.MagicNumberTerminator = TheRecordingBinaryReader.ReadByte();
            TheENCRecordingGlobalHeader.RecordType = TheRecordingBinaryReader.ReadInt16();
            TheENCRecordingGlobalHeader.RecordLength = TheRecordingBinaryReader.ReadInt32();
            TheENCRecordingGlobalHeader.VersionMajor = TheRecordingBinaryReader.ReadInt16();
            TheENCRecordingGlobalHeader.VersionMinor = TheRecordingBinaryReader.ReadInt16();
            TheENCRecordingGlobalHeader.Time = TheRecordingBinaryReader.ReadInt16();
            TheENCRecordingGlobalHeader.Date = TheRecordingBinaryReader.ReadInt16();
            TheENCRecordingGlobalHeader.Type = TheRecordingBinaryReader.ReadSByte();
            TheENCRecordingGlobalHeader.NetworkEncapsulationType = TheRecordingBinaryReader.ReadByte();
            TheENCRecordingGlobalHeader.FormatVersion = TheRecordingBinaryReader.ReadSByte();
            TheENCRecordingGlobalHeader.TimestampUnits = TheRecordingBinaryReader.ReadByte();
            TheENCRecordingGlobalHeader.CompressionVersion = TheRecordingBinaryReader.ReadSByte();
            TheENCRecordingGlobalHeader.CompressionLevel = TheRecordingBinaryReader.ReadSByte();
            TheENCRecordingGlobalHeader.Reserved = TheRecordingBinaryReader.ReadInt32();

            //Validate fields from the ENC recording global header
            TheResult = ValidateENCRecordingGlobalHeader(TheENCRecordingGlobalHeader);

            return TheResult;
        }

        public override bool ProcessRecordingPacketHeader(System.IO.BinaryReader TheRecordingBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the ENC recording record header
            ENCRecordingStructures.ENCRecordingRecordHeaderStructure TheENCRecordingRecordHeader = new ENCRecordingStructures.ENCRecordingRecordHeaderStructure();

            //Populate the ENC recording record header from the recording
            TheENCRecordingRecordHeader.RecordType = TheRecordingBinaryReader.ReadUInt16();
            TheENCRecordingRecordHeader.RecordLength = TheRecordingBinaryReader.ReadUInt32();

            TheResult = ValidateENCRecordingRecordHeader(TheENCRecordingRecordHeader);

            if (TheResult)
            {
                switch (TheENCRecordingRecordHeader.RecordType)
                {
                    case ENCRecordingConstants.ENCRecordingRecordHeaderSnifferType2RecordType:
                        {
                            //Create an instance of the ENC recording Sniffer type 2 data record
                            ENCRecordingStructures.ENCRecordingSnifferType2RecordStructure TheENCRecordingSnifferType2Record = new ENCRecordingStructures.ENCRecordingSnifferType2RecordStructure();

                            //Populate the ENC recording Sniffer type 2 data record from the recording
                            TheENCRecordingSnifferType2Record.TimestampLow = TheRecordingBinaryReader.ReadUInt16();
                            TheENCRecordingSnifferType2Record.TimestampMiddle = TheRecordingBinaryReader.ReadUInt16();
                            TheENCRecordingSnifferType2Record.TimestampHigh = TheRecordingBinaryReader.ReadByte();
                            TheENCRecordingSnifferType2Record.TimeDays = TheRecordingBinaryReader.ReadByte();
                            TheENCRecordingSnifferType2Record.Size = TheRecordingBinaryReader.ReadInt16();
                            TheENCRecordingSnifferType2Record.FrameErrorStatusBits = TheRecordingBinaryReader.ReadByte();
                            TheENCRecordingSnifferType2Record.Flags = TheRecordingBinaryReader.ReadByte();
                            TheENCRecordingSnifferType2Record.TrueSize = TheRecordingBinaryReader.ReadInt16();
                            TheENCRecordingSnifferType2Record.Reserved = TheRecordingBinaryReader.ReadInt16();
                            break;
                        }

                    case ENCRecordingConstants.ENCRecordingRecordHeaderSnifferEndOfFileRecordType:
                        {
                            //No further reading required for the ENC recording Sniffer end of file data record as it only consists of the ENC recording record header!
                            break;
                        }

                    default:
                        {
                            //We've got an ENC recording containing an unknown record type

                            //Processing of ENC recordings with record types not enumerated above are obviously not currently supported!

                            System.Diagnostics.Debug.WriteLine("The ENC recording contains an unexpected record type of {0}", TheENCRecordingRecordHeader.RecordType);

                            TheResult = false;

                            break;
                        }
                }
            }

            return TheResult;
        }

        //
        //Private methods - provide methods specific to ENC recordings, not required to derive from the abstract base class
        //

        private bool ValidateENCRecordingGlobalHeader(ENCRecordingStructures.ENCRecordingGlobalHeaderStructure TheENCRecordingGlobalHeader)
        {
            bool TheResult = true;

            //Validate fields from the ENC recording global header

            if (TheENCRecordingGlobalHeader.MagicNumberHigh != ENCRecordingConstants.ENCRecordingExpectedMagicNumberHigh)
            {
                System.Diagnostics.Debug.WriteLine("The ENC recording does not contain the expected high bytes for the magic number, is {0:X} not {1:X}", TheENCRecordingGlobalHeader.MagicNumberHigh, ENCRecordingConstants.ENCRecordingExpectedMagicNumberHigh);

                TheResult = false;
            }

            if (TheENCRecordingGlobalHeader.MagicNumberLow != ENCRecordingConstants.ENCRecordingExpectedMagicNumberLow)
            {
                System.Diagnostics.Debug.WriteLine("The ENC recording does not contain the expected low bytes for the magic number, is {0:X} not {1:X}", TheENCRecordingGlobalHeader.MagicNumberLow, ENCRecordingConstants.ENCRecordingExpectedMagicNumberLow);

                TheResult = false;
            }

            if (TheENCRecordingGlobalHeader.MagicNumberTerminator != ENCRecordingConstants.ENCRecordingExpectedMagicNumberTerminator)
            {
                System.Diagnostics.Debug.WriteLine("The ENC recording does not contain the expected magic number terminating character, is {0} not {1}", TheENCRecordingGlobalHeader.MagicNumberTerminator, ENCRecordingConstants.ENCRecordingExpectedMagicNumberTerminator);

                TheResult = false;
            }

            if (TheENCRecordingGlobalHeader.VersionMajor != ENCRecordingConstants.ENCRecordingExpectedVersionMajor)
            {
                System.Diagnostics.Debug.WriteLine("The ENC recording does not contain the expected major version number, is {0} not {1}", TheENCRecordingGlobalHeader.VersionMajor, ENCRecordingConstants.ENCRecordingExpectedVersionMajor);

                TheResult = false;
            }

            if (TheENCRecordingGlobalHeader.VersionMinor != ENCRecordingConstants.ENCRecordingExpectedVersionMinor)
            {
                System.Diagnostics.Debug.WriteLine("The ENC recording does not contain the expected minor version number, is {0} not {1}", TheENCRecordingGlobalHeader.VersionMinor, ENCRecordingConstants.ENCRecordingExpectedVersionMinor);

                TheResult = false;
            }

            if (TheENCRecordingGlobalHeader.Type != ENCRecordingConstants.ENCRecordingExpectedType)
            {
                System.Diagnostics.Debug.WriteLine("The ENC recording does not contain the expected record type, is {0} not {1}", TheENCRecordingGlobalHeader.Type, ENCRecordingConstants.ENCRecordingExpectedType);

                TheResult = false;
            }

            if (TheENCRecordingGlobalHeader.NetworkEncapsulationType != ENCRecordingConstants.ENCRecordingExpectedNetworkEncapsulationType)
            {
                System.Diagnostics.Debug.WriteLine("The ENC recording does not contain the expected network encapsulation type, is {0} not {1}", TheENCRecordingGlobalHeader.NetworkEncapsulationType, ENCRecordingConstants.ENCRecordingExpectedNetworkEncapsulationType);

                TheResult = false;
            }

            if (TheENCRecordingGlobalHeader.FormatVersion != ENCRecordingConstants.ENCRecordingExpectedFormatVersion)
            {
                System.Diagnostics.Debug.WriteLine("The ENC recording does not contain the expected format version, is {0} not {1}", TheENCRecordingGlobalHeader.FormatVersion, ENCRecordingConstants.ENCRecordingExpectedFormatVersion);

                TheResult = false;
            }

            return TheResult;
        }

        private bool ValidateENCRecordingRecordHeader(ENCRecordingStructures.ENCRecordingRecordHeaderStructure TheENCRecordingRecordHeader)
        {
            bool TheResult = true;

            //Validate fields from the ENC recording record header
            if (TheENCRecordingRecordHeader.RecordType != ENCRecordingConstants.ENCRecordingRecordHeaderSnifferType2RecordType &&
                TheENCRecordingRecordHeader.RecordType != ENCRecordingConstants.ENCRecordingRecordHeaderSnifferEndOfFileRecordType)
            {
                System.Diagnostics.Debug.WriteLine("The ENC recording does not contain the expected record type, is {0}", TheENCRecordingRecordHeader.RecordType);

                TheResult = false;
            }

            return TheResult;
        }
    }
}