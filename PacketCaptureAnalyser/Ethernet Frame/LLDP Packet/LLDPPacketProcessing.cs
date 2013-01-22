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

namespace EthernetFrameNamespace.LLDPPacketNamespace
{
    class LLDPPacketProcessing
    {
        private System.IO.BinaryReader TheBinaryReader;

        public LLDPPacketProcessing(System.IO.BinaryReader TheBinaryReader)
        {
            this.TheBinaryReader = TheBinaryReader;
        }

        public bool Process(long ThePayloadLength)
        {
            bool TheResult = true;

            if (ThePayloadLength < LLDPPacketConstants.LLDPPacketLength)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The payload length of the Ethernet frame is lower than the length of the LLDP packet!!!"
                    );

                return false;
            }

            //Create an instance of the LLDP packet
            LLDPPacketStructures.LLDPPacketStructure ThePacket = new LLDPPacketStructures.LLDPPacketStructure();

            //Just read off the bytes for the LLDP packet from the packet capture so we can move on
            ThePacket.UnusedField1 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField2 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField3 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField4 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField5 = TheBinaryReader.ReadUInt64();
            ThePacket.UnusedField6 = TheBinaryReader.ReadUInt32();
            ThePacket.UnusedField7 = TheBinaryReader.ReadUInt16();

            return TheResult;
        }
    }
}