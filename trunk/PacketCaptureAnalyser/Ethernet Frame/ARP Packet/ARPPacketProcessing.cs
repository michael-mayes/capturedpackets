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

namespace EthernetFrameNamespace.ARPPacketNamespace
{
    class ARPPacketProcessing
    {
        public bool ProcessARPPacket(System.IO.BinaryReader ThePackageCaptureBinaryReader)
        {
            bool TheResult = true;

            //Create an instance of the ARP packet
            ARPPacketStructures.ARPPacketStructure TheARPPacket = new ARPPacketStructures.ARPPacketStructure();

            //Just read off the bytes for the ARP packet from the packet capture so we can move on
            TheARPPacket.HardwareType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheARPPacket.ProtocolType = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheARPPacket.HardwareAddressLength = ThePackageCaptureBinaryReader.ReadByte();
            TheARPPacket.ProtocolAddressLength = ThePackageCaptureBinaryReader.ReadByte();
            TheARPPacket.Operation = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt16());
            TheARPPacket.SenderHardwareAddressHigh = ThePackageCaptureBinaryReader.ReadUInt32();
            TheARPPacket.SenderHardwareAddressLow = ThePackageCaptureBinaryReader.ReadUInt16();
            TheARPPacket.SenderProtocolAddress = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt32());
            TheARPPacket.TargetHardwareAddressHigh = ThePackageCaptureBinaryReader.ReadUInt32();
            TheARPPacket.TargetHardwareAddressLow = ThePackageCaptureBinaryReader.ReadUInt16();
            TheARPPacket.TargetProtocolAddress = (System.UInt32)System.Net.IPAddress.NetworkToHostOrder(ThePackageCaptureBinaryReader.ReadInt32());

            return TheResult;
        }
    }
}