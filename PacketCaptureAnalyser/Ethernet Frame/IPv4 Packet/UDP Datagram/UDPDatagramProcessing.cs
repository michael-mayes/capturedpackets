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

            //Process the UDP datagram header
            TheResult = ProcessUDPDatagramHeader(ThePackageCaptureBinaryReader);

            //Just read off the remaining bytes of the UDP datagram from the packet capture so we can move on
            //The remaining length is the total supplied length of the UDP datagram minus the length for the UDP datagram header
            for (int i = 0; i < (TheUDPDatagramLength - UDPDatagramConstants.UDPDatagramHeaderLength); ++i)
            {
                ThePackageCaptureBinaryReader.ReadByte();
            }

            return TheResult;
        }

        private bool ProcessUDPDatagramHeader(System.IO.BinaryReader ThePackageCaptureBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the UDP datagram header
            UDPDatagramStructures.UDPDatagramHeaderStructure TheUDPDatagramHeader = new UDPDatagramStructures.UDPDatagramHeaderStructure();

            //Read the values for the UDP datagram header from the packet capture
            TheUDPDatagramHeader.SourcePort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheUDPDatagramHeader.DestinationPort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheUDPDatagramHeader.Length = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheUDPDatagramHeader.Checksum = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());

            return TheResult;
        }
    }
}