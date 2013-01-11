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

namespace EthernetFrameNamespace.ARPPacketNamespace
{
    class ARPPacketProcessing
    {
        private System.IO.BinaryReader TheBinaryReader;

        public ARPPacketProcessing(System.IO.BinaryReader TheBinaryReader)
        {
            this.TheBinaryReader = TheBinaryReader;
        }

        public bool Process(long ThePayloadLength)
        {
            bool TheResult = true;

            if (ThePayloadLength < ARPPacketConstants.ARPPacketLength)
            {
                System.Diagnostics.Debug.WriteLine("The payload length of the Ethernet frame is lower than the length of the ARP packet!!!");

                return false;
            }

            //Create an instance of the ARP packet
            ARPPacketStructures.ARPPacketStructure ThePacket = new ARPPacketStructures.ARPPacketStructure();

            //Just read off the bytes for the ARP packet from the packet capture so we can move on
            ThePacket.HardwareType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            ThePacket.ProtocolType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            ThePacket.HardwareAddressLength = TheBinaryReader.ReadByte();
            ThePacket.ProtocolAddressLength = TheBinaryReader.ReadByte();
            ThePacket.Operation = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            ThePacket.SenderHardwareAddressHigh = TheBinaryReader.ReadUInt32();
            ThePacket.SenderHardwareAddressLow = TheBinaryReader.ReadUInt16();
            ThePacket.SenderProtocolAddress = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());
            ThePacket.TargetHardwareAddressHigh = TheBinaryReader.ReadUInt32();
            ThePacket.TargetHardwareAddressLow = TheBinaryReader.ReadUInt16();
            ThePacket.TargetProtocolAddress = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt32());

            return TheResult;
        }
    }
}