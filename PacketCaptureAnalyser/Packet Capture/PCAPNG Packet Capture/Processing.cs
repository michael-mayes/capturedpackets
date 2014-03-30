//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.PCAPNGPackageCapture
{
    class Processing : CommonProcessing
    {
        private bool IsTheSectionHeaderBlockLittleEndian = true;

        //
        //Concrete methods - override abstract methods on the base class
        //

        public override bool ProcessGlobalHeader(Analysis.DebugInformation TheDebugInformation, System.IO.BinaryReader TheBinaryReader, out System.UInt32 TheNetworkDataLinkType, out double TheTimestampAccuracy)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the network datalink type
            TheNetworkDataLinkType = (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Invalid;

            //Set up the output parameter for the timestamp accuracy - not used for PCAP Next Generation packet captures so default to zero
            TheTimestampAccuracy = 0.0;

            //Create the single instance of the PCAP Next Generation packet capture section header block
            Structures.SectionHeaderBlockStructure TheSectionHeaderBlock =
                new Structures.SectionHeaderBlockStructure();

            //Read the block type for the PCAP Next Generation packet capture section header block
            TheSectionHeaderBlock.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture section header block, adjusting it up to the next even four byte boundary
            TheSectionHeaderBlock.BlockTotalLength = AdjustBlockTotalLength(TheBinaryReader.ReadUInt32());

            //Read the magic number of the PCAP Next Generation packet capture section header block from the packet capture
            TheSectionHeaderBlock.ByteOrderMagic = TheBinaryReader.ReadUInt32();

            //The endianism of the remainder of the values in the PCAP Next Generation packet capture section header block will be corrected to little endian if the magic number indicates big endian representation
            if (TheSectionHeaderBlock.ByteOrderMagic == Constants.LittleEndianByteOrderMagic)
            {
                TheDebugInformation.WriteInformationEvent
                    (
                    "The PCAP Next Generation packet capture contains the little endian byte-order magic"
                    );

                IsTheSectionHeaderBlockLittleEndian = true;
            }
            else if (TheSectionHeaderBlock.ByteOrderMagic == Constants.BigEndianByteOrderMagic)
            {
                TheDebugInformation.WriteInformationEvent
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
            TheBinaryReader.ReadBytes((int)(TheSectionHeaderBlock.BlockTotalLength - (uint)Constants.BlockTotalLength.SectionHeaderBlock));

            //Validate fields from the PCAP Next Generation packet capture section header block
            TheResult = ValidateSectionHeaderBlock(TheDebugInformation, TheSectionHeaderBlock);

            if (TheResult)
            {
                //Set up the output parameter for the network data link type to the default value as it is not supplied by the section header block
                TheNetworkDataLinkType = (uint)PacketCapture.CommonConstants.NetworkDataLinkType.Ethernet;
            }

            return TheResult;
        }

        public override bool ProcessPacketHeader(Analysis.DebugInformation TheDebugInformation, System.IO.BinaryReader TheBinaryReader, System.UInt32 TheNetworkDataLinkType, double TheTimestampAccuracy, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //Peek a view at the block type for the PCAP Next Generation packet capture block
            System.UInt32 TheBlockType = TheBinaryReader.ReadUInt32();

            //Wind back the binary stream by four to hide that we have just peeked a view at the block type
            TheBinaryReader.BaseStream.Position -= 4;

            switch (TheBlockType)
            {
                case (uint)Constants.BlockType.InterfaceDescriptionBlock:
                    {
                        //We have got a PCAP Next Generation packet capture interface description block

                        TheResult = ProcessInterfaceDescriptionBlock(TheBinaryReader, out ThePayloadLength);

                        break;
                    }

                case (uint)Constants.BlockType.PacketBlock:
                    {
                       //We have got a PCAP Next Generation packet capture packet block

                        TheResult = ProcessPacketBlock(TheBinaryReader, out ThePayloadLength, out TheTimestamp);

                        break;
                    }

                case (uint)Constants.BlockType.SimplePacketBlock:
                    {
                        //We have got a PCAP Next Generation packet capture simple packet block

                        TheResult = ProcessSimplePacketBlock(TheBinaryReader, out ThePayloadLength);

                        break;
                    }

                case (uint)Constants.BlockType.EnhancedPacketBlock:
                    {
                        //We have got a PCAP Next Generation packet capture enhanced packet block

                        TheResult = ProcessEnhancedPacketBlock(TheBinaryReader, out ThePayloadLength, out TheTimestamp);

                        break;
                    }

                case (uint)Constants.BlockType.InterfaceStatisticsBlock:
                    {
                        //We have got a PCAP Next Generation packet capture interface statistics block

                        TheResult = ProcessInterfaceStatisticsBlock(TheBinaryReader, out ThePayloadLength, out TheTimestamp);

                        break;
                    }

                default:
                    {
                        //We have got an PCAP Next Generation packet capture packet containing an unknown Block Type
                        TheDebugInformation.WriteErrorEvent
                        (
                        "The PCAP Next Generation packet capture block contains an unexpected Block Type of 0x" +
                        string.Format("{0:X}", TheBlockType) +
                        "!!!"
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

        private bool ValidateSectionHeaderBlock(Analysis.DebugInformation TheDebugInformation, Structures.SectionHeaderBlockStructure TheSectionHeaderBlock)
        {
            bool TheResult = true;

            //Validate fields from the PCAP Next Generation packet capture section block header

            if (TheSectionHeaderBlock.BlockType != (uint)Constants.BlockType.SectionHeaderBlock)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected block type, is " +
                    TheSectionHeaderBlock.BlockType.ToString() +
                    " not " +
                    Constants.BlockType.SectionHeaderBlock.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            if (TheSectionHeaderBlock.ByteOrderMagic != Constants.LittleEndianByteOrderMagic &&
                TheSectionHeaderBlock.ByteOrderMagic != Constants.BigEndianByteOrderMagic)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected magic number, is " +
                    TheSectionHeaderBlock.ByteOrderMagic.ToString() +
                    " not " +
                    Constants.LittleEndianByteOrderMagic.ToString() +
                    " or " +
                    Constants.BigEndianByteOrderMagic.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            if (TheSectionHeaderBlock.MajorVersion != Constants.ExpectedMajorVersion)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected major version number, is " +
                    TheSectionHeaderBlock.MajorVersion.ToString() +
                    " not " +
                    Constants.ExpectedMajorVersion.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            if (TheSectionHeaderBlock.MinorVersion != Constants.ExpectedMinorVersion)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The PCAP Next Generation packet capture section header block does not contain the expected minor version number, is " +
                    TheSectionHeaderBlock.MinorVersion.ToString() +
                    " not " +
                    Constants.ExpectedMinorVersion.ToString() +
                    "!!!"
                    );

                TheResult = false;
            }

            return TheResult;
        }

        private bool ProcessInterfaceDescriptionBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength)
        {
            bool TheResult = true;

            //We have got a PCAP Next Generation packet capture interface description block

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Create the single instance of the PCAP Next Generation packet capture interface description block
            Structures.InterfaceDescriptionBlockStructure TheInterfaceDescriptionBlock =
                new Structures.InterfaceDescriptionBlockStructure();

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
            TheBinaryReader.ReadBytes((int)(TheInterfaceDescriptionBlock.BlockTotalLength - (uint)Constants.BlockTotalLength.InterfaceDescriptionBlock));

            //The PCAP Next Generation packet capture interface description block does not contain an Ethernet frame
            //Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            //Set this output parameter to a value of zero to prevent any attempt to process an Ethernet frame from the PCAP Next Generation packet capture packet payload
            ThePayloadLength = 0;

            return TheResult;
        }

        private bool ProcessPacketBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //We have got a PCAP Next Generation packet capture packet block

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //Create an instance of the PCAP Next Generation packet capture packet block
            Structures.PacketBlockStructure ThePacketBlock =
                new Structures.PacketBlockStructure();

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
            ThePayloadLength = ThePacketBlock.BlockTotalLength - (uint)Constants.BlockTotalLength.PacketBlock - 12;

            //Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            TheTimestamp =
                ((ThePacketBlock.TimestampHigh * 4294967296) +
                (ThePacketBlock.TimestampLow));

            return TheResult;
        }

        private bool ProcessSimplePacketBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength)
        {
            bool TheResult = true;

            //We have got a PCAP Next Generation packet capture simple packet block

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Create an instance of the PCAP Next Generation packet capture simple packet block
            Structures.SimplePacketBlockStructure TheSimplePacketBlock =
                new Structures.SimplePacketBlockStructure();

            //Read the block type for the PCAP Next Generation packet capture simple packet block
            TheSimplePacketBlock.BlockType = TheBinaryReader.ReadUInt32();

            //Read the block total length for the PCAP Next Generation packet capture simple packet block, adjusting it up to the next even four byte boundary
            TheSimplePacketBlock.BlockTotalLength = AdjustBlockTotalLength(TheBinaryReader.ReadUInt32());

            //Read the actual length for the packet within the PCAP Next Generation packet capture simple packet block
            TheSimplePacketBlock.PacketLength = TheBinaryReader.ReadUInt32();

            //Set up the output parameter for the length of the PCAP Next Generation packet capture packet payload
            //Subtract the normal Ethernet trailer of twelve bytes as this would typically not be exposed in the packet capture
            ThePayloadLength = TheSimplePacketBlock.BlockTotalLength - (uint)Constants.BlockTotalLength.SimplePacketBlock - 12;

            return TheResult;
        }

        private bool ProcessEnhancedPacketBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //We have got a PCAP Next Generation packet capture enhanced packet block

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //Create an instance of the PCAP Next Generation packet capture enhanced packet block
            Structures.EnhancedPacketBlockStructure TheEnhancedPacketBlock =
                new Structures.EnhancedPacketBlockStructure();

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
            ThePayloadLength = TheEnhancedPacketBlock.BlockTotalLength - (uint)Constants.BlockTotalLength.EnhancedPacketBlock - 12;

            //Set up the output parameter for the timestamp based on the timestamp from the PCAP Next Generation packet capture packet payload
            TheTimestamp =
                ((TheEnhancedPacketBlock.TimestampHigh * 4294967296) +
                (TheEnhancedPacketBlock.TimestampLow));

            return TheResult;
        }

        private bool ProcessInterfaceStatisticsBlock(System.IO.BinaryReader TheBinaryReader, out long ThePayloadLength, out double TheTimestamp)
        {
            bool TheResult = true;

            //We have got a PCAP Next Generation packet capture interface statistics block

            //Provide a default value to the output parameter for the length of the PCAP Next Generation packet capture block payload
            ThePayloadLength = 0;

            //Provide a default value to the output parameter for the timestamp
            TheTimestamp = 0.0;

            //Create an instance of the PCAP Next Generation packet capture interface statistics block
            Structures.InterfaceStatisticsBlockStructure TheInterfaceStatisticsBlock =
                new Structures.InterfaceStatisticsBlockStructure();

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
            TheBinaryReader.ReadBytes((int)(TheInterfaceStatisticsBlock.BlockTotalLength - (uint)Constants.BlockTotalLength.InterfaceStatisticsBlock));

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
