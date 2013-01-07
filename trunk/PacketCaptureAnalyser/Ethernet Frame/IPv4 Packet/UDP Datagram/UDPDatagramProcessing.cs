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
        private System.IO.BinaryReader TheBinaryReader;
        private LatencyAnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing;

        public UDPDatagramProcessing(System.IO.BinaryReader TheBinaryReader, LatencyAnalysisNamespace.LatencyAnalysisProcessing TheLatencyAnalysisProcessing)
        {
            this.TheBinaryReader = TheBinaryReader;
            this.TheLatencyAnalysisProcessing = TheLatencyAnalysisProcessing;
        }

        public bool Process(double TheTimestamp, int TheLength)
        {
            bool TheResult = true;

            int ThePayloadLength = 0;
            int TheSourcePort = 0;
            int TheDestinationPort = 0;

            //Process the UDP datagram header
            TheResult = ProcessHeader(TheLength, out ThePayloadLength, out TheSourcePort, out TheDestinationPort);

            if (TheResult)
            {
                //Process the payload of the UDP datagram, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the UDP datagram header
                TheResult = ProcessPayload(TheTimestamp, ThePayloadLength, TheSourcePort, TheDestinationPort);
            }

            return TheResult;
        }

        private bool ProcessHeader(int TheLength, out int ThePayloadLength, out int TheSourcePort, out int TheDestinationPort)
        {
            bool TheResult = true;

            //Provide default values for the output parameters for source port and destination port
            TheSourcePort = 0;
            TheDestinationPort = 0;

            //Create an instance of the UDP datagram header
            UDPDatagramStructures.UDPDatagramHeaderStructure TheHeader = new UDPDatagramStructures.UDPDatagramHeaderStructure();

            //Read the values for the UDP datagram header from the packet capture
            TheHeader.SourcePort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.DestinationPort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.Length = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.Checksum = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());

            //Set up the output parameter for the length of the payload of the UDP datagram, which is the total length of the UDP datagram read from the UDP datagram header minus the length of the UDP datagram header
            ThePayloadLength = TheHeader.Length - UDPDatagramConstants.UDPDatagramHeaderLength;

            if (TheResult)
            {
                //Set up the output parameters for source port and destination port using the value read from the UDP datagram header
                TheSourcePort = TheHeader.SourcePort;
                TheDestinationPort = TheHeader.DestinationPort;
            }

            return TheResult;
        }

        private bool ProcessPayload(double TheTimestamp, int ThePayloadLength, int TheSourcePort, int TheDestinationPort)
        {
            bool TheResult = true;

            //Only process this UDP datagram if the payload has a non-zero payload length i.e. it actually includes data (unlikely to not include data, but retain check for consistency with TCP packet processing)
            if (ThePayloadLength > 0)
            {
                switch (TheSourcePort)
                {
                    default:
                        {
                            //Just read off the remaining bytes of the UDP datagram from the packet capture so we can move on
                            //The remaining length is the supplied length of the UDP datagram payload
                            TheBinaryReader.ReadBytes(ThePayloadLength);
                            break;
                        }
                }
            }

            return TheResult;
        }
    }
}