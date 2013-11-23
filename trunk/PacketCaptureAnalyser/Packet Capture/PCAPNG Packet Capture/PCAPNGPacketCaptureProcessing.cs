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

            //Read the block total length for the PCAP Next Generation packet capture section header block, adjusting it up to the next even four byte boundary
            TheSectionHeaderBlock.BlockTotalLength = AdjustBlockTotalLength(TheBinaryReader.ReadUInt32());

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
            }

            return TheResult;
        }

        public override bool ProcessPacketHeader(System.IO.BinaryReader TheBinaryReader, System.UInt32 TheNetworkDataLinkType, double TheTimestampAccuracy, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            int TheNextByte = TheBinaryReader.PeekChar();

            switch (TheNextByte)
            {
                case (int)PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureInterfaceDescriptionBlockExpectedBlockType:
                    {
                        TheResult = ProcessInterfaceDescriptionBlockExpectedBlock(TheBinaryReader, out ThePayloadLength);

                        break;
                    }

                case (int)PCAPNGPackageCaptureConstants.PCAPNGPackageCapturePacketBlockExpectedBlockType:
                    {

                        TheResult = ProcessPacketBlock(TheBinaryReader, out ThePayloadLength, out TheTimestamp);

                        break;
                    }

                case (int)PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureSimplePacketBlockExpectedBlockType:
                    {
                        TheResult = ProcessSimplePacketBlock(TheBinaryReader, out ThePayloadLength);

                        break;
                    }

                case (int)PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureEnhancedPacketBlockExpectedBlockType:
                    {
                        TheResult = ProcessEnhancedPacketBlock(TheBinaryReader, out ThePayloadLength, out TheTimestamp);

                        break;
                    }

                case (int)PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureInterfaceStatisticsBlockExpectedBlockType:
                    {
                        TheResult = ProcessInterfaceStatisticsBlock(TheBinaryReader, out ThePayloadLength, out TheTimestamp);

                        break;
                    }

                default:
                    {
                        //We have got an PCAP Next Generation packet capture packet containing an unknown Block Type
                        System.Diagnostics.Trace.WriteLine
                        (
                        "The PCAP Next Generation packet capture block contains an unexpected Block Type of 0x" +
                        string.Format("{0:X}", TheNextByte)
                        );

                        TheResult = false;

                        break;
                    }
            }

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

        private bool ProcessInterfaceDescriptionBlockExpectedBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Create the single instance of the PCAP Next Generation packet capture interface description block
            PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureInterfaceDescriptionBlockStructure TheInterfaceDescriptionBlock =
                new PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureInterfaceDescriptionBlockStructure();

            //Read the block type for the PCAP Next Generation packet capture interface description block
            TheInterfaceDescriptionBlock.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture interface description block, adjusting it up to the next even four byte boundary
            TheInterfaceDescriptionBlock.BlockTotalLength = AdjustBlockTotalLength(TheBinaryReader.ReadUInt32());

            //Read the reserved field for the PCAP Next Generation packet capture interface description block
            TheInterfaceDescriptionBlock.Reserved = TheBinaryReader.ReadUInt16();

            //Read the network data link type for the PCAP Next Generation packet capture interface description block
            TheInterfaceDescriptionBlock.LinkType = TheBinaryReader.ReadUInt16();

            //Read the snap length for the PCAP Next Generation packet capture interface description block
            TheInterfaceDescriptionBlock.SnapLen = TheBinaryReader.ReadUInt32();

            //The PCAP Next Generation packet capture interface description block does not contain an Ethernet frame
            //Just read the bytes off the remaining bytes from the PCAP Next Generation packet capture interface description block so we can continue
            TheBinaryReader.ReadBytes((int)(TheInterfaceDescriptionBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureInterfaceDescriptionBlockLength));

            //The PCAP Next Generation packet capture interface description block does not contain an Ethernet frame
            //Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            //Set this output parameter to a value of zero to prevent any attempt to process an Ethernet frame from the PCAP Next Generation packet capture packet payload
            ThePayloadLength = 0;

            return TheResult;
        }

        private bool ProcessPacketBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //We have got a PCAP Next Generation packet capture packet block

            //Create an instance of the PCAP Next Generation packet capture packet block
            PCAPNGPackageCaptureStructures.PCAPNGPackageCapturePacketBlockStructure ThePacketBlock =
                new PCAPNGPackageCaptureStructures.PCAPNGPackageCapturePacketBlockStructure();

            //Read the block type for the PCAP Next Generation packet capture packet block
            ThePacketBlock.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture packet block, adjusting it up to the next even four byte boundary
            ThePacketBlock.BlockTotalLength = AdjustBlockTotalLength(TheBinaryReader.ReadUInt32());

            //Read the interface Id for the PCAP Next Generation packet capture packet block
            ThePacketBlock.InterfaceId = TheBinaryReader.ReadUInt16();

            //Read the drops count for the PCAP Next Generation packet capture packet block
            ThePacketBlock.DropsCount = TheBinaryReader.ReadUInt16();

            //Read the high bytes of the timestamp for the PCAP Next Generation packet capture packet block
            ThePacketBlock.TimestampHigh = TheBinaryReader.ReadUInt32();

            //Read the low bytes of the timestamp for the PCAP Next Generation packet capture packet block
            ThePacketBlock.TimestampLow = TheBinaryReader.ReadUInt32();

            //Read the capture length for packet within the PCAP Next Generation packet capture packet block
            ThePacketBlock.CapturedLength = TheBinaryReader.ReadUInt32();

            //Read the actual length for the packet within the PCAP Next Generation packet capture packet block
            ThePacketBlock.PacketLength = TheBinaryReader.ReadUInt32();

            //Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            //Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            ThePayloadLength = ThePacketBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCapturePacketBlockLength - 12;

            //Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            TheTimestamp =
                ((ThePacketBlock.TimestampHigh * 4294967296) +
                (ThePacketBlock.TimestampLow));

            return TheResult;
        }

        private bool ProcessSimplePacketBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //We have got a PCAP Next Generation packet capture simple packet block

            //Create an instance of the PCAP Next Generation packet capture simple packet block
            PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureSimplePacketBlockStructure TheSimplePacketBlock =
                new PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureSimplePacketBlockStructure();

            //Read the block type for the PCAP Next Generation packet capture simple packet block
            TheSimplePacketBlock.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture simple packet block, adjusting it up to the next even four byte boundary
            TheSimplePacketBlock.BlockTotalLength = AdjustBlockTotalLength(TheBinaryReader.ReadUInt32());

            //Read the actual length for the packet within the PCAP Next Generation packet capture simple packet block
            TheSimplePacketBlock.PacketLength = TheBinaryReader.ReadUInt32();

            //Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            //Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            ThePayloadLength = TheSimplePacketBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureSimplePacketBlockLength - 12;

            return TheResult;
        }

        private bool ProcessEnhancedPacketBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //We have got a PCAP Next Generation packet capture enhanced packet block

            //Create an instance of the PCAP Next Generation packet capture enhanced packet block
            PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureEnhancedPacketBlockStructure TheEnhancedPacketBlock =
                new PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureEnhancedPacketBlockStructure();

            //Read the block type for the PCAP Next Generation packet capture enhanced packet block
            TheEnhancedPacketBlock.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture enhanced packet block, adjusting it up to the next even four byte boundary
            TheEnhancedPacketBlock.BlockTotalLength = AdjustBlockTotalLength(TheBinaryReader.ReadUInt32());

            //Read the interface Id for the PCAP Next Generation packet capture enhanced packet block
            TheEnhancedPacketBlock.InterfaceId = TheBinaryReader.ReadUInt32();

            //Read the high bytes of the timestamp for the PCAP Next Generation packet capture enhanced packet block
            TheEnhancedPacketBlock.TimestampHigh = TheBinaryReader.ReadUInt32();

            //Read the low bytes of the timestamp for the PCAP Next Generation packet capture enhanced packet block
            TheEnhancedPacketBlock.TimestampLow = TheBinaryReader.ReadUInt32();

            //Read the capture length for packet within the PCAP Next Generation packet capture enhanced packet block
            TheEnhancedPacketBlock.CapturedLength = TheBinaryReader.ReadUInt32();

            //Read the actual length for the packet within the PCAP Next Generation packet capture enhanced packet block
            TheEnhancedPacketBlock.PacketLength = TheBinaryReader.ReadUInt32();

            //Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            //Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            ThePayloadLength = TheEnhancedPacketBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureEnhancedPacketBlockLength - 12;

            //Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            TheTimestamp =
                ((TheEnhancedPacketBlock.TimestampHigh * 4294967296) +
                (TheEnhancedPacketBlock.TimestampLow));

            return TheResult;
        }

        private bool ProcessInterfaceStatisticsBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //We have got a PCAP Next Generation packet capture interface statistics block

            //Create an instance of the PCAP Next Generation packet capture interface statistics block
            PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureInterfaceStatisticsBlockStructure TheInterfaceStatisticsBlock =
                new PCAPNGPackageCaptureStructures.PCAPNGPackageCaptureInterfaceStatisticsBlockStructure();

            //Read the block type for the PCAP Next Generation packet capture interface statistics block
            TheInterfaceStatisticsBlock.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture interface statistics block, adjusting it up to the next even four byte boundary
            TheInterfaceStatisticsBlock.BlockTotalLength = AdjustBlockTotalLength(TheBinaryReader.ReadUInt32());

            //Read the interface Id for the PCAP Next Generation packet capture interface statistics block
            TheInterfaceStatisticsBlock.InterfaceId = TheBinaryReader.ReadUInt32();

            //Read the high bytes of the timestamp for the PCAP Next Generation packet capture interface statistics block
            TheInterfaceStatisticsBlock.TimestampHigh = TheBinaryReader.ReadUInt32();

            //Read the low bytes of the timestamp for the PCAP Next Generation packet capture interface statistics block
            TheInterfaceStatisticsBlock.TimestampLow = TheBinaryReader.ReadUInt32();

            //The PCAP Next Generation packet capture interface statistics block does not contain an Ethernet frame
            //Just read the bytes off the remaining bytes from the PCAP Next Generation packet capture interface statistics block so we can continue
            TheBinaryReader.ReadBytes((int)(TheInterfaceStatisticsBlock.BlockTotalLength - PCAPNGPackageCaptureConstants.PCAPNGPackageCaptureInterfaceStatisticsBlockLength));

            //The PCAP Next Generation packet capture interface statistics block does not contain an Ethernet frame
            //Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            //Set this output parameter to a value of zero to prevent any attempt to process an Ethernet frame from the PCAP Next Generation packet capture packet payload
            ThePayloadLength = 0;

            //Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            TheTimestamp =
                ((TheInterfaceStatisticsBlock.TimestampHigh * 4294967296) +
                (TheInterfaceStatisticsBlock.TimestampLow));

            return TheResult;
        }

        private System.UInt32 AdjustBlockTotalLength(System.UInt32 BlockTotalLength)
        {
            System.UInt32 AdjustedBlockTotalLength = BlockTotalLength;

            //Adjust the supplied block total length up to the next even four byte boundary

            //A non-obvious characteristic of the current block total length field is that the block total length may not actually
            //indicate the length of the actual data block (the payload). If the number of payload octets is not evenly divisable
            //by four then the one or more padding bytes will have been added after the payload and before the block trailer to
            //pad the length of the payload to a 32 bit boundry. To seek forwards or backwards to the next or previous block
            //one will need to add from 0 to 3 to the reported block total length to detremine where the next block will start

            switch (AdjustedBlockTotalLength % 4)
            {
                case 0:
                    {
                        break;
                    }

                case 1:
                    {
                        AdjustedBlockTotalLength += 3;
                        break;
                    }

                case 2:
                    {
                        AdjustedBlockTotalLength += 2;
                        break;
                    }

                case 3:
                    {
                        AdjustedBlockTotalLength += 1;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            return AdjustedBlockTotalLength;
        }
    }
}