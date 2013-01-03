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
    class ARPPacketStructures
    {
        //
        //ARP packet - 28 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = ARPPacketConstants.ARPPacketHeaderLength)]

        public struct ARPPacketStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt16 HardwareType; //The network protocol type (e.g. Ethernet is 1)

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 ProtocolType; //The internetwork protocol for which the ARP request is intended (e.g. IPv4 is 0x0800) - same possible values as for Ether Type in Ethernet frame header

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.Byte HardwareAddressLength; //Length (in octets) of a hardware address (e.g. Ethernet addresses size is 6)

            [System.Runtime.InteropServices.FieldOffset(5)]
            public System.Byte ProtocolAddressLength; //Length (in octets) of addresses used in the upper layer internetwork protocol (e.g. IPv4 address size is 4)

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.UInt16 Operation; //The operation that the sender is performing - 1 for request and 2 for reply
            
            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt32 SenderHardwareAddressHigh; //The high bytes of the media address of the sender (four bytes)

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.UInt16 SenderHardwareAddressLow; //The low bytes of the media address of the sender (two bytes)

            [System.Runtime.InteropServices.FieldOffset(14)]
            public System.UInt32 SenderProtocolAddress; //The internetwork address of the sender
            
            [System.Runtime.InteropServices.FieldOffset(18)]
            public System.UInt32 TargetHardwareAddressHigh; //The high bytes of the media address of the intended receiver (four bytes) - ignored in requests

            [System.Runtime.InteropServices.FieldOffset(22)]
            public System.UInt16 TargetHardwareAddressLow; //The low bytes of the media address of the intended receiver (two bytes) - ignored in requests

            [System.Runtime.InteropServices.FieldOffset(24)]
            public System.UInt32 TargetProtocolAddress; //The internetwork address of the intended receiver
        }
    }
}