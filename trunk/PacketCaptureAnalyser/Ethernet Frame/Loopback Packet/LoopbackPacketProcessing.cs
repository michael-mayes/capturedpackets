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

namespace EthernetFrameNamespace.LoopbackPacketNamespace
{
    class LoopbackPacketProcessing
    {
        private System.IO.BinaryReader TheBinaryReader;

        private LoopbackPacketStructures.LoopbackPacketHeaderStructure TheHeader;

        public LoopbackPacketProcessing(System.IO.BinaryReader TheBinaryReader)
        {
            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the Loopback packet header
            TheHeader = new LoopbackPacketStructures.LoopbackPacketHeaderStructure();
        }

        public bool Process(long ThePayloadLength)
        {
            bool TheResult = true;

            if (ThePayloadLength < (LoopbackPacketConstants.LoopbackPacketHeaderLength + LoopbackPacketConstants.LoopbackPacketPayloadLength))
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The payload length of the Ethernet frame is lower than the length of the Loopback packet!!!"
                    );

                return false;
            }

            //Process the Loopback packet header
            TheResult = ProcessHeader();

            //Just read off the remaining bytes of the Loopback packet from the packet capture so we can move on
            //The remaining length is the length for the Loopback packet payload
            TheBinaryReader.ReadBytes(LoopbackPacketConstants.LoopbackPacketPayloadLength);

            return TheResult;
        }

        private bool ProcessHeader()
        {
            bool TheResult = true;

            //Read the values for the Loopback packet header from the packet capture
            TheHeader.SkipCount = TheBinaryReader.ReadUInt16();
            TheHeader.Function = TheBinaryReader.ReadUInt16();
            TheHeader.ReceiptNumber = TheBinaryReader.ReadUInt16();

            return TheResult;
        }
    }
}