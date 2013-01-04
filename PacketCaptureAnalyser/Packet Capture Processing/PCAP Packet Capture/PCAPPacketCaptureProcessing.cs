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
    class PCAPPackageCaptureProcessing : CommonPacketCaptureProcessing
    {
        //
        //Concrete methods - override abstract methods on the base class
        //

        public override bool ProcessGlobalHeader(System.IO.BinaryReader TheBinaryReader, out double TheTimestampAccuracy)
        {
            bool TheResult = true;

            //Create the single instance of the PCAP packet capture global header
            PCAPPackageCaptureStructures.PCAPPackageCaptureGlobalHeaderStructure TheGlobalHeader = new PCAPPackageCaptureStructures.PCAPPackageCaptureGlobalHeaderStructure();

            //Populate the PCAP packet capture global header from the packet capture
            TheGlobalHeader.MagicNumber = TheBinaryReader.ReadUInt32();
            TheGlobalHeader.VersionMajor = TheBinaryReader.ReadUInt16();
            TheGlobalHeader.VersionMinor = TheBinaryReader.ReadUInt16();
            TheGlobalHeader.ThisTimeZone = TheBinaryReader.ReadInt32();
            TheGlobalHeader.SignificantFigures = TheBinaryReader.ReadUInt32();
            TheGlobalHeader.SnapshotLength = TheBinaryReader.ReadUInt32();
            TheGlobalHeader.NetworkDataLinkType = TheBinaryReader.ReadUInt32();

            //Validate fields from the PCAP packet capture global header
            TheResult = ValidateGlobalHeader(TheGlobalHeader);

            //Set up the output parameter for the timestamp accuracy - not used for PCAP packet captures so default to zero
            TheTimestampAccuracy = 0.0;

            return TheResult;
        }

        public override bool ProcessPacketHeader(System.IO.BinaryReader TheBinaryReader, double TheTimestampAccuracy, out double TheTimestamp)
        {
            bool TheResult = true;

            //Create an instance of the PCAP packet capture packet header
            PCAPPackageCaptureStructures.PCAPPackageCapturePacketHeaderStructure ThePacketHeader = new PCAPPackageCaptureStructures.PCAPPackageCapturePacketHeaderStructure();

            //Populate the PCAP packet capture packet header from the packet capture
            ThePacketHeader.TimestampSeconds = TheBinaryReader.ReadUInt32();
            ThePacketHeader.TimestampMicroseconds = TheBinaryReader.ReadUInt32();
            ThePacketHeader.SavedLength = TheBinaryReader.ReadUInt32();
            ThePacketHeader.ActualLength = TheBinaryReader.ReadUInt32();

            //Set up the output parameter for the timestamp based on the timestamp values in seconds and microseconds
            TheTimestamp = (double)ThePacketHeader.TimestampSeconds + ((double)(ThePacketHeader.TimestampMicroseconds) / 1000000.0);

            //No need to validate fields from the PCAP packet capture packet header

            return TheResult;
        }

        //
        //Private methods - provide methods specific to Sniffer packet captures, not required to derive from the abstract base class
        //

        private bool ValidateGlobalHeader(PCAPPackageCaptureStructures.PCAPPackageCaptureGlobalHeaderStructure TheGlobalHeader)
        {
            bool TheResult = true;

            //Validate fields from the PCAP packet capture global header

            if (TheGlobalHeader.MagicNumber != PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedMagicNumber)
            {
                System.Diagnostics.Debug.WriteLine("The PCAP packet capture does not contain the expected magic number, is {0} not {1}", TheGlobalHeader.MagicNumber, PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedMagicNumber);

                TheResult = false;
            }

            if (TheGlobalHeader.NetworkDataLinkType != PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedNetworkDataLinkType)
            {
                System.Diagnostics.Debug.WriteLine("The PCAP packet capture does not contain the expected network data link type, is {0} not {1}", TheGlobalHeader.NetworkDataLinkType, PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedNetworkDataLinkType);

                TheResult = false;
            }

            return TheResult;
        }
    }
}