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

namespace EthernetFrameNamespace.IPv4PacketNamespace.UDPDatagramNamespace
{
    class UDPDatagramProcessing
    {
        public bool ProcessUDPDatagram(System.IO.BinaryReader ThePackageCaptureBinaryReader, int TheUDPDatagramLength)
        {
            bool TheResult = true;

            int TheUDPDatagramPayloadLength = 0;
            int TheUDPDatagramSourcePort = 0;
            int TheUDPDatagramDestinationPort = 0;

            //Process the UDP datagram header
            TheResult = ProcessUDPDatagramHeader(ThePackageCaptureBinaryReader, TheUDPDatagramLength, out TheUDPDatagramPayloadLength, out TheUDPDatagramSourcePort, out TheUDPDatagramDestinationPort);

            if (TheResult)
            {
                //Process the payload of the UDP datagram, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the UDP datagram header
                TheResult = ProcessUDPDatagramPayload(ThePackageCaptureBinaryReader, TheUDPDatagramPayloadLength, TheUDPDatagramSourcePort, TheUDPDatagramDestinationPort);
            }

            return TheResult;
        }

        private bool ProcessUDPDatagramHeader(System.IO.BinaryReader ThePackageCaptureBinaryReader, int TheUDPDatagramLength, out int TheUDPDatagramPayloadLength, out int TheUDPDatagramSourcePort, out int TheUDPDatagramDestinationPort)
        {
            bool TheResult = true;

            //Create an instance of the UDP datagram header
            UDPDatagramStructures.UDPDatagramHeaderStructure TheUDPDatagramHeader = new UDPDatagramStructures.UDPDatagramHeaderStructure();

            //Read the values for the UDP datagram header from the packet capture
            TheUDPDatagramHeader.SourcePort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheUDPDatagramHeader.DestinationPort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheUDPDatagramHeader.Length = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheUDPDatagramHeader.Checksum = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());

            //Set up the output parameter for the length of the payload of the UDP datagram, which is the total length of the UDP datagram read from the UDP datagram header minus the length of the UDP datagram header
            TheUDPDatagramPayloadLength = TheUDPDatagramHeader.Length - UDPDatagramConstants.UDPDatagramHeaderLength;

            //Set up the output parameters for source port and destination port using the value read from the UDP datagram header
            TheUDPDatagramSourcePort = TheUDPDatagramHeader.SourcePort;
            TheUDPDatagramDestinationPort = TheUDPDatagramHeader.DestinationPort;

            return TheResult;
        }

        private bool ProcessUDPDatagramPayload(System.IO.BinaryReader ThePackageCaptureBinaryReader, int TheUDPDatagramPayloadLength, int TheUDPDatagramSourcePort, int TheUDPDatagramDestinationPort)
        {
            bool TheResult = true;

            switch (TheUDPDatagramDestinationPort)
            {
                default:
                    {
                        //Just read off the remaining bytes of the UDP datagram from the packet capture so we can move on
                        //The remaining length is the supplied length of the UDP datagram payload
                        ThePackageCaptureBinaryReader.ReadBytes(TheUDPDatagramPayloadLength);

                        break;
                    }
            }

            return TheResult;
        }
    }
}