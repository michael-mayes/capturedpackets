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

namespace EthernetFrameNamespace.IPv4PacketNamespace
{
    class IPv4PacketProcessing
    {
        public bool ProcessIPv4Packet(System.IO.BinaryReader ThePackageCaptureBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the IPv4 packet header
            IPv4PacketStructures.IPv4PacketHeaderStructure TheIPv4PacketHeader = new IPv4PacketStructures.IPv4PacketHeaderStructure();

            //Read the values for the IPv4 packet header from the packet capture
            TheIPv4PacketHeader.VersionAndHeaderLength = ThePackageCaptureBinaryReader.ReadByte();
            TheIPv4PacketHeader.TypeOfService = ThePackageCaptureBinaryReader.ReadByte();
            TheIPv4PacketHeader.TotalLength = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheIPv4PacketHeader.Identifier = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheIPv4PacketHeader.FlagsAndOffset = ThePackageCaptureBinaryReader.ReadUInt16();
            TheIPv4PacketHeader.TimeToLive = ThePackageCaptureBinaryReader.ReadByte();
            TheIPv4PacketHeader.Protocol = ThePackageCaptureBinaryReader.ReadByte();
            TheIPv4PacketHeader.HeaderChecksum = ThePackageCaptureBinaryReader.ReadUInt16();
            TheIPv4PacketHeader.SourceAddress = ThePackageCaptureBinaryReader.ReadInt32();
            TheIPv4PacketHeader.DestinationAddress = ThePackageCaptureBinaryReader.ReadInt32();

            //Determine the length of the IPv4 packet header
            //Need to first extract the length value from the combined IP version/IP header length field
            //We want the lower four bits from the combined IP version/IP header length field (as it's in a big endian representation) so do a bitwise OR with 0xF (i.e. 00001111 in binary)
            //The extracted length value is the length of the IPv4 packet header in 32-bit words so multiply by four to get the actual length in bytes of the IPv4 packet header
            int TheIPv4PacketHeaderLength = ((TheIPv4PacketHeader.VersionAndHeaderLength & 0xF) * 4);

            //The length of the payload of the IPv4 packet (e.g. a TCP packet) is the total length of the IPv4 packet minus the length of the IPv4 packet header just calculated
            int TheIPv4PacketPayloadLength = (TheIPv4PacketHeader.TotalLength - TheIPv4PacketHeaderLength);

            //Validate length of the IPv4 packet header
            if (TheIPv4PacketHeaderLength != IPv4PacketConstants.IPv4PacketHeaderLength)
            {
                System.Diagnostics.Debug.WriteLine("The IPv4 packet does not contain the expected header length, is {0:X} not {1:X}", TheIPv4PacketHeaderLength, IPv4PacketConstants.IPv4PacketHeaderLength);

                TheResult = false;
            }
            else
            {
                //Process the IPv4 packet based on the value indicated for the protocol in the the IPv4 packet header
                switch (TheIPv4PacketHeader.Protocol)
                {
                    case IPv4PacketConstants.IPv4PacketHeaderProtocolICMP:
                        {
                            ICMPv4PacketNamespace.ICMPv4PacketProcessing TheICMPv4PacketProcessing = new ICMPv4PacketNamespace.ICMPv4PacketProcessing();

                            //We've got an IPv4 packet containing an ICMPv4 packet so process it
                            TheResult = TheICMPv4PacketProcessing.ProcessICMPv4Packet(ThePackageCaptureBinaryReader, TheIPv4PacketPayloadLength);
                            break;
                        }

                    case IPv4PacketConstants.IPv4PacketHeaderProtocolIGMP:
                        {
                            IGMPv2PacketNamespace.IGMPv2PacketProcessing TheIGMPv2PacketProcessing = new IGMPv2PacketNamespace.IGMPv2PacketProcessing();

                            //We've got an IPv4 packet containing an IGMPv2 packet so process it
                            TheResult = TheIGMPv2PacketProcessing.ProcessIGMPv2Packet(ThePackageCaptureBinaryReader, TheIPv4PacketPayloadLength);
                            break;
                        }

                    case IPv4PacketConstants.IPv4PacketHeaderProtocolTCP:
                        {
                            TCPPacketNamespace.TCPPacketProcessing TheTCPPacketProcessing = new TCPPacketNamespace.TCPPacketProcessing();

                            //We've got an IPv4 packet containing an TCP packet so process it
                            TheResult = TheTCPPacketProcessing.ProcessTCPPacket(ThePackageCaptureBinaryReader, TheIPv4PacketPayloadLength);
                            break;
                        }

                    case IPv4PacketConstants.IPv4PacketHeaderProtocolUDP:
                        {
                            UDPDatagramNamespace.UDPDatagramProcessing TheUDPDatagramProcessing = new UDPDatagramNamespace.UDPDatagramProcessing();

                            //We've got an IPv4 packet containing an UDP datagram so process it
                            TheResult = TheUDPDatagramProcessing.ProcessUDPDatagram(ThePackageCaptureBinaryReader, TheIPv4PacketPayloadLength);
                            break;
                        }

                    default:
                        {
                            //We've got an IPv4 packet containing an unknown protocol

                            //Processing of packets with network data link types not enumerated above are obviously not currently supported!

                            System.Diagnostics.Debug.WriteLine("The IPv4 packet contains an unexpected protocol of {0:X}", TheIPv4PacketHeader.Protocol);

                            TheResult = false;

                            break;
                        }
                }
            }

            return TheResult;
        }
    }
}