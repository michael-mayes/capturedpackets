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
        public bool ProcessIGMPv2Packet(System.IO.BinaryReader ThePackageCaptureBinaryReader, int TheIGMPv2PacketLength)
        {
            bool TheResult = true;

            //There is no separate header for the IGMPv2 packet

            //Create an instance of the IGMPv2 packet
            IGMPv2PacketStructures.IGMPv2PacketStructure TheIGMPv2Packet = new IGMPv2PacketStructures.IGMPv2PacketStructure();

            //Just read off the bytes for the IGMPv2 packet from the packet capture so we can move on
            TheIGMPv2Packet.Type = ThePackageCaptureBinaryReader.ReadByte();
            TheIGMPv2Packet.MaxResponseTime = ThePackageCaptureBinaryReader.ReadByte();
            TheIGMPv2Packet.Checksum = ThePackageCaptureBinaryReader.ReadUInt16();
            TheIGMPv2Packet.GroupAddress = ThePackageCaptureBinaryReader.ReadUInt32();

            return TheResult;
        }
    }
}