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
    class PCAPRecordingProcessing : CommonRecordingProcessing
    {
        //
        //Concrete methods - override abstract methods on the base class
        //

        public override bool ProcessRecordingGlobalHeader(System.IO.BinaryReader TheRecordingBinaryReader)
        {
            bool TheResult = true;

            //Create the single instance of the PCAP recording global header
            PCAPRecordingStructures.PCAPRecordingGlobalHeaderStructure ThePCAPRecordingGlobalHeader = new PCAPRecordingStructures.PCAPRecordingGlobalHeaderStructure();

            //Populate the PCAP recording global header from the recording
            ThePCAPRecordingGlobalHeader.MagicNumber = TheRecordingBinaryReader.ReadUInt32();
            ThePCAPRecordingGlobalHeader.VersionMajor = TheRecordingBinaryReader.ReadUInt16();
            ThePCAPRecordingGlobalHeader.VersionMinor = TheRecordingBinaryReader.ReadUInt16();
            ThePCAPRecordingGlobalHeader.ThisTimeZone = TheRecordingBinaryReader.ReadInt32();
            ThePCAPRecordingGlobalHeader.SignificantFigures = TheRecordingBinaryReader.ReadUInt32();
            ThePCAPRecordingGlobalHeader.SnapshotLength = TheRecordingBinaryReader.ReadUInt32();
            ThePCAPRecordingGlobalHeader.NetworkDataLinkType = TheRecordingBinaryReader.ReadUInt32();

            //Validate fields from the PCAP recording global header
            TheResult = ValidatePCAPRecordingGlobalHeader(ThePCAPRecordingGlobalHeader);

            return TheResult;
        }

        public override bool ProcessRecordingPacketHeader(System.IO.BinaryReader TheRecordingBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the PCAP recording packet header
            PCAPRecordingStructures.PCAPRecordingPacketHeaderStructure ThePCAPRecordingPacketHeader = new PCAPRecordingStructures.PCAPRecordingPacketHeaderStructure();

            //Populate the PCAP recording packet header from the recording
            ThePCAPRecordingPacketHeader.TimestampSeconds = TheRecordingBinaryReader.ReadUInt32();
            ThePCAPRecordingPacketHeader.TimestampMicroseconds = TheRecordingBinaryReader.ReadUInt32();
            ThePCAPRecordingPacketHeader.SavedLength = TheRecordingBinaryReader.ReadUInt32();
            ThePCAPRecordingPacketHeader.ActualLength = TheRecordingBinaryReader.ReadUInt32();

            //No need to validate fields from the PCAP recording packet header

            return TheResult;
        }

        //
        //Private methods - provide methods specific to ENC recordings, not required to derive from the abstract base class
        //

        private bool ValidatePCAPRecordingGlobalHeader(PCAPRecordingStructures.PCAPRecordingGlobalHeaderStructure ThePCAPRecordingGlobalHeader)
        {
            bool TheResult = true;

            //Validate fields from the PCAP recording global header

            if (ThePCAPRecordingGlobalHeader.MagicNumber != PCAPRecordingConstants.PCAPRecordingExpectedMagicNumber)
            {
                System.Diagnostics.Debug.WriteLine("The PCAP recording does not contain the expected magic number, is {0} not {1}", ThePCAPRecordingGlobalHeader.MagicNumber, PCAPRecordingConstants.PCAPRecordingExpectedMagicNumber);

                TheResult = false;
            }

            if (ThePCAPRecordingGlobalHeader.NetworkDataLinkType != PCAPRecordingConstants.PCAPRecordingExpectedNetworkDataLinkType)
            {
                System.Diagnostics.Debug.WriteLine("The PCAP recording does not contain the expected network data link type, is {0} not {1}", ThePCAPRecordingGlobalHeader.NetworkDataLinkType, PCAPRecordingConstants.PCAPRecordingExpectedNetworkDataLinkType);

                TheResult = false;
            }

            return TheResult;
        }
    }
}