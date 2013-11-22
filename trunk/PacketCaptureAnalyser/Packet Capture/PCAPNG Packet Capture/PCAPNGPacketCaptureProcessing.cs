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
                TheSectionHeaderBlock.SectionLength = TheBinaryReader.ReadUInt64();
            }
            else
            {
                TheSectionHeaderBlock.MajorVersion = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
                TheSectionHeaderBlock.MinorVersion = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
                TheSectionHeaderBlock.SectionLength = (System.UInt64)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt64());
            }

            //Just read the bytes off the remaining bytes from the section header block so we can continue
            TheBinaryReader.ReadBytes((int)(TheSectionHeaderBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureSectionHeaderBlockLength));

            //Validate fields from the PCAP Next Generation packet capture section header block
            TheResult = ValidateSectionHeaderBlock(TheSectionHeaderBlock);

            if (TheResult)
            {
                //Set up the output parameter for the network data link type to the default value as it is not supplied by the section header block
                TheNetworkDataLinkType = CommonPackageCaptureConstants.CommonPackageCaptureEthernetNetworkDataLinkType;

                //Create the single instance of the PCAP Next Generation packet capture interface description block
                PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureInterfaceDescriptionBlockStructure TheInterfaceDescriptionBlock =
                    new PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureInterfaceDescriptionBlockStructure();

                //Read the block type for the PCAP Next Generation packet capture interface description block
                TheInterfaceDescriptionBlock.BlockType = TheBinaryReader.ReadUInt32();

                //Read the block total length for the PCAP Next Generation packet capture interface description block
                TheInterfaceDescriptionBlock.BlockTotalLength = TheBinaryReader.ReadUInt32();

                //Read the reserved field for the PCAP Next Generation packet capture interface description block
                TheInterfaceDescriptionBlock.Reserved = TheBinaryReader.ReadUInt16();

                //Read the network data link type for the PCAP Next Generation packet capture interface description block
                TheInterfaceDescriptionBlock.LinkType = TheBinaryReader.ReadUInt16();

                //Read the snap length for the PCAP Next Generation packet capture interface description block
                TheInterfaceDescriptionBlock.SnapLen = TheBinaryReader.ReadUInt32();

                //Just read the bytes off the remaining bytes from the interface description block so we can continue
                TheBinaryReader.ReadBytes((int)(TheInterfaceDescriptionBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureInterfaceDescriptionBlockLength));

                //Validate fields from the PCAP Next Generation packet capture interface description block
                TheResult = ValidateInterfaceDescriptionBlock(TheInterfaceDescriptionBlock);
            }

            return TheResult;
        }

        public override bool ProcessPacketHeader(System.IO.BinaryReader TheBinaryReader, System.UInt32 TheNetworkDataLinkType, double TheTimestampAccuracy, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture enhanced packet block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //Create an instance of the PCAP Next Generation packet capture enhanced packet block
            PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureEnhancedPacketBlockStructure TheEnhancedPacketBlock =
                new PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureEnhancedPacketBlockStructure();

            //Read the block type for the PCAP Next Generation packet capture enhanced packet block
            TheEnhancedPacketBlock.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture enhanced packet block
            TheEnhancedPacketBlock.BlockTotalLength = TheBinaryReader.ReadUInt32();

            //Just read the bytes off the remaining bytes from the enhanced packet block so we can continue
            TheBinaryReader.ReadBytes((int)(TheEnhancedPacketBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureEnhancedPacketBlockLength));

            //Set up the output parameter for the length of the PCAP packet capture packet payload
            ThePayloadLength = TheEnhancedPacketBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureEnhancedPacketBlockLength;

            return TheResult;
        }

        //
        //Private methods - provide methods specific to PCAP Next Generation packet captures, not required to derive from the abstract base class
        //

        private bool ValidateSectionHeaderBlock(PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureSectionHeaderBlockStructure TheSectionHeaderBlock)
        {
            bool TheResult = true;

            //Validate fields from the PCAP Next Generation packet capture section block header

            if (TheSectionHeaderBlock.BlockType != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureSectionHeaderBlockExpectedBlockType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected block type, is " +
                    TheSectionHeaderBlock.BlockType.ToString() +
                    " not " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureSectionHeaderBlockExpectedBlockType.ToString()
                    );

                TheResult = false;
            }

            if (TheSectionHeaderBlock.ByteOrderMagic != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureLittleEndianByteOrderMagic &&
                TheSectionHeaderBlock.ByteOrderMagic != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureBigEndianByteOrderMagic)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected magic number, is " +
                    TheSectionHeaderBlock.ByteOrderMagic.ToString() +
                    " not " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureLittleEndianByteOrderMagic.ToString() +
                    " or " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureBigEndianByteOrderMagic.ToString()
                    );

                TheResult = false;
            }

            if (TheSectionHeaderBlock.MajorVersion != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureExpectedMajorVersion)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected major version number, is " +
                    TheSectionHeaderBlock.MajorVersion.ToString() +
                    " not " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureExpectedMajorVersion.ToString()
                    );

                TheResult = false;
            }

            if (TheSectionHeaderBlock.MinorVersion != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureExpectedMinorVersion)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected minor version number, is " +
                    TheSectionHeaderBlock.MinorVersion.ToString() +
                    " not " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureExpectedMinorVersion.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }

        private bool ValidateInterfaceDescriptionBlock(PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureInterfaceDescriptionBlockStructure TheInterfaceDescriptionBlock)
        {
            bool TheResult = true;

            //Validate fields from the PCAP Next Generation packet capture interface description block

            if (TheInterfaceDescriptionBlock.BlockType != PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureInterfaceDescriptionBlockExpectedBlockType)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The PCAP Next Generation packet capture interface description block does not contain the expected block type, is " +
                    TheInterfaceDescriptionBlock.BlockType.ToString() +
                    " not " +
                    PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureInterfaceDescriptionBlockExpectedBlockType.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}