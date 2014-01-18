﻿//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrameNamespace
{
    class Processing
    {
        private System.IO.BinaryReader TheBinaryReader;

        private Structures.EthernetFrameHeaderStructure TheEthernetFrameHeader;

        private bool PerformLatencyAnalysisProcessing;
        private AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing;

        private bool PerformTimeAnalysisProcessing;
        private AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing;

        private ARPPacketNamespace.Processing TheARPPacketProcessing;
        private IPPacketNamespace.IPv4PacketNamespace.Processing TheIPv4PacketProcessing;
        private IPPacketNamespace.IPv6PacketNamespace.Processing TheIPv6PacketProcessing;
        private LLDPPacketNamespace.Processing TheLLDPPacketProcessing;
        private LoopbackPacketNamespace.Processing TheLoopbackPacketProcessing;

        private long ThePayloadLength;

        private System.UInt16 TheEtherType;

        public Processing(System.IO.BinaryReader TheBinaryReader, bool PerformLatencyAnalysisProcessing, AnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing, bool PerformTimeAnalysisProcessing, AnalysisNamespace.TimeAnalysisProcessing TheTimeAnalysisProcessing)
        {
            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the Ethernet frame header
            TheEthernetFrameHeader = new Structures.EthernetFrameHeaderStructure();

            this.PerformLatencyAnalysisProcessing = PerformLatencyAnalysisProcessing;
            this.TheLatencyAnalysisProcessing = TheLatencyAnalysisProcessing;

            this.PerformTimeAnalysisProcessing = PerformTimeAnalysisProcessing;
            this.TheTimeAnalysisProcessing = TheTimeAnalysisProcessing;

            TheARPPacketProcessing = new ARPPacketNamespace.Processing(TheBinaryReader);
            TheIPv4PacketProcessing = new IPPacketNamespace.IPv4PacketNamespace.Processing(TheBinaryReader, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing);
            TheIPv6PacketProcessing = new IPPacketNamespace.IPv6PacketNamespace.Processing(TheBinaryReader, PerformLatencyAnalysisProcessing, TheLatencyAnalysisProcessing, PerformTimeAnalysisProcessing, TheTimeAnalysisProcessing);
            TheLLDPPacketProcessing = new LLDPPacketNamespace.Processing(TheBinaryReader);
            TheLoopbackPacketProcessing = new LoopbackPacketNamespace.Processing(TheBinaryReader);
        }

        public bool Process(ulong ThePacketNumber, long ThePayloadLength, double TheTimestamp)
        {
            bool TheResult = true;

            //Only process the payload as an Ethernet frame if it has a positive length
            if(ThePayloadLength > 0)
            {
                //Store the length of the payload of the Ethernet frame for use in further processing
                this.ThePayloadLength = ThePayloadLength;

                //Process the Ethernet frame header
                TheResult = ProcessHeader();

                if (TheResult)
                {
                    //Read the Ether Type for the Ethernet frame from the packet capture and process it
                    TheResult = ProcessEtherType(ThePacketNumber);

                    if (TheResult)
                    {
                        TheResult = ProcessPayload(ThePacketNumber, TheTimestamp);
                    }
                }
            }

            return TheResult;
        }

        private bool ProcessHeader()
        {
            bool TheResult = true;

            //Read the Destination MAC Address for the Ethernet frame from the packet capture
            TheEthernetFrameHeader.DestinationMACAddressHigh = TheBinaryReader.ReadUInt32();
            TheEthernetFrameHeader.DestinationMACAddressLow = TheBinaryReader.ReadUInt16();

            //Read the Source MAC Address for the Ethernet frame from the packet capture
            TheEthernetFrameHeader.SourceMACAddressHigh = TheBinaryReader.ReadUInt32();
            TheEthernetFrameHeader.SourceMACAddressLow = TheBinaryReader.ReadUInt16();

            //Read the Ether Type for the Ethernet frame from the packet capture
            TheEthernetFrameHeader.EtherType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());

            //Reduce the length of the payload of the Ethernet frame to reflect that the Ether Type of two bytes would have been included
            ThePayloadLength -= 2;

            //Store the Ether Type for use in further processing
            TheEtherType = TheEthernetFrameHeader.EtherType;

            return TheResult;
        }

        private bool ProcessEtherType(ulong ThePacketNumber)
        {
            bool TheResult = true;

            switch (TheEtherType)
            {
                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.ARP:
                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.IPv4:
                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.IPv6:
                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.LLDP:
                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.Loopback:
                    {
                        break;
                    }

                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.VLANTagged:
                    {
                        //We've got an Ethernet frame with a VLAN tag (IEEE 802.1Q) so must advance and re-read the Ether Type

                        //The "Ether Type" we've just read will actually be the IEEE 802.1Q Tag Protocol Identifier

                        //First just read off the IEEE 802.1Q Tag Control Identifier so we can move on
                        System.UInt16 TheTagControlIdentifier = TheBinaryReader.ReadUInt16();

                        //Then re-read the Ether Type, this time obtaining the real value (so long as there is only one VLAN tag of course!)
                        TheEtherType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());

                        //Reduce the length of the payload of the Ethernet frame to reflect that the VLAN tag of four bytes would have been included
                        ThePayloadLength -= 4;

                        break;
                    }

                default:
                    {
                        //We have got an Ethernet frame containing an unknown Ether Type

                        //Check against the minimum value for Ether Type - lower values indicate length of the Ethernet frame
                        if (TheEtherType < (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.MinimumValue)
                        {
                            //This Ethernet frame has a value for "Ether Type" lower than the minimum
                            //This is an IEEE 802.3 Ethernet frame rather than an Ethernet II frame
                            //This value is the length of the IEEE 802.3 Ethernet frame
                        }
                        else
                        {
                            System.Diagnostics.Trace.WriteLine
                                (
                                "The Ethernet frame in captured packet #" +
                                ThePacketNumber.ToString() +
                                " contains an unexpected Ether Type of 0x" +
                                string.Format("{0:X}", TheEtherType)
                                );

                            TheResult = false;
                        }

                        break;
                    }
            }

            return TheResult;
        }

        private bool ProcessPayload(ulong ThePacketNumber, double TheTimestamp)
        {
            bool TheResult = true;
            bool ThePacketProcessingResult = true;

            //Record the position in the stream for the packet capture so we can later determine how far has been progressed
            long TheStartingStreamPosition = TheBinaryReader.BaseStream.Position;

            //Check the value of the Ether Type for this Ethernet frame
            switch (TheEtherType)
            {
                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.ARP:
                    {
                        //We've got an Ethernet frame containing an ARP packet so process it
                        ThePacketProcessingResult = TheARPPacketProcessing.Process(ThePayloadLength);

                        break;
                    }

                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.IPv4:
                    {
                        //We've got an Ethernet frame containing an IPv4 packet so process it
                        ThePacketProcessingResult = TheIPv4PacketProcessing.Process(ThePayloadLength, ThePacketNumber, TheTimestamp);

                        break;
                    }

                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.IPv6:
                    {
                        //We've got an Ethernet frame containing an IPv6 packet so process it
                        ThePacketProcessingResult = TheIPv6PacketProcessing.Process(ThePayloadLength, ThePacketNumber, TheTimestamp);

                        break;
                    }

                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.LLDP:
                    {
                        //We've got an Ethernet frame containing an LLDP packet so process it
                        ThePacketProcessingResult = TheLLDPPacketProcessing.Process(ThePayloadLength);

                        break;
                    }

                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.Loopback:
                    {
                        //We've got an Ethernet frame containing an Configuration Test Protocol (Loopback) packet so process it
                        ThePacketProcessingResult = TheLoopbackPacketProcessing.Process(ThePayloadLength);

                        break;
                    }

                case (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.VLANTagged:
                    {
                        //We've got an Ethernet frame containing a second VLAN tag!

                        System.Diagnostics.Trace.WriteLine
                            (
                            "The Ethernet frame in captured packet #" +
                            ThePacketNumber.ToString() +
                            " contains a second VLAN tag!" +
                            " - Attempt to recover and continue processing"
                            );

                        ThePacketProcessingResult = ProcessEtherType(ThePacketNumber);

                        if (ThePacketProcessingResult)
                        {
                            //Now that the second VLAN tag has been dealt with rerecord the position in the stream for the packet capture so we can later determine how far has been progressed
                            TheStartingStreamPosition = TheBinaryReader.BaseStream.Position;

                            //Call this method again recursively to process the Ethernet frame stripped of the second VLAN tag
                            ThePacketProcessingResult = ProcessPayload(ThePacketNumber, TheTimestamp);
                        }

                        break;
                    }

                default:
                    {
                        //We have got an Ethernet frame containing an unknown Ether Type

                        //Check against the minimum value for Ether Type - lower values indicate length of the Ethernet frame
                        if (TheEtherType < (System.UInt16)Constants.EthernetFrameHeaderEtherTypeEnumeration.MinimumValue)
                        {
                            //This Ethernet frame has a value for "Ether Type" lower than the minimum
                            //This is an IEEE 802.3 Ethernet frame rather than an Ethernet II frame
                            //This value is the length of the IEEE 802.3 Ethernet frame

                            //Not going to process IEEE 802.3 Ethernet frames currently as they do not include any data of interest
                            //Just read off the bytes for the IEEE 802.3 Ethernet frame from the packet capture so we can move on
                            TheBinaryReader.ReadBytes(TheEtherType);

                            ThePacketProcessingResult = true;
                        }
                        else
                        {
                            //Processing of Ethernet frames with Ether Types not enumerated above are obviously not currently supported!

                            //Just record the event and fall through to the processing below that will read off the payload so we can move on
                            System.Diagnostics.Trace.WriteLine
                                (
                                "The Ethernet frame in captured packet #" +
                                ThePacketNumber.ToString() +
                                " contains an unexpected Ether Type of 0x" +
                                string.Format("{0:X}", TheEtherType)
                                );

                            ThePacketProcessingResult = false;
                        }

                        break;
                    }
            }

            if (!ThePacketProcessingResult)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "Processing of the Ethernet frame in captured packet #" +
                    ThePacketNumber.ToString() +
                    " encountered an error during processing of the payload!" +
                    " - Attempt to recover and continue processing"
                    );
            }

            //Calculate how far has been progressed through the stream by the actions above to read from the packet capture
            long TheStreamPositionDifference = TheBinaryReader.BaseStream.Position - TheStartingStreamPosition;

            //Check whether the Ethernet frame payload has extra trailer bytes
            //These would typically not be exposed in the packet capture by the recorder, but sometimes are for whatever reason!

            //This processing would also read off the payload of the Ethernet frame in the event of an unknown or unsupported network datalink type

            if (ThePayloadLength != TheStreamPositionDifference)
            {
                if (ThePayloadLength > TheStreamPositionDifference)
                {
                    //Trim the extra trailer bytes
                    TheBinaryReader.ReadBytes((int)(ThePayloadLength - TheStreamPositionDifference));
                }
                else
                {
                    //This is a strange error condition

                    //Back up the stream position to where it "should be", warn about it having arisen and then move on

                    TheBinaryReader.BaseStream.Position = TheBinaryReader.BaseStream.Position - (TheStreamPositionDifference - ThePayloadLength);

                    System.Diagnostics.Trace.WriteLine
                        (
                        "The length " +
                        ThePayloadLength.ToString() +
                        " of payload of Ethernet frame in captured packet #" +
                        ThePacketNumber.ToString() +
                        " does not match the progression " +
                        TheStreamPositionDifference.ToString() +
                        " through the stream!!!" +
                        " - Attempt to recover and continue processing"
                        );
                }
            }

            return TheResult;
        }
    }
}