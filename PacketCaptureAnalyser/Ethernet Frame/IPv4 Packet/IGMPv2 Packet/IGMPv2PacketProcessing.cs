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

namespace EthernetFrameNamespace.IPv4PacketNamespace.IGMPv2PacketNamespace
{
    class IGMPv2PacketProcessing
    {
        private System.IO.BinaryReader TheBinaryReader;

        public IGMPv2PacketProcessing(System.IO.BinaryReader TheBinaryReader)
        {
            this.TheBinaryReader = TheBinaryReader;
        }

        public bool Process(int TheIGMPv2PacketLength)
        {
            bool TheResult = true;

            //There is no separate header for the IGMPv2 packet

            //Create an instance of the IGMPv2 packet
            IGMPv2PacketStructures.IGMPv2PacketStructure ThePacket = new IGMPv2PacketStructures.IGMPv2PacketStructure();

            //Just read off the bytes for the IGMPv2 packet from the packet capture so we can move on
            ThePacket.Type = TheBinaryReader.ReadByte();
            ThePacket.MaxResponseTime = TheBinaryReader.ReadByte();
            ThePacket.Checksum = TheBinaryReader.ReadUInt16();
            ThePacket.GroupAddress = TheBinaryReader.ReadUInt32();

            return TheResult;
        }
    }
}