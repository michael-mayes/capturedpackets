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

namespace EthernetFrameNamespace
{
    class EthernetFrameProcessing
    {
        public bool Process(System.IO.BinaryReader TheBinaryReader, long ThePayloadLength)
        {
            bool TheResult = true;

            //Create an instance of the Ethernet frame header
            EthernetFrameStructures.EthernetFrameHeaderStructure TheEthernetFrameHeader = new EthernetFrameStructures.EthernetFrameHeaderStructure();

            //Read the Destination MAC Address for the Ethernet frame from the packet capture
            TheEthernetFrameHeader.DestinationMACAddressHigh = TheBinaryReader.ReadUInt32();
            TheEthernetFrameHeader.DestinationMACAddressLow = TheBinaryReader.ReadUInt16();

            //Read the Source MAC Address for the Ethernet frame from the packet capture
            TheEthernetFrameHeader.SourceMACAddressHigh = TheBinaryReader.ReadUInt32();
            TheEthernetFrameHeader.SourceMACAddressLow = TheBinaryReader.ReadUInt16();

            //Read the Ether Type for the Ethernet frame from the packet capture and process it
            TheResult = ProcessEtherType(TheBinaryReader, TheEthernetFrameHeader, ThePayloadLength);

            return TheResult;
        }

        //A re-read of the Ether Type for an Ethernet frame with a VLAN tag (IEEE 802.1Q), if required, will be acheived by another call to this method
        //Therefore this method must be re-entrant so no explicitly static entities and the like!
        public bool ProcessEtherType(System.IO.BinaryReader TheBinaryReader, EthernetFrameStructures.EthernetFrameHeaderStructure TheEthernetFrameHeader, long ThePayloadLength)
        {
            bool TheResult = true;

            //Record the position in the stream for the packet capture so we can later determine how far has been progressed
            long TheStartingStreamPosition = TheBinaryReader.BaseStream.Position;

            TheEthernetFrameHeader.EtherType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());

            //Check against the minimum value for Ether Type - lower values indicate length of the Ethernet frame
            if (TheEthernetFrameHeader.EtherType < (System.UInt16)EthernetFrameConstants.EthernetFrameHeaderEtherTypeEnumeration.MinimumValue)
            {
                //This Ethernet frame has a value for "Ether Type" lower than the minimum
                //This is an IEEE 802.3 Ethernet frame rather than an Ethernet II frame
                //This value is the length of the IEEE 802.3 Ethernet frame

                //Not going to process IEEE 802.3 Ethernet frames currently as they do not include any data of interest
                //Just read off the bytes for the IEEE 802.3 Ethernet frame from the packet capture so we can move on
                TheBinaryReader.ReadBytes(TheEthernetFrameHeader.EtherType);
            }
            else
            {
                switch (TheEthernetFrameHeader.EtherType)
                {
                    case (System.UInt16)EthernetFrameConstants.EthernetFrameHeaderEtherTypeEnumeration.ARP:
                        {
                            ARPPacketNamespace.ARPPacketProcessing TheARPPacketProcessing = new ARPPacketNamespace.ARPPacketProcessing();

                            //We've got an Ethernet frame containing an ARP packet so process it
                            TheResult = TheARPPacketProcessing.Process(TheBinaryReader);
                            break;
                        }

                    case (System.UInt16)EthernetFrameConstants.EthernetFrameHeaderEtherTypeEnumeration.IPv4:
                        {
                            IPv4PacketNamespace.IPv4PacketProcessing TheIPv4PacketProcessing = new IPv4PacketNamespace.IPv4PacketProcessing();

                            //We've got an Ethernet frame containing an IPv4 packet so process it
                            TheResult = TheIPv4PacketProcessing.Process(TheBinaryReader);
                            break;
                        }

                    case (System.UInt16)EthernetFrameConstants.EthernetFrameHeaderEtherTypeEnumeration.IPv6:
                        {
                            //We've got an Ethernet frame containing an IPv6 packet

                            //Processing of Ethernet frames containing an IPv6 packet is not currently supported!

                            System.Diagnostics.Debug.WriteLine("The Ethernet frame contains an IPv6 packet which is not currently supported!!!");

                            TheResult = false;

                            break;
                        }

                    case (System.UInt16)EthernetFrameConstants.EthernetFrameHeaderEtherTypeEnumeration.LLDP:
                        {
                            LLDPPacketNamespace.LLDPPacketProcessing TheLLDPPacketProcessing = new LLDPPacketNamespace.LLDPPacketProcessing();

                            //We've got an Ethernet frame containing an LLDP packet so process it
                            TheResult = TheLLDPPacketProcessing.Process(TheBinaryReader);
                            break;
                        }

                    case (System.UInt16)EthernetFrameConstants.EthernetFrameHeaderEtherTypeEnumeration.Loopback:
                        {
                            LoopbackPacketNamespace.LoopbackPacketProcessing TheLoopbackPacketProcessing = new LoopbackPacketNamespace.LoopbackPacketProcessing();

                            //We've got an Ethernet frame containing an Configuration Test Protocol (Loopback) packet so process it
                            TheResult = TheLoopbackPacketProcessing.Process(TheBinaryReader);
                            break;
                        }

                    case (System.UInt16)EthernetFrameConstants.EthernetFrameHeaderEtherTypeEnumeration.VLANTagged:
                        {
                            //We've got an Ethernet frame with a VLAN tag (IEEE 802.1Q) so must advance and re-read the Ether Type

                            //The "Ether Type" we've just read will actually be the IEEE 802.1Q Tag Protocol Identifier

                            //First just read off the IEEE 802.1Q Tag Control Identifier so we can move on
                            System.UInt16 TheTagControlIdentifier = TheBinaryReader.ReadUInt16();

                            //Then re-read the Ether Type, this time obtaining the real value (so long as there is only one VLAN tag of course!)
                            //Then re-read of the Ether Type will be acheived by another call to this method so it must be re-entrant
                            TheResult = ProcessEtherType(TheBinaryReader, TheEthernetFrameHeader, ThePayloadLength);

                            break;
                        }

                    default:
                        {
                            //We've got an Ethernet frame containing an unknown network data link type

                            //Processing of Ethernet frames with network data link types not enumerated above are obviously not currently supported!

                            System.Diagnostics.Debug.WriteLine("The Ethernet frame contains an unexpected network data link type of {0:X}", TheEthernetFrameHeader.EtherType);

                            TheResult = false;

                            break;
                        }
                }
            }

            if (TheResult)
            {
                //Calculate how far has been progressed through the stream by the actions above to read from the packet capture
                long TheStreamPositionDifference = TheBinaryReader.BaseStream.Position - TheStartingStreamPosition;

                //Check whether the Ethernet frame payload has extra trailer bytes
                //These would typically not be exposed in the packet capture by the recorder, but sometimes are for whatever reason!
                if (ThePayloadLength != TheStreamPositionDifference)
                {
                    if (ThePayloadLength > TheStreamPositionDifference)
                    {
                        //Trim the extra trailer bytes
                        TheBinaryReader.ReadBytes((int)(ThePayloadLength - TheStreamPositionDifference));
                    }
                    else
                    {
                        //This is a strange error condition so indicate a failure

                        System.Diagnostics.Debug.WriteLine("The length {0} of payload of Ethernet frame does not match the progression {1} through the stream ", ThePayloadLength, TheStreamPositionDifference);

                        TheResult = false;
                    }
                }
            }

            return TheResult;
        }
    }
}