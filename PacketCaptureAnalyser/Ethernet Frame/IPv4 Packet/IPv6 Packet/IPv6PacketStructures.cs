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

namespace EthernetFrameNamespace.IPPacketNamespace
{
    class IPv6PacketStructures
    {
        //
        //IPv6 packet header - 40 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = IPv6PacketConstants.IPv6PacketHeaderLength)]

        public struct IPv6PacketHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.Byte VersionAndTrafficClass; //IP packet version and traffic class

            [System.Runtime.InteropServices.FieldOffset(1)]
            public System.Byte TrafficClassAndFlowLabel; //Traffic class and flow label

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 FlowLabel; //Traffic class and flow label

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt16 PayloadLength; //Total length of the IPv6 packet

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.Byte NextHeader; //The next header within the IPv6 packet - indicates the protocol for the payload (ICMP, IGMP, TCP, UDP etc)

            [System.Runtime.InteropServices.FieldOffset(7)]
            public System.Byte HopLimit; //Hop limit - the maximum number of routers that the IPv6 packet is allowed to travel through before being discarded

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.Int64 SourceAddressHigh; //The high bytes of the source IP address (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.Int64 SourceAddressLow; //The low bytes of the source IP address (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(24)]
            public System.Int64 DestinationAddressHigh; //The high bytes of the destination IP address (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(32)]
            public System.Int64 DestinationAddressLow; //The low bytes of the destination IP address (eight bytes)
        }
    }
}