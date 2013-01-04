﻿//This is free and unencumbered software released into the public domain.

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

        public override bool ProcessPacketCaptureGlobalHeader(System.IO.BinaryReader TheBinaryReader)
        {
            bool TheResult = true;

            //Create the single instance of the PCAP packet capture global header
            PCAPPackageCaptureStructures.PCAPPackageCaptureGlobalHeaderStructure ThePCAPPackageCaptureGlobalHeader = new PCAPPackageCaptureStructures.PCAPPackageCaptureGlobalHeaderStructure();

            //Populate the PCAP packet capture global header from the packet capture
            ThePCAPPackageCaptureGlobalHeader.MagicNumber = TheBinaryReader.ReadUInt32();
            ThePCAPPackageCaptureGlobalHeader.VersionMajor = TheBinaryReader.ReadUInt16();
            ThePCAPPackageCaptureGlobalHeader.VersionMinor = TheBinaryReader.ReadUInt16();
            ThePCAPPackageCaptureGlobalHeader.ThisTimeZone = TheBinaryReader.ReadInt32();
            ThePCAPPackageCaptureGlobalHeader.SignificantFigures = TheBinaryReader.ReadUInt32();
            ThePCAPPackageCaptureGlobalHeader.SnapshotLength = TheBinaryReader.ReadUInt32();
            ThePCAPPackageCaptureGlobalHeader.NetworkDataLinkType = TheBinaryReader.ReadUInt32();

            //Validate fields from the PCAP packet capture global header
            TheResult = ValidatePCAPPackageCaptureGlobalHeader(ThePCAPPackageCaptureGlobalHeader);

            return TheResult;
        }

        public override bool ProcessPacketCapturePacketHeader(System.IO.BinaryReader TheBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the PCAP packet capture packet header
            PCAPPackageCaptureStructures.PCAPPackageCapturePacketHeaderStructure ThePCAPPackageCapturePacketHeader = new PCAPPackageCaptureStructures.PCAPPackageCapturePacketHeaderStructure();

            //Populate the PCAP packet capture packet header from the packet capture
            ThePCAPPackageCapturePacketHeader.TimestampSeconds = TheBinaryReader.ReadUInt32();
            ThePCAPPackageCapturePacketHeader.TimestampMicroseconds = TheBinaryReader.ReadUInt32();
            ThePCAPPackageCapturePacketHeader.SavedLength = TheBinaryReader.ReadUInt32();
            ThePCAPPackageCapturePacketHeader.ActualLength = TheBinaryReader.ReadUInt32();

            //No need to validate fields from the PCAP packet capture packet header

            return TheResult;
        }

        //
        //Private methods - provide methods specific to Sniffer packet captures, not required to derive from the abstract base class
        //

        private bool ValidatePCAPPackageCaptureGlobalHeader(PCAPPackageCaptureStructures.PCAPPackageCaptureGlobalHeaderStructure ThePCAPPackageCaptureGlobalHeader)
        {
            bool TheResult = true;

            //Validate fields from the PCAP packet capture global header

            if (ThePCAPPackageCaptureGlobalHeader.MagicNumber != PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedMagicNumber)
            {
                System.Diagnostics.Debug.WriteLine("The PCAP packet capture does not contain the expected magic number, is {0} not {1}", ThePCAPPackageCaptureGlobalHeader.MagicNumber, PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedMagicNumber);

                TheResult = false;
            }

            if (ThePCAPPackageCaptureGlobalHeader.NetworkDataLinkType != PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedNetworkDataLinkType)
            {
                System.Diagnostics.Debug.WriteLine("The PCAP packet capture does not contain the expected network data link type, is {0} not {1}", ThePCAPPackageCaptureGlobalHeader.NetworkDataLinkType, PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedNetworkDataLinkType);

                TheResult = false;
            }

            return TheResult;
        }
    }
}