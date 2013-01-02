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
        public bool ProcessEthernetFrame(System.IO.BinaryReader TheRecordingBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the Ethernet frame header
            EthernetFrameStructures.EthernetFrameHeaderStructure TheEthernetFrameHeader = new EthernetFrameStructures.EthernetFrameHeaderStructure();

            //Read the Destination MAC Address for the Ethernet frame from the recording
            TheEthernetFrameHeader.DestinationMACAddressHigh = TheRecordingBinaryReader.ReadUInt32();
            TheEthernetFrameHeader.DestinationMACAddressLow = TheRecordingBinaryReader.ReadUInt16();

            //Read the Source MAC Address for the Ethernet frame from the recording
            TheEthernetFrameHeader.SourceMACAddressHigh = TheRecordingBinaryReader.ReadUInt32();
            TheEthernetFrameHeader.SourceMACAddressLow = TheRecordingBinaryReader.ReadUInt16();

            //Read the Ether Type for the Ethernet frame from the recording and process it
            TheResult = ProcessEthernetFrameEtherType(TheRecordingBinaryReader, TheEthernetFrameHeader);

            return TheResult;
        }

        //A re-read of the Ether Type for an Ethernet frame with a VLAN tag (IEEE 802.1Q), if required, will be acheived by another call to this method
        //Therefore this method must be re-entrant so no explicitly static entities and the like!
        public bool ProcessEthernetFrameEtherType(System.IO.BinaryReader TheRecordingBinaryReader, EthernetFrameStructures.EthernetFrameHeaderStructure TheEthernetFrameHeader)
        {
            bool TheResult = true;

            TheEthernetFrameHeader.EtherType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheRecordingBinaryReader.ReadInt16());

            switch (TheEthernetFrameHeader.EtherType)
            {
                case EthernetFrameConstants.EthernetFrameHeaderEtherTypeARP:
                    {
                        ARPPacketNamespace.ARPPacketProcessing TheARPPacketProcessing = new ARPPacketNamespace.ARPPacketProcessing();

                        //We've got an Ethernet frame containing an ARP packet so process it
                        TheResult = TheARPPacketProcessing.ProcessARPPacket(TheRecordingBinaryReader);
                        break;
                    }

                case EthernetFrameConstants.EthernetFrameHeaderEtherTypeIPv4:
                    {
                        IPv4PacketNamespace.IPv4PacketProcessing TheIPv4PacketProcessing = new IPv4PacketNamespace.IPv4PacketProcessing();

                        //We've got an Ethernet frame containing an IPv4 packet so process it
                        TheResult = TheIPv4PacketProcessing.ProcessIPv4Packet(TheRecordingBinaryReader);
                        break;
                    }

                case EthernetFrameConstants.EthernetFrameHeaderEtherTypeIPv6:
                    {
                        //We've got an Ethernet frame containing an IPv6 packet

                        //Processing of Ethernet frames containing an IPv6 packet is not currently supported!

                        System.Diagnostics.Debug.WriteLine("The Ethernet frame contains an IPv6 packet which is not currently supported!!!");

                        TheResult = false;

                        break;
                    }

                case EthernetFrameConstants.EthernetFrameHeaderEtherTypeVLANTagged:
                    {
                        //We've got an Ethernet frame with a VLAN tag (IEEE 802.1Q) so must advance and re-read the Ether Type

                        //The "Ether Type" we've just read will actually be the IEEE 802.1Q Tag Protocol Identifier

                        //First just read off the IEEE 802.1Q Tag Control Identifier so we can move on
                        System.UInt16 TagControlIdentifier = TheRecordingBinaryReader.ReadUInt16();

                        //Then re-read the Ether Type, this time obtaining the real value (so long as there is only one VLAN tag of course!)
                        //Then re-read of the Ether Type will be acheived by another call to this method so it must be re-entrant
                        TheResult = ProcessEthernetFrameEtherType(TheRecordingBinaryReader, TheEthernetFrameHeader);

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

            return TheResult;
        }
    }
}
