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

namespace EthernetFrameNamespace.IPv4PacketNamespace.ICMPv4PacketNamespace
{
    class ICMPv4PacketProcessing
    {
        public bool ProcessICMPv4Packet(System.IO.BinaryReader TheBinaryReader, int TheICMPv4PacketLength)
        {
            bool TheResult = true;

            //Process the ICMPv4 packet header
            TheResult = ProcessICMPv4PacketHeader(TheBinaryReader);
            
            //Just read off the remaining bytes of the ICMPv4 packet from the packet capture so we can move on
            //The remaining length is the total supplied length of the ICMPv4 packet minus the length for the ICMPv4 packet header
            TheBinaryReader.ReadBytes(TheICMPv4PacketLength - ICMPv4PacketConstants.ICMPv4PacketHeaderLength);

            return TheResult;
        }

        private bool ProcessICMPv4PacketHeader(System.IO.BinaryReader TheBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the ICMPv4 packet header
            ICMPv4PacketStructures.ICMPv4PacketHeaderStructure TheICMPPacketHeader = new ICMPv4PacketStructures.ICMPv4PacketHeaderStructure();

            //Read the values for the ICMPv4 packet header from the packet capture
            TheICMPPacketHeader.Type = TheBinaryReader.ReadByte();
            TheICMPPacketHeader.Code = TheBinaryReader.ReadByte();
            TheICMPPacketHeader.Checksum = TheBinaryReader.ReadUInt16();

            return TheResult;
        }
    }
}