//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureProcessingNamespace
{
    class PCAPNGPackageCaptureProcessing : CommonPacketCaptureProcessing
    {
        private bool IsTheSectionHeaderBlockLittleEndian = true;

        //
        //Concrete methods - override abstract methods on the base class
        //

        public override bool ProcessGlobalHeader(System.IO.BinaryReader TheBinaryReader, out System.UInt32 TheNetworkDataLinkType, out double TheTimestampAccuracy)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the network datalink type
            TheNetworkDataLinkType = CommonPackageCaptureConstants.CommonPackageCaptureInvalidNetworkDataLinkType;

            //Set up the output parameter for the timestamp accuracy - not used for PCAP Next Generation packet captures so default to zero
            TheTimestampAccuracy = 0.0;

            //Create the single instance of the PCAP Next Generation packet capture section header block
            PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureSectionHeaderBlockStructure TheSectionHeaderBlock =
                new PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureSectionHeaderBlockStructure();

            //Read the block type for the PCAP Next Generation packet capture section header block
            TheSectionHeaderBlock.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture section header block
            TheSectionHeaderBlock.BlockTotalLength = TheBinaryReader.ReadUInt32();

            //Read the magic number of the PCAP Next Generation packet capture section header block from the packet capture
            TheSectionHeaderBlock.ByteOrderMagic = TheBinaryReader.ReadUInt32();

            //The endianism of the remainder of the values in the PCAP Next Generation packet capture section header block will be corrected to little endian if the magic number indicates big endian representation
            if (TheSectionHeaderBlock.ByteOrderMagic == PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureLittleEndianByteOrderMagic)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture contains the little endian byte-order magic"
                    );

                IsTheSectionHeaderBlockLittleEndian = true;
            }
            else if (TheSectionHeaderBlock.ByteOrderMagic == PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureBigEndianByteOrderMagic)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture contains the big endian byte-order magic"
                    );

                IsTheSectionHeaderBlockLittleEndian = false;
            }

            //Read the remainder of the PCAP Next Generation packet capture section header block from the packet capture
            if (IsTheSectionHeaderBlockLittleEndian)
            {
                TheSectionHeaderBlock.MajorVersion = TheBinaryReader.ReadUInt16();
                TheSectionHeaderBlock.MinorVersion = TheBinaryReader.ReadUInt16();
            }
            else
            {
                TheSectionHeaderBlock.MajorVersion = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
                TheSectionHeaderBlock.MinorVersion = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            }

            //Just read the bytes off the remaining bytes from the section header block so we can continue
            TheBinaryReader.ReadBytes((int)(TheSectionHeaderBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureSectionHeaderBlockLength));

            //Validate fields from the PCAP Next Generation packet capture section header block
            TheResult = ValidateSectionHeaderBlock(TheSectionHeaderBlock);

            //Set up the output parameter for the network data link type to the default value as it is not supplied by the section header block
            TheNetworkDataLinkType = CommonPackageCaptureConstants.CommonPackageCaptureEthernetNetworkDataLinkType;

            return TheResult;
        }

        public override bool ProcessPacketHeader(System.IO.BinaryReader TheBinaryReader, System.UInt32 TheNetworkDataLinkType, double TheTimestampAccuracy, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //Create an instance of the PCAP Next Generation packet capture block header
            PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureBlockHeaderStructure TheBlockHeader =
                new PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureBlockHeaderStructure();

            //Read the block type for the PCAP Next Generation packet capture block
            TheBlockHeader.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture block
            TheBlockHeader.BlockTotalLength = TheBinaryReader.ReadUInt32();

            //Just read the bytes off the remaining bytes from the section header block so we can continue
            TheBinaryReader.ReadBytes((int)(TheBlockHeader.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureBlockHeaderLength));

            return TheResult;
        }

        //
        //Private methods - provide methods specific to PCAP Next Generation packet captures, not required to derive from the abstract base class
        //

        private bool ValidateSectionHeaderBlock(PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureSectionHeaderBlockStructure TheSectionBlockHeader)
        {
            bool TheResult = true;

            //Validate fields from the PCAP Next Generation packet capture section block header

            if (TheSectionBlockHeader.BlockType != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureSectionHeaderExpectedBlockType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected block type, is " +
                    TheSectionBlockHeader.BlockType.ToString() +
                    " not " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureSectionHeaderExpectedBlockType.ToString()
                    );

                TheResult = false;
            }

            if (TheSectionBlockHeader.ByteOrderMagic != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureLittleEndianByteOrderMagic &&
                TheSectionBlockHeader.ByteOrderMagic != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureBigEndianByteOrderMagic)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected magic number, is " +
                    TheSectionBlockHeader.ByteOrderMagic.ToString() +
                    " not " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureLittleEndianByteOrderMagic.ToString() +
                    " or " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureBigEndianByteOrderMagic.ToString()
                    );

                TheResult = false;
            }

            if (TheSectionBlockHeader.MajorVersion != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureExpectedMajorVersion)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected major version number, is " +
                    TheSectionBlockHeader.MajorVersion.ToString() +
                    " not " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureExpectedMajorVersion.ToString()
                    );

                TheResult = false;
            }

            if (TheSectionBlockHeader.MinorVersion != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureExpectedMinorVersion)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected minor version number, is " +
                    TheSectionBlockHeader.MinorVersion.ToString() +
                    " not " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureExpectedMinorVersion.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}