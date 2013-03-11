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
        private bool IsTheGlobalHeaderLittleEndian = true;

        //
        //Concrete methods - override abstract methods on the base class
        //

        public override bool ProcessGlobalHeader(System.IO.BinaryReader TheBinaryReader, out System.UInt32 TheNetworkDataLinkType, out double TheTimestampAccuracy)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the network datalink type
            TheNetworkDataLinkType = CommonPackageCaptureConstants.CommonPackageCaptureInvalidNetworkDataLinkType;

            //Set up the output parameter for the timestamp accuracy - not used for PCAP packet captures so default to zero
            TheTimestampAccuracy = 0.0;

            //Create the single instance of the PCAP packet capture global header
            PCAPPackageCaptureStructures.PCAPPackageCaptureGlobalHeaderStructure TheGlobalHeader = new PCAPPackageCaptureStructures.PCAPPackageCaptureGlobalHeaderStructure();

            //Read the magic number of the PCAP packet capture global header from the packet capture
            TheGlobalHeader.MagicNumber = TheBinaryReader.ReadUInt32();

            //The endianism of the remainder of the values in the PCAP packet capture global header will be corrected to little endian if the magic number indicates big endian representation
            if (TheGlobalHeader.MagicNumber == PCAPPackageCaptureConstants.PCAPPackageCaptureLittleEndianMagicNumber)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP packet capture contains the little endian magic number"
                    );

                IsTheGlobalHeaderLittleEndian = true;
            }
            else if (TheGlobalHeader.MagicNumber == PCAPPackageCaptureConstants.PCAPPackageCaptureBigEndianMagicNumber)
            {
                System.Diagnostics.Trace.WriteLine
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
            TheResult = ValidateGlobalHeader(TheGlobalHeader);

            if (TheResult)
            {
                //Set up the output parameter for the network data link type
                TheNetworkDataLinkType = TheGlobalHeader.NetworkDataLinkType;
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

            //Create an instance of the PCAP packet capture packet header
            PCAPPackageCaptureStructures.PCAPPackageCapturePacketHeaderStructure ThePacketHeader = new PCAPPackageCaptureStructures.PCAPPackageCapturePacketHeaderStructure();

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
                    case CommonPackageCaptureConstants.CommonPackageCaptureEthernetNetworkDataLinkType:
                        {
                            //Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
                            ThePayloadLength = ThePacketHeader.SavedLength - 12;

                            break;
                        }

                    case CommonPackageCaptureConstants.CommonPackageCaptureNullLoopBackNetworkDataLinkType:
                    case CommonPackageCaptureConstants.CommonPackageCaptureCiscoHDLCNetworkDataLinkType:
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
        //Private methods - provide methods specific to Sniffer packet captures, not required to derive from the abstract base class
        //

        private bool ValidateGlobalHeader(PCAPPackageCaptureStructures.PCAPPackageCaptureGlobalHeaderStructure TheGlobalHeader)
        {
            bool TheResult = true;

            //Validate fields from the PCAP packet capture global header

            if (TheGlobalHeader.MagicNumber != PCAPPackageCaptureConstants.PCAPPackageCaptureLittleEndianMagicNumber &&
                TheGlobalHeader.MagicNumber != PCAPPackageCaptureConstants.PCAPPackageCaptureBigEndianMagicNumber)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP packet capture global header does not contain the expected magic number, is " +
                    TheGlobalHeader.MagicNumber.ToString() +
                    " not " +
                    PCAPPackageCaptureConstants.PCAPPackageCaptureLittleEndianMagicNumber.ToString() +
                    " or " +
                    PCAPPackageCaptureConstants.PCAPPackageCaptureBigEndianMagicNumber.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.VersionMajor != PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedVersionMajor)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP packet capture global header does not contain the expected major version number, is " +
                    TheGlobalHeader.VersionMajor.ToString() +
                    " not " +
                    PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedVersionMajor.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.VersionMinor != PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedVersionMinor)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP packet capture global header does not contain the expected minor version number, is " +
                    TheGlobalHeader.VersionMinor.ToString() +
                    " not " +
                    PCAPPackageCaptureConstants.PCAPPackageCaptureExpectedVersionMinor.ToString()
                    );

                TheResult = false;
            }

            if (TheGlobalHeader.NetworkDataLinkType != CommonPackageCaptureConstants.CommonPackageCaptureNullLoopBackNetworkDataLinkType &&
                TheGlobalHeader.NetworkDataLinkType != CommonPackageCaptureConstants.CommonPackageCaptureEthernetNetworkDataLinkType &&
                TheGlobalHeader.NetworkDataLinkType != CommonPackageCaptureConstants.CommonPackageCaptureCiscoHDLCNetworkDataLinkType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP packet capture global header does not contain the expected network data link type, is " +
                    TheGlobalHeader.NetworkDataLinkType.ToString() +
                    " not " +
                    CommonPackageCaptureConstants.CommonPackageCaptureNullLoopBackNetworkDataLinkType.ToString() +
                    " or " +
                    CommonPackageCaptureConstants.CommonPackageCaptureEthernetNetworkDataLinkType.ToString() +
                    " or " +
                    CommonPackageCaptureConstants.CommonPackageCaptureCiscoHDLCNetworkDataLinkType.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}